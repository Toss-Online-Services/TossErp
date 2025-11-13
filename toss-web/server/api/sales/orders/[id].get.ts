import { defineEventHandler } from 'h3'
import { MockSalesService } from '~/services/mock'

export default defineEventHandler((event) => {
  const id = event.context.params?.id as string
  const data = MockSalesService.getSalesOrder(id)
  return { data }
})
