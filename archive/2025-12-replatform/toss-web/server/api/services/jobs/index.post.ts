import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ category?: string; title?: string; payout?: number; location?: string }>(event)
  if (!body?.category || !body?.title || !body?.payout) {
    throw createError({ statusCode: 400, statusMessage: 'category, title, payout required' })
  }
  return {
    id: `sj-${Math.random().toString(36).slice(2)}`,
    status: 'open',
    ...body,
    createdAt: new Date().toISOString(),
  }
})


