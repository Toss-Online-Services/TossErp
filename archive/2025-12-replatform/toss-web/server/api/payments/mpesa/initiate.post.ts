/**
 * M-Pesa Payment Initiation Endpoint
 * Handles M-Pesa STK Push (Lipa Na M-Pesa Online)
 */

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()

  const { phone, amount, reference, description, merchantCode } = body

  // Validate request
  if (!phone || !amount || !reference) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Phone number, amount, and reference are required'
    })
  }

  try {
    // Check if M-Pesa credentials are configured
    const consumerKey = config.mpesaConsumerKey || process.env.MPESA_CONSUMER_KEY
    const consumerSecret = config.mpesaConsumerSecret || process.env.MPESA_CONSUMER_SECRET
    const shortcode = merchantCode || config.mpesaShortcode || process.env.MPESA_SHORTCODE
    const passkey = config.mpesaPasskey || process.env.MPESA_PASSKEY

    if (!consumerKey || !consumerSecret) {
      console.warn('M-Pesa credentials not configured, using mock response')
      return getMockResponse(phone, amount, reference)
    }

    // Step 1: Get OAuth token
    const authToken = await getMPesaToken(consumerKey, consumerSecret)

    // Step 2: Initiate STK Push
    const timestamp = getTimestamp()
    const password = Buffer.from(`${shortcode}${passkey}${timestamp}`).toString('base64')

    const stkPushResponse = await $fetch('https://api.safaricom.co.ke/mpesa/stkpush/v1/processrequest', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${authToken}`,
        'Content-Type': 'application/json'
      },
      body: {
        BusinessShortCode: shortcode,
        Password: password,
        Timestamp: timestamp,
        TransactionType: 'CustomerPayBillOnline',
        Amount: Math.round(amount),
        PartyA: phone,
        PartyB: shortcode,
        PhoneNumber: phone,
        CallBackURL: `${config.public.apiBase}/api/payments/mpesa/callback`,
        AccountReference: reference,
        TransactionDesc: description || 'Payment for goods'
      }
    })

    const response = stkPushResponse as any

    return {
      success: true,
      transactionId: response.CheckoutRequestID,
      reference,
      message: 'Payment request sent to your phone. Please enter your M-Pesa PIN.',
      providerResponse: response
    }
  } catch (error: any) {
    console.error('M-Pesa initiation error:', error)

    // Return mock response on error for development
    if (process.env.NODE_ENV === 'development') {
      return getMockResponse(phone, amount, reference)
    }

    throw createError({
      statusCode: 500,
      statusMessage: error.message || 'M-Pesa payment initiation failed'
    })
  }
})

/**
 * Get M-Pesa OAuth token
 */
async function getMPesaToken(consumerKey: string, consumerSecret: string): Promise<string> {
  const auth = Buffer.from(`${consumerKey}:${consumerSecret}`).toString('base64')

  const response = await $fetch('https://api.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials', {
    headers: {
      'Authorization': `Basic ${auth}`
    }
  })

  return (response as any).access_token
}

/**
 * Generate timestamp in M-Pesa format
 */
function getTimestamp(): string {
  const date = new Date()
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  
  return `${year}${month}${day}${hours}${minutes}${seconds}`
}

/**
 * Mock response for development/testing
 */
function getMockResponse(phone: string, amount: number, reference: string) {
  const mockTransactionId = `MOCK${Date.now()}`
  
  console.log('🔔 MOCK M-PESA PAYMENT:', {
    phone,
    amount: `R${amount}`,
    reference,
    transactionId: mockTransactionId
  })

  return {
    success: true,
    transactionId: mockTransactionId,
    reference,
    message: '✅ MOCK: Payment request sent successfully. Customer should enter M-Pesa PIN on their phone.',
    providerResponse: {
      MerchantRequestID: mockTransactionId,
      CheckoutRequestID: mockTransactionId,
      ResponseCode: '0',
      ResponseDescription: 'Success. Request accepted for processing',
      CustomerMessage: 'Success. Request accepted for processing'
    }
  }
}

