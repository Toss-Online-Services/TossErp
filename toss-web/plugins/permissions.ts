import { Permission } from '~/types/permissions'

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.directive('permission', {
    mounted(el, binding) {
      const { hasPermission } = usePermissions()
      const permission = binding.value as Permission
      
      if (!hasPermission(permission)) {
        el.style.display = 'none'
      }
    },
    updated(el, binding) {
      const { hasPermission } = usePermissions()
      const permission = binding.value as Permission
      
      if (!hasPermission(permission)) {
        el.style.display = 'none'
      } else {
        el.style.display = ''
      }
    },
  })

  nuxtApp.vueApp.directive('role', {
    mounted(el, binding) {
      const { hasRole } = usePermissions()
      const role = binding.value
      
      if (!hasRole(role)) {
        el.style.display = 'none'
      }
    },
    updated(el, binding) {
      const { hasRole } = usePermissions()
      const role = binding.value
      
      if (!hasRole(role)) {
        el.style.display = 'none'
      } else {
        el.style.display = ''
      }
    },
  })
})

