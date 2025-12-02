import { defineEventHandler, getRouterParam, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  const body = await readBody<{ status?: string }>(event)
  if (!id || !body?.status) {
    throw createError({ statusCode: 400, statusMessage: 'job id and status required' })
  }
  return { id, status: body.status, updatedAt: new Date().toISOString() }
})


