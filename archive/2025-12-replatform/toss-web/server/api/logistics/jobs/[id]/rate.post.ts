import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  const body = await readBody<{ rating?: number; comment?: string }>(event)
  if (!id || typeof body?.rating !== 'number' || body.rating < 1 || body.rating > 5) {
    throw createError({ statusCode: 400, statusMessage: 'job id and rating 1-5 required' })
  }
  return {
    id,
    rating: body.rating,
    comment: body.comment,
    ratedAt: new Date().toISOString(),
  }
})


