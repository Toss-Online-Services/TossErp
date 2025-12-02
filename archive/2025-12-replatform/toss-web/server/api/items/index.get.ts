import { defineEventHandler, getQuery } from 'h3'

const mockItems = () => [
  {
    id: '1', tenantId: 'tenant1', sku: 'BREAD-001', name: 'White Bread Loaf', description: 'Fresh white bread loaf',
    category: 'Bakery', unit: 'loaf', sellingPrice: 12.0, costPrice: 8.5, reorderLevel: 20, reorderQty: 50,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T10:30:00Z', quantityOnHand: 45
  },
  {
    id: '2', tenantId: 'tenant1', sku: 'MILK-001', name: 'Fresh Milk 1L', description: 'Fresh full cream milk',
    category: 'Dairy', unit: 'liter', sellingPrice: 25.0, costPrice: 18.0, reorderLevel: 25, reorderQty: 30,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T08:15:00Z', quantityOnHand: 15
  },
  {
    id: '3', tenantId: 'tenant1', sku: 'RICE-001', name: 'Basmati Rice 2kg', description: 'Premium basmati rice',
    category: 'Grains', unit: 'kg', sellingPrice: 48.0, costPrice: 35.0, reorderLevel: 10, reorderQty: 20,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-14T16:45:00Z', quantityOnHand: 8
  },
  {
    id: '4', tenantId: 'tenant1', sku: 'SOAP-001', name: 'Washing Powder 1kg', description: 'All-purpose washing powder',
    category: 'Cleaning', unit: 'kg', sellingPrice: 32.0, costPrice: 22.0, reorderLevel: 15, reorderQty: 25,
    isActive: true, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-13T11:20:00Z', quantityOnHand: 12
  },
]

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const page = Number(q.page || 1)
  const pageSize = Number(q.pageSize || 50)
  const searchTerm = (q.searchTerm as string) || ''
  const category = (q.category as string) || ''

  let items = mockItems()
  if (searchTerm) items = items.filter(i => `${i.name} ${i.sku}`.toLowerCase().includes(searchTerm.toLowerCase()))
  if (category) items = items.filter(i => i.category === category)

  const start = (page - 1) * pageSize
  const paged = items.slice(start, start + pageSize)
  return { items: paged, totalCount: items.length }
})


