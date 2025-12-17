import { defineEventHandler, readBody } from 'h3'
import { changeQuotationStatus, QuotationStatus } from '~/server/utils/quotations'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id
  if (!id) {
    throw createError({ statusCode: 400, statusMessage: 'Missing quotation id' })
  }

  const body = await readBody(event)
  if (!body?.status) {
    throw createError({ statusCode: 400, statusMessage: 'Status is required' })
  }

  const record = await changeQuotationStatus(id, body.status as QuotationStatus)

  return {
    data: record,
    message: 'Status updated successfully'
  }
})
