// toss-web/tests/composables/useFoo.test.ts
import { describe, it, expect, vi } from 'vitest'
import { useFoo } from '~/composables/useFoo'
import { mockNuxtImport } from '@nuxt/test-utils/runtime'

mockNuxtImport('useFoo', () => {
  return () => ({
    bar: () => 'mocked bar'
  })
})

describe('useFoo', () => {
  it('should return mocked bar', () => {
    const { bar } = useFoo()
    expect(bar()).toBe('mocked bar')
  })
})
