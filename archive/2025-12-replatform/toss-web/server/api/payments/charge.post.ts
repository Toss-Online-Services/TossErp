import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ amount?: number; currency?: string; source?: string; reference?: string }>(event)
  if (!body?.amount || !body?.currency) {
    throw createError({ statusCode: 400, statusMessage: 'amount and currency required' })
  }
  // Mock successful charge
  return {
    id: `ch_${Math.random().toString(36).slice(2)}`,
    amount: body.amount,
    currency: body.currency,
    status: 'succeeded',
    reference: body.reference,
    createdAt: new Date().toISOString(),
  }
})


