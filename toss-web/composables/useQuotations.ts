import { ref } from 'vue'
import { useI18n } from 'vue-i18n'
import { useNuxtApp } from 'nuxt/app'
import type {
  Quotation,
  QuotationCustomer,
  QuotationStatus,
  SalesOrder
} from '~/types/sales'

interface QuotationFilters {
  status?: QuotationStatus | 'all'
  customerId?: string
  dateFrom?: string
  dateTo?: string
  search?: string
}

interface QuotationItemInput {
  productId: string
  name: string
  quantity: number
  unitPrice: number
  sku?: string
  description?: string
  discountRate?: number
  taxRate?: number
}

interface CreateQuotationPayload {
  customer: QuotationCustomer
  items: QuotationItemInput[]
  discountRate?: number
  vatRate?: number
  terms?: string
  notes?: string
  salesPerson?: string
  date?: string
  validUntil?: string
  status?: QuotationStatus
  attachments?: string[]
}

interface UpdateQuotationPayload extends Partial<CreateQuotationPayload> {}

interface EmailPayload {
  to: string
  subject: string
  message: string
  cc?: string[]
  bcc?: string[]
}

interface PdfResponse {
  filename: string
  base64: string
}

interface EmailResponse {
  sentAt: string
}

interface QuotationStats {
  draft: number
  sent: number
  accepted: number
  rejected: number
  expired: number
  converted: number
}

const unwrap = <T>(response: { data: T } | T): T =>
  response && typeof response === 'object' && 'data' in response ? (response as { data: T }).data : (response as T)

export const useQuotations = () => {
  const nuxtApp = useNuxtApp()
  const apiFetch = nuxtApp.$fetch as typeof globalThis.$fetch
  const { t } = useI18n()

  const quotations = ref<Quotation[]>([])
  const currentQuotation = ref<Quotation | null>(null)
  const stats = ref<QuotationStats | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const runWithState = async <T>(callback: () => Promise<T>): Promise<T> => {
    loading.value = true
    error.value = null
    try {
      return await callback()
    } catch (err: any) {
      error.value = err?.message ?? t('errors.unexpectedError', 'Unexpected error')
      throw err
    } finally {
      loading.value = false
    }
  }

  const fetchQuotations = async (filters?: QuotationFilters) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: Quotation[] } | Quotation[]>(
        '/api/sales/quotations',
        {
          method: 'GET',
          params: filters
        }
      )
      const data = unwrap(response) ?? []
      quotations.value = data
      return data
    })

  const fetchStats = async () =>
    runWithState(async () => {
      const response = await apiFetch<{ data: QuotationStats } | QuotationStats>(
        '/api/sales/quotations/stats',
        {
          method: 'GET'
        }
      )
      const data = unwrap(response)
      stats.value = data
      return data
    })

  const fetchQuotation = async (id: string) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: Quotation } | Quotation>(
        `/api/sales/quotations/${id}`,
        {
          method: 'GET'
        }
      )
      const data = unwrap(response) ?? null
      currentQuotation.value = data
      return data
    })

  const createQuotation = async (payload: CreateQuotationPayload) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: Quotation } | Quotation>(
        '/api/sales/quotations',
        {
          method: 'POST',
          body: payload
        }
      )
      const created = unwrap(response)
      quotations.value = [created, ...quotations.value]
      currentQuotation.value = created
      return created
    })

  const updateQuotation = async (id: string, payload: UpdateQuotationPayload) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: Quotation } | Quotation>(
        `/api/sales/quotations/${id}`,
        {
          method: 'PUT',
          body: payload
        }
      )
      const updated = unwrap(response)
      const index = quotations.value.findIndex((quotation) => quotation.id === id)
      if (index !== -1) {
        quotations.value.splice(index, 1, updated)
      }
      if (currentQuotation.value?.id === id) {
        currentQuotation.value = updated
      }
      return updated
    })

  const deleteQuotation = async (id: string) =>
    runWithState(async () => {
      await apiFetch(`/api/sales/quotations/${id}`, {
        method: 'DELETE'
      })
      quotations.value = quotations.value.filter((quotation) => quotation.id !== id)
      if (currentQuotation.value?.id === id) {
        currentQuotation.value = null
      }
    })

  const changeStatus = async (id: string, status: QuotationStatus, notes?: string) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: Quotation } | Quotation>(
        `/api/sales/quotations/${id}/status`,
        {
          method: 'PATCH',
          body: { status, notes }
        }
      )
      const updated = unwrap(response)
      const index = quotations.value.findIndex((quotation) => quotation.id === id)
      if (index !== -1) {
        quotations.value.splice(index, 1, updated)
      }
      if (currentQuotation.value?.id === id) {
        currentQuotation.value = updated
      }
      return updated
    })

  const duplicateQuotation = async (id: string) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: Quotation } | Quotation>(
        `/api/sales/quotations/${id}/duplicate`,
        {
          method: 'POST'
        }
      )
      const duplicated = unwrap(response)
      quotations.value = [duplicated, ...quotations.value]
      return duplicated
    })

  const convertToSalesOrder = async (id: string) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: SalesOrder } | SalesOrder>(
        `/api/sales/quotations/${id}/convert`,
        {
          method: 'POST'
        }
      )
      const order = unwrap(response)
      await fetchQuotation(id)
      return order
    })

  const sendEmail = async (id: string, payload: EmailPayload) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: EmailResponse } | EmailResponse>(
        `/api/sales/quotations/${id}/email`,
        {
          method: 'POST',
          body: payload
        }
      )
      return unwrap(response)
    })

  const generatePDF = async (id: string) =>
    runWithState(async () => {
      const response = await apiFetch<{ data: PdfResponse } | PdfResponse>(
        `/api/sales/quotations/${id}/pdf`,
        {
          method: 'GET'
        }
      )
      return unwrap(response)
    })

  const calculateTotals = (items: Array<{ quantity: number; unitPrice: number; discountRate?: number; taxRate?: number }>) => {
    let subtotal = 0
    let discountAmount = 0
    let vatAmount = 0

    items.forEach((item) => {
      const base = Number(item.quantity ?? 0) * Number(item.unitPrice ?? 0)
      const discount = (base * Number(item.discountRate ?? 0)) / 100
      const net = base - discount
      const vat = (net * Number(item.taxRate ?? 0)) / 100

      subtotal += base
      discountAmount += discount
      vatAmount += vat
    })

    const total = subtotal - discountAmount + vatAmount
    return { subtotal, discountAmount, vatAmount, total }
  }

  return {
    quotations,
    currentQuotation,
    stats,
    loading,
    error,
    fetchQuotations,
    fetchStats,
    fetchQuotation,
    createQuotation,
    updateQuotation,
    deleteQuotation,
    changeStatus,
    duplicateQuotation,
    convertToSalesOrder,
    sendEmail,
    generatePDF,
    calculateTotals
  }
}
