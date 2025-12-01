import { describe, it, expect, beforeEach } from 'vitest'
import { useToast } from '../useToast'

describe('useToast', () => {
  beforeEach(() => {
    // Reset toasts before each test
    const { toasts } = useToast()
    toasts.value = []
  })

  it('should show a toast message', () => {
    const { showToast, toasts } = useToast()
    
    showToast('Test message', 'info')
    
    expect(toasts.value.length).toBe(1)
    expect(toasts.value[0].message).toBe('Test message')
    expect(toasts.value[0].type).toBe('info')
  })

  it('should remove a toast by id', () => {
    const { showToast, removeToast, toasts } = useToast()
    
    const id = showToast('Test message', 'info')
    expect(toasts.value.length).toBe(1)
    
    removeToast(id)
    expect(toasts.value.length).toBe(0)
  })

  it('should provide convenience methods', () => {
    const { success, error, warning, info, toasts } = useToast()
    
    success('Success message')
    error('Error message')
    warning('Warning message')
    info('Info message')
    
    expect(toasts.value.length).toBe(4)
    expect(toasts.value[0].type).toBe('success')
    expect(toasts.value[1].type).toBe('error')
    expect(toasts.value[2].type).toBe('warning')
    expect(toasts.value[3].type).toBe('info')
  })
})

