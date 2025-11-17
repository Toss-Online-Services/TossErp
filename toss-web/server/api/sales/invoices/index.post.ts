import { defineEventHandler, readBody } from 'h3'
import { MockSalesService } from '~/services/mock'

export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  const data = MockSalesService.createSalesInvoice(body)
  return { data }
})
