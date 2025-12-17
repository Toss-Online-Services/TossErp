import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ driverId?: string; available?: boolean; location?: { lat: number; lng: number } }>(event)
  if (!body?.driverId || typeof body.available !== 'boolean') {
    throw createError({ statusCode: 400, statusMessage: 'driverId and available are required' })
  }

  return {
    driverId: body.driverId,
    available: body.available,
    location: body.location ?? { lat: -26.2041, lng: 28.0473 },
    updatedAt: new Date().toISOString(),
  }
})


