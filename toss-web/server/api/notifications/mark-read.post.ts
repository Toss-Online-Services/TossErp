import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ id?: string }>(event)
  if (!body?.id) {
    throw createError({ statusCode: 400, statusMessage: 'notification id required' })
  }
  return { success: true, id: body.id, readAt: new Date().toISOString() }
})


