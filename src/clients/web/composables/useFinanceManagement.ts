// Finance Management Composable
import type { 
  Transaction, 
  FinancialOverview, 
  ExpenseCategory,
  CreateTransactionRequest,
  FinancialReport,
  BudgetItem
} from '~/services/financeService'
import { FinanceService } from '~/services/financeService'

export const useFinanceManagement = () => {
  // Reactive state
  const transactions = ref<Transaction[]>([])
  const financialOverview = ref<FinancialOverview | null>(null)
  const expenseCategories = ref<ExpenseCategory[]>([])
  const budgetItems = ref<BudgetItem[]>([])
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Computed properties
  const totalRevenue = computed(() => financialOverview.value?.totalRevenue || 0)
  const totalExpenses = computed(() => financialOverview.value?.totalExpenses || 0)
  const netProfit = computed(() => financialOverview.value?.netProfit || 0)
  const cashFlow = computed(() => financialOverview.value?.cashFlow || 0)
  const healthScore = computed(() => financialOverview.value?.healthScore || 0)
  const profitMargin = computed(() => financialOverview.value?.profitMargin || 0)
  const revenueGrowth = computed(() => financialOverview.value?.revenueGrowth || 0)
  const expenseGrowth = computed(() => financialOverview.value?.expenseGrowth || 0)

  // Recent transactions (last 10)
  const recentTransactions = computed(() => transactions.value.slice(0, 10))

  // Income vs Expense totals
  const totalIncome = computed(() => 
    transactions.value
      .filter(t => t.type === 'income')
      .reduce((sum, t) => sum + t.amount, 0)
  )

  const totalExpensesFromTransactions = computed(() => 
    transactions.value
      .filter(t => t.type === 'expense')
      .reduce((sum, t) => sum + t.amount, 0)
  )

  // Load financial overview
  const loadFinancialOverview = async (period: 'today' | 'week' | 'month' | 'quarter' | 'year' = 'month') => {
    try {
      loading.value = true
      error.value = null
      financialOverview.value = await FinanceService.getOverview(period)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load financial overview'
      console.error('Error loading financial overview:', err)
    } finally {
      loading.value = false
    }
  }

  // Load transactions
  const loadTransactions = async (params?: {
    type?: 'income' | 'expense'
    category?: string
    startDate?: string
    endDate?: string
    page?: number
    limit?: number
  }) => {
    try {
      loading.value = true
      error.value = null
      transactions.value = await FinanceService.getTransactions(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load transactions'
      console.error('Error loading transactions:', err)
    } finally {
      loading.value = false
    }
  }

  // Get single transaction
  const getTransaction = async (id: string): Promise<Transaction | null> => {
    try {
      loading.value = true
      error.value = null
      return await FinanceService.getTransaction(id)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load transaction'
      console.error('Error loading transaction:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Create new transaction
  const createTransaction = async (data: CreateTransactionRequest): Promise<Transaction | null> => {
    try {
      loading.value = true
      error.value = null
      const newTransaction = await FinanceService.createTransaction(data)
      
      // Add to local transactions array if loaded
      if (transactions.value.length > 0) {
        transactions.value.unshift(newTransaction)
      }
      
      // Refresh overview to get updated stats
      await loadFinancialOverview()
      
      return newTransaction
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create transaction'
      console.error('Error creating transaction:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Update transaction
  const updateTransaction = async (id: string, data: Partial<CreateTransactionRequest>): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      const updatedTransaction = await FinanceService.updateTransaction(id, data)
      
      // Update local transactions array
      const index = transactions.value.findIndex(t => t.id === id)
      if (index !== -1) {
        transactions.value[index] = updatedTransaction
      }
      
      // Refresh overview
      await loadFinancialOverview()
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update transaction'
      console.error('Error updating transaction:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Delete transaction
  const deleteTransaction = async (id: string): Promise<boolean> => {
    try {
      loading.value = true
      error.value = null
      await FinanceService.deleteTransaction(id)
      
      // Remove from local transactions array
      const index = transactions.value.findIndex(t => t.id === id)
      if (index !== -1) {
        transactions.value.splice(index, 1)
      }
      
      // Refresh overview
      await loadFinancialOverview()
      
      return true
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to delete transaction'
      console.error('Error deleting transaction:', err)
      return false
    } finally {
      loading.value = false
    }
  }

  // Load expense categories
  const loadExpenseCategories = async (params?: {
    period?: 'today' | 'week' | 'month' | 'quarter' | 'year'
  }) => {
    try {
      loading.value = true
      error.value = null
      expenseCategories.value = await FinanceService.getExpenseCategories(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load expense categories'
      console.error('Error loading expense categories:', err)
    } finally {
      loading.value = false
    }
  }

  // Generate financial report
  const generateReport = async (params: {
    type: 'profit_loss' | 'cash_flow' | 'balance_sheet' | 'budget_variance'
    startDate: string
    endDate: string
    format?: 'json' | 'csv' | 'pdf'
  }): Promise<FinancialReport | null> => {
    try {
      loading.value = true
      error.value = null
      return await FinanceService.generateReport(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to generate report'
      console.error('Error generating report:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Load budget comparison
  const loadBudgetComparison = async (params?: {
    period?: string
    category?: string
  }) => {
    try {
      loading.value = true
      error.value = null
      budgetItems.value = await FinanceService.getBudgetComparison(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load budget comparison'
      console.error('Error loading budget comparison:', err)
    } finally {
      loading.value = false
    }
  }

  // Export financial data
  const exportData = async (params: {
    type: 'transactions' | 'summary' | 'categories'
    format: 'csv' | 'excel' | 'pdf'
    startDate?: string
    endDate?: string
  }) => {
    try {
      loading.value = true
      error.value = null
      return await FinanceService.exportData(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to export data'
      console.error('Error exporting data:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Get cash flow projection
  const getCashFlowProjection = async (params: {
    months: number
    includeActuals: boolean
  }) => {
    try {
      loading.value = true
      error.value = null
      return await FinanceService.getCashFlowProjection(params)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to get cash flow projection'
      console.error('Error getting cash flow projection:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Calculate financial health score
  const calculateHealthScore = async () => {
    try {
      loading.value = true
      error.value = null
      return await FinanceService.calculateHealthScore()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to calculate health score'
      console.error('Error calculating health score:', err)
      return null
    } finally {
      loading.value = false
    }
  }

  // Utility functions
  const formatCurrency = (amount: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    }).format(amount)
  }

  const formatDate = (dateString: string): string => {
    const date = new Date(dateString)
    return new Intl.DateTimeFormat('en-ZA', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date)
  }

  const formatPercentage = (value: number): string => {
    return new Intl.NumberFormat('en-ZA', {
      style: 'percent',
      minimumFractionDigits: 1,
      maximumFractionDigits: 1
    }).format(value / 100)
  }

  const getTransactionTypeClass = (type: Transaction['type']): string => {
    switch (type) {
      case 'income':
        return 'bg-green-100 text-green-600 dark:bg-green-900 dark:text-green-400'
      case 'expense':
        return 'bg-red-100 text-red-600 dark:bg-red-900 dark:text-red-400'
      default:
        return 'bg-gray-100 text-gray-600 dark:bg-gray-900 dark:text-gray-400'
    }
  }

  const getTransactionTypeText = (type: Transaction['type']): string => {
    switch (type) {
      case 'income':
        return 'Income'
      case 'expense':
        return 'Expense'
      default:
        return 'Unknown'
    }
  }

  const getHealthScoreClass = (score: number): string => {
    if (score >= 80) return 'bg-green-100 text-green-600 dark:bg-green-900 dark:text-green-400'
    if (score >= 60) return 'bg-yellow-100 text-yellow-600 dark:bg-yellow-900 dark:text-yellow-400'
    return 'bg-red-100 text-red-600 dark:bg-red-900 dark:text-red-400'
  }

  const getHealthScoreText = (score: number): string => {
    if (score >= 80) return 'Excellent'
    if (score >= 60) return 'Good'
    if (score >= 40) return 'Fair'
    return 'Poor'
  }

  const getCategoryIcon = (category: string): string => {
    switch (category.toLowerCase()) {
      case 'sales':
      case 'revenue':
        return 'M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z'
      case 'overhead':
      case 'rent':
        return 'M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4'
      case 'marketing':
        return 'M11 5.882V19.24a1.76 1.76 0 01-3.417.592l-2.147-6.15M18 13a3 3 0 100-6M5.436 13.683A4.001 4.001 0 017 6h1.832c4.1 0 7.625-1.234 9.168-3v14c-1.543-1.766-5.067-3-9.168-3H7a3.988 3.988 0 01-1.564-.317z'
      case 'cost of goods':
      case 'inventory':
        return 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4'
      default:
        return 'M9 5H7a2 2 0 00-2 2v10a2 2 0 002 2h8a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2'
    }
  }

  return {
    // Reactive state
    transactions: readonly(transactions),
    financialOverview: readonly(financialOverview),
    expenseCategories: readonly(expenseCategories),
    budgetItems: readonly(budgetItems),
    loading: readonly(loading),
    error: readonly(error),
    
    // Computed properties
    totalRevenue,
    totalExpenses,
    netProfit,
    cashFlow,
    healthScore,
    profitMargin,
    revenueGrowth,
    expenseGrowth,
    recentTransactions,
    totalIncome,
    totalExpensesFromTransactions,
    
    // Methods
    loadFinancialOverview,
    loadTransactions,
    getTransaction,
    createTransaction,
    updateTransaction,
    deleteTransaction,
    loadExpenseCategories,
    generateReport,
    loadBudgetComparison,
    exportData,
    getCashFlowProjection,
    calculateHealthScore,
    
    // Utility functions
    formatCurrency,
    formatDate,
    formatPercentage,
    getTransactionTypeClass,
    getTransactionTypeText,
    getHealthScoreClass,
    getHealthScoreText,
    getCategoryIcon
  }
}

