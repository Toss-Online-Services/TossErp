import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const status = (q.status as string) || 'open'

  const jobs = [
    {
      id: 'job-001',
      status: 'open',
      pickup: { name: "Supplier Hub", lat: -26.2, lng: 28.04 },
      dropoff: { name: "Township Central Warehouse", lat: -26.18, lng: 28.06 },
      weightKg: 200,
      payout: 450,
      createdAt: new Date().toISOString(),
    },
    {
      id: 'job-002',
      status: 'assigned',
      driverId: 'drv-abc',
      pickup: { name: "Thabo's Spaza Shop", lat: -26.21, lng: 28.05 },
      dropoff: { name: 'Customer: Naledi', lat: -26.22, lng: 28.09 },
      weightKg: 30,
      payout: 120,
      createdAt: new Date().toISOString(),
    },
  ]

  return jobs.filter(j => status === 'all' ? true : j.status === status)
})


