/**
 * Mobile Money Integration Composable
 * Supports M-Pesa, Airtel Money, and MTN Mobile Money
 */

import { ref } from 'vue'

export type MobileMoneyProvider = 'mpesa' | 'airtel' | 'mtn'

export interface MobileMoneyPayment {
  amount: number
  phoneNumber: string
  provider: MobileMoneyProvider
  reference: string
  description?: string
  merchantCode?: string
}

export interface PaymentResult {
  success: boolean
  transactionId?: string
  reference: string
  message: string
  providerResponse?: any
}

export interface PaymentStatus {
  status: 'pending' | 'completed' | 'failed' | 'cancelled'
  transactionId: string
  amount: number
  phoneNumber: string
  completedAt?: Date
  failureReason?: string
}

export const useMobileMoney = () => {
  const isProcessing = ref(false)
  const error = ref<string | null>(null)
  const lastTransaction = ref<PaymentResult | null>(null)

  /**
   * Initiate M-Pesa payment (Safaricom - Kenya & South Africa)
   */
  const payWithMPesa = async (payment: MobileMoneyPayment): Promise<PaymentResult> => {
    isProcessing.value = true
    error.value = null

    try {
      // Validate phone number format
      const cleanPhone = payment.phoneNumber.replace(/\D/g, '')
      if (!cleanPhone.startsWith('254') && !cleanPhone.startsWith('27')) {
        throw new Error('M-Pesa requires Kenyan (+254) or South African (+27) phone number')
      }

      // Call backend M-Pesa API
      const response = await $fetch<PaymentResult>('/api/payments/mpesa/initiate', {
        method: 'POST',
        body: {
          phone: cleanPhone,
          amount: payment.amount,
          reference: payment.reference,
          description: payment.description || 'Payment for goods',
          merchantCode: payment.merchantCode
        }
      })

      lastTransaction.value = response
      return response
    } catch (err: any) {
      error.value = err.message || 'M-Pesa payment failed'
      console.error('M-Pesa error:', err)
      
      return {
        success: false,
        reference: payment.reference,
        message: error.value
      }
    } finally {
      isProcessing.value = false
    }
  }

  /**
   * Initiate Airtel Money payment
   */
  const payWithAirtel = async (payment: MobileMoneyPayment): Promise<PaymentResult> => {
    isProcessing.value = true
    error.value = null

    try {
      // Validate phone number
      const cleanPhone = payment.phoneNumber.replace(/\D/g, '')
      if (!cleanPhone.startsWith('27') && !cleanPhone.startsWith('256')) {
        throw new Error('Airtel Money requires South African (+27) or Ugandan (+256) phone number')
      }

      // Call backend Airtel Money API
      const response = await $fetch<PaymentResult>('/api/payments/airtel/initiate', {
        method: 'POST',
        body: {
          phone: cleanPhone,
          amount: payment.amount,
          reference: payment.reference,
          description: payment.description || 'Payment for goods'
        }
      })

      lastTransaction.value = response
      return response
    } catch (err: any) {
      error.value = err.message || 'Airtel Money payment failed'
      console.error('Airtel Money error:', err)
      
      return {
        success: false,
        reference: payment.reference,
        message: error.value
      }
    } finally {
      isProcessing.value = false
    }
  }

  /**
   * Initiate MTN Mobile Money payment
   */
  const payWithMTN = async (payment: MobileMoneyPayment): Promise<PaymentResult> => {
    isProcessing.value = true
    error.value = null

    try {
      // Validate phone number
      const cleanPhone = payment.phoneNumber.replace(/\D/g, '')
      if (!cleanPhone.startsWith('27') && !cleanPhone.startsWith('256') && !cleanPhone.startsWith('233')) {
        throw new Error('MTN Mobile Money requires South African (+27), Ugandan (+256), or Ghanaian (+233) phone number')
      }

      // Call backend MTN Mobile Money API
      const response = await $fetch<PaymentResult>('/api/payments/mtn/initiate', {
        method: 'POST',
        body: {
          phone: cleanPhone,
          amount: payment.amount,
          reference: payment.reference,
          description: payment.description || 'Payment for goods'
        }
      })

      lastTransaction.value = response
      return response
    } catch (err: any) {
      error.value = err.message || 'MTN Mobile Money payment failed'
      console.error('MTN Mobile Money error:', err)
      
      return {
        success: false,
        reference: payment.reference,
        message: error.value
      }
    } finally {
      isProcessing.value = false
    }
  }

  /**
   * Unified payment method - automatically selects provider
   */
  const pay = async (payment: MobileMoneyPayment): Promise<PaymentResult> => {
    switch (payment.provider) {
      case 'mpesa':
        return payWithMPesa(payment)
      case 'airtel':
        return payWithAirtel(payment)
      case 'mtn':
        return payWithMTN(payment)
      default:
        error.value = 'Unsupported mobile money provider'
        return {
          success: false,
          reference: payment.reference,
          message: error.value
        }
    }
  }

  /**
   * Check payment status
   */
  const checkPaymentStatus = async (transactionId: string, provider: MobileMoneyProvider): Promise<PaymentStatus> => {
    try {
      const response = await $fetch<PaymentStatus>(`/api/payments/${provider}/status/${transactionId}`)
      return response
    } catch (err: any) {
      console.error('Failed to check payment status:', err)
      throw err
    }
  }

  /**
   * Generate payment link (for WhatsApp, SMS, etc.)
   */
  const generatePaymentLink = async (payment: Omit<MobileMoneyPayment, 'phoneNumber'>): Promise<string> => {
    try {
      const response = await $fetch<{ link: string }>('/api/payments/link/generate', {
        method: 'POST',
        body: {
          amount: payment.amount,
          provider: payment.provider,
          reference: payment.reference,
          description: payment.description
        }
      })

      return response.link
    } catch (err: any) {
      error.value = err.message || 'Failed to generate payment link'
      console.error('Payment link error:', err)
      throw err
    }
  }

  /**
   * Generate QR code for payment
   */
  const generatePaymentQR = async (payment: Omit<MobileMoneyPayment, 'phoneNumber'>): Promise<string> => {
    try {
      const response = await $fetch<{ qrCode: string }>('/api/payments/qr/generate', {
        method: 'POST',
        body: {
          amount: payment.amount,
          provider: payment.provider,
          reference: payment.reference,
          description: payment.description
        }
      })

      return response.qrCode // Base64 encoded QR code image
    } catch (err: any) {
      error.value = err.message || 'Failed to generate QR code'
      console.error('QR code error:', err)
      throw err
    }
  }

  /**
   * Validate phone number for provider
   */
  const validatePhoneNumber = (phoneNumber: string, provider: MobileMoneyProvider): boolean => {
    const cleanPhone = phoneNumber.replace(/\D/g, '')

    switch (provider) {
      case 'mpesa':
        return cleanPhone.startsWith('254') || cleanPhone.startsWith('27')
      case 'airtel':
        return cleanPhone.startsWith('27') || cleanPhone.startsWith('256')
      case 'mtn':
        return cleanPhone.startsWith('27') || cleanPhone.startsWith('256') || cleanPhone.startsWith('233')
      default:
        return false
    }
  }

  /**
   * Format phone number for display
   */
  const formatPhoneNumber = (phoneNumber: string): string => {
    const clean = phoneNumber.replace(/\D/g, '')
    
    if (clean.startsWith('27')) {
      // South Africa: +27 XX XXX XXXX
      return `+27 ${clean.slice(2, 4)} ${clean.slice(4, 7)} ${clean.slice(7)}`
    } else if (clean.startsWith('254')) {
      // Kenya: +254 XXX XXX XXX
      return `+254 ${clean.slice(3, 6)} ${clean.slice(6, 9)} ${clean.slice(9)}`
    } else if (clean.startsWith('256')) {
      // Uganda: +256 XXX XXX XXX
      return `+256 ${clean.slice(3, 6)} ${clean.slice(6, 9)} ${clean.slice(9)}`
    } else if (clean.startsWith('233')) {
      // Ghana: +233 XX XXX XXXX
      return `+233 ${clean.slice(3, 5)} ${clean.slice(5, 8)} ${clean.slice(8)}`
    }

    return phoneNumber
  }

  /**
   * Get provider name for display
   */
  const getProviderName = (provider: MobileMoneyProvider): string => {
    switch (provider) {
      case 'mpesa':
        return 'M-Pesa'
      case 'airtel':
        return 'Airtel Money'
      case 'mtn':
        return 'MTN Mobile Money'
      default:
        return provider
    }
  }

  /**
   * Get provider logo/icon
   */
  const getProviderIcon = (provider: MobileMoneyProvider): string => {
    switch (provider) {
      case 'mpesa':
        return '💚' // M-Pesa green
      case 'airtel':
        return '🔴' // Airtel red
      case 'mtn':
        return '🟡' // MTN yellow
      default:
        return '💰'
    }
  }

  /**
   * Get supported providers for country code
   */
  const getSupportedProviders = (countryCode: string): MobileMoneyProvider[] => {
    const clean = countryCode.replace(/\D/g, '')
    
    if (clean === '27') {
      // South Africa: All providers
      return ['mpesa', 'airtel', 'mtn']
    } else if (clean === '254') {
      // Kenya: M-Pesa only
      return ['mpesa']
    } else if (clean === '256') {
      // Uganda: Airtel & MTN
      return ['airtel', 'mtn']
    } else if (clean === '233') {
      // Ghana: MTN only
      return ['mtn']
    }

    return []
  }

  return {
    // State
    isProcessing,
    error,
    lastTransaction,

    // Payment methods
    pay,
    payWithMPesa,
    payWithAirtel,
    payWithMTN,

    // Status & tracking
    checkPaymentStatus,

    // Payment links & QR
    generatePaymentLink,
    generatePaymentQR,

    // Utilities
    validatePhoneNumber,
    formatPhoneNumber,
    getProviderName,
    getProviderIcon,
    getSupportedProviders
  }
}

