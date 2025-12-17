import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ conversationId?: string; to?: string; text?: string }>(event)
  if (!body?.text || (!body.conversationId && !body.to)) {
    throw createError({ statusCode: 400, statusMessage: 'text and (conversationId or to) are required' })
  }
  return {
    id: Math.random().toString(36).slice(2),
    conversationId: body.conversationId || `conv-${Math.random().toString(36).slice(2)}`,
    to: body.to,
    text: body.text,
    sentAt: new Date().toISOString(),
    status: 'sent',
  }
})

