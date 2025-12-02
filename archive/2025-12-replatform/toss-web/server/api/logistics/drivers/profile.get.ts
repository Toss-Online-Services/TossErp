import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const driverId = (q.driverId as string) || 'drv-abc'
  return {
    id: driverId,
    fullName: 'Sipho D.',
    phone: '+27 71 000 0000',
    vehicleType: 'bakkie',
    capacityKg: 1200,
    plate: 'GP 12 AB GP',
    status: 'verified',
    rating: 4.9,
    completedJobs: 124,
    documents: [
      { type: 'license', status: 'verified' },
      { type: 'vehicle_registration', status: 'verified' },
      { type: 'insurance', status: 'pending' },
    ],
  }
})


