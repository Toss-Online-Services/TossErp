import { defineEventHandler } from 'h3'

export default defineEventHandler(async () => {
  const warehouses = [
    { id: '1', tenantId: 'tenant1', code: 'MAIN', name: 'Main Store', description: 'Primary retail location', isGroup: false, address: 'Township Center, Main Road', isActive: true, type: 'RETAIL', itemCount: 45, stockValue: 2500.0, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T00:00:00Z' },
    { id: '2', tenantId: 'tenant1', code: 'COLD', name: 'Cold Storage Facility', description: 'Refrigerated storage for perishables', isGroup: false, address: 'Industrial Area, Cold Chain Hub', isActive: true, type: 'COLD_STORAGE', itemCount: 23, stockValue: 1850.0, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T00:00:00Z' },
    { id: '3', tenantId: 'tenant1', code: 'SHARED', name: 'Township Central Warehouse', description: 'Shared community warehouse facility', isGroup: true, address: 'Community Hub, Shared Facilities', isActive: true, type: 'SHARED', itemCount: 67, stockValue: 4200.0, createdAt: '2024-01-01T00:00:00Z', updatedAt: '2024-01-15T00:00:00Z' },
  ]
  return { warehouses, totalCount: warehouses.length }
})


