import { defineEventHandler } from 'h3'
import { findQuotation } from '~/server/utils/quotations'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id
  if (!id) {
    throw createError({ statusCode: 400, statusMessage: 'Missing quotation id' })
  }

  const quotation = await findQuotation(id)
  if (!quotation) {
    throw createError({ statusCode: 404, statusMessage: 'Quotation not found' })
  }

  return { data: quotation }
})
