/**
 * AI Chat API Endpoint
 * Handles OpenAI API calls for TOSS AI Assistant
 */

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()

  const { messages, temperature = 0.7, maxTokens = 500, model = 'gpt-4-turbo-preview' } = body

  // Validate request
  if (!messages || !Array.isArray(messages)) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Messages array is required'
    })
  }

  try {
    // Check if OpenAI API key is configured
    const apiKey = config.openaiApiKey || process.env.NUXT_PUBLIC_OPENAI_API_KEY

    if (!apiKey) {
      console.warn('OpenAI API key not configured, using fallback responses')
      return {
        response: getFallbackResponse(messages[messages.length - 1]?.content || '')
      }
    }

    // Call OpenAI API
    const response = await $fetch('https://api.openai.com/v1/chat/completions', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${apiKey}`,
        'Content-Type': 'application/json'
      },
      body: {
        model,
        messages,
        temperature,
        max_tokens: maxTokens
      }
    })

    const aiResponse = (response as any).choices[0]?.message?.content || 'I apologize, I couldn\'t generate a response.'

    return {
      response: aiResponse,
      model,
      usage: (response as any).usage
    }
  } catch (error: any) {
    console.error('OpenAI API error:', error)

    // Return fallback response instead of failing
    return {
      response: getFallbackResponse(messages[messages.length - 1]?.content || ''),
      fallback: true,
      error: error.message
    }
  }
})

/**
 * Fallback response generator when OpenAI is unavailable
 */
function getFallbackResponse(userMessage: string): string {
  const lowerMessage = userMessage.toLowerCase()

  // Sales queries
  if (lowerMessage.includes('sales') || lowerMessage.includes('revenue') || lowerMessage.includes('selling')) {
    return 'Your sales are tracking well! To get detailed analytics, I need to connect to the AI service. For now, you can check your dashboard for real-time sales metrics.'
  }

  // Inventory queries
  if (lowerMessage.includes('stock') || lowerMessage.includes('inventory') || lowerMessage.includes('low')) {
    return 'I can help you manage inventory! Please check your Stock page for current levels and low-stock alerts. Would you like me to help you create a reorder list?'
  }

  // Group buying
  if (lowerMessage.includes('group') || lowerMessage.includes('bulk') || lowerMessage.includes('together')) {
    return 'Group buying is a great way to save money! Check the Group Buying page to see active pools or create your own. You can save 10-20% by ordering together with other shops.'
  }

  // Financial queries
  if (lowerMessage.includes('cash') || lowerMessage.includes('money') || lowerMessage.includes('profit') || lowerMessage.includes('financial')) {
    return 'Your financial data is available on the Dashboard. I can provide detailed insights once connected to the AI service. For now, check your Cash Flow and Profit reports.'
  }

  // Orders and purchasing
  if (lowerMessage.includes('order') || lowerMessage.includes('purchase') || lowerMessage.includes('buy')) {
    return 'Ready to order supplies? Go to the Purchasing page to browse products from your suppliers. Don\'t forget to check for group buying opportunities to save money!'
  }

  // Delivery and logistics
  if (lowerMessage.includes('delivery') || lowerMessage.includes('shipping') || lowerMessage.includes('driver')) {
    return 'Track your deliveries on the Logistics page. You can see real-time updates when orders are out for delivery and share delivery costs with other shops in your area.'
  }

  // General help
  if (lowerMessage.includes('help') || lowerMessage.includes('how') || lowerMessage.includes('what can')) {
    return 'I\'m your AI business assistant! I can help with: \n• Sales analysis and trends\n• Inventory management\n• Group buying recommendations\n• Cash flow insights\n• Order tracking\n\nWhat would you like to explore?'
  }

  // Default response
  return 'I\'m here to help you run your business better! You can ask me about sales, inventory, orders, cash flow, or anything related to your operations. What would you like to know?'
}
