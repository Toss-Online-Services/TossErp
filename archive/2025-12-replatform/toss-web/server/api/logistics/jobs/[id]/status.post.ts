import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'

type JobStatus = 'open' | 'assigned' | 'picked_up' | 'in_transit' | 'delivered' | 'cancelled'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  const body = await readBody<{ status?: JobStatus }>(event)
  if (!id || !body?.status) {
    throw createError({ statusCode: 400, statusMessage: 'job id and status are required' })
  }
  return {
    id,
    status: body.status,
    updatedAt: new Date().toISOString(),
  }
})


