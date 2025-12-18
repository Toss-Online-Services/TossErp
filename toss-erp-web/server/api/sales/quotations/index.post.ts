import { defineEventHandler, readBody } from 'h3'
import { createQuotationRecord } from '../../../utils/quotations'

export default defineEventHandler(async (event) => {
  const body = await readBody(event)

  const required = ['customerId', 'quotationDate', 'validUntil', 'items'] as const
  for (const field of required) {
    if (!body?.[field]) {
      throw createError({ statusCode: 400, statusMessage: `${field} is required` })
    }
  }

  const record = await createQuotationRecord({
    customerId: body.customerId,
    quotationDate: body.quotationDate,
    validUntil: body.validUntil,
    priceList: body.priceList || 'standard',
    currency: body.currency || 'ZAR',
    termsAndConditions: body.termsAndConditions,
    notes: body.notes,
    items: body.items,
  })

  return {
    data: record,
    message: 'Quotation created successfully'
  }
})
