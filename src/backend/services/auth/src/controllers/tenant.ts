import { PrismaClient } from '@prisma/client';
import { Redis } from 'ioredis';
import { FastifyBaseLogger } from 'fastify';
import { ApiResponse } from '../../../shared/types';
import { AppError } from '../../../shared/errors';
import { createAuditLog } from '../utils/audit';

export class TenantController {
  constructor(
    private prisma: PrismaClient,
    private redis: Redis,
    private logger: FastifyBaseLogger
  ) {}

  // Get current tenant details
  async get(request: any, reply: any): Promise<ApiResponse<any>> {
    try {
      const { tenantId, userId } = request.user;

      const tenant = await this.prisma.tenant.findUnique({
        where: { id: tenantId },
        select: {
          id: true,
          name: true,
          type: true,
          subdomain: true,
          settings: true,
          isActive: true,
          createdAt: true,
        },
      });

      if (!tenant) {
        throw new AppError('Tenant not found', 404);
      }

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'TENANT_READ',
        resource: 'Tenant',
        resourceId: tenantId,
      });

      return {
        success: true,
        data: tenant,
      };
    } catch (error) {
      this.logger.error('Error getting tenant:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to get tenant', 500);
    }
  }

  // Update tenant settings
  async update(request: any, reply: any): Promise<ApiResponse<any>> {
    try {
      const { tenantId, userId } = request.user;
      const { name, settings } = request.body;

      // Check if user has permission to update tenant
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('TENANT_UPDATE'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      const updateData: any = {};
      if (name !== undefined) updateData.name = name;
      if (settings !== undefined) updateData.settings = settings;

      const updatedTenant = await this.prisma.tenant.update({
        where: { id: tenantId },
        data: updateData,
        select: {
          id: true,
          name: true,
          type: true,
          subdomain: true,
          settings: true,
          isActive: true,
          createdAt: true,
        },
      });

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'TENANT_UPDATE',
        resource: 'Tenant',
        resourceId: tenantId,
        details: updateData,
      });

      return {
        success: true,
        data: updatedTenant,
      };
    } catch (error) {
      this.logger.error('Error updating tenant:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to update tenant', 500);
    }
  }

  // Get tenant statistics
  async getStats(request: any, reply: any): Promise<ApiResponse<any>> {
    try {
      const { tenantId, userId } = request.user;

      // Check permissions
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('TENANT_READ'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      const [
        totalUsers,
        activeUsers,
        totalRoles,
        totalSessions,
        recentActivity
      ] = await Promise.all([
        this.prisma.user.count({ where: { tenantId } }),
        this.prisma.user.count({ where: { tenantId, isActive: true } }),
        this.prisma.permission.count({ where: { tenantId } }),
        this.prisma.refreshToken.count({ 
          where: { 
            user: { tenantId },
            expiresAt: { gt: new Date() }
          }
        }),
        this.prisma.auditLog.findFirst({
          where: { tenantId },
          orderBy: { createdAt: 'desc' },
          select: { createdAt: true },
        }),
      ]);

      // Get storage and API usage from cache or calculate
      const storageUsed = await this.getStorageUsage(tenantId);
      const apiCalls = await this.getApiUsage(tenantId);

      const stats = {
        totalUsers,
        activeUsers,
        totalRoles,
        totalSessions,
        storageUsed,
        apiCalls,
        lastActivity: recentActivity?.createdAt?.toISOString() || null,
      };

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'TENANT_STATS_READ',
        resource: 'Tenant',
        resourceId: tenantId,
      });

      return {
        success: true,
        data: stats,
      };
    } catch (error) {
      this.logger.error('Error getting tenant stats:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to get tenant statistics', 500);
    }
  }

  // Get billing information
  async getBilling(request: any, reply: any): Promise<ApiResponse<any>> {
    try {
      const { tenantId, userId } = request.user;

      // Check permissions
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('TENANT_BILLING'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      // Get tenant with billing information
      const tenant = await this.prisma.tenant.findUnique({
        where: { id: tenantId },
        select: {
          settings: true,
        },
      });

      // Calculate current usage
      const usage = await this.calculateUsage(tenantId);
      const limits = this.getPlanLimits(tenant?.settings?.plan || 'basic');

      const billingInfo = {
        plan: tenant?.settings?.plan || 'basic',
        billingCycle: tenant?.settings?.billingCycle || 'monthly',
        nextBillingDate: tenant?.settings?.nextBillingDate || null,
        usage,
        limits,
      };

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'TENANT_BILLING_READ',
        resource: 'Tenant',
        resourceId: tenantId,
      });

      return {
        success: true,
        data: billingInfo,
      };
    } catch (error) {
      this.logger.error('Error getting billing info:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to get billing information', 500);
    }
  }

  // Get audit logs
  async getAuditLogs(request: any, reply: any): Promise<ApiResponse<any[]>> {
    try {
      const { tenantId, userId } = request.user;
      const { 
        page = 1, 
        limit = 20, 
        userId: filterUserId, 
        action, 
        startDate, 
        endDate 
      } = request.query;

      // Check permissions
      const currentUser = await this.prisma.user.findUnique({
        where: { id: userId },
        include: { permissions: true },
      });

      if (!currentUser?.permissions?.some(p => p.permission.includes('AUDIT_READ'))) {
        throw new AppError('Insufficient permissions', 403);
      }

      const where: any = { tenantId };
      
      if (filterUserId) where.userId = filterUserId;
      if (action) where.action = action;
      if (startDate || endDate) {
        where.createdAt = {};
        if (startDate) where.createdAt.gte = new Date(startDate);
        if (endDate) where.createdAt.lte = new Date(endDate);
      }

      const [logs, total] = await Promise.all([
        this.prisma.auditLog.findMany({
          where,
          include: {
            user: {
              select: {
                firstName: true,
                lastName: true,
                email: true,
              },
            },
          },
          skip: (page - 1) * limit,
          take: limit,
          orderBy: { createdAt: 'desc' },
        }),
        this.prisma.auditLog.count({ where }),
      ]);

      await createAuditLog(this.prisma, {
        tenantId,
        userId,
        action: 'AUDIT_LOGS_READ',
        resource: 'AuditLog',
        details: { filters: { filterUserId, action, startDate, endDate } },
      });

      return {
        success: true,
        data: logs,
        meta: {
          total,
          page,
          limit,
          hasNext: page * limit < total,
          hasPrev: page > 1,
        },
      };
    } catch (error) {
      this.logger.error('Error getting audit logs:', error);
      if (error instanceof AppError) throw error;
      throw new AppError('Failed to get audit logs', 500);
    }
  }

  // Helper methods
  private async getStorageUsage(tenantId: string): Promise<number> {
    // This would typically check file storage, database size, etc.
    // For now, return a placeholder value
    const cacheKey = `storage:${tenantId}`;
    const cached = await this.redis.get(cacheKey);
    
    if (cached) {
      return parseInt(cached);
    }

    // Calculate actual storage usage here
    const usage = 0; // Placeholder
    
    // Cache for 1 hour
    await this.redis.setex(cacheKey, 3600, usage.toString());
    
    return usage;
  }

  private async getApiUsage(tenantId: string): Promise<number> {
    // Get API call count for current billing period
    const cacheKey = `api_usage:${tenantId}`;
    const cached = await this.redis.get(cacheKey);
    
    if (cached) {
      return parseInt(cached);
    }

    // Calculate actual API usage from audit logs or metrics
    const usage = 0; // Placeholder
    
    // Cache for 1 hour
    await this.redis.setex(cacheKey, 3600, usage.toString());
    
    return usage;
  }

  private async calculateUsage(tenantId: string): Promise<any> {
    // Calculate current usage metrics
    return {
      users: await this.prisma.user.count({ where: { tenantId } }),
      storage: await this.getStorageUsage(tenantId),
      apiCalls: await this.getApiUsage(tenantId),
      // Add more usage metrics as needed
    };
  }

  private getPlanLimits(plan: string): any {
    // Define plan limits
    const limits = {
      basic: {
        users: 10,
        storage: 1024 * 1024 * 1024, // 1GB
        apiCalls: 10000,
      },
      pro: {
        users: 100,
        storage: 10 * 1024 * 1024 * 1024, // 10GB
        apiCalls: 100000,
      },
      enterprise: {
        users: -1, // Unlimited
        storage: -1, // Unlimited
        apiCalls: -1, // Unlimited
      },
    };

    return limits[plan] || limits.basic;
  }
}
