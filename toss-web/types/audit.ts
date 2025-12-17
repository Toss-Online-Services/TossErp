// Security Audit Logging Types

export enum AuditAction {
  // Authentication actions
  LOGIN = 'auth.login',
  LOGOUT = 'auth.logout',
  LOGIN_FAILED = 'auth.login_failed',
  TOKEN_REFRESH = 'auth.token_refresh',
  PASSWORD_CHANGE = 'auth.password_change',
  PASSWORD_RESET = 'auth.password_reset',
  
  // User actions
  USER_CREATE = 'user.create',
  USER_UPDATE = 'user.update',
  USER_DELETE = 'user.delete',
  USER_ROLE_CHANGE = 'user.role_change',
  
  // Data actions
  DATA_CREATE = 'data.create',
  DATA_READ = 'data.read',
  DATA_UPDATE = 'data.update',
  DATA_DELETE = 'data.delete',
  DATA_EXPORT = 'data.export',
  
  // Permission actions
  PERMISSION_GRANT = 'permission.grant',
  PERMISSION_REVOKE = 'permission.revoke',
  ACCESS_DENIED = 'permission.access_denied',
  
  // System actions
  SETTINGS_CHANGE = 'system.settings_change',
  SYSTEM_ERROR = 'system.error',
  SECURITY_ALERT = 'system.security_alert',
}

export enum AuditSeverity {
  INFO = 'info',
  WARNING = 'warning',
  ERROR = 'error',
  CRITICAL = 'critical',
}

export interface AuditLog {
  id: string
  timestamp: Date
  action: AuditAction
  severity: AuditSeverity
  userId?: string
  userEmail?: string
  ipAddress?: string
  userAgent?: string
  resource?: string
  resourceId?: string
  details?: Record<string, any>
  success: boolean
  errorMessage?: string
}

export interface AuditLogFilter {
  startDate?: Date
  endDate?: Date
  userId?: string
  action?: AuditAction
  severity?: AuditSeverity
  resource?: string
  success?: boolean
}

export interface SessionInfo {
  sessionId: string
  userId: string
  createdAt: Date
  lastActivity: Date
  expiresAt: Date
  ipAddress: string
  userAgent: string
  isActive: boolean
}

