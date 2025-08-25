import { PrismaClient } from '@prisma/client';
import { Redis } from 'ioredis';
import { FastifyRequest, FastifyReply } from 'fastify';
import bcrypt from 'bcryptjs';
import jwt from 'jsonwebtoken';
import { v4 as uuidv4 } from 'uuid';
import { AuthService } from '../services/auth-service';
import { EmailService } from '../services/email-service';

export class AuthController {
  private authService: AuthService;
  private emailService: EmailService;

  constructor(
    private prisma: PrismaClient,
    private redis: Redis,
    private logger: any
  ) {
    this.authService = new AuthService(prisma, redis, logger);
    this.emailService = new EmailService(logger);
  }

  async register(request: FastifyRequest, reply: FastifyReply) {
    try {
      const { 
        email, 
        password, 
        firstName, 
        lastName, 
        tenantName, 
        tenantType,
        phone,
        mobile 
      } = request.body as any;

      // Check if user already exists
      const existingUser = await this.prisma.user.findUnique({
        where: { email },
      });

      if (existingUser) {
        return reply.status(409).send({
          success: false,
          error: {
            code: 'USER_EXISTS',
            message: 'User with this email already exists',
          },
        });
      }

      // Hash password
      const hashedPassword = await bcrypt.hash(password, 12);

      // Create tenant and user in a transaction
      const result = await this.prisma.$transaction(async (tx) => {
        // Create tenant
        const tenant = await tx.tenant.create({
          data: {
            id: uuidv4(),
            name: tenantName,
            type: tenantType,
            isActive: true,
          },
        });

        // Create user
        const user = await tx.user.create({
          data: {
            id: uuidv4(),
            email,
            password: hashedPassword,
            firstName,
            lastName,
            role: 'TENANT_ADMIN',
            tenantId: tenant.id,
            isActive: true,
          },
          include: {
            tenant: true,
          },
        });

        // Create user profile
        const profile = await tx.userProfile.create({
          data: {
            id: uuidv4(),
            userId: user.id,
            phone,
            mobile,
          },
        });

        return { user, tenant, profile };
      });

      // Generate tokens
      const tokens = await this.authService.generateTokens(result.user);

      // Create login session
      await this.authService.createLoginSession(
        result.user.id,
        request.ip,
        request.headers['user-agent']
      );

      // Send verification email
      await this.emailService.sendVerificationEmail(result.user.email, result.user.firstName);

      // Log audit event
      await this.authService.logAuditEvent({
        userId: result.user.id,
        tenantId: result.tenant.id,
        action: 'USER_REGISTERED',
        resource: 'user',
        resourceId: result.user.id,
        ipAddress: request.ip,
        userAgent: request.headers['user-agent'],
      });

      const responseData = {
        user: {
          id: result.user.id,
          email: result.user.email,
          firstName: result.user.firstName,
          lastName: result.user.lastName,
          role: result.user.role,
          tenantId: result.user.tenantId,
        },
        tenant: {
          id: result.tenant.id,
          name: result.tenant.name,
          type: result.tenant.type,
        },
        tokens,
      };

      reply.status(201).send({
        success: true,
        data: responseData,
      });
    } catch (error) {
      this.logger.error('Registration failed:', error);
      reply.status(500).send({
        success: false,
        error: {
          code: 'REGISTRATION_FAILED',
          message: 'Failed to register user',
        },
      });
    }
  }

  async login(request: FastifyRequest, reply: FastifyReply) {
    try {
      const { email, password, rememberMe } = request.body as any;

      // Find user with tenant
      const user = await this.prisma.user.findUnique({
        where: { email },
        include: {
          tenant: true,
          profile: true,
        },
      });

      if (!user || !user.isActive) {
        return reply.status(401).send({
          success: false,
          error: {
            code: 'INVALID_CREDENTIALS',
            message: 'Invalid email or password',
          },
        });
      }

      // Verify password
      const isValidPassword = await bcrypt.compare(password, user.password);
      if (!isValidPassword) {
        return reply.status(401).send({
          success: false,
          error: {
            code: 'INVALID_CREDENTIALS',
            message: 'Invalid email or password',
          },
        });
      }

      // Check if tenant is active
      if (!user.tenant.isActive) {
        return reply.status(403).send({
          success: false,
          error: {
            code: 'TENANT_INACTIVE',
            message: 'Account is temporarily suspended',
          },
        });
      }

      // Generate tokens
      const tokens = await this.authService.generateTokens(user, rememberMe);

      // Update last login
      await this.prisma.user.update({
        where: { id: user.id },
        data: { lastLoginAt: new Date() },
      });

      // Create login session
      await this.authService.createLoginSession(
        user.id,
        request.ip,
        request.headers['user-agent']
      );

      // Log audit event
      await this.authService.logAuditEvent({
        userId: user.id,
        tenantId: user.tenantId,
        action: 'USER_LOGGED_IN',
        resource: 'user',
        resourceId: user.id,
        ipAddress: request.ip,
        userAgent: request.headers['user-agent'],
      });

      const responseData = {
        user: {
          id: user.id,
          email: user.email,
          firstName: user.firstName,
          lastName: user.lastName,
          role: user.role,
          tenantId: user.tenantId,
          emailVerified: user.emailVerified,
          profile: user.profile,
        },
        tokens,
      };

      reply.send({
        success: true,
        data: responseData,
      });
    } catch (error) {
      this.logger.error('Login failed:', error);
      reply.status(500).send({
        success: false,
        error: {
          code: 'LOGIN_FAILED',
          message: 'Failed to authenticate user',
        },
      });
    }
  }

  async refresh(request: FastifyRequest, reply: FastifyReply) {
    try {
      const { refreshToken } = request.body as any;

      const tokens = await this.authService.refreshTokens(refreshToken);

      reply.send({
        success: true,
        data: tokens,
      });
    } catch (error) {
      this.logger.error('Token refresh failed:', error);
      reply.status(401).send({
        success: false,
        error: {
          code: 'INVALID_REFRESH_TOKEN',
          message: 'Invalid or expired refresh token',
        },
      });
    }
  }

  async logout(request: FastifyRequest, reply: FastifyReply) {
    try {
      const { refreshToken } = request.body as any;
      const user = (request as any).user;

      if (refreshToken) {
        await this.authService.revokeRefreshToken(refreshToken);
      }

      // End login session
      await this.authService.endLoginSession(user.id);

      // Log audit event
      await this.authService.logAuditEvent({
        userId: user.id,
        tenantId: user.tenantId,
        action: 'USER_LOGGED_OUT',
        resource: 'user',
        resourceId: user.id,
        ipAddress: request.ip,
        userAgent: request.headers['user-agent'],
      });

      reply.send({
        success: true,
        message: 'Logged out successfully',
      });
    } catch (error) {
      this.logger.error('Logout failed:', error);
      reply.status(500).send({
        success: false,
        error: {
          code: 'LOGOUT_FAILED',
          message: 'Failed to logout user',
        },
      });
    }
  }

  async verifyEmail(request: FastifyRequest, reply: FastifyReply) {
    try {
      const { token } = request.body as any;

      await this.authService.verifyEmail(token);

      reply.send({
        success: true,
        message: 'Email verified successfully',
      });
    } catch (error) {
      this.logger.error('Email verification failed:', error);
      reply.status(400).send({
        success: false,
        error: {
          code: 'VERIFICATION_FAILED',
          message: 'Invalid or expired verification token',
        },
      });
    }
  }

  async forgotPassword(request: FastifyRequest, reply: FastifyReply) {
    try {
      const { email } = request.body as any;

      await this.authService.createPasswordReset(email);

      reply.send({
        success: true,
        message: 'Password reset email sent if account exists',
      });
    } catch (error) {
      this.logger.error('Password reset request failed:', error);
      reply.status(500).send({
        success: false,
        error: {
          code: 'PASSWORD_RESET_FAILED',
          message: 'Failed to process password reset request',
        },
      });
    }
  }

  async resetPassword(request: FastifyRequest, reply: FastifyReply) {
    try {
      const { token, password } = request.body as any;

      await this.authService.resetPassword(token, password);

      reply.send({
        success: true,
        message: 'Password reset successfully',
      });
    } catch (error) {
      this.logger.error('Password reset failed:', error);
      reply.status(400).send({
        success: false,
        error: {
          code: 'PASSWORD_RESET_FAILED',
          message: 'Invalid or expired reset token',
        },
      });
    }
  }

  async getProfile(request: FastifyRequest, reply: FastifyReply) {
    try {
      const user = (request as any).user;

      const profile = await this.prisma.user.findUnique({
        where: { id: user.id },
        include: {
          profile: true,
          tenant: {
            select: {
              id: true,
              name: true,
              type: true,
            },
          },
        },
      });

      if (!profile) {
        return reply.status(404).send({
          success: false,
          error: {
            code: 'USER_NOT_FOUND',
            message: 'User not found',
          },
        });
      }

      const { password, ...userWithoutPassword } = profile;

      reply.send({
        success: true,
        data: userWithoutPassword,
      });
    } catch (error) {
      this.logger.error('Get profile failed:', error);
      reply.status(500).send({
        success: false,
        error: {
          code: 'PROFILE_FETCH_FAILED',
          message: 'Failed to fetch user profile',
        },
      });
    }
  }

  async updateProfile(request: FastifyRequest, reply: FastifyReply) {
    try {
      const user = (request as any).user;
      const updateData = request.body as any;

      const result = await this.prisma.$transaction(async (tx) => {
        // Update user basic info
        const updatedUser = await tx.user.update({
          where: { id: user.id },
          data: {
            firstName: updateData.firstName,
            lastName: updateData.lastName,
          },
        });

        // Update or create profile
        const updatedProfile = await tx.userProfile.upsert({
          where: { userId: user.id },
          update: {
            phone: updateData.phone,
            mobile: updateData.mobile,
            street: updateData.street,
            city: updateData.city,
            province: updateData.province,
            postalCode: updateData.postalCode,
            country: updateData.country,
            dateOfBirth: updateData.dateOfBirth ? new Date(updateData.dateOfBirth) : undefined,
            gender: updateData.gender,
            language: updateData.language,
            timezone: updateData.timezone,
          },
          create: {
            id: uuidv4(),
            userId: user.id,
            phone: updateData.phone,
            mobile: updateData.mobile,
            street: updateData.street,
            city: updateData.city,
            province: updateData.province,
            postalCode: updateData.postalCode,
            country: updateData.country || 'South Africa',
            dateOfBirth: updateData.dateOfBirth ? new Date(updateData.dateOfBirth) : undefined,
            gender: updateData.gender,
            language: updateData.language || 'en',
            timezone: updateData.timezone || 'Africa/Johannesburg',
          },
        });

        return { user: updatedUser, profile: updatedProfile };
      });

      // Log audit event
      await this.authService.logAuditEvent({
        userId: user.id,
        tenantId: user.tenantId,
        action: 'USER_PROFILE_UPDATED',
        resource: 'user',
        resourceId: user.id,
        newValues: updateData,
        ipAddress: request.ip,
        userAgent: request.headers['user-agent'],
      });

      reply.send({
        success: true,
        data: result,
      });
    } catch (error) {
      this.logger.error('Update profile failed:', error);
      reply.status(500).send({
        success: false,
        error: {
          code: 'PROFILE_UPDATE_FAILED',
          message: 'Failed to update user profile',
        },
      });
    }
  }

  async changePassword(request: FastifyRequest, reply: FastifyReply) {
    try {
      const user = (request as any).user;
      const { currentPassword, newPassword } = request.body as any;

      // Get user with current password
      const userWithPassword = await this.prisma.user.findUnique({
        where: { id: user.id },
      });

      if (!userWithPassword) {
        return reply.status(404).send({
          success: false,
          error: {
            code: 'USER_NOT_FOUND',
            message: 'User not found',
          },
        });
      }

      // Verify current password
      const isValidPassword = await bcrypt.compare(currentPassword, userWithPassword.password);
      if (!isValidPassword) {
        return reply.status(400).send({
          success: false,
          error: {
            code: 'INVALID_PASSWORD',
            message: 'Current password is incorrect',
          },
        });
      }

      // Hash new password
      const hashedNewPassword = await bcrypt.hash(newPassword, 12);

      // Update password
      await this.prisma.user.update({
        where: { id: user.id },
        data: { password: hashedNewPassword },
      });

      // Revoke all existing refresh tokens
      await this.prisma.refreshToken.updateMany({
        where: { userId: user.id },
        data: { isRevoked: true },
      });

      // Log audit event
      await this.authService.logAuditEvent({
        userId: user.id,
        tenantId: user.tenantId,
        action: 'PASSWORD_CHANGED',
        resource: 'user',
        resourceId: user.id,
        ipAddress: request.ip,
        userAgent: request.headers['user-agent'],
      });

      reply.send({
        success: true,
        message: 'Password changed successfully',
      });
    } catch (error) {
      this.logger.error('Change password failed:', error);
      reply.status(500).send({
        success: false,
        error: {
          code: 'PASSWORD_CHANGE_FAILED',
          message: 'Failed to change password',
        },
      });
    }
  }
}
