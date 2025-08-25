export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  // Validate required fields
  const { email, password } = body
  
  if (!email || !password) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Email and password are required'
    })
  }

  // Demo authentication - replace with real authentication
  const demoUsers = [
    {
      id: '1',
      email: 'owner@demo.toss.co.za',
      password: 'password123',
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
      password: 'password123',
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
      password: 'password123',
      firstName: 'Sipho',
      lastName: 'Mthembu',
      businessName: 'Thabo\'s Spaza Shop',
      businessId: 'business_1',
      role: 'employee',
      status: 'active'
    }
  ]

  // Find user
  const user = demoUsers.find(u => u.email === email && u.password === password)
  
  if (!user) {
    throw createError({
      statusCode: 401,
      statusMessage: 'Invalid email or password'
    })
  }

  // Generate session token (in real app, use JWT or proper session management)
  const token = Buffer.from(`${user.id}:${Date.now()}`).toString('base64')
  
  // Set session cookie
  setCookie(event, 'auth-token', token, {
    maxAge: 60 * 60 * 24 * 7, // 7 days
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'strict',
    httpOnly: true
  })

  // Return user data (excluding password)
  const { password: _, ...userWithoutPassword } = user
  
  return {
    success: true,
    user: {
      ...userWithoutPassword,
      createdAt: '2024-01-01T00:00:00Z',
      updatedAt: new Date().toISOString()
    },
    token,
    permissions: user.role === 'owner' ? ['admin'] : [user.role]
  }
})
