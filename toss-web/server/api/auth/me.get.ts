import { defineEventHandler, getCookie, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const token = getCookie(event, 'auth-token')
  if (!token) {
    throw createError({ statusCode: 401, statusMessage: 'Unauthorized' })
  }

  // In dev, return a consistent user for any token
  const user = {
    id: 'user-dev-1',
    email: 'user@toss.dev',
    firstName: 'Test',
    lastName: 'User',
    avatar: undefined,
    businessId: 'tenant1',
    businessName: "Thabo's Spaza Shop",
    role: 'owner',
    status: 'active' as const,
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }

  const permissions = [
    'dashboard:view',
    'inventory:*',
    'sales:*',
    'purchasing:*',
    'logistics:*',
    'crm:*',
    'reports:view',
    'admin',
  ]

  return { user, permissions }
})

export default defineEventHandler(async (event) => {
  // Get the auth token from cookie
  const token = getCookie(event, 'auth-token')
  
  if (!token) {
    throw createError({
      statusCode: 401,
      statusMessage: 'No authentication token provided'
    })
  }

  // Decode token (in real app, verify JWT or check session store)
  try {
    const decoded = Buffer.from(token, 'base64').toString('utf-8')
    const [userId] = decoded.split(':')
    
    // Demo users data
    const demoUsers = [
      {
        id: '1',
        email: 'owner@demo.toss.co.za',
        firstName: 'Thabo',
        lastName: 'Molefe',
        businessName: 'Thabo\'s Spaza Shop',
        businessId: 'business_1',
        role: 'owner',
        status: 'active'
      },
      {
        id: '2',
        email: 'manager@demo.toss.co.za',
        firstName: 'Nomsa',
        lastName: 'Dlamini',
        businessName: 'Thabo\'s Spaza Shop',
        businessId: 'business_1',
        role: 'manager',
        status: 'active'
      },
      {
        id: '3',
        email: 'employee@demo.toss.co.za',
        firstName: 'Sipho',
        lastName: 'Mthembu',
        businessName: 'Thabo\'s Spaza Shop',
        businessId: 'business_1',
        role: 'employee',
        status: 'active'
      }
    ]

    const user = demoUsers.find(u => u.id === userId)
    
    if (!user) {
      throw createError({
        statusCode: 401,
        statusMessage: 'Invalid token'
      })
    }

    return {
      success: true,
      user: {
        ...user,
        createdAt: '2024-01-01T00:00:00Z',
        updatedAt: new Date().toISOString()
      },
      permissions: user.role === 'owner' ? ['admin'] : [user.role]
    }
  } catch (error) {
    throw createError({
      statusCode: 401,
      statusMessage: 'Invalid token'
    })
  }
})
