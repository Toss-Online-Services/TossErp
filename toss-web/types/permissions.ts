// RBAC Permission System for TOSS ERP III

export enum Role {
  SUPER_ADMIN = 'super_admin',
  ADMIN = 'admin',
  MANAGER = 'manager',
  EMPLOYEE = 'employee',
  ACCOUNTANT = 'accountant',
  SALES_REP = 'sales_rep',
  WAREHOUSE_STAFF = 'warehouse_staff',
  VIEWER = 'viewer',
}

export enum Permission {
  // Dashboard permissions
  DASHBOARD_VIEW = 'dashboard:view',
  DASHBOARD_EDIT = 'dashboard:edit',
  
  // User management
  USERS_VIEW = 'users:view',
  USERS_CREATE = 'users:create',
  USERS_EDIT = 'users:edit',
  USERS_DELETE = 'users:delete',
  
  // Sales permissions
  SALES_VIEW = 'sales:view',
  SALES_CREATE = 'sales:create',
  SALES_EDIT = 'sales:edit',
  SALES_DELETE = 'sales:delete',
  SALES_APPROVE = 'sales:approve',
  
  // Inventory permissions
  INVENTORY_VIEW = 'inventory:view',
  INVENTORY_CREATE = 'inventory:create',
  INVENTORY_EDIT = 'inventory:edit',
  INVENTORY_DELETE = 'inventory:delete',
  INVENTORY_ADJUST = 'inventory:adjust',
  
  // Manufacturing permissions
  MANUFACTURING_VIEW = 'manufacturing:view',
  MANUFACTURING_CREATE = 'manufacturing:create',
  MANUFACTURING_EDIT = 'manufacturing:edit',
  MANUFACTURING_DELETE = 'manufacturing:delete',
  MANUFACTURING_APPROVE = 'manufacturing:approve',
  
  // Accounting permissions
  ACCOUNTING_VIEW = 'accounting:view',
  ACCOUNTING_CREATE = 'accounting:create',
  ACCOUNTING_EDIT = 'accounting:edit',
  ACCOUNTING_DELETE = 'accounting:delete',
  ACCOUNTING_APPROVE = 'accounting:approve',
  
  // HR permissions
  HR_VIEW = 'hr:view',
  HR_CREATE = 'hr:create',
  HR_EDIT = 'hr:edit',
  HR_DELETE = 'hr:delete',
  
  // CRM permissions
  CRM_VIEW = 'crm:view',
  CRM_CREATE = 'crm:create',
  CRM_EDIT = 'crm:edit',
  CRM_DELETE = 'crm:delete',
  
  // Reports permissions
  REPORTS_VIEW = 'reports:view',
  REPORTS_EXPORT = 'reports:export',
  REPORTS_SCHEDULE = 'reports:schedule',
  
  // Settings permissions
  SETTINGS_VIEW = 'settings:view',
  SETTINGS_EDIT = 'settings:edit',
  SETTINGS_SYSTEM = 'settings:system',
}

export const RolePermissions: Record<Role, Permission[]> = {
  [Role.SUPER_ADMIN]: Object.values(Permission), // All permissions
  
  [Role.ADMIN]: [
    Permission.DASHBOARD_VIEW,
    Permission.DASHBOARD_EDIT,
    Permission.USERS_VIEW,
    Permission.USERS_CREATE,
    Permission.USERS_EDIT,
    Permission.SALES_VIEW,
    Permission.SALES_CREATE,
    Permission.SALES_EDIT,
    Permission.SALES_APPROVE,
    Permission.INVENTORY_VIEW,
    Permission.INVENTORY_CREATE,
    Permission.INVENTORY_EDIT,
    Permission.INVENTORY_ADJUST,
    Permission.MANUFACTURING_VIEW,
    Permission.MANUFACTURING_CREATE,
    Permission.MANUFACTURING_EDIT,
    Permission.MANUFACTURING_APPROVE,
    Permission.ACCOUNTING_VIEW,
    Permission.ACCOUNTING_CREATE,
    Permission.ACCOUNTING_EDIT,
    Permission.HR_VIEW,
    Permission.HR_CREATE,
    Permission.HR_EDIT,
    Permission.CRM_VIEW,
    Permission.CRM_CREATE,
    Permission.CRM_EDIT,
    Permission.REPORTS_VIEW,
    Permission.REPORTS_EXPORT,
    Permission.REPORTS_SCHEDULE,
    Permission.SETTINGS_VIEW,
    Permission.SETTINGS_EDIT,
  ],
  
  [Role.MANAGER]: [
    Permission.DASHBOARD_VIEW,
    Permission.USERS_VIEW,
    Permission.SALES_VIEW,
    Permission.SALES_CREATE,
    Permission.SALES_EDIT,
    Permission.SALES_APPROVE,
    Permission.INVENTORY_VIEW,
    Permission.INVENTORY_CREATE,
    Permission.INVENTORY_EDIT,
    Permission.MANUFACTURING_VIEW,
    Permission.MANUFACTURING_CREATE,
    Permission.MANUFACTURING_EDIT,
    Permission.ACCOUNTING_VIEW,
    Permission.HR_VIEW,
    Permission.HR_CREATE,
    Permission.HR_EDIT,
    Permission.CRM_VIEW,
    Permission.CRM_CREATE,
    Permission.CRM_EDIT,
    Permission.REPORTS_VIEW,
    Permission.REPORTS_EXPORT,
    Permission.SETTINGS_VIEW,
  ],
  
  [Role.EMPLOYEE]: [
    Permission.DASHBOARD_VIEW,
    Permission.SALES_VIEW,
    Permission.SALES_CREATE,
    Permission.INVENTORY_VIEW,
    Permission.MANUFACTURING_VIEW,
    Permission.CRM_VIEW,
    Permission.CRM_CREATE,
    Permission.REPORTS_VIEW,
  ],
  
  [Role.ACCOUNTANT]: [
    Permission.DASHBOARD_VIEW,
    Permission.ACCOUNTING_VIEW,
    Permission.ACCOUNTING_CREATE,
    Permission.ACCOUNTING_EDIT,
    Permission.ACCOUNTING_APPROVE,
    Permission.REPORTS_VIEW,
    Permission.REPORTS_EXPORT,
    Permission.REPORTS_SCHEDULE,
  ],
  
  [Role.SALES_REP]: [
    Permission.DASHBOARD_VIEW,
    Permission.SALES_VIEW,
    Permission.SALES_CREATE,
    Permission.SALES_EDIT,
    Permission.INVENTORY_VIEW,
    Permission.CRM_VIEW,
    Permission.CRM_CREATE,
    Permission.CRM_EDIT,
    Permission.REPORTS_VIEW,
  ],
  
  [Role.WAREHOUSE_STAFF]: [
    Permission.DASHBOARD_VIEW,
    Permission.INVENTORY_VIEW,
    Permission.INVENTORY_CREATE,
    Permission.INVENTORY_EDIT,
    Permission.INVENTORY_ADJUST,
    Permission.MANUFACTURING_VIEW,
    Permission.REPORTS_VIEW,
  ],
  
  [Role.VIEWER]: [
    Permission.DASHBOARD_VIEW,
    Permission.SALES_VIEW,
    Permission.INVENTORY_VIEW,
    Permission.MANUFACTURING_VIEW,
    Permission.ACCOUNTING_VIEW,
    Permission.HR_VIEW,
    Permission.CRM_VIEW,
    Permission.REPORTS_VIEW,
  ],
}

export interface PermissionCheck {
  permission: Permission
  granted: boolean
  reason?: string
}

export interface RoleAssignment {
  userId: string
  role: Role
  permissions: Permission[]
  customPermissions?: Permission[]
  assignedAt: Date
  assignedBy: string
}

