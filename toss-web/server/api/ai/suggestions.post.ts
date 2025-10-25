/**
 * AI Suggestions API Endpoint
 * Provides proactive business suggestions based on current data
 */

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const config = useRuntimeConfig()

  const {
    shopName,
    todaySales,
    inventory,
    customers,
    lowStockAlerts,
    recentOrders
  } = body

  try {
    const apiKey = config.openaiApiKey || process.env.NUXT_PUBLIC_OPENAI_API_KEY

    if (!apiKey) {
      // Return fallback suggestions
      return {
        suggestions: generateFallbackSuggestions(body),
        fallback: true
      }
    }

    // Build context-aware prompt
    const prompt = buildSuggestionsPrompt(body)

    // Call OpenAI
    const response = await $fetch('https://api.openai.com/v1/chat/completions', {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${apiKey}`,
        'Content-Type': 'application/json'
      },
      body: {
        model: 'gpt-4-turbo-preview',
        messages: [
          {
            role: 'system',
            content: 'You are a business advisor for township shops. Provide 3-5 short, actionable suggestions based on the business data. Each suggestion should be one sentence. Focus on practical, immediate actions.'
          },
          {
            role: 'user',
            content: prompt
          }
        ],
        temperature: 0.8,
        max_tokens: 300
      }
    })

    const aiResponse = (response as any).choices[0]?.message?.content || ''
    
    // Parse suggestions (assuming AI returns numbered list)
    const suggestions = aiResponse
      .split('\n')
      .filter((line: string) => line.trim())
      .map((line: string) => line.replace(/^\d+\.\s*/, '').trim())
      .filter((s: string) => s.length > 0)
      .slice(0, 5)

    return {
      suggestions,
      model: 'gpt-4-turbo-preview'
    }
  } catch (error: any) {
    console.error('AI suggestions error:', error)
    
    return {
      suggestions: generateFallbackSuggestions(body),
      fallback: true,
      error: error.message
    }
  }
})

/**
 * Build prompt for AI suggestions
 */
function buildSuggestionsPrompt(context: any): string {
  let prompt = 'Business snapshot:\n'

  if (context.shopName) {
    prompt += `Shop: ${context.shopName}\n`
  }

  if (context.todaySales !== undefined) {
    prompt += `Today's sales: R${context.todaySales}\n`
  }

  if (context.inventory) {
    prompt += `Inventory: ${context.inventory.length} products\n`
  }

  if (context.lowStockAlerts && context.lowStockAlerts.length > 0) {
    prompt += `Low stock items: ${context.lowStockAlerts.length}\n`
    const items = context.lowStockAlerts.slice(0, 3).map((item: any) => item.productName).join(', ')
    prompt += `Critical items: ${items}\n`
  }

  if (context.customers && context.customers.length > 0) {
    prompt += `Customers: ${context.customers.length}\n`
  }

  if (context.recentOrders && context.recentOrders.length > 0) {
    prompt += `Pending orders: ${context.recentOrders.length}\n`
  }

  prompt += '\nProvide 3-5 specific, actionable suggestions to improve this business today.'

  return prompt
}

/**
 * Fallback suggestions when AI is unavailable
 */
function generateFallbackSuggestions(context: any): string[] {
  const suggestions: string[] = []

  // Check low stock
  if (context.lowStockAlerts && context.lowStockAlerts.length > 0) {
    suggestions.push(`Reorder ${context.lowStockAlerts.length} low-stock items to avoid running out`)
    
    if (context.lowStockAlerts.length >= 3) {
      suggestions.push('Consider joining a group buying pool to save 10-20% on reorders')
    }
  }

  // Sales-based suggestions
  if (context.todaySales) {
    if (context.todaySales > 3000) {
      suggestions.push('Great sales today! Review your best-sellers to ensure adequate stock')
    } else if (context.todaySales < 1000) {
      suggestions.push('Sales are slow today - consider running a promotion or special offer')
    }
  }

  // Customer engagement
  if (context.customers && context.customers.length > 50) {
    suggestions.push('You have a good customer base - consider starting a loyalty program')
  }

  // Delivery optimization
  if (context.recentOrders && context.recentOrders.length > 2) {
    suggestions.push('Multiple orders pending - coordinate shared delivery to save on transport costs')
  }

  // Weather-based (fallback - would use weather API normally)
  const isWeekend = new Date().getDay() === 0 || new Date().getDay() === 6
  if (isWeekend) {
    suggestions.push('It\'s the weekend - stock up on cold drinks and fresh bread for higher demand')
  }

  // Default suggestions if none generated
  if (suggestions.length === 0) {
    suggestions.push('Review today\'s sales trends on your dashboard')
    suggestions.push('Check for group buying opportunities to save money')
    suggestions.push('Update low-stock reorder points for automatic alerts')
  }

  return suggestions.slice(0, 5)
}

