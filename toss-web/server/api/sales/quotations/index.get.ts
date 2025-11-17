import { defineEventHandler, getQuery } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { QuotationStatus } from '~/types/sales'

export default defineEventHandler((event) => {
  const query = getQuery(event)
  const status = (query.status as QuotationStatus | 'all' | undefined) ?? 'all'
  const customerId = query.customerId as string | undefined
  const search = query.search as string | undefined
  const dateFrom = query.dateFrom as string | undefined
  const dateTo = query.dateTo as string | undefined

  const data = MockSalesService.listQuotations({
    status,
    customerId,
    search,
    dateFrom,
    dateTo
  })

  return { data }
})
