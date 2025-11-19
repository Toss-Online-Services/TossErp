/**
 * Airtel Money Payment Initiation Endpoint
 */

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()

  const { phone, amount, reference, description } = body

  // Validate request
  if (!phone || !amount || !reference) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Phone number, amount, and reference are required'
    })
  }

  try {
    // Check if Airtel Money credentials are configured
    const clientId = config.airtelClientId || process.env.AIRTEL_CLIENT_ID
    const clientSecret = config.airtelClientSecret || process.env.AIRTEL_CLIENT_SECRET

    if (!clientId || !clientSecret) {
      console.warn('Airtel Money credentials not configured, using mock response')
      return getMockResponse(phone, amount, reference)
    }

    // Step 1: Get OAuth token
    const authToken = await getAirtelToken(clientId, clientSecret)

    // Step 2: Initiate payment request
    const paymentResponse = await $fetch('https://openapi.airtel.africa/merchant/v1/payments/', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${authToken}`,
        'Content-Type': 'application/json',
        'X-Country': 'ZA', // South Africa
        'X-Currency': 'ZAR'
      },
      body: {
        reference,
        subscriber: {
          country: 'ZA',
          currency: 'ZAR',
          msisdn: phone
        },
        transaction: {
          amount: amount.toString(),
          country: 'ZA',
          currency: 'ZAR',
          id: reference
        }
      }
    })

    const response = paymentResponse as any

    return {
      success: response.status?.code === '200',
      transactionId: response.data?.transaction?.id || reference,
      reference,
      message: response.status?.message || 'Payment request sent successfully',
      providerResponse: response
    }
  } catch (error: any) {
    console.error('Airtel Money error:', error)

    // Return mock response on error for development
    if (process.env.NODE_ENV === 'development') {
      return getMockResponse(phone, amount, reference)
    }

    throw createError({
      statusCode: 500,
      statusMessage: error.message || 'Airtel Money payment initiation failed'
    })
  }
})

/**
 * Get Airtel Money OAuth token
 */
async function getAirtelToken(clientId: string, clientSecret: string): Promise<string> {
  const response = await $fetch('https://openapi.airtel.africa/auth/oauth2/token', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: {
      client_id: clientId,
      client_secret: clientSecret,
      grant_type: 'client_credentials'
    }
  })

  return (response as any).access_token
}

/**
 * Mock response for development/testing
 */
function getMockResponse(phone: string, amount: number, reference: string) {
  const mockTransactionId = `AIRT${Date.now()}`
  
  console.log('🔴 MOCK AIRTEL MONEY PAYMENT:', {
    phone,
    amount: `R${amount}`,
    reference,
    transactionId: mockTransactionId
  })

  return {
    success: true,
    transactionId: mockTransactionId,
    reference,
    message: '✅ MOCK: Airtel Money payment request sent. Customer should approve on their phone.',
    providerResponse: {
      status: {
        code: '200',
        message: 'Success',
        result_code: 'ESB000010',
        success: true
      },
      data: {
        transaction: {
          id: mockTransactionId,
          status: 'TS'
        }
      }
    }
  }
}

