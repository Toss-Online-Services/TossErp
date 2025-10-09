import { v4 as uuidv4 } from 'uuid'
import type { AuditLog, AuditAction, AuditSeverity } from '~/types/audit'

export const useAudit = () => {
  const { user } = useAuth()
  const config = useRuntimeConfig()

  const logAuditEvent = async (
    action: AuditAction,
    severity: AuditSeverity,
    details?: {
      resource?: string
      resourceId?: string
      success?: boolean
      errorMessage?: string
      metadata?: Record<string, any>
    }
  ): Promise<void> => {
    try {
      const auditLog: AuditLog = {
        id: uuidv4(),
        timestamp: new Date(),
        action,
        severity,
        userId: user.value?.id?.toString(),
        userEmail: user.value?.email,
        ipAddress: await getClientIP(),
        userAgent: navigator.userAgent,
        resource: details?.resource,
        resourceId: details?.resourceId,
        details: details?.metadata,
        success: details?.success ?? true,
        errorMessage: details?.errorMessage,
      }

      // Send to backend
      await $fetch('/api/audit/log', {
        method: 'POST',
        body: auditLog,
      })

      // Also log to console in development
      if (process.dev) {
        console.log('[AUDIT]', auditLog)
      }
    } catch (error) {
      console.error('Failed to log audit event:', error)
    }
  }

  const getClientIP = async (): Promise<string> => {
    try {
      const response = await fetch('https://api.ipify.org?format=json')
      const data = await response.json()
      return data.ip
    } catch {
      return 'unknown'
    }
  }

  const logLogin = (success: boolean, errorMessage?: string) => {
    return logAuditEvent(
      success ? 'auth.login' as AuditAction : 'auth.login_failed' as AuditAction,
      success ? 'info' as AuditSeverity : 'warning' as AuditSeverity,
      { success, errorMessage }
    )
  }

  const logLogout = () => {
    return logAuditEvent('auth.logout' as AuditAction, 'info' as AuditSeverity, { success: true })
  }

  const logTokenRefresh = (success: boolean) => {
    return logAuditEvent(
      'auth.token_refresh' as AuditAction,
      success ? 'info' as AuditSeverity : 'warning' as AuditSeverity,
      { success }
    )
  }

  const logDataAccess = (resource: string, resourceId: string, action: 'create' | 'read' | 'update' | 'delete') => {
    const auditAction = `data.${action}` as AuditAction
    return logAuditEvent(auditAction, 'info' as AuditSeverity, {
      resource,
      resourceId,
      success: true,
    })
  }

  const logAccessDenied = (resource: string, reason: string) => {
    return logAuditEvent('permission.access_denied' as AuditAction, 'warning' as AuditSeverity, {
      resource,
      success: false,
      errorMessage: reason,
    })
  }

  const logSecurityAlert = (message: string, metadata?: Record<string, any>) => {
    return logAuditEvent('system.security_alert' as AuditAction, 'critical' as AuditSeverity, {
      success: false,
      errorMessage: message,
      metadata,
    })
  }

  return {
    logAuditEvent,
    logLogin,
    logLogout,
    logTokenRefresh,
    logDataAccess,
    logAccessDenied,
    logSecurityAlert,
  }
}

