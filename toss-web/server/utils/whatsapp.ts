// WhatsApp Integration Utilities for TOSS
// Handles sending pool invites, delivery updates, and notifications

import type { Pool } from '../types/group-buying'
import type { DeliveryStop } from '../types/logistics'

interface WhatsAppMessage {
  to: string // Phone number in international format (+27...)
  body: string
  media?: string[] // URLs to images/documents
}

interface WhatsAppConfig {
  apiKey: string
  apiUrl: string
  from: string // Sender phone number or WhatsApp Business ID
}

// In production, load from environment variables
const config: WhatsAppConfig = {
  apiKey: process.env.WHATSAPP_API_KEY || '',
  apiUrl: process.env.WHATSAPP_API_URL || 'https://api.whatsapp.com/send',
  from: process.env.WHATSAPP_FROM || ''
}

/**
 * Send WhatsApp message via API
 * In MVP, this uses a mock implementation
 * In production, integrate with WhatsApp Business API or Twilio
 */
async function sendWhatsAppMessage(message: WhatsAppMessage): Promise<{ success: boolean; messageId?: string }> {
  // Mock implementation for MVP
  console.log('[WhatsApp] Sending message to:', message.to)
  console.log('[WhatsApp] Message:', message.body)
  
  // TODO: Implement actual WhatsApp API call
  // Example with Twilio:
  // const client = twilio(accountSid, authToken)
  // const sent = await client.messages.create({
  //   from: `whatsapp:${config.from}`,
  //   to: `whatsapp:${message.to}`,
  //   body: message.body,
  //   mediaUrl: message.media
  // })
  
  return {
    success: true,
    messageId: `msg-${Date.now()}`
  }
}

/**
 * Send pool invitation to a phone number
 */
export async function sendPoolInvite(pool: Pool, phoneNumber: string, customMessage?: string): Promise<boolean> {
  const appUrl = process.env.APP_URL || 'https://toss.app'
  const inviteLink = `${appUrl}/pools/${pool.id}/join`
  
  const defaultMessage = `
🛒 *Join Group Buy: ${pool.title || 'Group Purchase'}*

📦 Item: ${pool.sku || 'N/A'}
🎯 Target: ${pool.targetQuantity || 0} units
💰 *Save ${pool.savingsPercentage || 0}%* (R${pool.currentPrice || 0} → R${pool.poolPrice || 0})
📍 Area: ${pool.area || 'N/A'}
⏰ Closes: ${pool.deadline ? formatDate(pool.deadline) : 'TBD'}
👥 ${pool.participantCount || 0}/${pool.maxParticipants || 0} shops joined

${customMessage || 'Join now to unlock bulk pricing!'}

Join here: ${inviteLink}

Led by ${pool.leadShopName || 'Lead Shop'}
`.trim()
  
  const result = await sendWhatsAppMessage({
    to: phoneNumber,
    body: defaultMessage
  })
  
  return result.success
}

/**
 * Send bulk invites to multiple phone numbers
 */
export async function sendBulkPoolInvites(
  pool: Pool,
  phoneNumbers: string[],
  customMessage?: string
): Promise<{ sent: number; failed: number }> {
  let sent = 0
  let failed = 0
  
  for (const phoneNumber of phoneNumbers) {
    try {
      const success = await sendPoolInvite(pool, phoneNumber, customMessage)
      if (success) {
        sent++
      } else {
        failed++
      }
    } catch (error) {
      console.error(`Failed to send invite to ${phoneNumber}:`, error)
      failed++
    }
  }
  
  return { sent, failed }
}

/**
 * Send pool status update to all participants
 */
export async function notifyPoolProgress(pool: Pool): Promise<void> {
  const targetQty = pool.targetQuantity || 1
  const currentCommit = pool.currentCommitment || 0
  const progressPercentage = (currentCommit / targetQty) * 100
  const message = `
🔔 *Pool Update: ${pool.title || 'Group Purchase'}*

Progress: ${progressPercentage.toFixed(0)}% (${currentCommit}/${targetQty} units)
👥 ${pool.participantCount || 0} shops participating
⏰ ${pool.deadline ? getTimeRemaining(pool.deadline) : 'No deadline set'}

${progressPercentage >= 100 ? '✅ Target reached! Waiting for confirmation.' : `💪 ${targetQty - currentCommit} more units needed!`}
`.trim()
  
  if (pool.participants && Array.isArray(pool.participants)) {
    for (const participant of pool.participants) {
      // TODO: Get participant phone number from database
      // await sendWhatsAppMessage({
      //   to: participant.phone,
      //   body: message
      // })
    }
  }
}

/**
 * Send pool confirmation notification with payment link
 */
export async function notifyPoolConfirmed(pool: Pool, participantId: string, paymentLink: string): Promise<void> {
  if (!pool.participants || !Array.isArray(pool.participants)) return
  
  const participant = pool.participants.find(p => p.id === participantId)
  if (!participant) return
  
  const quantityCommitted = participant.quantityCommitted || 0
  const costShare = participant.costShare || 0
  const deliveryFeeShare = participant.deliveryFeeShare || 0
  const currentPrice = pool.currentPrice || 0
  const savings = (quantityCommitted * currentPrice) - costShare
  
  const message = `
✅ *Pool Confirmed: ${pool.title || 'Group Purchase'}*

Your share:
📦 Quantity: ${quantityCommitted} units
💵 Amount: R${costShare.toFixed(2)}
🚚 Delivery: R${deliveryFeeShare.toFixed(2)}
*Total: R${(costShare + deliveryFeeShare).toFixed(2)}*

💰 *You saved R${savings.toFixed(2)}!*

Pay now: ${paymentLink}

⏰ Payment due within 24 hours
`.trim()
  
  // TODO: Send to participant phone
  await sendWhatsAppMessage({
    to: '+27000000000', // TODO: Get from database
    body: message
  })
}

/**
 * Send delivery update notification
 */
export async function sendDeliveryUpdate(
  stop: DeliveryStop,
  status: 'scheduled' | 'out-for-delivery' | 'nearby' | 'delivered'
): Promise<void> {
  const appUrl = process.env.APP_URL || 'https://toss.app'
  const trackingLink = `${appUrl}/deliveries/${stop.id}`
  
  let statusMessage = ''
  let emoji = '📦'
  
  switch (status) {
    case 'scheduled':
      statusMessage = `scheduled for ${formatTime(stop.estimatedArrival)}`
      emoji = '📅'
      break
    case 'out-for-delivery':
      statusMessage = 'out for delivery'
      emoji = '🚚'
      break
    case 'nearby':
      statusMessage = `arriving soon (ETA: ${formatTime(stop.estimatedArrival)})`
      emoji = '📍'
      break
    case 'delivered':
      statusMessage = 'delivered successfully'
      emoji = '✅'
      break
  }
  
  const message = `
${emoji} *Delivery Update*

Order: ${stop.orderId}
Status: ${statusMessage}
📍 ${stop.location.address}
📦 ${stop.items.length} item(s)

${status !== 'delivered' ? `Track delivery: ${trackingLink}` : 'Thank you for your order!'}
`.trim()
  
  if (!stop.shopPhone) {
    console.warn(`No phone number for shop ${stop.shopName || stop.shopId}. Cannot send delivery update.`)
    return
  }
  
  await sendWhatsAppMessage({
    to: stop.shopPhone,
    body: message
  })
}

/**
 * Send driver arrival notification
 */
export async function notifyDriverArrival(stop: DeliveryStop, driverName: string, minutesAway: number): Promise<void> {
  const message = `
🚚 *Driver Arriving Soon!*

Driver: ${driverName}
📍 ETA: ${minutesAway} minute${minutesAway !== 1 ? 's' : ''}
📦 ${stop.items.length} item(s) for delivery

Please be ready to receive your order.
`.trim()
  
  if (!stop.shopPhone) {
    console.warn(`No phone number for shop ${stop.shopName || stop.shopId}. Cannot send arrival notification.`)
    return
  }
  
  await sendWhatsAppMessage({
    to: stop.shopPhone,
    body: message
  })
}

/**
 * Send payment confirmation
 */
export async function notifyPaymentReceived(shopPhone: string, amount: number, reference: string): Promise<void> {
  const message = `
✅ *Payment Received*

Amount: R${amount.toFixed(2)}
Reference: ${reference}

Thank you! Your order will be processed shortly.
`.trim()
  
  await sendWhatsAppMessage({
    to: shopPhone,
    body: message
  })
}

// Helper functions

function formatDate(date: Date | string): string {
  const options: Intl.DateTimeFormatOptions = {
    weekday: 'short',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  }
  return new Date(date).toLocaleDateString('en-ZA', options)
}

function formatTime(date: Date): string {
  const options: Intl.DateTimeFormatOptions = {
    hour: '2-digit',
    minute: '2-digit'
  }
  return new Date(date).toLocaleTimeString('en-ZA', options)
}

function getTimeRemaining(deadline: Date | string): string {
  const now = new Date()
  const diff = new Date(deadline).getTime() - now.getTime()
  
  if (diff <= 0) return 'Expired'
  
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const days = Math.floor(hours / 24)
  
  if (days > 1) return `${days} days remaining`
  if (days === 1) return '1 day remaining'
  if (hours > 1) return `${hours} hours remaining`
  
  const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))
  return `${minutes} minutes remaining`
}

