import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const driverId = getRouterParam(event, 'driverId')
  const body = await readBody<{ rating?: number; comment?: string; jobId?: string }>(event)
  if (!driverId || typeof body?.rating !== 'number' || body.rating < 1 || body.rating > 5) {
    throw createError({ statusCode: 400, statusMessage: 'driverId and rating 1-5 required' })
  }
  return {
    driverId,
    rating: body.rating,
    comment: body.comment,
    jobId: body.jobId,
    createdAt: new Date().toISOString(),
  }
})


