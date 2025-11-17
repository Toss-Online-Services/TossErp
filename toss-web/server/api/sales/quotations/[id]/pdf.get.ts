import { defineEventHandler } from 'h3'
import type { H3Event } from 'h3'
import { MockSalesService } from '~/services/mock'

export default defineEventHandler((event: H3Event) => {
  const id = event.context.params?.id as string
  const data = MockSalesService.generateQuotationPdf(id)
  return { data }
})
