import { vi } from 'vitest'
import { config } from '@vue/test-utils'
import { ref } from 'vue'

// Define global Nuxt composables
global.useHead = vi.fn()
global.useRouter = vi.fn(() => ({
  push: vi.fn(),
  replace: vi.fn(),
  back: vi.fn(),
  currentRoute: { value: { path: '/', query: {} } }
}))
global.useRoute = vi.fn(() => ({
  path: '/',
  params: {},
  query: {}
}))
global.navigateTo = vi.fn()
global.useState = vi.fn((key, init) => {
  const state = ref(typeof init === 'function' ? init() : init)
  return state
})
global.useRuntimeConfig = vi.fn(() => ({
  public: {
    apiBase: 'https://localhost:5001'
  }
}))

// Mock Nuxt composables as modules too
vi.mock('#app', () => ({
  useHead: vi.fn(),
  useRouter: vi.fn(() => ({
    push: vi.fn(),
    replace: vi.fn(),
    back: vi.fn(),
    currentRoute: { value: { path: '/', query: {} } }
  })),
  useRoute: vi.fn(() => ({
    path: '/',
    params: {},
    query: {}
  })),
  navigateTo: vi.fn(),
  useState: vi.fn((key, init) => {
    const state = ref(typeof init === 'function' ? init() : init)
    return state
  }),
  useRuntimeConfig: vi.fn(() => ({
    public: {
      apiBase: 'https://localhost:5001'
    }
  }))
}))

// Mock useToast composable
vi.mock('~/composables/useToast', () => ({
  useToast: vi.fn(() => ({
    success: vi.fn(),
    error: vi.fn(),
    warning: vi.fn(),
    info: vi.fn()
  }))
}))

// Global test configuration
config.global.stubs = {
  NuxtLink: {
    template: '<a :href="to" @click="$emit(\'click\', $event)"><slot /></a>',
    props: ['to']
  },
  ClientOnly: {
    template: '<div><slot /></div>'
  },
  Teleport: true
}

// Provide router mocks for components
config.global.provide = {
  router: {
    push: vi.fn(),
    replace: vi.fn(),
    back: vi.fn(),
    currentRoute: ref({ value: { path: '/', query: {} } })
  }
}
