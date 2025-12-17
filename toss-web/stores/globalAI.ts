import { ref, computed } from 'vue'

// Global AI Assistant State
export const useGlobalAI = () => {
  // Core state
  const isOpen = ref(false)
  const isMinimized = ref(false)
  const isTyping = ref(false)
  const unreadCount = ref(3)
  
  // Chat history - persisted across modules
  const messages = ref<Array<{
    id: string
    type: 'user' | 'assistant'
    content: string
    timestamp: Date
    module?: string
    data?: any
  }>>([])
  
  // Context tracking
  const currentContext = ref({
    module: 'Dashboard',
    path: '/',
    pageTitle: '',
    availableActions: []
  })
  
  // Business data cache for AI responses
  const businessData = ref({
    revenue: {
      daily: 2450,
      weekly: 19600,
      monthly: 89450,
      growth: 12.5
    },
    customers: {
      total: 234,
      active: 23,
      repeatRate: 68
    },
    inventory: {
      totalItems: 156,
      lowStockAlerts: 3,
      criticalItems: ['Milk', 'White Bread', 'Coca Cola']
    },
    employees: {
      total: 8,
      monthlyPayroll: 45600,
      attendance: 96
    },
    financial: {
      cashOnHand: 43500,
      profitMargin: 28,
      accountsReceivable: 12300
    }
  })
  
  // AI capabilities per module
  const moduleCapabilities = {
    'Dashboard': {
      actions: ['Business Overview', 'Performance Summary', 'Quick Insights'],
      dataAccess: ['revenue', 'customers', 'inventory', 'employees']
    },
    'Sales & Selling': {
      actions: ['Sales Forecast', 'Product Analysis', 'Customer Insights', 'Revenue Optimization'],
      dataAccess: ['revenue', 'customers', 'inventory']
    },
    'Customer Relationship Management': {
      actions: ['Customer Segmentation', 'Lead Analysis', 'Pipeline Review', 'Campaign Optimization'],
      dataAccess: ['customers']
    },
    'Inventory Management': {
      actions: ['Stock Optimization', 'Reorder Recommendations', 'Demand Forecasting', 'Supplier Analysis'],
      dataAccess: ['inventory']
    },
    'Human Resources': {
      actions: ['Employee Analytics', 'Payroll Insights', 'Performance Review', 'Attendance Analysis'],
      dataAccess: ['employees']
    },
    'Accounting & Finance': {
      actions: ['Financial Analysis', 'Cash Flow Insights', 'Expense Optimization', 'Profit Analysis'],
      dataAccess: ['financial', 'revenue']
    }
  }
  
  // Computed properties
  const hasUnreadMessages = computed(() => unreadCount.value > 0)
  const currentModuleCapabilities = computed(() => 
    moduleCapabilities[currentContext.value.module] || moduleCapabilities['Dashboard']
  )
  
  // Actions
  const updateContext = (module: string, path: string, pageTitle?: string) => {
    currentContext.value = {
      module,
      path,
      pageTitle: pageTitle || module,
      availableActions: moduleCapabilities[module]?.actions || []
    }
  }
  
  const addMessage = (message: {
    type: 'user' | 'assistant'
    content: string
    data?: any
  }) => {
    messages.value.push({
      id: Date.now().toString(),
      ...message,
      timestamp: new Date(),
      module: currentContext.value.module
    })
  }
  
  const clearMessages = () => {
    messages.value = []
  }
  
  const markAllRead = () => {
    unreadCount.value = 0
  }
  
  const addUnreadCount = (count: number = 1) => {
    unreadCount.value += count
  }
  
  const toggleAssistant = () => {
    isOpen.value = !isOpen.value
    isMinimized.value = false
    if (isOpen.value) {
      markAllRead()
    }
  }
  
  const minimizeAssistant = () => {
    isOpen.value = false
    isMinimized.value = true
  }
  
  const expandAssistant = () => {
    isMinimized.value = false
    isOpen.value = true
  }
  
  // AI Response Generator
  const generateContextualResponse = (userMessage: string) => {
    const lowerMessage = userMessage.toLowerCase()
    const module = currentContext.value.module.toLowerCase()
    const capabilities = currentModuleCapabilities.value
    
    // Context-aware responses based on current module
    if (module.includes('sales') || lowerMessage.includes('sales') || lowerMessage.includes('revenue')) {
      if (lowerMessage.includes('forecast') || lowerMessage.includes('predict')) {
        return {
          content: `Based on Thabo's Spaza Shop sales patterns, I predict next week's revenue will be R${businessData.value.revenue.weekly.toLocaleString()} with 87% confidence.`,
          data: {
            metrics: [
              { label: 'Next Week', value: `R${businessData.value.revenue.weekly.toLocaleString()}` },
              { label: 'Confidence', value: '87%' },
              { label: 'Growth', value: `+${businessData.value.revenue.growth}%` }
            ],
            actions: [
              { title: 'View Sales Analytics', route: '/sales/analytics' },
              { title: 'Check Inventory', route: '/inventory' }
            ]
          }
        }
      }
      
      if (lowerMessage.includes('product') || lowerMessage.includes('selling')) {
        return {
          content: "Top performing products: White Bread (156 units), Coca Cola 2L (89 units), Milk 1L (78 units). These represent 65% of daily revenue.",
          data: {
            metrics: [
              { label: 'Top Seller', value: 'White Bread' },
              { label: 'Units Sold', value: '156' },
              { label: 'Revenue Share', value: '32%' }
            ],
            actions: [
              { title: 'View Product Details', route: '/inventory' },
              { title: 'Optimize Pricing', route: '/sales/analytics' }
            ]
          }
        }
      }
    }
    
    if (module.includes('customer') || module.includes('crm') || lowerMessage.includes('customer')) {
      return {
        content: `You have ${businessData.value.customers.total} total customers with a ${businessData.value.customers.repeatRate}% repeat rate. ${businessData.value.customers.active} customers are currently active.`,
        data: {
          metrics: [
            { label: 'Total Customers', value: businessData.value.customers.total.toString() },
            { label: 'Repeat Rate', value: `${businessData.value.customers.repeatRate}%` },
            { label: 'Active Today', value: businessData.value.customers.active.toString() }
          ],
          actions: [
            { title: 'View Customer List', route: '/crm/customers' },
            { title: 'Create Campaign', route: '/crm/automation' }
          ]
        }
      }
    }
    
    if (module.includes('inventory') || module.includes('stock') || lowerMessage.includes('inventory') || lowerMessage.includes('stock')) {
      return {
        content: `${businessData.value.inventory.lowStockAlerts} items need attention: ${businessData.value.inventory.criticalItems.join(', ')}. Immediate reorder recommended.`,
        data: {
          metrics: [
            { label: 'Total Items', value: businessData.value.inventory.totalItems.toString() },
            { label: 'Low Stock Alerts', value: businessData.value.inventory.lowStockAlerts.toString() },
            { label: 'Critical Items', value: businessData.value.inventory.criticalItems.length.toString() }
          ],
          actions: [
            { title: 'View Inventory', route: '/inventory' },
            { title: 'Create Purchase Order', route: '/purchasing/orders' }
          ]
        }
      }
    }
    
    if (module.includes('human') || module.includes('hr') || lowerMessage.includes('employee') || lowerMessage.includes('payroll')) {
      return {
        content: `${businessData.value.employees.total} active employees. Monthly payroll: R${businessData.value.employees.monthlyPayroll.toLocaleString()}. Attendance rate: ${businessData.value.employees.attendance}%.`,
        data: {
          metrics: [
            { label: 'Employees', value: businessData.value.employees.total.toString() },
            { label: 'Monthly Payroll', value: `R${businessData.value.employees.monthlyPayroll.toLocaleString()}` },
            { label: 'Attendance', value: `${businessData.value.employees.attendance}%` }
          ],
          actions: [
            { title: 'View Employees', route: '/hr/employees' },
            { title: 'Process Payroll', route: '/hr/payroll' }
          ]
        }
      }
    }
    
    if (module.includes('accounting') || module.includes('finance') || lowerMessage.includes('financial') || lowerMessage.includes('accounting')) {
      return {
        content: `Financial health is strong. Cash: R${businessData.value.financial.cashOnHand.toLocaleString()}, Profit margin: ${businessData.value.financial.profitMargin}%, Receivables: R${businessData.value.financial.accountsReceivable.toLocaleString()}.`,
        data: {
          metrics: [
            { label: 'Cash on Hand', value: `R${businessData.value.financial.cashOnHand.toLocaleString()}` },
            { label: 'Profit Margin', value: `${businessData.value.financial.profitMargin}%` },
            { label: 'Receivables', value: `R${businessData.value.financial.accountsReceivable.toLocaleString()}` }
          ],
          actions: [
            { title: 'View Accounts', route: '/accounts' },
            { title: 'Generate Report', route: '/reports/financial' }
          ]
        }
      }
    }
    
    // General business overview
    if (lowerMessage.includes('overview') || lowerMessage.includes('business') || lowerMessage.includes('summary')) {
      return {
        content: `Thabo's Spaza Shop overview: Daily revenue R${businessData.value.revenue.daily.toLocaleString()}, ${businessData.value.customers.active} customers today, ${businessData.value.inventory.lowStockAlerts} stock alerts.`,
        data: {
          metrics: [
            { label: 'Daily Revenue', value: `R${businessData.value.revenue.daily.toLocaleString()}` },
            { label: 'Customers Today', value: businessData.value.customers.active.toString() },
            { label: 'Stock Alerts', value: businessData.value.inventory.lowStockAlerts.toString() }
          ],
          actions: [
            { title: 'View Dashboard', route: '/' },
            { title: 'Check Sales', route: '/sales' },
            { title: 'Review Inventory', route: '/inventory' }
          ]
        }
      }
    }
    
    // Default response with module context
    return {
      content: `I'm ready to help with ${currentContext.value.module}! I have access to ${capabilities?.dataAccess?.join(', ') || 'business'} data and can assist with ${capabilities?.actions?.join(', ') || 'general business operations'}.`,
      data: {
        actions: [
          { title: `Explore ${currentContext.value.module}`, route: currentContext.value.path },
          { title: 'View Dashboard', route: '/' }
        ]
      }
    }
  }
  
  return {
    // State
    isOpen,
    isMinimized,
    isTyping,
    unreadCount,
    messages,
    currentContext,
    businessData,
    
    // Computed
    hasUnreadMessages,
    currentModuleCapabilities,
    
    // Actions
    updateContext,
    addMessage,
    clearMessages,
    markAllRead,
    addUnreadCount,
    toggleAssistant,
    minimizeAssistant,
    expandAssistant,
    generateContextualResponse
  }
}
