import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useStockStore } from '~/stores/stock'
import { useSalesStore } from '~/stores/sales'
import { useCrmStore } from '~/stores/crm'
import { useHrStore } from '~/stores/hr'
import { useProjectsStore } from '~/stores/projects'
import { useDashboardStore } from '~/stores/dashboard'
import { useAccountingStore } from '~/stores/accounting'
import { useInventoryApi } from '~/composables/useInventoryApi'
import { useSalesApi } from '~/composables/useSalesApi'
import { useCrmApi } from '~/composables/useCrmApi'

export interface ChatMessage {
  role: 'user' | 'assistant'
  content: string
  timestamp: Date
  action?: {
    type: 'navigate' | 'create' | 'update'
    path?: string
    label?: string
  }
}

export function useChatbot() {
  const router = useRouter()
  const messages = ref<ChatMessage[]>([])
  const isLoading = ref(false)

  const stockStore = useStockStore()
  const salesStore = useSalesStore()
  const crmStore = useCrmStore()
  const hrStore = useHrStore()
  const projectsStore = useProjectsStore()
  const dashboardStore = useDashboardStore()
  const accountingStore = useAccountingStore()

  // Initialize with welcome message
  if (messages.value.length === 0) {
    messages.value.push({
      role: 'assistant',
      content: 'Hello! I\'m your TOSS AI assistant. I can help you with:\n\n• Checking stock levels and low stock alerts\n• Viewing sales and revenue data\n• Managing customers and orders\n• Employee and payroll information\n• Project and task tracking\n• Financial summaries\n\nWhat would you like to know?',
      timestamp: new Date()
    })
  }

  function formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-ZA', {
      style: 'currency',
      currency: 'ZAR'
    }).format(amount)
  }

  function formatDate(date: Date): string {
    return new Intl.DateTimeFormat('en-ZA', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    }).format(date)
  }

  async function processIntent(message: string): Promise<string> {
    const lowerMessage = message.toLowerCase()
    let response = ''
    let action: ChatMessage['action'] | undefined

    // Stock-related queries
    if (lowerMessage.includes('stock') || lowerMessage.includes('inventory') || lowerMessage.includes('item')) {
      if (lowerMessage.includes('low') || lowerMessage.includes('running out') || lowerMessage.includes('below')) {
        await stockStore.fetchItems()
        const lowStock = stockStore.lowStockItems
        if (lowStock.length === 0) {
          response = 'Great news! You don\'t have any low stock items at the moment. All your inventory levels are above the minimum threshold.'
        } else {
          response = `You have ${lowStock.length} item(s) running low:\n\n`
          lowStock.slice(0, 5).forEach(item => {
            response += `• ${item.name}: ${item.currentStock} ${item.unit} (minimum: ${item.minStock})\n`
          })
          if (lowStock.length > 5) {
            response += `\n...and ${lowStock.length - 5} more.`
          }
          action = {
            type: 'navigate',
            path: '/stock/items',
            label: 'View All Stock Items'
          }
        }
      } else if (lowerMessage.includes('search') || lowerMessage.includes('find')) {
        const searchTerm = message.replace(/.*(?:search|find)\s+(?:for\s+)?(.+)/i, '$1').trim()
        if (searchTerm) {
          await stockStore.fetchItems()
          const results = stockStore.searchItems(searchTerm)
          if (results.length === 0) {
            response = `I couldn't find any items matching "${searchTerm}".`
          } else {
            response = `Found ${results.length} item(s):\n\n`
            results.slice(0, 5).forEach(item => {
              response += `• ${item.name} (${item.code}): ${item.currentStock} ${item.unit} - ${formatCurrency(item.sellingPrice)}\n`
            })
          }
        }
      } else {
        await stockStore.fetchItems()
        const totalItems = stockStore.items.length
        const totalValue = stockStore.totalStockValue
        response = `You have ${totalItems} item(s) in stock with a total value of ${formatCurrency(totalValue)}.`
        action = {
          type: 'navigate',
          path: '/stock/items',
          label: 'View Stock'
        }
      }
    }
    // Sales-related queries
    else if (lowerMessage.includes('sale') || lowerMessage.includes('revenue') || lowerMessage.includes('income')) {
      if (lowerMessage.includes('today') || lowerMessage.includes('this day')) {
        await dashboardStore.fetchDashboardData()
        response = `Today's sales: ${formatCurrency(dashboardStore.stats.todaySales)}`
      } else if (lowerMessage.includes('week') || lowerMessage.includes('this week')) {
        await dashboardStore.fetchDashboardData()
        response = `This week:\n• Money in: ${formatCurrency(dashboardStore.stats.cashIn)}\n• Money out: ${formatCurrency(dashboardStore.stats.cashOut)}\n• Net: ${formatCurrency(dashboardStore.stats.cashIn - dashboardStore.stats.cashOut)}`
        action = {
          type: 'navigate',
          path: '/money',
          label: 'View Financial Details'
        }
      } else if (lowerMessage.includes('order') && (lowerMessage.includes('pending') || lowerMessage.includes('waiting'))) {
        await dashboardStore.fetchDashboardData()
        response = `You have ${dashboardStore.stats.pendingOrders} pending order(s).`
        action = {
          type: 'navigate',
          path: '/sales/orders',
          label: 'View Orders'
        }
      } else {
        await dashboardStore.fetchDashboardData()
        response = `Sales Summary:\n• Today: ${formatCurrency(dashboardStore.stats.todaySales)}\n• This week in: ${formatCurrency(dashboardStore.stats.cashIn)}\n• This week out: ${formatCurrency(dashboardStore.stats.cashOut)}`
      }
    }
    // Customer-related queries
    else if (lowerMessage.includes('customer') || lowerMessage.includes('client') || lowerMessage.includes('people')) {
      await crmStore.fetchCustomers()
      const customers = crmStore.customers
      const activeCustomers = crmStore.activeCustomers
      
      if (lowerMessage.includes('list') || lowerMessage.includes('all') || lowerMessage.includes('show')) {
        if (customers.length === 0) {
          response = 'You don\'t have any customers yet.'
        } else {
          response = `You have ${customers.length} customer(s), ${activeCustomers.length} active:\n\n`
          activeCustomers.slice(0, 5).forEach(customer => {
            response += `• ${customer.name} (${customer.phone})\n`
          })
          if (activeCustomers.length > 5) {
            response += `\n...and ${activeCustomers.length - 5} more.`
          }
        }
        action = {
          type: 'navigate',
          path: '/people/customers',
          label: 'View All Customers'
        }
      } else if (lowerMessage.includes('owe') || lowerMessage.includes('debt') || lowerMessage.includes('outstanding')) {
        const totalOutstanding = crmStore.totalOutstanding
        response = `Total outstanding amount: ${formatCurrency(totalOutstanding)}`
        action = {
          type: 'navigate',
          path: '/people/customers',
          label: 'View Customers'
        }
      } else {
        response = `You have ${activeCustomers.length} active customer(s) out of ${customers.length} total.`
      }
    }
    // Employee-related queries
    else if (lowerMessage.includes('employee') || lowerMessage.includes('staff') || lowerMessage.includes('worker')) {
      await hrStore.fetchEmployees()
      const employees = hrStore.activeEmployees
      
      if (employees.length === 0) {
        response = 'You don\'t have any active employees.'
      } else {
        response = `You have ${employees.length} active employee(s):\n\n`
        employees.slice(0, 5).forEach(emp => {
          response += `• ${emp.name} (${emp.rateType}: ${formatCurrency(emp.rate)}${emp.rateType === 'Hourly' ? '/hr' : emp.rateType === 'Daily' ? '/day' : '/month'})\n`
        })
        if (employees.length > 5) {
          response += `\n...and ${employees.length - 5} more.`
        }
      }
      action = {
        type: 'navigate',
        path: '/hr/employees',
        label: 'View Employees'
      }
    }
    // Project-related queries
    else if (lowerMessage.includes('project') || lowerMessage.includes('task')) {
      await projectsStore.fetchProjects()
      const activeProjects = projectsStore.activeProjects
      
      if (lowerMessage.includes('task')) {
        // Fetch tasks for all projects
        for (const project of projectsStore.projects) {
          await projectsStore.fetchTasksByProject(project.id)
        }
        const totalTasks = projectsStore.tasks.length
        const todoTasks = projectsStore.tasksByStatus.ToDo.length
        const inProgressTasks = projectsStore.tasksByStatus.InProgress.length
        const doneTasks = projectsStore.tasksByStatus.Done.length
        
        if (totalTasks === 0) {
          response = 'You don\'t have any tasks yet.'
        } else {
          response = `Task Summary:\n• Total: ${totalTasks}\n• To Do: ${todoTasks}\n• In Progress: ${inProgressTasks}\n• Done: ${doneTasks}`
        }
        action = {
          type: 'navigate',
          path: '/projects/tasks',
          label: 'View Tasks'
        }
      } else {
        if (activeProjects.length === 0) {
          response = 'You don\'t have any active projects.'
        } else {
          response = `You have ${activeProjects.length} active project(s):\n\n`
          activeProjects.slice(0, 3).forEach(project => {
            response += `• ${project.title} (${project.status})\n`
          })
          if (activeProjects.length > 3) {
            response += `\n...and ${activeProjects.length - 3} more.`
          }
        }
        action = {
          type: 'navigate',
          path: '/projects/list',
          label: 'View Projects'
        }
      }
    }
    // Financial/Money queries
    else if (lowerMessage.includes('money') || lowerMessage.includes('cash') || lowerMessage.includes('cashflow') || lowerMessage.includes('financial')) {
      await dashboardStore.fetchDashboardData()
      const net = dashboardStore.stats.cashIn - dashboardStore.stats.cashOut
      response = `Financial Summary:\n• Money in this week: ${formatCurrency(dashboardStore.stats.cashIn)}\n• Money out this week: ${formatCurrency(dashboardStore.stats.cashOut)}\n• Net: ${formatCurrency(net)}`
      
      if (net < 0) {
        response += '\n\n⚠️ Warning: You\'re spending more than you\'re earning this week.'
      }
      
      action = {
        type: 'navigate',
        path: '/money',
        label: 'View Financial Details'
      }
    }
    // Dashboard/Summary queries
    else if (lowerMessage.includes('summary') || lowerMessage.includes('overview') || lowerMessage.includes('dashboard') || lowerMessage.includes('status')) {
      await dashboardStore.fetchDashboardData()
      await stockStore.fetchItems()
      await crmStore.fetchCustomers()
      
      response = `Business Overview:\n\n`
      response += `💰 Sales: ${formatCurrency(dashboardStore.stats.todaySales)} today\n`
      response += `📦 Stock: ${stockStore.items.length} items, ${stockStore.lowStockItems.length} low\n`
      response += `👥 Customers: ${crmStore.activeCustomers.length} active\n`
      response += `📋 Orders: ${dashboardStore.stats.pendingOrders} pending\n`
      response += `💵 Cashflow: ${formatCurrency(dashboardStore.stats.cashIn - dashboardStore.stats.cashOut)} net this week`
      
      action = {
        type: 'navigate',
        path: '/',
        label: 'Go to Dashboard'
      }
    }
    // Help/What can you do
    else if (lowerMessage.includes('help') || lowerMessage.includes('what can') || lowerMessage.includes('what do') || lowerMessage.includes('capabilities')) {
      response = 'I can help you with:\n\n'
      response += '📦 **Stock & Inventory**\n'
      response += '• Check stock levels\n'
      response += '• Find low stock items\n'
      response += '• Search for products\n\n'
      response += '💰 **Sales & Revenue**\n'
      response += '• View today\'s sales\n'
      response += '• Check weekly cashflow\n'
      response += '• See pending orders\n\n'
      response += '👥 **Customers**\n'
      response += '• List customers\n'
      response += '• Check outstanding amounts\n\n'
      response += '👷 **Employees**\n'
      response += '• View active employees\n'
      response += '• Check attendance\n\n'
      response += '📊 **Projects & Tasks**\n'
      response += '• View active projects\n'
      response += '• Check task status\n\n'
      response += '💵 **Financial**\n'
      response += '• View cashflow\n'
      response += '• Get financial summary\n\n'
      response += 'Just ask me in plain language!'
    }
    // Default response
    else {
      response = 'I understand you\'re asking about: "' + message + '"\n\n'
      response += 'I can help you with:\n'
      response += '• Stock and inventory queries\n'
      response += '• Sales and revenue information\n'
      response += '• Customer management\n'
      response += '• Employee and HR data\n'
      response += '• Project and task tracking\n'
      response += '• Financial summaries\n\n'
      response += 'Try asking something like:\n'
      response += '• "Show me low stock items"\n'
      response += '• "What are today\'s sales?"\n'
      response += '• "List all customers"\n'
      response += '• "Check cashflow"'
    }

    return response
  }

  async function sendMessage(message: string) {
    // Add user message
    messages.value.push({
      role: 'user',
      content: message,
      timestamp: new Date()
    })

    isLoading.value = true

    try {
      // Process the message and get response
      const response = await processIntent(message)
      
      // Extract action if any
      let action: ChatMessage['action'] | undefined
      if (response.includes('View All Stock Items') || response.includes('View Stock')) {
        action = { type: 'navigate', path: '/stock/items', label: 'View Stock' }
      } else if (response.includes('View Financial Details') || response.includes('View Money')) {
        action = { type: 'navigate', path: '/money', label: 'View Money' }
      } else if (response.includes('View All Customers') || response.includes('View Customers')) {
        action = { type: 'navigate', path: '/people/customers', label: 'View Customers' }
      } else if (response.includes('View Employees')) {
        action = { type: 'navigate', path: '/hr/employees', label: 'View Employees' }
      } else if (response.includes('View Tasks')) {
        action = { type: 'navigate', path: '/projects/tasks', label: 'View Tasks' }
      } else if (response.includes('View Projects')) {
        action = { type: 'navigate', path: '/projects/list', label: 'View Projects' }
      } else if (response.includes('View Orders')) {
        action = { type: 'navigate', path: '/sales/orders', label: 'View Orders' }
      } else if (response.includes('Go to Dashboard')) {
        action = { type: 'navigate', path: '/', label: 'Go to Dashboard' }
      }

      // Add assistant response
      messages.value.push({
        role: 'assistant',
        content: response,
        timestamp: new Date(),
        action
      })
    } catch (error) {
      console.error('Chatbot error:', error)
      messages.value.push({
        role: 'assistant',
        content: 'Sorry, I encountered an error processing your request. Please try again.',
        timestamp: new Date()
      })
    } finally {
      isLoading.value = false
    }
  }

  function clearChat() {
    messages.value = [{
      role: 'assistant',
      content: 'Chat cleared! How can I help you today?',
      timestamp: new Date()
    }]
  }

  return {
    messages,
    isLoading,
    sendMessage,
    clearChat
  }
}

