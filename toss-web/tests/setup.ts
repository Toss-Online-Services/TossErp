import { vi } from 'vitest'

// Mock Nuxt composables
global.navigateTo = vi.fn()
global.useHead = vi.fn()
global.definePageMeta = vi.fn()
global.useRoute = vi.fn(() => ({
  path: '/',
  params: {},
  query: {}
}))
global.useRouter = vi.fn(() => ({
  push: vi.fn(),
  replace: vi.fn(),
  back: vi.fn()
}))

// Mock Pinia stores
global.useUserStore = vi.fn(() => ({
  user: { firstName: 'Test', lastName: 'User', businessName: 'Test Business' },
  isAuthenticated: true,
  hasPermission: { value: vi.fn(() => true) }
}))

global.useNotificationStore = vi.fn(() => ({
  add: vi.fn(),
  success: vi.fn(),
  error: vi.fn(),
  warning: vi.fn(),
  info: vi.fn()
}))

global.useSettingsStore = vi.fn(() => ({
  darkMode: false,
  language: 'en'
}))

// Mock Vue composables
global.ref = vi.fn((value) => ({ value }))
global.computed = vi.fn((fn) => ({ value: fn() }))
global.reactive = vi.fn((value) => value)
global.readonly = vi.fn((value) => value)
global.watch = vi.fn()
global.nextTick = vi.fn(() => Promise.resolve())
global.onMounted = vi.fn()
global.onUnmounted = vi.fn()

// Mock $fetch
global.$fetch = vi.fn()

// Mock browser APIs
Object.defineProperty(window, 'localStorage', {
  value: {
    getItem: vi.fn(),
    setItem: vi.fn(),
    removeItem: vi.fn(),
    clear: vi.fn(),
  },
  writable: true,
})
