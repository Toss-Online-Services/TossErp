import { defineEventHandler, readBody, setCookie, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    const body = await readBody<{ email?: string; password?: string; rememberMe?: boolean }>(event)

    if (!body?.email || !body?.password) {
      throw createError({ statusCode: 400, statusMessage: 'Email and password are required' })
    }

    // In development, accept any credentials and issue a fake token
    const fakeToken = `dev-${Math.random().toString(36).slice(2)}`

    // Set cookie (httpOnly disabled in dev so client can read if needed)
    setCookie(event, 'auth-token', fakeToken, {
      httpOnly: true,
      sameSite: 'strict',
      secure: false,
      path: '/',
      maxAge: body.rememberMe ? 60 * 60 * 24 * 7 : 60 * 60 * 4,
    })

    const user = {
      id: 'user-dev-1',
      email: body.email,
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

    return {
      token: fakeToken,
      user,
      permissions,
    }
  } catch (err) {
    if ((err as any)?.statusCode) throw err
    throw createError({ statusCode: 500, statusMessage: 'Login failed' })
  }
})

// (Removed duplicate default export block)
