import { defineEventHandler, getQuery } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { SalesInvoiceStatus } from '~/types/sales'

export default defineEventHandler((event) => {
  const query = getQuery(event)
  const status = (query.status as SalesInvoiceStatus | 'all' | undefined) ?? 'all'
  const customerId = query.customerId as string | undefined
  const salesOrderId = query.salesOrderId as string | undefined

  const data = MockSalesService.listSalesInvoices({
    status,
    customerId,
    salesOrderId
  })

  return { data }
})
