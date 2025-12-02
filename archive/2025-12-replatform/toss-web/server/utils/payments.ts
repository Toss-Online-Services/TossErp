// Payment Integration Utilities for TOSS
// Handles payment link generation and webhook processing

import type { PoolParticipant } from '~/types/group-buying'

interface PaymentProvider {
  name: 'payfast' | 'yoco' | 'ozow' | 'snapscan'
  apiKey: string
  merchantId: string
  baseUrl: string
}

interface CreatePaymentRequest {
  amount: number
  currency: string
  reference: string
  description: string
  notifyUrl: string
  returnUrl: string
  cancelUrl: string
  customerEmail?: string
  customerPhone?: string
}

interface PaymentResponse {
  paymentId: string
  paymentUrl: string
  expiresAt: Date
  status: 'pending' | 'paid' | 'failed' | 'expired'
}

// In production, load from environment variables
const provider: PaymentProvider = {
  name: process.env.PAYMENT_PROVIDER as any || 'payfast',
  apiKey: process.env.PAYMENT_API_KEY || '',
  merchantId: process.env.PAYMENT_MERCHANT_ID || '',
  baseUrl: process.env.PAYMENT_BASE_URL || 'https://www.payfast.co.za/eng/process'
}

/**
 * Generate payment link for pool participant
 */
export async function generatePayLink(
  participant: PoolParticipant,
  amount: number,
  description?: string
): Promise<string> {
  const appUrl = process.env.APP_URL || 'https://toss.app'
  
  const paymentRequest: CreatePaymentRequest = {
    amount,
    currency: 'ZAR',
    reference: `POOL-${participant.poolId}-${participant.shopId}-${Date.now()}`,
    description: description || `Pool payment for ${participant.shopName}`,
    notifyUrl: `${appUrl}/api/webhooks/payment`,
    returnUrl: `${appUrl}/pools/${participant.poolId}/payment-success`,
    cancelUrl: `${appUrl}/pools/${participant.poolId}/payment-cancel`
  }
  
  const payment = await createPayment(paymentRequest)
  
  // Store payment ID with participant for tracking
  // TODO: Update participant record with paymentLinkId
  // await db.poolParticipants.update(participant.id, {
  //   paymentLinkId: payment.paymentId,
  //   paymentLinkUrl: payment.paymentUrl
  // })
  
  return payment.paymentUrl
}

/**
 * Generate payment link for shared logistics delivery
 */
export async function generateDeliveryPayLink(
  shopId: string,
  runId: string,
  amount: number
): Promise<string> {
  const appUrl = process.env.APP_URL || 'https://toss.app'
  
  const paymentRequest: CreatePaymentRequest = {
    amount,
    currency: 'ZAR',
    reference: `DELIVERY-${runId}-${shopId}-${Date.now()}`,
    description: `Shared delivery fee for run ${runId}`,
    notifyUrl: `${appUrl}/api/webhooks/delivery-payment`,
    returnUrl: `${appUrl}/deliveries/${runId}/payment-success`,
    cancelUrl: `${appUrl}/deliveries/${runId}/payment-cancel`
  }
  
  const payment = await createPayment(paymentRequest)
  return payment.paymentUrl
}

/**
 * Create payment with provider
 */
async function createPayment(request: CreatePaymentRequest): Promise<PaymentResponse> {
  // Mock implementation for MVP
  const paymentId = `pay-${Date.now()}`
  const paymentUrl = generateMockPaymentUrl(request, paymentId)
  
  console.log('[Payment] Created payment:', {
    paymentId,
    amount: request.amount,
    reference: request.reference
  })
  
  // TODO: Implement actual payment provider integration
  // Example with PayFast:
  // const response = await fetch(`${provider.baseUrl}/payment`, {
  //   method: 'POST',
  //   headers: {
  //     'Authorization': `Bearer ${provider.apiKey}`,
  //     'Content-Type': 'application/json'
  //   },
  //   body: JSON.stringify({
  //     merchant_id: provider.merchantId,
  //     merchant_key: provider.apiKey,
  //     amount: request.amount.toFixed(2),
  //     item_name: request.description,
  //     return_url: request.returnUrl,
  //     cancel_url: request.cancelUrl,
  //     notify_url: request.notifyUrl,
  //     m_payment_id: request.reference,
  //     email_address: request.customerEmail,
  //     cell_number: request.customerPhone
  //   })
  // })
  
  return {
    paymentId,
    paymentUrl,
    expiresAt: new Date(Date.now() + 24 * 60 * 60 * 1000), // 24 hours
    status: 'pending'
  }
}

/**
 * Verify payment webhook signature
 */
export function verifyPaymentWebhook(payload: any, signature: string): boolean {
  // TODO: Implement signature verification
  // Example with PayFast:
  // const crypto = require('crypto')
  // const passphrase = process.env.PAYMENT_PASSPHRASE || ''
  // const pfParamString = Object.keys(payload)
  //   .sort()
  //   .map(key => `${key}=${encodeURIComponent(payload[key])}`)
  //   .join('&')
  // const pfParamStringWithPassphrase = `${pfParamString}&passphrase=${encodeURIComponent(passphrase)}`
  // const calculatedSignature = crypto
  //   .createHash('md5')
  //   .update(pfParamStringWithPassphrase)
  //   .digest('hex')
  // return signature === calculatedSignature
  
  // Mock: always return true for development
  return true
}

/**
 * Process payment webhook
 */
export async function processPaymentWebhook(payload: any): Promise<{
  success: boolean
  reference: string
  status: 'paid' | 'failed'
}> {
  // Extract payment details from webhook payload
  // Structure varies by payment provider
  
  const reference = payload.m_payment_id || payload.reference
  const status = payload.payment_status === 'COMPLETE' ? 'paid' : 'failed'
  const amount = parseFloat(payload.amount_gross || payload.amount || '0')
  
  console.log('[Payment Webhook] Received:', {
    reference,
    status,
    amount
  })
  
  // TODO: Update database
  // if (reference.startsWith('POOL-')) {
  //   // Pool payment
  //   const [_, poolId, shopId] = reference.split('-')
  //   await db.poolParticipants.updatePaymentStatus(poolId, shopId, status)
  //   
  //   // Check if all participants have paid
  //   const pool = await db.pools.findById(poolId)
  //   const allPaid = pool.participants.every(p => p.paymentStatus === 'paid')
  //   if (allPaid) {
  //     // Trigger order processing
  //     await processPoolOrder(pool)
  //   }
  // } else if (reference.startsWith('DELIVERY-')) {
  //   // Delivery payment
  //   const [_, runId, shopId] = reference.split('-')
  //   await db.deliveryStops.updatePaymentStatus(runId, shopId, status)
  // }
  
  // TODO: Send confirmation notifications
  // await notifyPaymentReceived(...)
  
  return {
    success: true,
    reference,
    status
  }
}

/**
 * Check payment status
 */
export async function checkPaymentStatus(paymentId: string): Promise<{
  status: 'pending' | 'paid' | 'failed' | 'expired'
  paidAt?: Date
  amount?: number
}> {
  // TODO: Query payment provider API
  // const response = await fetch(`${provider.baseUrl}/query/${paymentId}`, {
  //   headers: {
  //     'Authorization': `Bearer ${provider.apiKey}`
  //   }
  // })
  
  // Mock implementation
  return {
    status: 'pending',
    amount: 0
  }
}

/**
 * Refund payment
 */
export async function refundPayment(
  paymentId: string,
  amount: number,
  reason: string
): Promise<{ success: boolean; refundId?: string }> {
  // TODO: Implement refund via payment provider
  console.log('[Payment] Refund requested:', {
    paymentId,
    amount,
    reason
  })
  
  return {
    success: true,
    refundId: `refund-${Date.now()}`
  }
}

// Helper functions

function generateMockPaymentUrl(request: CreatePaymentRequest, paymentId: string): string {
  // In MVP, generate a mock payment URL for testing
  // In production, this would be the actual payment provider URL
  const params = new URLSearchParams({
    merchant_id: provider.merchantId,
    amount: request.amount.toFixed(2),
    item_name: request.description,
    return_url: request.returnUrl,
    cancel_url: request.cancelUrl,
    payment_id: paymentId
  })
  
  return `${provider.baseUrl}?${params.toString()}`
}

/**
 * Calculate payment fees (some providers charge transaction fees)
 */
export function calculatePaymentFees(amount: number): {
  subtotal: number
  fees: number
  total: number
} {
  // Example: PayFast charges 3.9% + R2.00 per transaction
  const feePercentage = 0.039 // 3.9%
  const fixedFee = 2.00
  
  const fees = (amount * feePercentage) + fixedFee
  const total = amount + fees
  
  return {
    subtotal: amount,
    fees: Number(fees.toFixed(2)),
    total: Number(total.toFixed(2))
  }
}

