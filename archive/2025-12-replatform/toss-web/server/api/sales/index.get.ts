import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  return [
    {
      id: 'so-1001',
      customerName: 'Local Restaurant',
      status: 'Completed',
      total: 1450.0,
      createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24).toISOString(),
      items: [
        { sku: 'BREAD-001', name: 'White Bread Loaf', qty: 50, price: 12 },
        { sku: 'MILK-001', name: 'Fresh Milk 1L', qty: 20, price: 25 },
      ],
    },
    {
      id: 'so-1002',
      customerName: 'Community School',
      status: 'Pending',
      total: 980.0,
      createdAt: new Date().toISOString(),
      items: [
        { sku: 'RICE-001', name: 'Basmati Rice 2kg', qty: 15, price: 48 },
      ],
    },
  ]
})

