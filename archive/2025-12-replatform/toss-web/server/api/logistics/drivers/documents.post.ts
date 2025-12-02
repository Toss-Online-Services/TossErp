import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ driverId?: string; type?: string }>(event)
  if (!body?.driverId || !body?.type) {
    throw createError({ statusCode: 400, statusMessage: 'driverId and type required' })
  }
  // Mock upload ack
  return {
    driverId: body.driverId,
    type: body.type,
    status: 'submitted',
    receivedAt: new Date().toISOString(),
  }
})


