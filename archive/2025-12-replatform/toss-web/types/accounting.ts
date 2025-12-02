// Accounting and Financial Reporting Types

export interface Account {
  id: string
  code: string
  name: string
  type: AccountType
  category: AccountCategory
  balance: number
  currency: string
  parentAccountId?: string
  isActive: boolean
  createdAt: Date
  updatedAt: Date
}

export enum AccountType {
  ASSET = 'asset',
  LIABILITY = 'liability',
  EQUITY = 'equity',
  REVENUE = 'revenue',
  EXPENSE = 'expense',
}

export enum AccountCategory {
  CURRENT_ASSET = 'current_asset',
  FIXED_ASSET = 'fixed_asset',
  CURRENT_LIABILITY = 'current_liability',
  LONG_TERM_LIABILITY = 'long_term_liability',
  EQUITY = 'equity',
  OPERATING_REVENUE = 'operating_revenue',
  NON_OPERATING_REVENUE = 'non_operating_revenue',
  OPERATING_EXPENSE = 'operating_expense',
  NON_OPERATING_EXPENSE = 'non_operating_expense',
}

export interface JournalEntry {
  id: string
  date: Date
  reference: string
  description: string
  lines: JournalLine[]
  status: 'draft' | 'posted' | 'cancelled'
  createdBy: string
  createdAt: Date
  postedAt?: Date
}

export interface JournalLine {
  id: string
  accountId: string
  accountCode: string
  accountName: string
  debit: number
  credit: number
  description?: string
}

export interface BalanceSheet {
  date: Date
  currency: string
  assets: {
    currentAssets: AccountBalance[]
    fixedAssets: AccountBalance[]
    totalAssets: number
  }
  liabilities: {
    currentLiabilities: AccountBalance[]
    longTermLiabilities: AccountBalance[]
    totalLiabilities: number
  }
  equity: {
    accounts: AccountBalance[]
    totalEquity: number
  }
  totalLiabilitiesAndEquity: number
}

export interface ProfitAndLoss {
  startDate: Date
  endDate: Date
  currency: string
  revenue: {
    operating: AccountBalance[]
    nonOperating: AccountBalance[]
    totalRevenue: number
  }
  expenses: {
    operating: AccountBalance[]
    nonOperating: AccountBalance[]
    totalExpenses: number
  }
  grossProfit: number
  operatingProfit: number
  netProfit: number
  profitMargin: number
}

export interface CashFlow {
  startDate: Date
  endDate: Date
  currency: string
  operatingActivities: {
    items: CashFlowItem[]
    total: number
  }
  investingActivities: {
    items: CashFlowItem[]
    total: number
  }
  financingActivities: {
    items: CashFlowItem[]
    total: number
  }
  netCashFlow: number
  openingBalance: number
  closingBalance: number
}

export interface CashFlowItem {
  description: string
  amount: number
}

export interface AccountBalance {
  accountId: string
  accountCode: string
  accountName: string
  balance: number
  percentage?: number
}

export interface TrialBalance {
  date: Date
  accounts: {
    accountId: string
    accountCode: string
    accountName: string
    debit: number
    credit: number
  }[]
  totalDebit: number
  totalCredit: number
  isBalanced: boolean
}

export interface FinancialRatio {
  name: string
  value: number
  formula: string
  interpretation: string
  category: 'liquidity' | 'profitability' | 'efficiency' | 'leverage'
}

export interface BudgetVsActual {
  period: string
  accountCode: string
  accountName: string
  budgeted: number
  actual: number
  variance: number
  variancePercentage: number
}

