import { defineEventHandler, getQuery } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { SalesReturnStatus } from '~/types/sales'

export default defineEventHandler((event) => {
  const query = getQuery(event)
  const status = (query.status as SalesReturnStatus | 'all' | undefined) ?? 'all'
  const customerId = query.customerId as string | undefined
  const invoiceId = query.invoiceId as string | undefined

  const data = MockSalesService.listSalesReturns({
    status,
    customerId,
    invoiceId
  })

  return { data }
})
