import { PrismaClient } from '@prisma/client';

export interface AuditLogData {
  tenantId: string;
  userId: string;
  action: string;
  resource: string;
  resourceId?: string;
  details?: any;
  ipAddress?: string;
  userAgent?: string;
}

export async function createAuditLog(
  prisma: PrismaClient,
  data: AuditLogData
): Promise<void> {
  try {
    await prisma.auditLog.create({
      data: {
        tenantId: data.tenantId,
        userId: data.userId,
        action: data.action,
        resource: data.resource,
        resourceId: data.resourceId,
        details: data.details || {},
        ipAddress: data.ipAddress,
        userAgent: data.userAgent,
        createdAt: new Date(),
      },
    });
  } catch (error) {
    // Log error but don't throw to avoid breaking main functionality
    console.error('Failed to create audit log:', error);
  }
}

export function extractClientInfo(request: any): { ipAddress?: string; userAgent?: string } {
  return {
    ipAddress: request.ip || request.headers['x-forwarded-for'] || request.connection?.remoteAddress,
    userAgent: request.headers['user-agent'],
  };
}
