import { useApi, useAuthApi } from './useApi'
import type { Sale, SalesOrder, Quotation, DeliveryNote, Invoice } from '~/stores/sales'

export function useSalesApi() {
  const { getHeaders } = useAuthApi()
  const config = useRuntimeConfig()
  const baseURL = config.public.apiBase || 'http://localhost:5000/api'

  async function getSales(shopId?: number, status?: string, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    if (shopId) params.append('shopId', shopId.toString())
    if (status) params.append('status', status)
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/sales?${params.toString()}`
    const { data, error, execute } = useApi<{ items: Sale[], totalCount: number, pageNumber: number, totalPages: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getSaleById(id: number) {
    const { data, error, execute } = useApi<Sale>(`/sales/${id}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getDailySummary(shopId: number, date?: string) {
    const params = new URLSearchParams()
    params.append('shopId', shopId.toString())
    if (date) params.append('date', date)
    
    const url = `/sales/daily-summary?${params.toString()}`
    const { data, error, execute } = useApi<{ totalSales: number, totalItems: number, totalTransactions: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createSale(sale: Partial<Sale>) {
    const { data, error, execute } = useApi<{ id: number }>('/sales', {
      method: 'POST',
      body: sale,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function posCheckout(cartItems: any[], paymentMethod: string, amountPaid: number, customerId?: number) {
    const { data, error, execute } = useApi<{ saleId: number }>('/sales/pos/checkout', {
      method: 'POST',
      body: { cartItems, paymentMethod, amountPaid, customerId },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function voidSale(id: number, reason?: string) {
    const { data, error, execute } = useApi(`/sales/${id}/void`, {
      method: 'POST',
      body: { reason },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateSaleStatus(id: number, status: string) {
    const { data, error, execute } = useApi(`/sales/${id}/status`, {
      method: 'POST',
      body: { status },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function processRefund(id: number, amount: number, restockItems: boolean) {
    const { data, error, execute } = useApi(`/sales/${id}/refund`, {
      method: 'POST',
      body: { amount, restockItems },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getInvoices(shopId?: number, status?: string, pageNumber = 1, pageSize = 50) {
    const params = new URLSearchParams()
    if (shopId) params.append('shopId', shopId.toString())
    if (status) params.append('status', status)
    params.append('pageNumber', pageNumber.toString())
    params.append('pageSize', pageSize.toString())
    
    const url = `/sales/invoices?${params.toString()}`
    const { data, error, execute } = useApi<{ items: Invoice[], totalCount: number }>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getQuotes(shopId?: number) {
    const url = shopId ? `/sales/quotes?shopId=${shopId}` : '/sales/quotes'
    const { data, error, execute } = useApi<Quotation[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getOrders(shopId?: number) {
    const url = shopId ? `/sales/orders?shopId=${shopId}` : '/sales/orders'
    const { data, error, execute } = useApi<SalesOrder[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getDeliveryNotes(shopId?: number) {
    const url = shopId ? `/sales/delivery-notes?shopId=${shopId}` : '/sales/delivery-notes'
    const { data, error, execute } = useApi<DeliveryNote[]>(url, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function createInvoice(invoice: Partial<Invoice>) {
    const { data, error, execute } = useApi<{ id: number }>('/sales/invoices', {
      method: 'POST',
      body: invoice,
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function updateInvoiceStatus(id: number, status: string) {
    const { data, error, execute } = useApi(`/sales/invoices/${id}/status`, {
      method: 'POST',
      body: { status },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function payInvoice(id: number, amount: number) {
    const { data, error, execute } = useApi(`/sales/invoices/${id}/pay`, {
      method: 'POST',
      body: { amount },
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  async function getQueueOrders(shopId: number) {
    const { data, error, execute } = useApi<Sale[]>(`/sales/queue?shopId=${shopId}`, {
      method: 'GET',
      headers: getHeaders()
    })
    await execute()
    return { data, error }
  }

  return {
    getSales,
    getSaleById,
    getDailySummary,
    createSale,
    posCheckout,
    voidSale,
    updateSaleStatus,
    processRefund,
    getInvoices,
    createInvoice,
    updateInvoiceStatus,
    payInvoice,
    getQueueOrders,
    getQuotes,
    getOrders,
    getDeliveryNotes
  }
}



