import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  return {
    totalItems: 156,
    lowStockItems: 3,
    outOfStockItems: 0,
    totalValue: 62500,
    totalCategories: 5,
    categorySummary: [
      { category: 'Bakery', itemCount: 24, totalValue: 9800 },
      { category: 'Dairy', itemCount: 18, totalValue: 11200 },
      { category: 'Grains', itemCount: 12, totalValue: 8600 },
    ],
  }
})


