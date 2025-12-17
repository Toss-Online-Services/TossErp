import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  return [
    { id: 'c1', with: 'Driver: Sipho D.', lastMessage: 'Arriving in 10 minutes', updatedAt: new Date().toISOString() },
    { id: 'c2', with: 'Customer: Naledi', lastMessage: 'Package delivered, thanks!', updatedAt: new Date().toISOString() },
  ]
})

