// Common utility functions for TOSS ERP Web Client

export const formatCurrency = (amount: number, currency: string = 'ZAR'): string => {
  return new Intl.NumberFormat('en-ZA', {
    style: 'currency',
    currency: currency,
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
  }).format(amount)
}

export const formatDate = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  return new Intl.DateTimeFormat('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(dateObj)
}

export const formatDateTime = (date: Date | string): string => {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  return new Intl.DateTimeFormat('en-ZA', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }).format(dateObj)
}

export const debounce = <T extends (...args: any[]) => any>(
  func: T,
  wait: number
): ((...args: Parameters<T>) => void) => {
  let timeout: NodeJS.Timeout
  return (...args: Parameters<T>) => {
    clearTimeout(timeout)
    timeout = setTimeout(() => func(...args), wait)
  }
}

export const generateId = (): string => {
  return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
}

export const truncateText = (text: string, length: number): string => {
  if (text.length <= length) return text
  return text.substr(0, length) + '...'
}

export const validateEmail = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

export const validatePhoneNumber = (phone: string): boolean => {
  // South African phone number validation
  const phoneRegex = /^(\+27|0)[6-8][0-9]{8}$/
  return phoneRegex.test(phone.replace(/\s/g, ''))
}

export const capitalizeFirst = (text: string): string => {
  if (!text) return ''
  return text.charAt(0).toUpperCase() + text.slice(1).toLowerCase()
}

export const deepClone = <T>(obj: T): T => {
  return JSON.parse(JSON.stringify(obj))
}

export const groupBy = <T>(array: T[], key: keyof T): Record<string, T[]> => {
  return array.reduce((groups, item) => {
    const group = String(item[key])
    groups[group] = groups[group] || []
    groups[group].push(item)
    return groups
  }, {} as Record<string, T[]>)
}

export const sortBy = <T>(array: T[], key: keyof T, direction: 'asc' | 'desc' = 'asc'): T[] => {
  return [...array].sort((a, b) => {
    const aVal = a[key]
    const bVal = b[key]
    
    if (aVal < bVal) return direction === 'asc' ? -1 : 1
    if (aVal > bVal) return direction === 'asc' ? 1 : -1
    return 0
  })
}

export const downloadFile = (data: any, filename: string, type: string = 'application/json'): void => {
  const blob = new Blob([typeof data === 'string' ? data : JSON.stringify(data, null, 2)], { type })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  link.click()
  URL.revokeObjectURL(url)
}

export const copyToClipboard = async (text: string): Promise<boolean> => {
  try {
    await navigator.clipboard.writeText(text)
    return true
  } catch (err) {
    // Fallback for older browsers
    const textArea = document.createElement('textarea')
    textArea.value = text
    document.body.appendChild(textArea)
    textArea.select()
    document.execCommand('copy')
    document.body.removeChild(textArea)
    return true
  }
}

export const isMobile = (): boolean => {
  return typeof window !== 'undefined' && window.innerWidth < 768
}

export const isTablet = (): boolean => {
  return typeof window !== 'undefined' && window.innerWidth >= 768 && window.innerWidth < 1024
}

export const isDesktop = (): boolean => {
  return typeof window !== 'undefined' && window.innerWidth >= 1024
}

export const getDeviceType = (): 'mobile' | 'tablet' | 'desktop' => {
  if (isMobile()) return 'mobile'
  if (isTablet()) return 'tablet'
  return 'desktop'
}

// Storage utilities
export const storage = {
  get: (key: string, defaultValue: any = null) => {
    if (typeof window === 'undefined') return defaultValue
    try {
      const item = localStorage.getItem(key)
      return item ? JSON.parse(item) : defaultValue
    } catch {
      return defaultValue
    }
  },
  
  set: (key: string, value: any) => {
    if (typeof window === 'undefined') return
    try {
      localStorage.setItem(key, JSON.stringify(value))
    } catch (err) {
      console.warn('Failed to save to localStorage:', err)
    }
  },
  
  remove: (key: string) => {
    if (typeof window === 'undefined') return
    localStorage.removeItem(key)
  },
  
  clear: () => {
    if (typeof window === 'undefined') return
    localStorage.clear()
  }
}

// Theme utilities
export const theme = {
  isDark: () => {
    if (typeof window === 'undefined') return false
    return document.documentElement.classList.contains('dark')
  },
  
  toggle: () => {
    if (typeof window === 'undefined') return
    document.documentElement.classList.toggle('dark')
    storage.set('theme', theme.isDark() ? 'dark' : 'light')
  },
  
  set: (mode: 'light' | 'dark') => {
    if (typeof window === 'undefined') return
    if (mode === 'dark') {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
    storage.set('theme', mode)
  },
  
  init: () => {
    if (typeof window === 'undefined') return
    const savedTheme = storage.get('theme')
    if (savedTheme) {
      theme.set(savedTheme)
    } else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
      theme.set('dark')
    }
  }
}

// Network utilities
export const network = {
  isOnline: () => {
    return typeof window !== 'undefined' ? navigator.onLine : true
  },
  
  onStatusChange: (callback: (isOnline: boolean) => void) => {
    if (typeof window === 'undefined') return () => {}
    
    const handleOnline = () => callback(true)
    const handleOffline = () => callback(false)
    
    window.addEventListener('online', handleOnline)
    window.addEventListener('offline', handleOffline)
    
    return () => {
      window.removeEventListener('online', handleOnline)
      window.removeEventListener('offline', handleOffline)
    }
  }
}

// Error handling utilities
export const errorHandler = {
  handle: (error: any, context?: string) => {
    console.error(`Error${context ? ` in ${context}` : ''}:`, error)
    
    // You can extend this to send errors to a logging service
    // e.g., Sentry, LogRocket, etc.
    
    return {
      message: error.message || 'An unexpected error occurred',
      code: error.code || 'UNKNOWN_ERROR',
      context
    }
  },
  
  async: async <T>(
    asyncFn: () => Promise<T>,
    context?: string
  ): Promise<{ data?: T; error?: any }> => {
    try {
      const data = await asyncFn()
      return { data }
    } catch (error) {
      return { error: errorHandler.handle(error, context) }
    }
  }
}

// Performance utilities
export const performance = {
  measure: <T>(fn: () => T, label?: string): T => {
    const start = Date.now()
    const result = fn()
    const end = Date.now()
    
    if (label) {
      console.log(`${label} took ${end - start}ms`)
    }
    
    return result
  },
  
  measureAsync: async <T>(fn: () => Promise<T>, label?: string): Promise<T> => {
    const start = Date.now()
    const result = await fn()
    const end = Date.now()
    
    if (label) {
      console.log(`${label} took ${end - start}ms`)
    }
    
    return result
  }
}

// URL utilities
export const url = {
  getQueryParam: (name: string): string | null => {
    if (typeof window === 'undefined') return null
    const urlParams = new URLSearchParams(window.location.search)
    return urlParams.get(name)
  },
  
  setQueryParam: (name: string, value: string) => {
    if (typeof window === 'undefined') return
    const url = new URL(window.location.href)
    url.searchParams.set(name, value)
    window.history.replaceState({}, '', url.toString())
  },
  
  removeQueryParam: (name: string) => {
    if (typeof window === 'undefined') return
    const url = new URL(window.location.href)
    url.searchParams.delete(name)
    window.history.replaceState({}, '', url.toString())
  },
  
  buildQuery: (params: Record<string, any>): string => {
    const searchParams = new URLSearchParams()
    Object.entries(params).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        searchParams.append(key, String(value))
      }
    })
    return searchParams.toString()
  }
}

// Validation utilities
export const validation = {
  required: (value: any): boolean => {
    return value !== null && value !== undefined && value !== ''
  },
  
  minLength: (value: string, min: number): boolean => {
    return typeof value === 'string' && value.length >= min
  },
  
  maxLength: (value: string, max: number): boolean => {
    return typeof value === 'string' && value.length <= max
  },
  
  pattern: (value: string, pattern: RegExp): boolean => {
    return typeof value === 'string' && pattern.test(value)
  },
  
  isNumber: (value: any): boolean => {
    return !isNaN(Number(value))
  },
  
  isPositive: (value: number): boolean => {
    return typeof value === 'number' && value > 0
  },
  
  isValidUrl: (value: string): boolean => {
    try {
      new URL(value)
      return true
    } catch {
      return false
    }
  }
}

// Math utilities
export const math = {
  clamp: (value: number, min: number, max: number): number => {
    return Math.min(Math.max(value, min), max)
  },
  
  round: (value: number, decimals: number = 2): number => {
    return Math.round(value * Math.pow(10, decimals)) / Math.pow(10, decimals)
  },
  
  percentage: (value: number, total: number): number => {
    return total === 0 ? 0 : math.round((value / total) * 100)
  },
  
  average: (numbers: number[]): number => {
    return numbers.length === 0 ? 0 : numbers.reduce((a, b) => a + b, 0) / numbers.length
  },
  
  sum: (numbers: number[]): number => {
    return numbers.reduce((a, b) => a + b, 0)
  },
  
  randomBetween: (min: number, max: number): number => {
    return Math.random() * (max - min) + min
  }
}
