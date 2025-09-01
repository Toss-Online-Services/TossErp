import { defineEventHandler, readBody, createError } from 'h3'

export default defineEventHandler(async (event) => {
  const body = await readBody<{ name?: string; category?: string; phone?: string; skills?: string[] }>(event)
  if (!body?.name || !body?.category) {
    throw createError({ statusCode: 400, statusMessage: 'name and category are required' })
  }
  return {
    id: `prov-${Math.random().toString(36).slice(2)}`,
    name: body.name,
    category: body.category,
    phone: body.phone,
    skills: body.skills ?? [],
    status: 'pending_verification',
    rating: 5.0,
    completed: 0,
  }
})


