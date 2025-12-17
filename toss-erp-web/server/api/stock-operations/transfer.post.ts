import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  return { success: true, reference: `TRF-${Math.random().toString(36).slice(2)}` }
})


