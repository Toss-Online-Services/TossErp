import { defineEventHandler, readBody, setCookie, createError } from 'h3'

export default defineEventHandler(async (event) => {
  try {
    const body = await readBody<{ email?: string; password?: string; rememberMe?: boolean }>(event)

    if (!body?.email || !body?.password) {
      throw createError({ statusCode: 400, statusMessage: 'Email and password are required' })
    }

    // In development, accept any credentials and issue a fake token
    const fakeToken = `dev-token-${Math.random().toString(36).slice(2)}`
    const fakeRefreshToken = `dev-refresh-${Math.random().toString(36).slice(2)}`
    const expiresIn = body.rememberMe ? 60 * 60 * 24 * 7 : 60 * 60 * 4 // 7 days or 4 hours

    // Set cookie (httpOnly disabled in dev so client can read if needed)
    setCookie(event, 'auth-token', fakeToken, {
      httpOnly: true,
      sameSite: 'strict',
      secure: false,
      path: '/',
      maxAge: expiresIn,
    })

    setCookie(event, 'auth-refresh-token', fakeRefreshToken, {
      httpOnly: true,
      sameSite: 'strict',
      secure: false,
      path: '/',
      maxAge: 60 * 60 * 24 * 30, // 30 days
    })

    const user = {
      id: 1,
      name: 'Test User',
      email: body.email,
      roles: ['owner', 'admin'],
      permissions: [
        'dashboard:view',
        'inventory:*',
        'sales:*',
        'purchasing:*',
        'logistics:*',
        'crm:*',
        'manufacturing:*',
        'reports:view',
        'admin',
      ],
      avatar: undefined,
    }

    return {
      token: fakeToken,
      refreshToken: fakeRefreshToken,
      user,
      expiresIn,
    }
  } catch (err) {
    if ((err as any)?.statusCode) throw err
    throw createError({ statusCode: 500, statusMessage: 'Login failed' })
  }
})
