import { computed } from 'vue'
import { Role, Permission, RolePermissions, type PermissionCheck } from '~/types/permissions'

export const usePermissions = () => {
  const { user } = useAuth()

  const userRoles = computed<Role[]>(() => {
    if (!user.value?.roles) return []
    return user.value.roles as Role[]
  })

  const userPermissions = computed<Permission[]>(() => {
    if (!user.value?.permissions) return []
    return user.value.permissions as Permission[]
  })

  const allPermissions = computed<Permission[]>(() => {
    const rolePerms = new Set<Permission>()
    
    // Add permissions from roles
    userRoles.value.forEach(role => {
      const perms = RolePermissions[role] || []
      perms.forEach(p => rolePerms.add(p))
    })
    
    // Add custom permissions
    userPermissions.value.forEach(p => rolePerms.add(p))
    
    return Array.from(rolePerms)
  })

  const hasPermission = (permission: Permission): boolean => {
    // Super admin has all permissions
    if (userRoles.value.includes(Role.SUPER_ADMIN)) {
      return true
    }
    
    return allPermissions.value.includes(permission)
  }

  const hasAnyPermission = (permissions: Permission[]): boolean => {
    return permissions.some(p => hasPermission(p))
  }

  const hasAllPermissions = (permissions: Permission[]): boolean => {
    return permissions.every(p => hasPermission(p))
  }

  const hasRole = (role: Role): boolean => {
    return userRoles.value.includes(role)
  }

  const hasAnyRole = (roles: Role[]): boolean => {
    return roles.some(r => hasRole(r))
  }

  const checkPermission = (permission: Permission): PermissionCheck => {
    const granted = hasPermission(permission)
    
    return {
      permission,
      granted,
      reason: granted ? undefined : 'Insufficient permissions',
    }
  }

  const canView = (module: string): boolean => {
    const viewPermission = `${module}:view` as Permission
    return hasPermission(viewPermission)
  }

  const canCreate = (module: string): boolean => {
    const createPermission = `${module}:create` as Permission
    return hasPermission(createPermission)
  }

  const canEdit = (module: string): boolean => {
    const editPermission = `${module}:edit` as Permission
    return hasPermission(editPermission)
  }

  const canDelete = (module: string): boolean => {
    const deletePermission = `${module}:delete` as Permission
    return hasPermission(deletePermission)
  }

  const canApprove = (module: string): boolean => {
    const approvePermission = `${module}:approve` as Permission
    return hasPermission(approvePermission)
  }

  const isAdmin = computed(() => {
    return hasAnyRole([Role.SUPER_ADMIN, Role.ADMIN])
  })

  const isSuperAdmin = computed(() => {
    return hasRole(Role.SUPER_ADMIN)
  })

  const getModulePermissions = (module: string) => {
    return {
      canView: canView(module),
      canCreate: canCreate(module),
      canEdit: canEdit(module),
      canDelete: canDelete(module),
      canApprove: canApprove(module),
    }
  }

  return {
    userRoles,
    userPermissions,
    allPermissions,
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
    checkPermission,
    canView,
    canCreate,
    canEdit,
    canDelete,
    canApprove,
    isAdmin,
    isSuperAdmin,
    getModulePermissions,
  }
}

