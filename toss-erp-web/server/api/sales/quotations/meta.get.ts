import { defineEventHandler } from 'h3'
import { getQuotationMeta } from '../../../utils/quotations'

export default defineEventHandler(async () => {
  const meta = await getQuotationMeta()
  return { data: meta }
})
