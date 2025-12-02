import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  const body = await readBody<{ providerId?: string }>(event)
  if (!id || !body?.providerId) {
    throw createError({ statusCode: 400, statusMessage: 'job id and providerId required' })
  }
  return { id, status: 'assigned', providerId: body.providerId, assignedAt: new Date().toISOString() }
})


