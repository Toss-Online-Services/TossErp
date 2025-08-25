<template>
  <div class="fixed top-4 right-4 z-50 space-y-4">
    <TransitionGroup name="notification" tag="div" class="space-y-4">
      <div 
        v-for="notification in notifications" 
        :key="notification.id"
        class="bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 p-4 max-w-sm"
        :class="getNotificationClasses(notification.type)"
      >
        <div class="flex items-start space-x-3">
          <div class="flex-shrink-0">
            <svg :class="getIconClasses(notification.type)" class="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
              <path v-if="notification.type === 'success'" fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clip-rule="evenodd" />
              <path v-else-if="notification.type === 'warning'" fill-rule="evenodd" d="M8.485 3.495c.673-1.167 2.357-1.167 3.03 0l6.28 10.875c.673 1.167-.17 2.625-1.516 2.625H3.72c-1.347 0-2.189-1.458-1.515-2.625L8.485 3.495zM10 6a.75.75 0 01.75.75v3.5a.75.75 0 01-1.5 0v-3.5A.75.75 0 0110 6zm0 9a1 1 0 100-2 1 1 0 000 2z" clip-rule="evenodd" />
              <path v-else-if="notification.type === 'error'" fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.28 7.22a.75.75 0 00-1.06 1.06L8.94 10l-1.72 1.72a.75.75 0 101.06 1.06L10 11.06l1.72 1.72a.75.75 0 101.06-1.06L11.06 10l1.72-1.72a.75.75 0 00-1.06-1.06L10 8.94 8.28 7.22z" clip-rule="evenodd" />
              <path v-else fill-rule="evenodd" d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a.75.75 0 000 1.5h.253a.25.25 0 01.244.304l-.459 2.066A1.75 1.75 0 0010.747 15H11a.75.75 0 000-1.5h-.253a.25.25 0 01-.244-.304l.459-2.066A1.75 1.75 0 009.253 9H9z" clip-rule="evenodd" />
            </svg>
          </div>
          <div class="flex-1">
            <h4 class="text-sm font-medium text-gray-900 dark:text-white">{{ notification.title }}</h4>
            <p class="text-sm text-gray-600 dark:text-gray-400 mt-1">{{ notification.message }}</p>
            <div v-if="notification.actions" class="mt-3 flex space-x-2">
              <button 
                v-for="action in notification.actions" 
                :key="action.label"
                @click="handleAction(action, notification.id)"
                :class="action.primary ? 'bg-blue-600 text-white hover:bg-blue-700' : 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'"
                class="px-3 py-1 text-xs rounded-lg transition-colors"
              >
                {{ action.label }}
              </button>
            </div>
          </div>
          <button @click="removeNotification(notification.id)" class="flex-shrink-0 text-gray-400 hover:text-gray-600 dark:hover:text-gray-300">
            <svg class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
              <path d="M6.28 5.22a.75.75 0 00-1.06 1.06L8.94 10l-3.72 3.72a.75.75 0 101.06 1.06L10 11.06l3.72 3.72a.75.75 0 101.06-1.06L11.06 10l3.72-3.72a.75.75 0 00-1.06-1.06L10 8.94 6.28 5.22z" />
            </svg>
          </button>
        </div>
      </div>
    </TransitionGroup>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

interface NotificationAction {
  label: string
  action: () => void
  primary?: boolean
}

interface Notification {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message: string
  duration?: number
  actions?: NotificationAction[]
}

const notifications = ref<Notification[]>([
  {
    id: '1',
    type: 'info',
    title: 'Group Purchase Available',
    message: 'A new bulk order for office supplies is available. Join now to save 25%!',
    actions: [
      { label: 'View Details', action: () => router.push('/group-buying'), primary: true },
      { label: 'Dismiss', action: () => {} }
    ]
  },
  {
    id: '2',
    type: 'warning',
    title: 'Low Stock Alert',
    message: 'You have 3 items running low in inventory. Consider reordering soon.',
    actions: [
      { label: 'View Inventory', action: () => router.push('/inventory'), primary: true },
      { label: 'Dismiss', action: () => {} }
    ]
  }
])

function getNotificationClasses(type: string): string {
  switch (type) {
    case 'success':
      return 'border-green-200 dark:border-green-800 bg-green-50 dark:bg-green-900/20'
    case 'error':
      return 'border-red-200 dark:border-red-800 bg-red-50 dark:bg-red-900/20'
    case 'warning':
      return 'border-yellow-200 dark:border-yellow-800 bg-yellow-50 dark:bg-yellow-900/20'
    case 'info':
      return 'border-blue-200 dark:border-blue-800 bg-blue-50 dark:bg-blue-900/20'
    default:
      return ''
  }
}

function getIconClasses(type: string): string {
  switch (type) {
    case 'success':
      return 'text-green-600'
    case 'error':
      return 'text-red-600'
    case 'warning':
      return 'text-yellow-600'
    case 'info':
      return 'text-blue-600'
    default:
      return 'text-gray-600'
  }
}

function removeNotification(id: string) {
  const index = notifications.value.findIndex(n => n.id === id)
  if (index > -1) {
    notifications.value.splice(index, 1)
  }
}

function handleAction(action: NotificationAction, notificationId: string) {
  action.action()
  if (action.label === 'Dismiss') {
    removeNotification(notificationId)
  }
}

// Auto-remove notifications after duration
function addNotification(notification: Notification) {
  notifications.value.push(notification)
  
  if (notification.duration) {
    setTimeout(() => {
      removeNotification(notification.id)
    }, notification.duration)
  }
}

// Expose methods for global use
defineExpose({
  addNotification,
  removeNotification
})
</script>

<style scoped>
.notification-enter-active,
.notification-leave-active {
  transition: all 0.3s ease;
}

.notification-enter-from {
  opacity: 0;
  transform: translateX(100%);
}

.notification-leave-to {
  opacity: 0;
  transform: translateX(100%);
}

.notification-move {
  transition: transform 0.3s ease;
}
</style>
