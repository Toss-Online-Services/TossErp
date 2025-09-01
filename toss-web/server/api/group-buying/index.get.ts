import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  // Mock opportunities for collaborative purchasing
  return [
    {
      id: 'gb-1',
      item: 'Maize Meal 10kg',
      category: 'Staples',
      unitPrice: 95,
      bulkPrice: 82,
      minQty: 50,
      joinedQty: 32,
      savingsPct: 13.7,
      closesAt: new Date(Date.now() + 1000 * 60 * 60 * 24).toISOString(),
      initiator: "Thabo's Spaza Shop",
    },
    {
      id: 'gb-2',
      item: 'Cooking Oil 2L',
      category: 'Staples',
      unitPrice: 68,
      bulkPrice: 59,
      minQty: 100,
      joinedQty: 76,
      savingsPct: 13.2,
      closesAt: new Date(Date.now() + 1000 * 60 * 60 * 12).toISOString(),
      initiator: 'Township Retail Co-op',
    },
  ]
})

