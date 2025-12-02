import { defineEventHandler } from 'h3'
import { deleteQuotationRecord } from '~/server/utils/quotations'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id
  if (!id) {
    throw createError({ statusCode: 400, statusMessage: 'Missing quotation id' })
  }

  await deleteQuotationRecord(id)

  return {
    message: 'Quotation deleted successfully'
  }
})
