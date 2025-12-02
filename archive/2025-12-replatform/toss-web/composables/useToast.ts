import { ref } from 'vue'

interface ToastOptions {
  type?: 'success' | 'error' | 'warning' | 'info'
  title?: string
  message: string
  duration?: number
  position?: 'top-right' | 'top-center' | 'bottom-right' | 'bottom-center'
}

interface Toast extends ToastOptions {
  id: string
  visible: boolean
}

const toasts = ref<Toast[]>([])

let toastIdCounter = 0

export const useToast = () => {
  const show = (options: ToastOptions) => {
    const id = `toast-${++toastIdCounter}`
    const toast: Toast = {
      id,
      type: options.type || 'info',
      title: options.title,
      message: options.message,
      duration: options.duration || 5000,
      position: options.position || 'top-right',
      visible: true
    }

    toasts.value.push(toast)

    // Auto remove after duration
    if (toast.duration > 0) {
      setTimeout(() => {
        remove(id)
      }, toast.duration)
    }

    return id
  }

  const remove = (id: string) => {
    const index = toasts.value.findIndex(t => t.id === id)
    if (index > -1) {
      toasts.value[index].visible = false
      // Remove from array after animation
      setTimeout(() => {
        toasts.value.splice(index, 1)
      }, 300)
    }
  }

  const success = (message: string, title?: string, duration?: number) => {
    return show({ type: 'success', message, title, duration })
  }

  const error = (message: string, title?: string, duration?: number) => {
    return show({ type: 'error', message, title, duration })
  }

  const warning = (message: string, title?: string, duration?: number) => {
    return show({ type: 'warning', message, title, duration })
  }

  const info = (message: string, title?: string, duration?: number) => {
    return show({ type: 'info', message, title, duration })
  }

  const clear = () => {
    toasts.value = []
  }

  return {
    toasts,
    show,
    remove,
    success,
    error,
    warning,
    info,
    clear
  }
}

