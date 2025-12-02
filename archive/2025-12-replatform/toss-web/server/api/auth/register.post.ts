export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()
  
  // Validate required fields matching the multi-step registration form
  const { 
    shopName, 
    area, 
    zone, 
    address, 
    firstName, 
    lastName, 
    phone, 
    email, 
    password, 
    whatsappAlerts 
  } = body
  
  if (!shopName || !area || !address || !firstName || !lastName || !phone || !password) {
    throw createError({
      statusCode: 400,
      statusMessage: 'All required fields must be provided'
    })
  }

  // Validate email format (email is optional but validate if provided)
  if (email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(email)) {
      throw createError({
        statusCode: 400,
        statusMessage: 'Invalid email format'
      })
    }
  }

  // Validate phone format (basic South African format)
  const phoneRegex = /^\+27\d{9}$/
  if (!phoneRegex.test(phone)) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Invalid phone format. Use +27XXXXXXXXX format'
    })
  }

  // Validate password strength
  if (password.length < 8) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Password must be at least 8 characters long'
    })
  }

  try {
    // Call backend registration API
    const backendUrl = config.public.apiBase
    const response = await $fetch(`${backendUrl}/api/registration/store-owner`, {
      method: 'POST',
      body: {
        shopName,
        area,
        zone,
        address,
        firstName,
        lastName,
        phone,
        email,
        password,
        whatsappAlerts: whatsappAlerts !== false
      }
    })

    return {
      success: true,
      message: '✅ Registration successful! Welcome to TOSS!',
      user: response.user,
      shop: response.store,
      token: response.token
    }
  } catch (error: any) {
    console.error('Backend registration error:', error)
    
    // If backend is not available, throw error
    throw createError({
      statusCode: error.statusCode || 500,
      statusMessage: error.message || error.data?.message || 'Registration failed. Please try again.'
    })
  }
})
