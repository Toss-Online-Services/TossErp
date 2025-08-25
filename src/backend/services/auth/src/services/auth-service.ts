import { PrismaClient } from '@prisma/client';
import { Redis } from 'ioredis';
import bcrypt from 'bcryptjs';
import jwt from 'jsonwebtoken';
import { v4 as uuidv4 } from 'uuid';

export interface AuditLogData {
  userId?: string;
  tenantId?: string;
  action: string;
  resource: string;
  resourceId?: string;
  oldValues?: any;
  newValues?: any;
  ipAddress?: string;
  userAgent?: string;
}

export class AuthService {
  constructor(
    private prisma: PrismaClient,
    private redis: Redis,
    private logger: any
  ) {}

  async generateTokens(user: any, rememberMe = false) {
    const jwtSecret = process.env.JWT_SECRET || 'toss-erp-secret-key';
    const accessTokenExpiry = process.env.JWT_EXPIRES_IN || '15m';
    const refreshTokenExpiry = rememberMe ? '30d' : '7d';

    const payload = {
      user: {
        id: user.id,
        email: user.email,
        firstName: user.firstName,
        lastName: user.lastName,
        role: user.role,
        tenantId: user.tenantId,
      },
    };

    const accessToken = jwt.sign(payload, jwtSecret, {
      expiresIn: accessTokenExpiry,
    });

    const refreshToken = jwt.sign(
      { userId: user.id, type: 'refresh' },
      jwtSecret,
      { expiresIn: refreshTokenExpiry }
    );

    // Store refresh token in database
    const expiresAt = new Date();
    expiresAt.setDate(expiresAt.getDate() + (rememberMe ? 30 : 7));

    await this.prisma.refreshToken.create({
      data: {
        id: uuidv4(),
        token: refreshToken,
        userId: user.id,
        expiresAt,
      },
    });

    return {
      accessToken,
      refreshToken,
    };
  }

  async refreshTokens(refreshToken: string) {
    const jwtSecret = process.env.JWT_SECRET || 'toss-erp-secret-key';

    try {
      // Verify and decode refresh token
      const decoded = jwt.verify(refreshToken, jwtSecret) as any;

      // Check if refresh token exists and is not revoked
      const storedToken = await this.prisma.refreshToken.findUnique({
        where: { token: refreshToken },
        include: {
          user: {
            include: {
              tenant: true,
            },
          },
        },
      });

      if (!storedToken || storedToken.isRevoked || storedToken.expiresAt < new Date()) {
        throw new Error('Invalid refresh token');
      }

      if (!storedToken.user.isActive || !storedToken.user.tenant.isActive) {
        throw new Error('User or tenant is inactive');
      }

      // Generate new tokens
      const tokens = await this.generateTokens(storedToken.user);

      // Revoke old refresh token
      await this.prisma.refreshToken.update({
        where: { token: refreshToken },
        data: { isRevoked: true },
      });

      return tokens;
    } catch (error) {
      throw new Error('Invalid refresh token');
    }
  }

  async revokeRefreshToken(refreshToken: string) {
    await this.prisma.refreshToken.updateMany({
      where: { token: refreshToken },
      data: { isRevoked: true },
    });
  }

  async createLoginSession(userId: string, ipAddress?: string, userAgent?: string) {
    const sessionId = uuidv4();

    await this.prisma.loginSession.create({
      data: {
        id: uuidv4(),
        userId,
        sessionId,
        ipAddress,
        userAgent,
      },
    });

    return sessionId;
  }

  async endLoginSession(userId: string) {
    await this.prisma.loginSession.updateMany({
      where: {
        userId,
        isActive: true,
      },
      data: {
        logoutAt: new Date(),
        isActive: false,
      },
    });
  }

  async verifyEmail(token: string) {
    const verification = await this.prisma.emailVerification.findUnique({
      where: { token },
      include: { user: true },
    });

    if (!verification || verification.isUsed || verification.expiresAt < new Date()) {
      throw new Error('Invalid or expired verification token');
    }

    await this.prisma.$transaction([
      this.prisma.user.update({
        where: { id: verification.userId },
        data: { emailVerified: true },
      }),
      this.prisma.emailVerification.update({
        where: { id: verification.id },
        data: { isUsed: true },
      }),
    ]);
  }

  async createPasswordReset(email: string) {
    const user = await this.prisma.user.findUnique({
      where: { email },
    });

    if (!user) {
      // Don't reveal if user exists for security
      return;
    }

    const token = uuidv4();
    const expiresAt = new Date();
    expiresAt.setHours(expiresAt.getHours() + 1); // 1 hour expiry

    await this.prisma.passwordReset.create({
      data: {
        id: uuidv4(),
        token,
        userId: user.id,
        expiresAt,
      },
    });

    // TODO: Send password reset email
    this.logger.info(`Password reset token for ${email}: ${token}`);
  }

  async resetPassword(token: string, newPassword: string) {
    const passwordReset = await this.prisma.passwordReset.findUnique({
      where: { token },
      include: { user: true },
    });

    if (!passwordReset || passwordReset.isUsed || passwordReset.expiresAt < new Date()) {
      throw new Error('Invalid or expired reset token');
    }

    const hashedPassword = await bcrypt.hash(newPassword, 12);

    await this.prisma.$transaction([
      this.prisma.user.update({
        where: { id: passwordReset.userId },
        data: { password: hashedPassword },
      }),
      this.prisma.passwordReset.update({
        where: { id: passwordReset.id },
        data: { isUsed: true },
      }),
      // Revoke all refresh tokens for security
      this.prisma.refreshToken.updateMany({
        where: { userId: passwordReset.userId },
        data: { isRevoked: true },
      }),
    ]);
  }

  async logAuditEvent(data: AuditLogData) {
    try {
      await this.prisma.auditLog.create({
        data: {
          id: uuidv4(),
          userId: data.userId,
          tenantId: data.tenantId,
          action: data.action,
          resource: data.resource,
          resourceId: data.resourceId,
          oldValues: data.oldValues,
          newValues: data.newValues,
          ipAddress: data.ipAddress,
          userAgent: data.userAgent,
        },
      });
    } catch (error) {
      this.logger.error('Failed to log audit event:', error);
    }
  }

  async validatePermission(userId: string, resource: string, action: string): Promise<boolean> {
    try {
      // Get user with roles and permissions
      const user = await this.prisma.user.findUnique({
        where: { id: userId },
        include: {
          userRoles: {
            include: {
              role: {
                include: {
                  rolePermissions: {
                    include: {
                      permission: true,
                    },
                  },
                },
              },
            },
          },
        },
      });

      if (!user || !user.isActive) {
        return false;
      }

      // Super admin has all permissions
      if (user.role === 'SUPER_ADMIN') {
        return true;
      }

      // Check role-based permissions
      for (const userRole of user.userRoles) {
        for (const rolePermission of userRole.role.rolePermissions) {
          const permission = rolePermission.permission;
          if (permission.resource === resource && permission.action === action) {
            return true;
          }
        }
      }

      return false;
    } catch (error) {
      this.logger.error('Permission validation failed:', error);
      return false;
    }
  }

  async getUserPermissions(userId: string): Promise<string[]> {
    try {
      const user = await this.prisma.user.findUnique({
        where: { id: userId },
        include: {
          userRoles: {
            include: {
              role: {
                include: {
                  rolePermissions: {
                    include: {
                      permission: true,
                    },
                  },
                },
              },
            },
          },
        },
      });

      if (!user || !user.isActive) {
        return [];
      }

      // Super admin has all permissions
      if (user.role === 'SUPER_ADMIN') {
        return ['*:*']; // Wildcard for all permissions
      }

      const permissions: string[] = [];
      for (const userRole of user.userRoles) {
        for (const rolePermission of userRole.role.rolePermissions) {
          const permission = rolePermission.permission;
          permissions.push(`${permission.resource}:${permission.action}`);
        }
      }

      return [...new Set(permissions)]; // Remove duplicates
    } catch (error) {
      this.logger.error('Failed to get user permissions:', error);
      return [];
    }
  }
}
