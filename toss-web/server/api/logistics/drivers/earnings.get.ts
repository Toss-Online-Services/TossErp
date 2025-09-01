import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const driverId = (q.driverId as string) || 'drv-abc'
  return {
    driverId,
    period: 'month',
    currency: 'ZAR',
    total: 8420,
    completed: 46,
    pendingPayout: 1570,
    history: [
      { id: 'job-110', amount: 180, date: new Date().toISOString() },
      { id: 'job-109', amount: 220, date: new Date(Date.now() - 86400000).toISOString() },
    ],
  }
})


