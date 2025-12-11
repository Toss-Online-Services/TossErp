import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useAccountingApi } from '~/composables/useAccountingApi'

export type AccountType = 'asset' | 'liability' | 'equity' | 'income' | 'expense'
export type AccountSubType = 
  | 'current_asset' | 'fixed_asset' | 'current_liability' | 'long_term_liability'
  | 'equity' | 'revenue' | 'cost_of_sales' | 'operating_expense' | 'other_expense'

export interface Account {
  id: string
  code: string
  name: string
  type: AccountType
  subType?: AccountSubType
  parentId?: string
  balance: number
  isActive: boolean
  description?: string
  createdAt: Date
  updatedAt: Date
}

export interface JournalEntryLine {
  id: string
  accountId: string
  accountCode: string
  accountName: string
  debit: number
  credit: number
  description?: string
}

export interface JournalEntry {
  id: string
  entryNumber: string
  date: Date
  reference?: string
  description?: string
  lines: JournalEntryLine[]
  totalDebit: number
  totalCredit: number
  isBalanced: boolean
  status: 'draft' | 'posted' | 'cancelled'
  postedBy?: string
  postedAt?: Date
  createdBy: string
  createdAt: Date
}

export const useAccountingStore = defineStore('accounting', () => {
  const accountingApi = useAccountingApi()
  // State
  const accounts = ref<Account[]>([])
  const journalEntries = ref<JournalEntry[]>([])
  const loading = ref(false)

  // Computed
  const activeAccounts = computed(() => {
    return accounts.value.filter(a => a.isActive)
  })

  const accountsByType = computed(() => {
    const grouped: Record<AccountType, Account[]> = {
      asset: [],
      liability: [],
      equity: [],
      income: [],
      expense: []
    }
    accounts.value.forEach(account => {
      if (grouped[account.type]) {
        grouped[account.type].push(account)
      }
    })
    return grouped
  })

  const chartOfAccounts = computed(() => {
    // Group by type and sort by code
    const grouped = accountsByType.value
    return Object.entries(grouped).map(([type, accounts]) => ({
      type: type as AccountType,
      accounts: accounts.sort((a, b) => a.code.localeCompare(b.code))
    }))
  })

  const postedEntries = computed(() => {
    return journalEntries.value.filter(entry => entry.status === 'posted')
  })

  const draftEntries = computed(() => {
    return journalEntries.value.filter(entry => entry.status === 'draft')
  })

  const totalAssets = computed(() => {
    return accounts.value
      .filter(a => a.type === 'asset' && a.isActive)
      .reduce((sum, a) => sum + a.balance, 0)
  })

  const totalLiabilities = computed(() => {
    return accounts.value
      .filter(a => a.type === 'liability' && a.isActive)
      .reduce((sum, a) => sum + a.balance, 0)
  })

  const totalEquity = computed(() => {
    return accounts.value
      .filter(a => a.type === 'equity' && a.isActive)
      .reduce((sum, a) => sum + a.balance, 0)
  })

  const totalIncome = computed(() => {
    return accounts.value
      .filter(a => a.type === 'income' && a.isActive)
      .reduce((sum, a) => sum + a.balance, 0)
  })

  const totalExpenses = computed(() => {
    return accounts.value
      .filter(a => a.type === 'expense' && a.isActive)
      .reduce((sum, a) => sum + a.balance, 0)
  })

  const netIncome = computed(() => {
    return totalIncome.value - totalExpenses.value
  })

  // Actions
  async function fetchAccounts(shopId?: number) {
    loading.value = true
    try {
      const { data, error } = await accountingApi.getAccounts(shopId)
      if (error.value) {
        console.error('Failed to fetch accounts:', error.value)
        return
      }
      accounts.value = (data.value ?? []).map(mapAccountFromApi)
    } finally {
      loading.value = false
    }
  }

  async function fetchJournalEntries() {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      // Mock data
      journalEntries.value = [
        {
          id: 'je-1',
          entryNumber: 'JE-2025-001',
          date: new Date('2025-01-15'),
          reference: 'INV-001',
          description: 'Record sales invoice',
          lines: [
            {
              id: 'jel-1',
              accountId: 'acc-2',
              accountCode: '1100',
              accountName: 'Accounts Receivable',
              debit: 11500,
              credit: 0,
              description: 'Invoice amount'
            },
            {
              id: 'jel-2',
              accountId: 'acc-9',
              accountCode: '4000',
              accountName: 'Sales Revenue',
              debit: 0,
              credit: 10000,
              description: 'Sales revenue'
            },
            {
              id: 'jel-3',
              accountId: 'acc-11',
              accountCode: '5000',
              accountName: 'Cost of Goods Sold',
              debit: 6000,
              credit: 0,
              description: 'COGS'
            },
            {
              id: 'jel-4',
              accountId: 'acc-3',
              accountCode: '1200',
              accountName: 'Inventory',
              debit: 0,
              credit: 6000,
              description: 'Inventory reduction'
            }
          ],
          totalDebit: 17500,
          totalCredit: 16000,
          isBalanced: false,
          status: 'draft',
          createdBy: 'admin',
          createdAt: new Date('2025-01-15')
        },
        {
          id: 'je-2',
          entryNumber: 'JE-2025-002',
          date: new Date('2025-01-20'),
          reference: 'PAY-001',
          description: 'Record payment received',
          lines: [
            {
              id: 'jel-5',
              accountId: 'acc-1',
              accountCode: '1000',
              accountName: 'Cash',
              debit: 11500,
              credit: 0,
              description: 'Payment received'
            },
            {
              id: 'jel-6',
              accountId: 'acc-2',
              accountCode: '1100',
              accountName: 'Accounts Receivable',
              debit: 0,
              credit: 11500,
              description: 'Clear receivable'
            }
          ],
          totalDebit: 11500,
          totalCredit: 11500,
          isBalanced: true,
          status: 'posted',
          postedBy: 'admin',
          postedAt: new Date('2025-01-20'),
          createdBy: 'admin',
          createdAt: new Date('2025-01-20')
        }
      ]
    } catch (error) {
      console.error('Failed to fetch journal entries:', error)
    } finally {
      loading.value = false
    }
  }

  async function createAccount(data: Omit<Account, 'id' | 'createdAt' | 'updatedAt'>) {
    loading.value = true
    try {
      const { data: created, error } = await accountingApi.createAccount(data)
      if (error.value) {
        console.error('Failed to create account:', error.value)
        throw error.value
      }

      const account: Account = {
        ...data,
        id: created.value?.id ? String(created.value.id) : `acc-${Date.now()}`,
        createdAt: new Date(),
        updatedAt: new Date()
      }

      accounts.value.push(account)
      return account
    } catch (error) {
      console.error('Failed to create account:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function updateAccount(id: string, updates: Partial<Account>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const index = accounts.value.findIndex(a => a.id === id)
      if (index !== -1) {
        accounts.value[index] = {
          ...accounts.value[index],
          ...updates,
          updatedAt: new Date()
        }
      }
    } catch (error) {
      console.error('Failed to update account:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function createJournalEntry(data: Omit<JournalEntry, 'id' | 'entryNumber' | 'totalDebit' | 'totalCredit' | 'isBalanced' | 'createdAt'>) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const totalDebit = data.lines.reduce((sum, line) => sum + line.debit, 0)
      const totalCredit = data.lines.reduce((sum, line) => sum + line.credit, 0)
      const isBalanced = totalDebit === totalCredit
      
      const entry: JournalEntry = {
        ...data,
        id: `je-${Date.now()}`,
        entryNumber: generateEntryNumber(),
        totalDebit,
        totalCredit,
        isBalanced,
        createdAt: new Date()
      }
      
      journalEntries.value.unshift(entry)
      return entry
    } catch (error) {
      console.error('Failed to create journal entry:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  async function postJournalEntry(id: string) {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 500))
      
      const entry = journalEntries.value.find(e => e.id === id)
      if (entry && entry.isBalanced) {
        entry.status = 'posted'
        entry.postedBy = 'admin' // TODO: Get from auth
        entry.postedAt = new Date()
        
        // Update account balances
        entry.lines.forEach(line => {
          const account = accounts.value.find(a => a.id === line.accountId)
          if (account) {
            if (line.debit > 0) {
              // Debit increases assets/expenses, decreases liabilities/equity/income
              if (account.type === 'asset' || account.type === 'expense') {
                account.balance += line.debit
              } else {
                account.balance -= line.debit
              }
            }
            if (line.credit > 0) {
              // Credit increases liabilities/equity/income, decreases assets/expenses
              if (account.type === 'liability' || account.type === 'equity' || account.type === 'income') {
                account.balance += line.credit
              } else {
                account.balance -= line.credit
              }
            }
          }
        })
      }
    } catch (error) {
      console.error('Failed to post journal entry:', error)
      throw error
    } finally {
      loading.value = false
    }
  }

  function generateEntryNumber(): string {
    const date = new Date()
    const year = date.getFullYear().toString().substr(-2)
    const month = (date.getMonth() + 1).toString().padStart(2, '0')
    const sequence = (journalEntries.value.length + 1).toString().padStart(3, '0')
    return `JE-${year}${month}-${sequence}`
  }

  function getAccountById(id: string): Account | undefined {
    return accounts.value.find(a => a.id === id)
  }

  function getJournalEntryById(id: string): JournalEntry | undefined {
    return journalEntries.value.find(e => e.id === id)
  }

  function getAccountTypeLabel(type: AccountType): string {
    const labels: Record<AccountType, string> = {
      asset: 'Asset',
      liability: 'Liability',
      equity: 'Equity',
      income: 'Income',
      expense: 'Expense'
    }
    return labels[type] || type
  }

  function mapAccountFromApi(account: any): Account {
    return {
      id: String(account.id ?? crypto.randomUUID()),
      code: account.code ?? '',
      name: account.name ?? 'Account',
      type: account.type ?? 'asset',
      subType: account.subType,
      parentId: account.parentId ? String(account.parentId) : undefined,
      balance: account.balance ?? 0,
      isActive: account.isActive ?? true,
      description: account.description,
      createdAt: account.createdAt ? new Date(account.createdAt) : new Date(),
      updatedAt: account.updatedAt ? new Date(account.updatedAt) : new Date()
    }
  }

  return {
    // State
    accounts,
    journalEntries,
    loading,
    // Computed
    activeAccounts,
    accountsByType,
    chartOfAccounts,
    postedEntries,
    draftEntries,
    totalAssets,
    totalLiabilities,
    totalEquity,
    totalIncome,
    totalExpenses,
    netIncome,
    // Actions
    fetchAccounts,
    fetchJournalEntries,
    createAccount,
    updateAccount,
    createJournalEntry,
    postJournalEntry,
    getAccountById,
    getJournalEntryById,
    getAccountTypeLabel
  }
})

