// Accounting composable for TOSS ERP III
import { ref } from 'vue'

// Types
export interface Account {
  id: string
  code: string
  name: string
  type: 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense'
  parentId?: string
  parent?: string
  isGroup: boolean
  balance: number
  isActive: boolean
  level: number
  hasChildren: boolean
  openingBalance?: number
}

export interface JournalEntry {
  id: string
  entryNumber: string
  date: string
  reference: string
  description: string
  lineItems: JournalLineItem[]
  totalDebit: number
  totalCredit: number
  status: 'Draft' | 'Posted' | 'Cancelled'
  postedBy?: string
  postedAt?: string
}

export interface JournalLineItem {
  id: string
  accountId: string
  accountCode: string
  accountName: string
  debit: number
  credit: number
  description?: string
}

export interface CreateAccountRequest {
  code: string
  name: string
  type: 'Asset' | 'Liability' | 'Equity' | 'Revenue' | 'Expense'
  parentId?: string
  isGroup: boolean
  openingBalance?: number
  isActive: boolean
}

export interface CreateJournalEntryRequest {
  date: string
  reference: string
  description: string
  lineItems: Array<{
    accountId: string
    debit: number
    credit: number
    description?: string
  }>
}

export const useAccounting = () => {
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  // Mock Chart of Accounts
  const getMockAccounts = (): Account[] => [
    { id: '1', code: '1000', name: 'Assets', type: 'Asset', isGroup: true, balance: 0, isActive: true, level: 0, hasChildren: true },
    { id: '2', code: '1100', name: 'Current Assets', type: 'Asset', parentId: '1', parent: 'Assets', isGroup: true, balance: 0, isActive: true, level: 1, hasChildren: true },
    { id: '3', code: '1110', name: 'Cash and Bank', type: 'Asset', parentId: '2', parent: 'Current Assets', isGroup: false, balance: 150000, isActive: true, level: 2, hasChildren: false },
    { id: '4', code: '1120', name: 'Accounts Receivable', type: 'Asset', parentId: '2', parent: 'Current Assets', isGroup: false, balance: 75000, isActive: true, level: 2, hasChildren: false },
    { id: '5', code: '1130', name: 'Inventory', type: 'Asset', parentId: '2', parent: 'Current Assets', isGroup: false, balance: 200000, isActive: true, level: 2, hasChildren: false },
    { id: '6', code: '2000', name: 'Liabilities', type: 'Liability', isGroup: true, balance: 0, isActive: true, level: 0, hasChildren: true },
    { id: '7', code: '2100', name: 'Current Liabilities', type: 'Liability', parentId: '6', parent: 'Liabilities', isGroup: true, balance: 0, isActive: true, level: 1, hasChildren: true },
    { id: '8', code: '2110', name: 'Accounts Payable', type: 'Liability', parentId: '7', parent: 'Current Liabilities', isGroup: false, balance: 60000, isActive: true, level: 2, hasChildren: false },
    { id: '9', code: '2120', name: 'Salaries Payable', type: 'Liability', parentId: '7', parent: 'Current Liabilities', isGroup: false, balance: 30000, isActive: true, level: 2, hasChildren: false },
    { id: '10', code: '3000', name: 'Equity', type: 'Equity', isGroup: true, balance: 0, isActive: true, level: 0, hasChildren: true },
    { id: '11', code: '3100', name: 'Share Capital', type: 'Equity', parentId: '10', parent: 'Equity', isGroup: false, balance: 300000, isActive: true, level: 1, hasChildren: false },
    { id: '12', code: '4000', name: 'Revenue', type: 'Revenue', isGroup: true, balance: 0, isActive: true, level: 0, hasChildren: true },
    { id: '13', code: '4100', name: 'Sales Revenue', type: 'Revenue', parentId: '12', parent: 'Revenue', isGroup: false, balance: 1200000, isActive: true, level: 1, hasChildren: false },
    { id: '14', code: '5000', name: 'Expenses', type: 'Expense', isGroup: true, balance: 0, isActive: true, level: 0, hasChildren: true },
    { id: '15', code: '5100', name: 'Cost of Goods Sold', type: 'Expense', parentId: '14', parent: 'Expenses', isGroup: false, balance: 650000, isActive: true, level: 1, hasChildren: false },
    { id: '16', code: '5200', name: 'Operating Expenses', type: 'Expense', parentId: '14', parent: 'Expenses', isGroup: false, balance: 215000, isActive: true, level: 1, hasChildren: false },
  ]

  // Get all accounts
  const getAccounts = async (): Promise<Account[]> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      return getMockAccounts()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load accounts'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Create account
  const createAccount = async (data: CreateAccountRequest): Promise<Account> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      
      const newAccount: Account = {
        id: Date.now().toString(),
        code: data.code,
        name: data.name,
        type: data.type,
        parentId: data.parentId,
        isGroup: data.isGroup,
        balance: data.openingBalance || 0,
        isActive: data.isActive,
        level: data.parentId ? 1 : 0,
        hasChildren: false
      }

      return newAccount
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create account'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Update account
  const updateAccount = async (id: string, data: Partial<CreateAccountRequest>): Promise<void> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      // Mock update
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to update account'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Delete account
  const deleteAccount = async (id: string): Promise<void> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      // Mock delete
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to delete account'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Mock Journal Entries
  const getMockJournalEntries = (): JournalEntry[] => [
    {
      id: '1',
      entryNumber: 'JE-2024-001',
      date: '2024-01-15',
      reference: 'INV-001',
      description: 'Sales Invoice Payment',
      lineItems: [
        { id: '1-1', accountId: '3', accountCode: '1110', accountName: 'Cash and Bank', debit: 11500, credit: 0 },
        { id: '1-2', accountId: '13', accountCode: '4100', accountName: 'Sales Revenue', debit: 0, credit: 10000 },
        { id: '1-3', accountId: 'vat', accountCode: '2130', accountName: 'VAT Payable', debit: 0, credit: 1500 },
      ],
      totalDebit: 11500,
      totalCredit: 11500,
      status: 'Posted',
      postedBy: 'Admin',
      postedAt: '2024-01-15T10:30:00Z'
    },
    {
      id: '2',
      entryNumber: 'JE-2024-002',
      date: '2024-01-20',
      reference: 'PAY-123',
      description: 'Supplier Payment',
      lineItems: [
        { id: '2-1', accountId: '8', accountCode: '2110', accountName: 'Accounts Payable', debit: 5000, credit: 0 },
        { id: '2-2', accountId: '3', accountCode: '1110', accountName: 'Cash and Bank', debit: 0, credit: 5000 },
      ],
      totalDebit: 5000,
      totalCredit: 5000,
      status: 'Posted',
      postedBy: 'Admin',
      postedAt: '2024-01-20T14:20:00Z'
    },
    {
      id: '3',
      entryNumber: 'JE-2024-003',
      date: '2024-02-01',
      reference: 'ADJ-001',
      description: 'Month-end Accruals',
      lineItems: [
        { id: '3-1', accountId: '16', accountCode: '5200', accountName: 'Operating Expenses', debit: 15000, credit: 0 },
        { id: '3-2', accountId: '9', accountCode: '2120', accountName: 'Salaries Payable', debit: 0, credit: 15000 },
      ],
      totalDebit: 15000,
      totalCredit: 15000,
      status: 'Posted'
    },
    {
      id: '4',
      entryNumber: 'JE-2024-004',
      date: '2024-02-10',
      reference: 'DEP-001',
      description: 'Depreciation Entry',
      lineItems: [
        { id: '4-1', accountId: '16', accountCode: '5200', accountName: 'Operating Expenses', debit: 3500, credit: 0 },
        { id: '4-2', accountId: 'acc', accountCode: '1299', accountName: 'Accumulated Depreciation', debit: 0, credit: 3500 },
      ],
      totalDebit: 3500,
      totalCredit: 3500,
      status: 'Posted'
    },
    {
      id: '5',
      entryNumber: 'JE-2024-005',
      date: '2024-02-15',
      reference: 'DRAFT',
      description: 'Pending Adjustment',
      lineItems: [
        { id: '5-1', accountId: '3', accountCode: '1110', accountName: 'Cash and Bank', debit: 7500, credit: 0 },
        { id: '5-2', accountId: '4', accountCode: '1120', accountName: 'Accounts Receivable', debit: 0, credit: 7500 },
      ],
      totalDebit: 7500,
      totalCredit: 7500,
      status: 'Draft'
    },
  ]

  // Get journal entries
  const getJournalEntries = async (): Promise<JournalEntry[]> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      return getMockJournalEntries()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to load journal entries'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Create journal entry
  const createJournalEntry = async (data: CreateJournalEntryRequest): Promise<JournalEntry> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))

      // Validate that debits equal credits
      const totalDebit = data.lineItems.reduce((sum, item) => sum + item.debit, 0)
      const totalCredit = data.lineItems.reduce((sum, item) => sum + item.credit, 0)

      if (Math.abs(totalDebit - totalCredit) > 0.01) {
        throw new Error('Debits must equal credits')
      }

      const newEntry: JournalEntry = {
        id: Date.now().toString(),
        entryNumber: `JE-${new Date().getFullYear()}-${Date.now().toString().slice(-3)}`,
        date: data.date,
        reference: data.reference,
        description: data.description,
        lineItems: data.lineItems.map((item, index) => ({
          id: `${Date.now()}-${index}`,
          accountId: item.accountId,
          accountCode: '', // Would be fetched from account
          accountName: '', // Would be fetched from account
          debit: item.debit,
          credit: item.credit,
          description: item.description
        })),
        totalDebit,
        totalCredit,
        status: 'Draft'
      }

      return newEntry
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to create journal entry'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Post journal entry
  const postJournalEntry = async (id: string): Promise<void> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      // Mock posting - would update ledger in real app
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to post journal entry'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  // Reverse journal entry
  const reverseJournalEntry = async (id: string, reversalDate: string): Promise<JournalEntry> => {
    isLoading.value = true
    error.value = null

    try {
      await new Promise(resolve => setTimeout(resolve, 300))
      
      // Mock reversal - creates opposite entry
      const newEntry: JournalEntry = {
        id: Date.now().toString(),
        entryNumber: `JE-${new Date().getFullYear()}-REV-${Date.now().toString().slice(-3)}`,
        date: reversalDate,
        reference: `REV-${id}`,
        description: `Reversal of ${id}`,
        lineItems: [],
        totalDebit: 0,
        totalCredit: 0,
        status: 'Posted'
      }

      return newEntry
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Failed to reverse journal entry'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  return {
    isLoading,
    error,
    getAccounts,
    createAccount,
    updateAccount,
    deleteAccount,
    getJournalEntries,
    createJournalEntry,
    postJournalEntry,
    reverseJournalEntry
  }
}

