import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ fullName?: string; phone?: string; vehicleType?: string; capacityKg?: number; plate?: string }>(event)
  if (!body?.fullName || !body?.phone || !body?.vehicleType) {
    throw createError({ statusCode: 400, statusMessage: 'fullName, phone, and vehicleType are required' })
  }

  return {
    id: `drv-${Math.random().toString(36).slice(2)}`,
    fullName: body.fullName,
    phone: body.phone,
    vehicleType: body.vehicleType,
    capacityKg: body.capacityKg ?? 1000,
    plate: body.plate ?? 'UNKNOWN',
    status: 'pending_verification',
    rating: 5.0,
    completedJobs: 0,
  }
})


