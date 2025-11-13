import { defineEventHandler, readBody } from 'h3'
import type { H3Event } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { SalesReturnStatus } from '~/types/sales'

export default defineEventHandler(async (event: H3Event) => {
  const id = event.context.params?.id as string
  const body = await readBody(event)
  const status = body.status as SalesReturnStatus
  const data = MockSalesService.updateSalesReturnStatus(id, status)
  return { data }
})
