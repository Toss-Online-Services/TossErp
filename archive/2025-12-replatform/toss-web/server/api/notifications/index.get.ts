import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  return [
    {
      id: 'n1',
      type: 'info',
      title: 'Group Buying Opportunity',
      message: 'Join bulk order for Cooking Oil 2L and save 13%.',
      timestamp: new Date().toISOString(),
    },
    {
      id: 'n2',
      type: 'warning',
      title: 'Low Stock Alert',
      message: 'Milk 1L below reorder level. Suggest reorder qty: 30.',
      timestamp: new Date().toISOString(),
    },
  ]
})

