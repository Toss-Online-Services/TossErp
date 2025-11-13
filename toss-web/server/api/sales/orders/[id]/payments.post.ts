import { defineEventHandler, readBody } from 'h3'
import { MockSalesService } from '~/services/mock'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id as string
  const body = await readBody(event)
  const data = MockSalesService.recordSalesOrderPayment(id, body)
  return { data }
})
