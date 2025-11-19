import { defineEventHandler } from 'h3'
import { convertQuotationToOrder } from '~/server/utils/quotations'

export default defineEventHandler(async (event) => {
  const id = event.context.params?.id
  if (!id) {
    throw createError({ statusCode: 400, statusMessage: 'Missing quotation id' })
  }

  const orderId = await convertQuotationToOrder(id)

  return {
    data: { orderId },
    message: 'Quotation converted successfully'
  }
})
