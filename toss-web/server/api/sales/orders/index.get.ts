import { defineEventHandler, getQuery } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { PaymentStatus, SalesOrderStatus } from '~/types/sales'

export default defineEventHandler((event) => {
  const query = getQuery(event)
  const status = (query.status as SalesOrderStatus | 'all' | undefined) ?? 'all'
  const paymentStatus = query.paymentStatus as PaymentStatus | undefined
  const customerId = query.customerId as string | undefined
  const search = query.search as string | undefined

  const data = MockSalesService.listSalesOrders({
    status,
    paymentStatus,
    customerId,
    search
  })

  return { data }
})
