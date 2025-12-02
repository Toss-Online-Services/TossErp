import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  const items: any[] = []
  return { items, totalCount: items.length }
})


