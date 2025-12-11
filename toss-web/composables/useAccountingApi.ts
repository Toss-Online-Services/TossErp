import { useApi, useAuthApi } from './useApi'
import type { Account } from '~/stores/accounting'

export function useAccountingApi() {
  const { getHeaders } = useAuthApi()
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase || 'http://localhost:5000/api'

  async function getAccounts(shopId?: number) {
    const url = shopId ? `/accounting/accounts?shopId=${shopId}` : '/accounting/accounts'
    const { data, error, execute } = useApi<Account[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createAccount(account: Partial<Account>) {
    const { data, error, execute } = useApi<{ id: number }>('/accounting/accounts', {
      method: 'POST',
      body: account,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getCashbookEntries(shopId?: number, startDate?: string, endDate?: string, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    if (shopId) params.append('shopId', shopId.toString())
    if (startDate) params.append('startDate', startDate)
    if (endDate) params.append('endDate', endDate)
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/accounting/cashbook/entries?${params.toString()}`
    const { data, error, execute } = useApi<{ items: any[], totalCount: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function recordCashIn(shopId: number, amount: number, accountId: number, description?: string, reference?: string) {
    const { data, error, execute } = useApi<{ id: number }>('/accounting/cashbook/in', {
      method: 'POST',
      body: { shopId, amount, accountId, description, reference },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function recordCashOut(shopId: number, amount: number, accountId: number, description?: string, reference?: string) {
    const { data, error, execute } = useApi<{ id: number }>('/accounting/cashbook/out', {
      method: 'POST',
      body: { shopId, amount, accountId, description, reference },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function recordTransfer(fromAccountId: number, toAccountId: number, amount: number, description?: string) {
    const { data, error, execute } = useApi<{ id: number }>('/accounting/cashbook/transfer', {
      method: 'POST',
      body: { fromAccountId, toAccountId, amount, description },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getCreditors(shopId?: number, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    if (shopId) params.append('shopId', shopId.toString())
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/accounting/creditors?${params.toString()}`
    const { data, error, execute } = useApi<{ items: any[], totalCount: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  return {
    getAccounts,
    createAccount,
    getCashbookEntries,
    recordCashIn,
    recordCashOut,
    recordTransfer,
    getCreditors
  }
}



