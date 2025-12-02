/**
 * OpenAI Integration Composable for TOSS AI Assistant
 * Provides real AI responses with business context
 */

import { ref } from 'vue'

export interface AIMessage {
  role: 'system' | 'user' | 'assistant'
  content: string
}

export interface AIRequestOptions {
  temperature?: number
  maxTokens?: number
  model?: string
  stream?: boolean
}

export interface BusinessContext {
  shopName?: string
  todaySales?: number
  inventory?: any[]
  customers?: any[]
  lowStockAlerts?: any[]
  recentOrders?: any[]
}

export const useOpenAI = () => {
  const config = useRuntimeConfig()
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const conversationHistory = ref<AIMessage[]>([])

  /**
   * Send a message to OpenAI with business context
   */
  const chat = async (
    userMessage: string,
    businessContext?: BusinessContext,
    options: AIRequestOptions = {}
  ): Promise<string> => {
    isLoading.value = true
    error.value = null

    try {
      // Build system prompt with business context
      const systemPrompt = buildSystemPrompt(businessContext)

      // Prepare messages
      const messages: AIMessage[] = [
        { role: 'system', content: systemPrompt },
        ...conversationHistory.value,
        { role: 'user', content: userMessage }
      ]

      // Call backend AI endpoint (which calls OpenAI)
      const response = await $fetch<{ response: string }>('/api/ai/chat', {
        method: 'POST',
        body: {
          messages,
          temperature: options.temperature ?? 0.7,
          maxTokens: options.maxTokens ?? 500,
          model: options.model ?? 'gpt-4-turbo-preview'
        }
      })

      // Add to conversation history
      conversationHistory.value.push(
        { role: 'user', content: userMessage },
        { role: 'assistant', content: response.response }
      )

      // Keep only last 10 messages to manage context window
      if (conversationHistory.value.length > 20) {
        conversationHistory.value = conversationHistory.value.slice(-20)
      }

      return response.response
    } catch (err: any) {
      error.value = err.message || 'Failed to get AI response'
      console.error('OpenAI error:', err)
      
      // Fallback to mock response
      return getFallbackResponse(userMessage, businessContext)
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Get proactive AI suggestions based on business data
   */
  const getSuggestions = async (context: BusinessContext): Promise<string[]> => {
    try {
      const response = await $fetch<{ suggestions: string[] }>('/api/ai/suggestions', {
        method: 'POST',
        body: context
      })

      return response.suggestions
    } catch (err) {
      console.error('Failed to get AI suggestions:', err)
      return getFallbackSuggestions(context)
    }
  }

  /**
   * Build system prompt with business context
   */
  const buildSystemPrompt = (context?: BusinessContext): string => {
    let prompt = `You are TOSS AI, an intelligent business assistant for township businesses in South Africa. 
You help shop owners make data-driven decisions, manage inventory, track sales, and optimize their operations.

You are friendly, supportive, and speak in simple terms. You understand the challenges of running a township business.
Provide practical, actionable advice. When suggesting actions, be specific and considerate of limited resources.`

    if (context) {
      prompt += `\n\nCurrent Business Context:\n`
      
      if (context.shopName) {
        prompt += `- Shop: ${context.shopName}\n`
      }
      
      if (context.todaySales !== undefined) {
        prompt += `- Today's Sales: R${context.todaySales.toLocaleString()}\n`
      }
      
      if (context.inventory && context.inventory.length > 0) {
        prompt += `- Total Products: ${context.inventory.length}\n`
      }
      
      if (context.lowStockAlerts && context.lowStockAlerts.length > 0) {
        prompt += `- Low Stock Alerts: ${context.lowStockAlerts.length} items need attention\n`
        prompt += `  Items: ${context.lowStockAlerts.map((item: any) => item.productName).join(', ')}\n`
      }
      
      if (context.customers && context.customers.length > 0) {
        prompt += `- Active Customers: ${context.customers.length}\n`
      }
      
      if (context.recentOrders && context.recentOrders.length > 0) {
        prompt += `- Recent Orders: ${context.recentOrders.length} orders this week\n`
      }
    }

    prompt += `\n\nAlways respond in a helpful, encouraging tone. Keep responses concise (2-3 sentences max) unless more detail is specifically requested.`

    return prompt
  }

  /**
   * Fallback response when OpenAI API fails
   */
  const getFallbackResponse = (userMessage: string, context?: BusinessContext): string => {
    const lowerMessage = userMessage.toLowerCase()

    if (lowerMessage.includes('sales') || lowerMessage.includes('revenue')) {
      const sales = context?.todaySales ?? 2450
      return `Today's sales are R${sales.toLocaleString()}. That's looking good! ${sales > 2000 ? 'You\'re doing well today!' : 'Let\'s see if we can boost those numbers.'}`
    }

    if (lowerMessage.includes('stock') || lowerMessage.includes('inventory')) {
      const alerts = context?.lowStockAlerts?.length ?? 0
      if (alerts > 0) {
        return `You have ${alerts} items running low. I recommend creating a group purchase order to save on costs. Would you like me to help with that?`
      }
      return 'Your inventory levels look healthy. No immediate action needed!'
    }

    if (lowerMessage.includes('help') || lowerMessage.includes('what can you')) {
      return 'I can help with sales analysis, inventory management, group buying recommendations, cash flow insights, and more. What would you like to know?'
    }

    return 'I\'m here to help! Could you tell me more about what you\'d like to know? I can help with sales, inventory, orders, or general business advice.'
  }

  /**
   * Fallback suggestions when API fails
   */
  const getFallbackSuggestions = (context: BusinessContext): string[] => {
    const suggestions: string[] = []

    if (context.lowStockAlerts && context.lowStockAlerts.length > 0) {
      suggestions.push(`Reorder ${context.lowStockAlerts.length} low-stock items`)
    }

    if (context.todaySales && context.todaySales > 3000) {
      suggestions.push('Great sales today! Review best-sellers')
    }

    if (context.recentOrders && context.recentOrders.length > 0) {
      suggestions.push('Check pending deliveries')
    }

    suggestions.push('Join a group buying pool to save costs')
    suggestions.push('Review this week\'s cash flow')

    return suggestions.slice(0, 3)
  }

  /**
   * Clear conversation history
   */
  const clearHistory = () => {
    conversationHistory.value = []
  }

  /**
   * Add message to history without sending
   */
  const addToHistory = (message: AIMessage) => {
    conversationHistory.value.push(message)
  }

  return {
    // State
    isLoading,
    error,
    conversationHistory,

    // Methods
    chat,
    getSuggestions,
    clearHistory,
    addToHistory
  }
}

