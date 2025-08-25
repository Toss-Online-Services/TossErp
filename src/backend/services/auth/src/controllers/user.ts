import { PrismaClient } from '@prisma/client';
import { Redis } from 'ioredis';
import { FastifyBaseLogger } from 'fastify';
import bcrypt from 'bcryptjs';
import { ApiResponse, ApiError } from '../../../shared/types';
import { AppError } from '../../../shared/errors';
import { createAuditLog } from '../utils/audit';

export class UserController {
  constructor(
    private prisma: PrismaClient,
    private redis: Redis,
    private logger: FastifyBaseLogger
  ) {}

  // List users in tenant
  async list(request: any, reply: any): Promise<ApiResponse<any[]>> {
    try {
      const { tenantId, userId } = request.user;
      const { page = 1, limit = 10, search, role, isActive } = request.query;

      // Check if user has permission to list users
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('USER_READ'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      const where: any = { tenantId };
      
      if (search) {
        where.OR = [
          { firstName: { contains: search, mode: 'insensitive' } },
          { lastName: { contains: search, mode: 'insensitive' } },
          { email: { contains: search, mode: 'insensitive' } },
        ];
      }

      if (role) where.role = role;
      if (typeof isActive === 'boolean') where.isActive = isActive;

      const [users, total] = await Promise.all([
        this.prisma.user.findMany({
          where,
          select: {
            id: true,
            email: true,
            firstName: true,
            lastName: true,
            role: true,
            isActive: true,
            createdAt: true,
          },
          skip: (page - 1) * limit,
          take: limit,
          orderBy: { createdAt: 'desc' },
        }),
        this.prisma.user.count({ where }),
      ]);

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'USER_LIST',
        resource: 'User',
        details: { search, role, isActive, count: users.length },
      });

      return {
        success: true,
        data: users,
        meta: {
          total,
          page,
          limit,
          hasNext: page * limit < total,
          hasPrev: page > 1,
        },
      };
    } catch (error) {
      this.logger.error('Error listing users:', error);
      throw new AppError('Failed to list users', 500);
    }
  }

  // Get user by ID
  async get(request: any, reply: any): Promise<ApiResponse<any>> {
    try {
      const { tenantId, userId: currentUserId } = request.user;
      const { id } = request.params;

      // Users can view their own profile or admins can view any user
      const currentUser = await this.prisma.user.findUnique({
        where: { id: currentUserId },
        include: { permissions: true },
      });

      const canViewAnyUser = currentUser?.permissions?.some(p => p.permission.includes('USER_READ'));
      
      if (id !== currentUserId && !canViewAnyUser) {
        throw new AppError('Insufficient permissions', 403);
      }

      const user = await this.prisma.user.findFirst({
        where: { id, tenantId },
        select: {
          id: true,
          email: true,
          firstName: true,
          lastName: true,
          role: true,
          isActive: true,
          profile: true,
          createdAt: true,
          permissions: {
            select: {
              permission: true,
            },
          },
        },
      });

      if (!user) {
        throw new AppError('User not found', 404);
      }

      await createAuditLog(this.prisma, {
        tenantId,
        userId: currentUserId,
        action: 'USER_READ',
        resource: 'User',
        resourceId: id,
        details: { viewedUser: id },
      });

      return {
        success: true,
        data: user,
      };
    } catch (error) {
      this.logger.error('Error getting user:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to get user', 500);
    }
  }

  // Create new user
  async create(request: any, reply: any): Promise<ApiResponse<any>> {
    try {
      const { tenantId, userId } = request.user;
      const { email, firstName, lastName, role, password, sendInvite = true } = request.body;

      // Check if user has permission to create users
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('USER_CREATE'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      // Check if user already exists
      const existingUser = await this.prisma.user.findFirst({
        where: { email, tenantId },
      });

      if (existingUser) {
        throw new AppError('User already exists with this email', 400);
      }

      // Hash password if provided, otherwise generate random password
      const tempPassword = password || Math.random().toString(36).slice(-8);
      const hashedPassword = await bcrypt.hash(tempPassword, 12);

      const newUser = await this.prisma.user.create({
        data: {
          email,
          firstName,
          lastName,
          role,
          hashedPassword,
          tenantId,
          isEmailVerified: !sendInvite, // If not sending invite, mark as verified
        },
        select: {
          id: true,
          email: true,
          firstName: true,
          lastName: true,
          role: true,
        },
      });

      // TODO: Send invitation email if sendInvite is true

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'USER_CREATE',
        resource: 'User',
        resourceId: newUser.id,
        details: { email, role, sendInvite },
      });

      reply.code(201);
      return {
        success: true,
        data: newUser,
      };
    } catch (error) {
      this.logger.error('Error creating user:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to create user', 500);
    }
  }

  // Update user
  async update(request: any, reply: any): Promise<ApiResponse<any>> {
    try {
      const { tenantId, userId: currentUserId } = request.user;
      const { id } = request.params;
      const { firstName, lastName, role, isActive } = request.body;

      // Check permissions - users can update themselves, admins can update anyone
      const currentUser = await this.prisma.user.findUnique({
        where: { id: currentUserId },
        include: { permissions: true },
      });

      const canUpdateAnyUser = currentUser?.permissions?.some(p => p.permission.includes('USER_UPDATE'));
      const isSelfUpdate = id === currentUserId;

      if (!isSelfUpdate && !canUpdateAnyUser) {
        throw new AppError('Insufficient permissions', 403);
      }

      // Prepare update data
      const updateData: any = {};
      if (firstName !== undefined) updateData.firstName = firstName;
      if (lastName !== undefined) updateData.lastName = lastName;
      
      // Only admins can change role and status
      if (canUpdateAnyUser) {
        if (role !== undefined) updateData.role = role;
        if (isActive !== undefined) updateData.isActive = isActive;
      }

      const updatedUser = await this.prisma.user.update({
        where: { id, tenantId },
        data: updateData,
        select: {
          id: true,
          email: true,
          firstName: true,
          lastName: true,
          role: true,
          isActive: true,
        },
      });

      await createAuditLog(this.prisma, {
        tenantId,
        userId: currentUserId,
        action: 'USER_UPDATE',
        resource: 'User',
        resourceId: id,
        details: updateData,
      });

      return {
        success: true,
        data: updatedUser,
      };
    } catch (error) {
      this.logger.error('Error updating user:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to update user', 500);
    }
  }

  // Delete user
  async delete(request: any, reply: any): Promise<ApiResponse<string>> {
    try {
      const { tenantId, userId } = request.user;
      const { id } = request.params;

      // Check if user has permission to delete users
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('USER_DELETE'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      // Prevent self-deletion
      if (id === userId) {
        throw new AppError('Cannot delete your own account', 400);
      }

      // Check if user exists
      const userToDelete = await this.prisma.user.findFirst({
        where: { id, tenantId },
      });

      if (!userToDelete) {
        throw new AppError('User not found', 404);
      }

      // Soft delete by deactivating instead of hard delete
      await this.prisma.user.update({
        where: { id },
        data: { 
          isActive: false,
          deletedAt: new Date(),
        },
      });

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'USER_DELETE',
        resource: 'User',
        resourceId: id,
        details: { deletedUser: userToDelete.email },
      });

      return {
        success: true,
        message: 'User deleted successfully',
      };
    } catch (error) {
      this.logger.error('Error deleting user:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to delete user', 500);
    }
  }

  // Update user status
  async updateStatus(request: any, reply: any): Promise<ApiResponse<string>> {
    try {
      const { tenantId, userId } = request.user;
      const { id } = request.params;
      const { isActive } = request.body;

      // Check permissions
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('USER_UPDATE'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      await this.prisma.user.update({
        where: { id, tenantId },
        data: { isActive },
      });

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'USER_STATUS_UPDATE',
        resource: 'User',
        resourceId: id,
        details: { isActive },
      });

      return {
        success: true,
        message: `User ${isActive ? 'activated' : 'deactivated'} successfully`,
      };
    } catch (error) {
      this.logger.error('Error updating user status:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to update user status', 500);
    }
  }

  // Reset user password
  async resetPassword(request: any, reply: any): Promise<ApiResponse<string>> {
    try {
      const { tenantId, userId } = request.user;
      const { id } = request.params;
      const { sendEmail = true } = request.body;

      // Check permissions
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('USER_UPDATE'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      // Generate temporary password
      const tempPassword = Math.random().toString(36).slice(-8);
      const hashedPassword = await bcrypt.hash(tempPassword, 12);

      await this.prisma.user.update({
        where: { id, tenantId },
        data: { 
          hashedPassword,
          mustChangePassword: true,
        },
      });

      // TODO: Send password reset email if sendEmail is true

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'USER_PASSWORD_RESET',
        resource: 'User',
        resourceId: id,
        details: { sendEmail },
      });

      return {
        success: true,
        message: 'Password reset successfully',
      };
    } catch (error) {
      this.logger.error('Error resetting password:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to reset password', 500);
    }
  }
}
