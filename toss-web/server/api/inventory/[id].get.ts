export default defineEventHandler(async (event) => {
  const id = getRouterParam(event, 'id')
  
  if (!id) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Item ID is required'
    })
  }

  // Demo inventory item - replace with database call
  const demoItems = {
    '1': {
      id: '1',
      name: 'Maize Meal (1kg)',
      sku: 'MM001',
      category: 'Food & Beverages',
      brand: 'ACE',
      description: 'High quality maize meal for cooking. Perfect for making pap, porridge, and traditional dishes.',
      currentStock: 45,
      minimumStock: 20,
      maximumStock: 100,
      unitCost: 15.50,
      sellingPrice: 22.99,
      supplier: 'Local Grain Supplier',
      supplierContact: '+27 11 123 4567',
      location: 'Shelf A1',
      barcode: '6001234567890',
      lastRestocked: '2024-08-20T10:00:00Z',
      expiryDate: '2025-02-15T00:00:00Z',
      status: 'active',
      images: ['/images/products/maize-meal.jpg', '/images/products/maize-meal-2.jpg'],
      stockMovements: [
        {
          id: 'sm1',
          type: 'restock',
          quantity: 50,
          unitCost: 15.50,
          date: '2024-08-20T10:00:00Z',
          reference: 'PO-2024-001',
          notes: 'Monthly restock from supplier'
        },
        {
          id: 'sm2',
          type: 'sale',
          quantity: -5,
          unitPrice: 22.99,
          date: '2024-08-21T14:30:00Z',
          reference: 'INV-2024-045',
          notes: 'Regular customer purchase'
        }
      ],
      nutritionalInfo: {
        energy: '1520 kJ per 100g',
        protein: '8.2g per 100g',
        carbohydrates: '78g per 100g',
        fat: '1.2g per 100g',
        fiber: '2.1g per 100g'
      },
      createdAt: '2024-01-15T09:00:00Z',
      updatedAt: '2024-08-20T10:00:00Z'
    }
  }

  const item = demoItems[id as keyof typeof demoItems]
  
  if (!item) {
    throw createError({
      statusCode: 404,
      statusMessage: 'Inventory item not found'
    })
  }

  // Calculate additional metrics
  const profitMargin = ((item.sellingPrice - item.unitCost) / item.sellingPrice * 100).toFixed(1)
  const stockValue = item.currentStock * item.unitCost
  const stockStatus = item.currentStock <= 0 ? 'out_of_stock' : 
                     item.currentStock <= item.minimumStock ? 'low_stock' : 
                     'in_stock'
  const daysUntilExpiry = item.expiryDate ? 
    Math.ceil((new Date(item.expiryDate).getTime() - new Date().getTime()) / (1000 * 60 * 60 * 24)) : null

  return {
    success: true,
    data: {
      ...item,
      metrics: {
        profitMargin: Number(profitMargin),
        stockValue,
        stockStatus,
        daysUntilExpiry,
        turnoverRate: 2.4, // Example calculation
        averageMonthlySales: 18
      }
    }
  }
})
