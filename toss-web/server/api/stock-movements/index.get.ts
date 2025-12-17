import { defineEventHandler, getQuery } from 'h3'

export default defineEventHandler(async (event) => {
  const q = getQuery(event)
  const itemId = q.itemId as string | undefined
  const warehouseId = q.warehouseId as string | undefined

  let movements = [
    { id: '1', tenantId: 'tenant1', itemId: '1', itemName: 'White Bread Loaf', itemSku: 'BREAD-001', warehouseId: '1', warehouseName: 'Main Store', movementType: 'IN', quantity: 50, balanceQty: 45, rate: 8.5, amount: 425.0, voucherType: 'Purchase', voucherNo: 'PO-2024-001', transactionDate: '2024-01-15T10:30:00Z', createdAt: '2024-01-15T10:30:00Z' },
    { id: '2', tenantId: 'tenant1', itemId: '2', itemName: 'Fresh Milk 1L', itemSku: 'MILK-001', warehouseId: '2', warehouseName: 'Cold Storage Facility', movementType: 'OUT', quantity: 10, balanceQty: 15, rate: 18.0, amount: 180.0, voucherType: 'Sale', voucherNo: 'SALE-2024-045', transactionDate: '2024-01-14T14:20:00Z', createdAt: '2024-01-14T14:20:00Z' },
    { id: '3', tenantId: 'tenant1', itemId: '4', itemName: 'Washing Powder 1kg', itemSku: 'SOAP-001', warehouseId: '3', warehouseName: 'Township Central Warehouse', movementType: 'TRANSFER', quantity: 5, balanceQty: 12, rate: 22.0, amount: 110.0, voucherType: 'Transfer', voucherNo: 'TRF-2024-012', transactionDate: '2024-01-13T09:15:00Z', createdAt: '2024-01-13T09:15:00Z' },
  ]
  if (itemId) movements = movements.filter(m => m.itemId === itemId)
  if (warehouseId) movements = movements.filter(m => m.warehouseId === warehouseId)
  return { movements, totalCount: movements.length }
})


