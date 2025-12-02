// Financial Reports composable for TOSS ERP III
import { ref } from 'vue'

// Types
export interface AccountBalance {
  accountId: string
  accountCode: string
  accountName: string
  balance: number
}

export interface BalanceSheet {
  asOfDate: Date
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
  operating: {
    items: AccountBalance[]
    netCash: number
  }
  investing: {
    items: AccountBalance[]
    netCash: number
  }
  financing: {
    items: AccountBalance[]
    netCash: number
  }
  netCashFlow: number
  openingBalance: number
  closingBalance: number
}

export interface TrialBalance {
  asOfDate: Date
  accounts: Array<{
    accountCode: string
    accountName: string
    debit: number
    credit: number
  }>
  totalDebits: number
  totalCredits: number
  isBalanced: boolean
}

export interface LedgerEntry {
  id: string
  date: Date
  reference: string
  description: string
  debit: number
  credit: number
  balance: number
  voucherType: string
  voucherNo: string
}

export const useFinancialReports = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Mock Balance Sheet
  const getBalanceSheet = async (asOfDate: Date): Promise<BalanceSheet> => {
    isLoading.value = true
    error.value = null

    try {
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 500))

      return {
        asOfDate,
        assets: {
          currentAssets: [
            { accountId: '1110', accountCode: '1110', accountName: 'Cash and Bank', balance: 150000 },
            { accountId: '1120', accountCode: '1120', accountName: 'Accounts Receivable', balance: 75000 },
            { accountId: '1130', accountCode: '1130', accountName: 'Inventory', balance: 200000 },
          ],
          fixedAssets: [
            { accountId: '1210', accountCode: '1210', accountName: 'Equipment', balance: 85000 },
            { accountId: '1220', accountCode: '1220', accountName: 'Vehicles', balance: 120000 },
            { accountId: '1230', accountCode: '1230', accountName: 'Buildings', balance: 450000 },
          ],
          totalAssets: 1080000
        },
        liabilities: {
          currentLiabilities: [
            { accountId: '2110', accountCode: '2110', accountName: 'Accounts Payable', balance: 60000 },
            { accountId: '2120', accountCode: '2120', accountName: 'Salaries Payable', balance: 30000 },
            { accountId: '2130', accountCode: '2130', accountName: 'VAT Payable', balance: 15000 },
          ],
          longTermLiabilities: [
            { accountId: '2210', accountCode: '2210', accountName: 'Bank Loan', balance: 200000 },
            { accountId: '2220', accountCode: '2220', accountName: 'Mortgage', balance: 350000 },
          ],
          totalLiabilities: 655000
        },
        equity: {
          accounts: [
            { accountId: '3100', accountCode: '3100', accountName: 'Share Capital', balance: 300000 },
            { accountId: '3200', accountCode: '3200', accountName: 'Retained Earnings', balance: 125000 },
          ],
          totalEquity: 425000
        },
        totalLiabilitiesAndEquity: 1080000
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load balance sheet'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Mock Profit & Loss
  const getProfitAndLoss = async (startDate: Date, endDate: Date): Promise<ProfitAndLoss> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 500))

      const totalRevenue = 1200000
      const totalExpenses = 850000

      return {
        startDate,
        endDate,
        revenue: {
          operating: [
            { accountId: '4100', accountCode: '4100', accountName: 'Sales Revenue', balance: 1150000 },
            { accountId: '4200', accountCode: '4200', accountName: 'Service Revenue', balance: 35000 },
          ],
          nonOperating: [
            { accountId: '4300', accountCode: '4300', accountName: 'Interest Income', balance: 15000 },
          ],
          totalRevenue
        },
        expenses: {
          operating: [
            { accountId: '5100', accountCode: '5100', accountName: 'Cost of Goods Sold', balance: 650000 },
            { accountId: '5200', accountCode: '5200', accountName: 'Salaries & Wages', balance: 120000 },
            { accountId: '5300', accountCode: '5300', accountName: 'Rent Expense', balance: 24000 },
            { accountId: '5400', accountCode: '5400', accountName: 'Utilities', balance: 18000 },
          ],
          nonOperating: [
            { accountId: '5500', accountCode: '5500', accountName: 'Interest Expense', balance: 38000 },
          ],
          totalExpenses
        },
        grossProfit: 550000,
        operatingProfit: 388000,
        netProfit: 350000,
        profitMargin: (350000 / totalRevenue) * 100
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load profit & loss'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Mock Cash Flow
  const getCashFlow = async (startDate: Date, endDate: Date): Promise<CashFlow> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 500))

      return {
        startDate,
        endDate,
        operating: {
          items: [
            { accountId: 'op1', accountCode: 'OP', accountName: 'Net Profit', balance: 350000 },
            { accountId: 'op2', accountCode: 'OP', accountName: 'Depreciation', balance: 25000 },
            { accountId: 'op3', accountCode: 'OP', accountName: 'Increase in Receivables', balance: -15000 },
            { accountId: 'op4', accountCode: 'OP', accountName: 'Increase in Payables', balance: 10000 },
          ],
          netCash: 370000
        },
        investing: {
          items: [
            { accountId: 'inv1', accountCode: 'INV', accountName: 'Purchase of Equipment', balance: -45000 },
            { accountId: 'inv2', accountCode: 'INV', accountName: 'Sale of Old Assets', balance: 15000 },
          ],
          netCash: -30000
        },
        financing: {
          items: [
            { accountId: 'fin1', accountCode: 'FIN', accountName: 'Bank Loan Received', balance: 100000 },
            { accountId: 'fin2', accountCode: 'FIN', accountName: 'Loan Repayment', balance: -50000 },
            { accountId: 'fin3', accountCode: 'FIN', accountName: 'Dividends Paid', balance: -75000 },
          ],
          netCash: -25000
        },
        netCashFlow: 315000,
        openingBalance: 85000,
        closingBalance: 400000
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load cash flow'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Mock Trial Balance
  const getTrialBalance = async (asOfDate: Date): Promise<TrialBalance> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 500))

      const accounts = [
        { accountCode: '1110', accountName: 'Cash and Bank', debit: 150000, credit: 0 },
        { accountCode: '1120', accountName: 'Accounts Receivable', debit: 75000, credit: 0 },
        { accountCode: '1130', accountName: 'Inventory', debit: 200000, credit: 0 },
        { accountCode: '2110', accountName: 'Accounts Payable', debit: 0, credit: 60000 },
        { accountCode: '2120', accountName: 'Salaries Payable', debit: 0, credit: 30000 },
        { accountCode: '3100', accountName: 'Share Capital', debit: 0, credit: 300000 },
        { accountCode: '4100', accountName: 'Sales Revenue', debit: 0, credit: 1150000 },
        { accountCode: '5100', accountName: 'Cost of Goods Sold', debit: 650000, credit: 0 },
        { accountCode: '5200', accountName: 'Salaries & Wages', debit: 120000, credit: 0 },
        { accountCode: '5300', accountName: 'Rent Expense', debit: 24000, credit: 0 },
      ]

      const totalDebits = accounts.reduce((sum, acc) => sum + acc.debit, 0)
      const totalCredits = accounts.reduce((sum, acc) => sum + acc.credit, 0)

      return {
        asOfDate,
        accounts,
        totalDebits,
        totalCredits,
        isBalanced: totalDebits === totalCredits
      }
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load trial balance'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Mock General Ledger
  const getGeneralLedger = async (
    accountId: string,
    startDate: Date,
    endDate: Date
  ): Promise<LedgerEntry[]> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 500))

      return [
        {
          id: '1',
          date: new Date('2024-01-15'),
          reference: 'JE-2024-001',
          description: 'Sales Invoice Payment',
          debit: 11500,
          credit: 0,
          balance: 11500,
          voucherType: 'Journal Entry',
          voucherNo: 'JE-2024-001'
        },
        {
          id: '2',
          date: new Date('2024-01-20'),
          reference: 'JE-2024-002',
          description: 'Supplier Payment',
          debit: 0,
          credit: 5000,
          balance: 6500,
          voucherType: 'Journal Entry',
          voucherNo: 'JE-2024-002'
        },
        {
          id: '3',
          date: new Date('2024-02-01'),
          reference: 'JE-2024-003',
          description: 'Cash Deposit',
          debit: 25000,
          credit: 0,
          balance: 31500,
          voucherType: 'Journal Entry',
          voucherNo: 'JE-2024-003'
        },
      ]
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load general ledger'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getBalanceSheet,
    getProfitAndLoss,
    getCashFlow,
    getTrialBalance,
    getGeneralLedger
  }
}
