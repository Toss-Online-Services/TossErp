import { describe, it, expect, vi } from 'vitest'
import { useUserStore } from '~/stores/user'

// Mock $fetch for the tests
const mockFetch = vi.fn()
global.$fetch = mockFetch as any

describe('User Store', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('handles successful login', async () => {
    const mockResponse = {
      user: {
        id: '1',
        email: 'test@example.com',
        firstName: 'Test',
        lastName: 'User',
        businessName: 'Test Business',
        role: 'owner'
      },
      token: 'test-token',
      permissions: ['admin']
    }

    mockFetch.mockResolvedValueOnce(mockResponse)

    const userStore = useUserStore()
    const result = await userStore.login({
      email: 'test@example.com',
      password: 'password123'
    })

    expect(result).toEqual(mockResponse)
    expect(userStore.isAuthenticated).toBe(true)
    expect(userStore.user?.firstName).toBe('Test')
  })

  it('handles login failure', async () => {
    mockFetch.mockRejectedValueOnce(new Error('Invalid credentials'))

    const userStore = useUserStore()
    
    await expect(userStore.login({
      email: 'test@example.com',
      password: 'wrong-password'
    })).rejects.toThrow('Invalid credentials')

    expect(userStore.isAuthenticated).toBe(false)
    expect(userStore.user).toBe(null)
  })

  it('computes user initials correctly', () => {
    const userStore = useUserStore()
    userStore.user = {
      id: '1',
      email: 'test@example.com',
      firstName: 'John',
      lastName: 'Doe',
      businessName: 'Test Business',
      role: 'owner',
      status: 'active',
      createdAt: '2024-01-01',
      updatedAt: '2024-01-01'
    }

    expect(userStore.userInitials).toBe('JD')
  })

  it('checks permissions correctly', () => {
    const userStore = useUserStore()
    userStore.permissions = ['admin', 'user']

    expect(userStore.hasPermission('admin')).toBe(true)
    expect(userStore.hasPermission('user')).toBe(true)
    expect(userStore.hasPermission('manager')).toBe(true) // admin has all permissions
  })
})
