// Finance & Accounting API Service
import { apiGet, apiPost, apiPut, apiDelete, API_ENDPOINTS, USE_MOCK_DATA, mockDelay } from '~/utils/api'

// Types
export interface Transaction {
  id: string
  type: 'income' | 'expense'
  description: string
  category: string
  amount: number
  date: string
  reference?: string
  notes?: string
  createdAt: string
  updatedAt: string
}

export interface FinancialOverview {
  totalRevenue: number
  totalExpenses: number
  netProfit: number
  cashFlow: number
  healthScore: number
  profitMargin: number
  revenueGrowth: number
  expenseGrowth: number
}

export interface ExpenseCategory {
  id: string
  name: string
  amount: number
  percentage: number
  color: string
  description?: string
}

export interface CreateTransactionRequest {
  type: 'income' | 'expense'
  description: string
  category: string
  amount: number
  date: string
  reference?: string
  notes?: string
}

export interface FinancialReport {
  id: string
  name: string
  type: 'profit_loss' | 'cash_flow' | 'balance_sheet' | 'budget_variance'
  period: {
    startDate: string
    endDate: string
  }
  data: any
  generatedAt: string
  downloadUrl?: string
}

export interface BudgetItem {
  id: string
  category: string
  budgetedAmount: number
  actualAmount: number
  variance: number
  percentage: number
  period: string
}

// Mock data
const mockTransactions: Transaction[] = [
  {
    id: '1',
    type: 'income',
    description: 'Sale - Samsung Galaxy S21',
    category: 'Sales',
    amount: 12999.00,
    date: '2024-01-16T10:30:00Z',
    reference: 'SAL-001',
    createdAt: '2024-01-16T10:30:00Z',
    updatedAt: '2024-01-16T10:30:00Z'
  },
  {
    id: '2',
    type: 'expense',
    description: 'Office Rent',
    category: 'Overhead',
    amount: 3500.00,
    date: '2024-01-16T08:00:00Z',
    reference: 'RENT-JAN',
    createdAt: '2024-01-16T08:00:00Z',
    updatedAt: '2024-01-16T08:00:00Z'
  },
  {
    id: '3',
    type: 'income',
    description: 'Sale - Nike Air Force 1',
    category: 'Sales',
    amount: 1899.00,
    date: '2024-01-16T11:15:00Z',
    reference: 'SAL-002',
    createdAt: '2024-01-16T11:15:00Z',
    updatedAt: '2024-01-16T11:15:00Z'
  },
  {
    id: '4',
    type: 'expense',
    description: 'Inventory Purchase',
    category: 'Cost of Goods',
    amount: 8750.00,
    date: '2024-01-15T14:20:00Z',
    reference: 'INV-001',
    createdAt: '2024-01-15T14:20:00Z',
    updatedAt: '2024-01-15T14:20:00Z'
  },
  {
    id: '5',
    type: 'expense',
    description: 'Marketing Campaign',
    category: 'Marketing',
    amount: 1200.00,
    date: '2024-01-15T09:00:00Z',
    reference: 'MKT-001',
    createdAt: '2024-01-15T09:00:00Z',
    updatedAt: '2024-01-15T09:00:00Z'
  }
]

const mockExpenseCategories: ExpenseCategory[] = [
  { id: '1', name: 'Cost of Goods', amount: 15750.00, percentage: 35, color: 'bg-blue-500' },
  { id: '2', name: 'Overhead', amount: 12250.00, percentage: 27, color: 'bg-red-500' },
  { id: '3', name: 'Marketing', amount: 8900.00, percentage: 20, color: 'bg-green-500' },
  { id: '4', name: 'Operations', amount: 5500.00, percentage: 12, color: 'bg-yellow-500' },
  { id: '5', name: 'Other', amount: 2723.75, percentage: 6, color: 'bg-purple-500' }
]

const mockFinancialOverview: FinancialOverview = {
  totalRevenue: 67890.50,
  totalExpenses: 45123.75,
  netProfit: 22766.75,
  cashFlow: 18450.25,
  healthScore: 85,
  profitMargin: 33.5,
  revenueGrowth: 15.2,
  expenseGrowth: 8.7
}

// Finance Service Class
export class FinanceService {
  /**
   * Get financial overview/summary
   */
  static async getOverview(period?: 'today' | 'week' | 'month' | 'quarter' | 'year'): Promise<FinancialOverview> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockFinancialOverview
    }
    
    return await apiGet<FinancialOverview>(`${API_ENDPOINTS.finance}/overview`, {
      query: { period }
    })
  }

  /**
   * Get all transactions with optional filtering
   */
  static async getTransactions(params?: {
    type?: 'income' | 'expense'
    category?: string
    startDate?: string
    endDate?: string
    page?: number
    limit?: number
  }): Promise<Transaction[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      let transactions = [...mockTransactions]
      
      if (params?.type) {
        transactions = transactions.filter(t => t.type === params.type)
      }
      
      if (params?.category) {
        transactions = transactions.filter(t => t.category === params.category)
      }
      
      return transactions.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
    }
    
    return await apiGet<Transaction[]>(`${API_ENDPOINTS.finance}/transactions`, {
      query: params
    })
  }

  /**
   * Get a single transaction by ID
   */
  static async getTransaction(id: string): Promise<Transaction> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const transaction = mockTransactions.find(t => t.id === id)
      if (!transaction) {
        throw new Error(`Transaction with ID ${id} not found`)
      }
      return transaction
    }
    
    return await apiGet<Transaction>(`${API_ENDPOINTS.finance}/transactions/${id}`)
  }

  /**
   * Create a new transaction
   */
  static async createTransaction(data: CreateTransactionRequest): Promise<Transaction> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      
      const newTransaction: Transaction = {
        id: Date.now().toString(),
        ...data,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      
      mockTransactions.unshift(newTransaction)
      return newTransaction
    }
    
    return await apiPost<Transaction>(`${API_ENDPOINTS.finance}/transactions`, data)
  }

  /**
   * Update a transaction
   */
  static async updateTransaction(id: string, data: Partial<CreateTransactionRequest>): Promise<Transaction> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockTransactions.findIndex(t => t.id === id)
      if (index === -1) {
        throw new Error(`Transaction with ID ${id} not found`)
      }
      
      mockTransactions[index] = {
        ...mockTransactions[index],
        ...data,
        updatedAt: new Date().toISOString()
      }
      
      return mockTransactions[index]
    }
    
    return await apiPut<Transaction>(`${API_ENDPOINTS.finance}/transactions/${id}`, data)
  }

  /**
   * Delete a transaction
   */
  static async deleteTransaction(id: string): Promise<void> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      const index = mockTransactions.findIndex(t => t.id === id)
      if (index === -1) {
        throw new Error(`Transaction with ID ${id} not found`)
      }
      mockTransactions.splice(index, 1)
      return
    }
    
    return await apiDelete(`${API_ENDPOINTS.finance}/transactions/${id}`)
  }

  /**
   * Get expense categories breakdown
   */
  static async getExpenseCategories(params?: {
    period?: 'today' | 'week' | 'month' | 'quarter' | 'year'
  }): Promise<ExpenseCategory[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return mockExpenseCategories
    }
    
    return await apiGet<ExpenseCategory[]>(`${API_ENDPOINTS.finance}/categories`, {
      query: params
    })
  }

  /**
   * Generate financial report
   */
  static async generateReport(params: {
    type: 'profit_loss' | 'cash_flow' | 'balance_sheet' | 'budget_variance'
    startDate: string
    endDate: string
    format?: 'json' | 'csv' | 'pdf'
  }): Promise<FinancialReport> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        id: Date.now().toString(),
        name: `${params.type.replace('_', ' ').toUpperCase()} Report`,
        type: params.type,
        period: {
          startDate: params.startDate,
          endDate: params.endDate
        },
        data: {
          totalRevenue: 67890.50,
          totalExpenses: 45123.75,
          netProfit: 22766.75,
          breakdown: mockExpenseCategories
        },
        generatedAt: new Date().toISOString(),
        downloadUrl: '#'
      }
    }
    
    return await apiPost<FinancialReport>(`${API_ENDPOINTS.finance}/reports`, params)
  }

  /**
   * Get budget vs actual comparison
   */
  static async getBudgetComparison(params?: {
    period?: string
    category?: string
  }): Promise<BudgetItem[]> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return [
        {
          id: '1',
          category: 'Sales Revenue',
          budgetedAmount: 70000.00,
          actualAmount: 67890.50,
          variance: -2109.50,
          percentage: 97.0,
          period: 'January 2024'
        },
        {
          id: '2',
          category: 'Cost of Goods',
          budgetedAmount: 18000.00,
          actualAmount: 15750.00,
          variance: 2250.00,
          percentage: 87.5,
          period: 'January 2024'
        },
        {
          id: '3',
          category: 'Operating Expenses',
          budgetedAmount: 25000.00,
          actualAmount: 29373.75,
          variance: -4373.75,
          percentage: 117.5,
          period: 'January 2024'
        }
      ]
    }
    
    return await apiGet<BudgetItem[]>(`${API_ENDPOINTS.finance}/budget-comparison`, {
      query: params
    })
  }

  /**
   * Export financial data
   */
  static async exportData(params: {
    type: 'transactions' | 'summary' | 'categories'
    format: 'csv' | 'excel' | 'pdf'
    startDate?: string
    endDate?: string
  }): Promise<{ downloadUrl: string; filename: string }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        downloadUrl: '#',
        filename: `financial_${params.type}_${new Date().getTime()}.${params.format}`
      }
    }
    
    return await apiPost(`${API_ENDPOINTS.finance}/export`, params)
  }

  /**
   * Get cash flow projection
   */
  static async getCashFlowProjection(params: {
    months: number
    includeActuals: boolean
  }): Promise<{
    projections: Array<{
      month: string
      projected: number
      actual?: number
    }>
    totalProjected: number
    confidence: number
  }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        projections: [
          { month: 'Feb 2024', projected: 22000, actual: undefined },
          { month: 'Mar 2024', projected: 24500, actual: undefined },
          { month: 'Apr 2024', projected: 26000, actual: undefined },
          { month: 'May 2024', projected: 28000, actual: undefined },
          { month: 'Jun 2024', projected: 30000, actual: undefined }
        ],
        totalProjected: 130500,
        confidence: 78
      }
    }
    
    return await apiGet(`${API_ENDPOINTS.finance}/cash-flow-projection`, {
      query: params
    })
  }

  /**
   * Calculate financial health score
   */
  static async calculateHealthScore(): Promise<{
    score: number
    factors: Array<{
      name: string
      score: number
      weight: number
      description: string
    }>
    recommendations: string[]
  }> {
    if (USE_MOCK_DATA) {
      await mockDelay()
      return {
        score: 85,
        factors: [
          {
            name: 'Profit Margin',
            score: 90,
            weight: 30,
            description: 'Strong profit margins indicate healthy pricing and cost control'
          },
          {
            name: 'Cash Flow',
            score: 85,
            weight: 25,
            description: 'Positive cash flow trends support operational stability'
          },
          {
            name: 'Revenue Growth',
            score: 80,
            weight: 20,
            description: 'Consistent revenue growth shows business expansion'
          },
          {
            name: 'Expense Control',
            score: 85,
            weight: 15,
            description: 'Well-managed expenses relative to revenue'
          },
          {
            name: 'Liquidity',
            score: 88,
            weight: 10,
            description: 'Sufficient cash reserves for operations'
          }
        ],
        recommendations: [
          'Consider diversifying revenue streams to reduce risk',
          'Monitor inventory turnover to optimize cash flow',
          'Review recurring expenses for potential savings'
        ]
      }
    }
    
    return await apiGet(`${API_ENDPOINTS.finance}/health-score`)
  }
}

// Export default service
export default FinanceService

