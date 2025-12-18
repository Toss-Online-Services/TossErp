import { defineEventHandler } from 'h3'
import { duplicateQuotationRecord } from '../../../../../utils/quotations'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id
  if (!id) {
    throw createError({ statusCode: 400, statusMessage: 'Missing quotation id' })
  }

  const quotation = await duplicateQuotationRecord(id)

  return {
    data: quotation,
    message: 'Quotation duplicated successfully'
  }
})
