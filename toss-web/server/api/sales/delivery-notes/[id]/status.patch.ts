import { defineEventHandler, readBody } from 'h3'
import type { H3Event } from 'h3'
import { MockSalesService } from '~/services/mock'
import type { DeliveryNoteStatus } from '~/types/sales'

export default defineEventHandler(async (event: H3Event) => {
  const id = event.context.params?.id as string
  const body = await readBody(event)
  const status = body.status as DeliveryNoteStatus
  const data = MockSalesService.updateDeliveryNoteStatus(id, status)
  return { data }
})
