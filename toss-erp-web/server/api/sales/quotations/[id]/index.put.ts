import { defineEventHandler, readBody } from 'h3'
import { updateQuotationRecord } from '../../../../utils/quotations'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id
  if (!id) {
    throw createError({ statusCode: 400, statusMessage: 'Missing quotation id' })
  }

  const body = await readBody(event)
  const record = await updateQuotationRecord(id, body)

  return {
    data: record,
    message: 'Quotation updated successfully'
  }
})
