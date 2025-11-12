import { describe, it, expect, vi } from 'vitest'
import { useApi } from '~/composables/useApi'

// Mock the useRuntimeConfig composable
vi.mock('#app', () => ({
  useRuntimeConfig: () => ({
    public: {
      apiBase: 'http://localhost:3000'
    }
  })
}))

describe('useApi', () => {
  it('should be defined', () => {
    expect(useApi).toBeDefined()
  })
})
