import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const category = (q.category as string) || 'driver'
  const status = (q.status as string) || 'open'
  const jobs = [
    { id: 'sj-1', category: 'plumber', status: 'open', title: 'Fix leaking tap', payout: 350, location: 'Soweto' },
    { id: 'sj-2', category: 'electrician', status: 'open', title: 'Install light fixtures', payout: 500, location: 'Tembisa' },
    { id: 'sj-3', category: 'driver', status: 'open', title: 'Deliver groceries', payout: 120, location: 'Alexandra' },
  ]
  return jobs.filter(j => (category === 'all' || j.category === category) && (status === 'all' || j.status === status))
})


