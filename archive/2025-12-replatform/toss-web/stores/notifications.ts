export interface Notification {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message: string
  duration?: number
  persistent?: boolean
  actions?: NotificationAction[]
  timestamp: Date
}

export interface NotificationAction {
  label: string
  action: () => void
  style?: 'primary' | 'secondary'
}

export const useNotificationStore = defineStore('notifications', () => {
  // State
  const notifications = ref<Notification[]>([])
  const maxNotifications = ref(5)

  // Getters
  const visibleNotifications = computed(() => 
    notifications.value.slice(0, maxNotifications.value)
  )

  const unreadCount = computed(() => 
    notifications.value.length
  )

  // Actions
  function add(notification: Omit<Notification, 'id' | 'timestamp'>) {
    const newNotification: Notification = {
      id: Date.now().toString() + Math.random().toString(36).substr(2, 9),
      timestamp: new Date(),
      duration: notification.duration ?? (notification.type === 'error' ? 0 : 5000),
      ...notification
    }

    notifications.value.unshift(newNotification)

    // Auto-remove if not persistent and has duration
    if (!notification.persistent && newNotification.duration > 0) {
      setTimeout(() => {
        remove(newNotification.id)
      }, newNotification.duration)
    }

    // Keep only recent notifications to prevent memory issues
    if (notifications.value.length > 50) {
      notifications.value = notifications.value.slice(0, 50)
    }

    return newNotification.id
  }

  function remove(id: string) {
    const index = notifications.value.findIndex(n => n.id === id)
    if (index > -1) {
      notifications.value.splice(index, 1)
    }
  }

  function clear() {
    notifications.value = []
  }

  function success(title: string, message?: string, options?: Partial<Notification>) {
    return add({
      type: 'success',
      title,
      message: message || '',
      ...options
    })
  }

  function error(title: string, message?: string, options?: Partial<Notification>) {
    return add({
      type: 'error',
      title,
      message: message || '',
      persistent: true, // Errors are persistent by default
      ...options
    })
  }

  function warning(title: string, message?: string, options?: Partial<Notification>) {
    return add({
      type: 'warning',
      title,
      message: message || '',
      ...options
    })
  }

  function info(title: string, message?: string, options?: Partial<Notification>) {
    return add({
      type: 'info',
      title,
      message: message || '',
      ...options
    })
  }

  // Business-specific notification helpers
  function inventoryAlert(item: string, quantity: number, threshold: number) {
    return warning(
      'Low Stock Alert',
      `${item} is running low (${quantity} remaining, threshold: ${threshold})`,
      {
        actions: [
          {
            label: 'Reorder',
            action: () => {
              // Navigate to reorder page
              navigateTo(`/inventory/reorder?item=${encodeURIComponent(item)}`)
            },
            style: 'primary'
          },
          {
            label: 'Update Threshold',
            action: () => {
              navigateTo(`/inventory/settings?item=${encodeURIComponent(item)}`)
            },
            style: 'secondary'
          }
        ]
      }
    )
  }

  function groupBuyingOpportunity(deal: string, savings: string) {
    return info(
      'Group Buying Opportunity',
      `Join "${deal}" and save ${savings}`,
      {
        actions: [
          {
            label: 'Join Now',
            action: () => {
              navigateTo('/group-buying')
            },
            style: 'primary'
          }
        ]
      }
    )
  }

  function paymentReminder(customer: string, amount: string, daysOverdue: number) {
    return warning(
      'Payment Overdue',
      `${customer} owes ${amount} (${daysOverdue} days overdue)`,
      {
        actions: [
          {
            label: 'Send Reminder',
            action: () => {
              // Send payment reminder
            },
            style: 'primary'
          },
          {
            label: 'View Details',
            action: () => {
              navigateTo(`/accounts/receivables?customer=${encodeURIComponent(customer)}`)
            },
            style: 'secondary'
          }
        ]
      }
    )
  }

  function salesMilestone(milestone: string, amount: string) {
    return success(
      'Sales Milestone Reached!',
      `Congratulations! You've reached ${milestone} with ${amount} in sales`,
      {
        duration: 8000,
        actions: [
          {
            label: 'View Report',
            action: () => {
              navigateTo('/reports/sales')
            },
            style: 'primary'
          }
        ]
      }
    )
  }

  return {
    // State
    notifications: readonly(notifications),
    maxNotifications: readonly(maxNotifications),
    
    // Getters
    visibleNotifications,
    unreadCount,
    
    // Actions
    add,
    remove,
    clear,
    success,
    error,
    warning,
    info,
    
    // Business-specific helpers
    inventoryAlert,
    groupBuyingOpportunity,
    paymentReminder,
    salesMilestone
  }
})
