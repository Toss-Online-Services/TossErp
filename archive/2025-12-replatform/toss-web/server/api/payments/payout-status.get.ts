import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const payoutId = (q.payoutId as string) || 'payout-demo'
  return {
    id: payoutId,
    status: 'processing',
    amount: 1570,
    currency: 'ZAR',
    initiatedAt: new Date(Date.now() - 3600000).toISOString(),
    eta: new Date(Date.now() + 6 * 3600000).toISOString(),
  }
})


