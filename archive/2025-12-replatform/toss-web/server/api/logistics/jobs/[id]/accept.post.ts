import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  const body = await readBody<{ driverId?: string }>(event)
  if (!id || !body?.driverId) {
    throw createError({ statusCode: 400, statusMessage: 'job id and driverId are required' })
  }
  return {
    id,
    status: 'assigned',
    driverId: body.driverId,
    assignedAt: new Date().toISOString(),
  }
})


