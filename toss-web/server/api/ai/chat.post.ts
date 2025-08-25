export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  // Validate required fields
  const { message, conversationId } = body
  
  if (!message || typeof message !== 'string' || message.trim().length === 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Message is required and must be a non-empty string'
    })
  }

  // Demo AI responses based on common business queries
  const aiResponses = {
    // Inventory queries
    inventory: {
      patterns: ['inventory', 'stock', 'items', 'products', 'what do i have', 'low stock'],
      responses: [
        {
          type: 'inventory_summary',
          text: "I can see your current inventory status. You have 5 items in stock with a total value of R186,950. Here are some key alerts:\n\n• **Low Stock Alert**: Sunflower Oil (8 units) and Rice (3 units) are below minimum levels\n• **Reorder Suggestion**: I recommend ordering 50 units of Rice and 40 units of Sunflower Oil\n• **Fast Moving**: Maize Meal and Bread are your top sellers this month\n\nWould you like me to create purchase orders for the low stock items?",
          data: {
            totalItems: 5,
            totalValue: 186950,
            lowStockItems: ['Sunflower Oil', 'Rice'],
            suggestions: [
              { item: 'Rice', currentStock: 3, suggested: 50 },
              { item: 'Sunflower Oil', currentStock: 8, suggested: 40 }
            ]
          }
        }
      ]
    },
    
    // Sales queries
    sales: {
      patterns: ['sales', 'revenue', 'money', 'income', 'customers', 'today sales'],
      responses: [
        {
          type: 'sales_summary',
          text: "Here's your sales performance:\n\n**Today's Sales**: R179.48 (1 transaction)\n**This Week**: R548.47 (3 transactions)\n**This Month**: R15,247.85 (45 transactions)\n\n**Top Customers**:\n• Sipho Mthembu - R2,547.85 lifetime value\n• Williams Construction - R15,847.32 (Business customer)\n• Mary Johnson - R1,245.67\n\n**Outstanding Payments**: R219.00 from 2 customers\n\nYour average order value this month is R338.84. Would you like me to send payment reminders to overdue customers?",
          data: {
            todaysSales: 179.48,
            weekSales: 548.47,
            monthSales: 15247.85,
            outstandingPayments: 219.00,
            averageOrderValue: 338.84
          }
        }
      ]
    },
    
    // Business insights
    insights: {
      patterns: ['analyze', 'insights', 'performance', 'recommendations', 'how am i doing'],
      responses: [
        {
          type: 'business_insights',
          text: "📊 **Business Performance Analysis**\n\n**Strengths**:\n• 📈 Sales up 23% vs last month\n• 🎯 Customer retention rate: 87%\n• 💰 Profit margin improved to 34%\n\n**Opportunities**:\n• 🛒 Group buying could save R2,500/month on bulk items\n• 📱 Mobile payment options could increase sales by 15%\n• 🤝 3 nearby businesses interested in shared delivery\n\n**Action Items**:\n1. Join the community flour group purchase (saves 18%)\n2. Set up mobile payment with SnapScan\n3. Contact Nomsa's Hardware about shared logistics\n\nShall I help you implement any of these recommendations?",
          data: {
            salesGrowth: 23,
            retentionRate: 87,
            profitMargin: 34,
            savingsOpportunity: 2500
          }
        }
      ]
    },
    
    // Group buying
    groupbuying: {
      patterns: ['group', 'bulk', 'buying', 'together', 'community', 'save money'],
      responses: [
        {
          type: 'group_buying',
          text: "🤝 **Group Buying Opportunities**\n\nThere are 3 active group purchases you can join:\n\n**1. Bulk Flour Purchase** 🌾\n• Organizer: Alexandra Business Network\n• Minimum: 50kg, Current: 380kg/500kg target\n• Savings: 18% (R4.50 per kg)\n• Deadline: 3 days\n• Your suggested quantity: 25kg\n\n**2. Cleaning Supplies** 🧽\n• Organizer: Thabo's Hardware\n• Savings: 22%\n• Deadline: 1 week\n\n**3. Delivery Fuel Co-op** ⛽\n• Weekly bulk fuel purchase\n• Savings: 8% per liter\n• Next order: Tomorrow\n\nWould you like me to add you to any of these group purchases?",
          data: {
            activeGroups: 3,
            potentialSavings: 856,
            recommendedPurchases: ['Bulk Flour Purchase']
          }
        }
      ]
    },
    
    // Default helpful response
    default: {
      patterns: ['help', 'hello', 'hi', 'what can you do'],
      responses: [
        {
          type: 'general_help',
          text: "👋 Hi! I'm your AI Business Co-Pilot. I can help you with:\n\n**📦 Inventory Management**\n• Check stock levels and get reorder alerts\n• Track product performance and margins\n• Suggest optimal stock levels\n\n**💰 Sales & Finance**\n• Analyze sales performance and trends\n• Track customer payments and send reminders\n• Generate financial reports and insights\n\n**🤝 Collaborative Features**\n• Find group buying opportunities\n• Connect with nearby businesses for shared logistics\n• Manage tool sharing and equipment rental\n\n**📊 Business Intelligence**\n• Provide performance insights and recommendations\n• Forecast demand and seasonal trends\n• Benchmark against similar businesses\n\nJust ask me questions like:\n• 'How are my sales this month?'\n• 'What items need reordering?'\n• 'Find me group buying opportunities'\n• 'Analyze my business performance'\n\nWhat would you like to know about your business?",
          data: {
            capabilities: ['inventory', 'sales', 'finance', 'groupbuying', 'insights', 'collaboration']
          }
        }
      ]
    }
  }

  // Simple pattern matching to determine response type
  function getResponseType(message: string): keyof typeof aiResponses {
    const messageLower = message.toLowerCase()
    
    for (const [category, config] of Object.entries(aiResponses)) {
      if (config.patterns.some(pattern => messageLower.includes(pattern))) {
        return category as keyof typeof aiResponses
      }
    }
    
    return 'default'
  }

  const responseType = getResponseType(message)
  const responseConfig = aiResponses[responseType]
  const response = responseConfig.responses[Math.floor(Math.random() * responseConfig.responses.length)]

  // Generate conversation ID if not provided
  const finalConversationId = conversationId || `conv_${Math.random().toString(36).substr(2, 9)}`

  // Create chat response
  const chatResponse = {
    id: Math.random().toString(36).substr(2, 9),
    conversationId: finalConversationId,
    userMessage: message.trim(),
    aiResponse: response.text,
    responseType: response.type,
    data: response.data || {},
    timestamp: new Date().toISOString(),
    processingTime: Math.floor(Math.random() * 500) + 200, // Simulate processing time
    language: 'en', // Could be detected or specified
    confidence: 0.95
  }

  // In real app:
  // 1. Save conversation to database
  // 2. Call actual AI service (OpenAI, Claude, etc.)
  // 3. Update user context and preferences
  // 4. Log analytics for AI training

  return {
    success: true,
    data: chatResponse
  }
})
