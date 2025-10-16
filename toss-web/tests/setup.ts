import { vi } from 'vitest'

// Mock Nuxt auto-imports
global.navigateTo = vi.fn()
global.useRouter = vi.fn(() => ({
  push: vi.fn(),
  replace: vi.fn(),
  back: vi.fn(),
  forward: vi.fn()
}))
global.useRoute = vi.fn(() => ({
  path: '/',
  params: {},
  query: {}
}))
global.useState = vi.fn((key, init) => {
  const value = init ? init() : undefined
  return { value }
})
global.useFetch = vi.fn()
global.useHead = vi.fn()
global.definePageMeta = vi.fn()

