export default defineEventHandler(async (event) => {
  // Clear the auth cookie
  setCookie(event, 'auth-token', '', {
    maxAge: 0,
    secure: process.env.NODE_ENV === 'production',
    sameSite: 'strict',
    httpOnly: true
  })

  return {
    success: true,
    message: 'Logged out successfully'
  }
})
