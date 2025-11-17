import { defineEventHandler, readBody } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { SalesOrderStatus } from '~/types/sales'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id as string
  const body = await readBody(event)
  const status = body.status as SalesOrderStatus
  const data = MockSalesService.updateSalesOrderStatus(id, status)
  return { data }
})
