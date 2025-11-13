import { defineEventHandler, readBody } from 'h3'
import type { H3Event } from 'h3'
import { MockSalesService } from '~/services/mock'

export default defineEventHandler(async (event: H3Event) => {
  const id = event.context.params?.id as string
  const body = await readBody(event)
  const data = MockSalesService.closePosSession(id, body)
  return { data }
})
