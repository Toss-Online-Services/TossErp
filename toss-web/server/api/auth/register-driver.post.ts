export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()
  
  // Validate required fields
  const { 
    firstName,
    lastName,
    phone, 
    email, 
    password,
    licenseNumber,
    vehicleType,
    vehicleRegistration
  } = body
  
  if (!firstName || !lastName || !phone || !password) {
    throw createError({
      statusCode: 400,
      statusMessage: 'All required fields must be provided'
    })
  }

  // Validate email format (if provided)
  if (email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    if (!emailRegex.test(email)) {
      throw createError({
        statusCode: 400,
        statusMessage: 'Invalid email format'
      })
    }
  }

  // Validate phone format
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
    const backendUrl = config.public.apiBase || 'http://localhost:5000'
    const response = await $fetch(`${backendUrl}/api/registration/driver`, {
      method: 'POST',
      body: {
        firstName,
        lastName,
        phone,
        email,
        password,
        licenseNumber,
        vehicleType,
        vehicleRegistration
      }
    })

    return {
      success: true,
      message: '✅ Driver registration successful!',
      user: response.user,
      driver: response.driver,
      token: response.token
    }
  } catch (error: any) {
    console.error('Backend driver registration error:', error)
    
    throw createError({
      statusCode: error.statusCode || 500,
      statusMessage: error.message || error.data?.message || 'Driver registration failed. Please try again.'
    })
  }
})

