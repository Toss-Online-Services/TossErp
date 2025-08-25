export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  // Validate required fields
  const { businessName, firstName, lastName, email, phone, password, businessType } = body
  
  if (!businessName || !firstName || !lastName || !email || !password || !businessType) {
    throw createError({
      statusCode: 400,
      statusMessage: 'All required fields must be provided'
    })
  }

  // Validate email format
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(email)) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Invalid email format'
    })
  }

  // Validate password strength
  if (password.length < 8) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Password must be at least 8 characters long'
    })
  }

  // Check if user already exists (demo check)
  const existingEmails = [
    'owner@demo.toss.co.za',
    'manager@demo.toss.co.za',
    'employee@demo.toss.co.za'
  ]
  
  if (existingEmails.includes(email)) {
    throw createError({
      statusCode: 409,
      statusMessage: 'An account with this email already exists'
    })
  }

  // In a real application, you would:
  // 1. Hash the password
  // 2. Save to database
  // 3. Send verification email
  // 4. Return user data

  // For demo purposes, we'll simulate successful registration
  const newUser = {
    id: `user_${Date.now()}`,
    email,
    firstName,
    lastName,
    businessName,
    businessId: `business_${Date.now()}`,
    phone,
    businessType,
    role: 'owner', // First user of a business becomes owner
    status: 'pending', // Requires email verification
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }

  return {
    success: true,
    message: 'Account created successfully. Please check your email to verify your account.',
    user: newUser
  }
})
