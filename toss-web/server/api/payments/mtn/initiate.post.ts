/**
 * MTN Mobile Money Payment Initiation Endpoint
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
    // Check if MTN MoMo credentials are configured
    const subscriptionKey = config.mtnSubscriptionKey || process.env.MTN_SUBSCRIPTION_KEY
    const apiUser = config.mtnApiUser || process.env.MTN_API_USER
    const apiKey = config.mtnApiKey || process.env.MTN_API_KEY

    if (!subscriptionKey || !apiUser || !apiKey) {
      console.warn('MTN Mobile Money credentials not configured, using mock response')
      return getMockResponse(phone, amount, reference)
    }

    // Step 1: Get OAuth token
    const authToken = await getMTNToken(subscriptionKey, apiUser, apiKey)

    // Step 2: Initiate payment request
    const referenceId = `${reference}-${Date.now()}`
    
    const paymentResponse = await $fetch(`https://sandbox.momodeveloper.mtn.com/collection/v1_0/requesttopay`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${authToken}`,
        'X-Reference-Id': referenceId,
        'X-Target-Environment': 'sandbox',
        'Content-Type': 'application/json',
        'Ocp-Apim-Subscription-Key': subscriptionKey
      },
      body: {
        amount: amount.toString(),
        currency: 'ZAR',
        externalId: reference,
        payer: {
          partyIdType: 'MSISDN',
          partyId: phone
        },
        payerMessage: description || 'Payment for goods',
        payeeNote: `Payment from ${phone}`
      }
    })

    return {
      success: true,
      transactionId: referenceId,
      reference,
      message: 'Payment request sent successfully. Customer should approve on their phone.',
      providerResponse: paymentResponse
    }
  } catch (error: any) {
    console.error('MTN Mobile Money error:', error)

    // Return mock response on error for development
    if (process.env.NODE_ENV === 'development') {
      return getMockResponse(phone, amount, reference)
    }

    throw createError({
      statusCode: 500,
      statusMessage: error.message || 'MTN Mobile Money payment initiation failed'
    })
  }
})

/**
 * Get MTN Mobile Money OAuth token
 */
async function getMTNToken(subscriptionKey: string, apiUser: string, apiKey: string): Promise<string> {
  const auth = Buffer.from(`${apiUser}:${apiKey}`).toString('base64')

  const response = await $fetch('https://sandbox.momodeveloper.mtn.com/collection/token/', {
    method: 'POST',
    headers: {
      'Authorization': `Basic ${auth}`,
      'Ocp-Apim-Subscription-Key': subscriptionKey
    }
  })

  return (response as any).access_token
}

/**
 * Mock response for development/testing
 */
function getMockResponse(phone: string, amount: number, reference: string) {
  const mockTransactionId = `MTN${Date.now()}`
  
  console.log('🟡 MOCK MTN MOBILE MONEY PAYMENT:', {
    phone,
    amount: `R${amount}`,
    reference,
    transactionId: mockTransactionId
  })

  return {
    success: true,
    transactionId: mockTransactionId,
    reference,
    message: '✅ MOCK: MTN Mobile Money payment request sent. Customer should approve on their phone.',
    providerResponse: {
      status: 'PENDING',
      financialTransactionId: mockTransactionId,
      externalId: reference,
      amount,
      currency: 'ZAR',
      payer: {
        partyIdType: 'MSISDN',
        partyId: phone
      }
    }
  }
}

