export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  // Validate required fields
  const { itemId, type, quantity, unitPrice, reference } = body
  
  if (!itemId || !type || !quantity || !reference) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Missing required fields: itemId, type, quantity, reference'
    })
  }

  // Validate movement type
  const validTypes = ['purchase', 'sale', 'adjustment', 'transfer', 'return', 'damage', 'restock']
  if (!validTypes.includes(type)) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Invalid movement type. Must be one of: ' + validTypes.join(', ')
    })
  }

  // Validate quantity based on type
  if ((type === 'sale' || type === 'transfer' || type === 'damage') && quantity > 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Quantity must be negative for sales, transfers, and damage movements'
    })
  }

  if ((type === 'purchase' || type === 'restock' || type === 'return') && quantity < 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Quantity must be positive for purchases, restocks, and returns'
    })
  }

  // Check if item exists (in real app, query database)
  const itemExists = true // Replace with actual database check
  
  if (!itemExists) {
    throw createError({
      statusCode: 404,
      statusMessage: 'Inventory item not found'
    })
  }

  // Generate movement ID
  const movementId = Math.random().toString(36).substr(2, 9)
  
  const stockMovement = {
    id: movementId,
    itemId,
    type,
    quantity: Number(quantity),
    unitPrice: unitPrice ? Number(unitPrice) : null,
    totalValue: unitPrice ? Number(quantity) * Number(unitPrice) : 0,
    reference: reference.trim(),
    notes: body.notes?.trim() || '',
    batchNumber: body.batchNumber?.trim() || null,
    expiryDate: body.expiryDate || null,
    location: body.location?.trim() || '',
    userId: body.userId || 'system', // In real app, get from session
    createdAt: new Date().toISOString()
  }

  // In real app: 
  // 1. Start transaction
  // 2. Create stock movement record
  // 3. Update inventory item current stock
  // 4. Create accounting entries if needed
  // 5. Commit transaction

  return {
    success: true,
    message: 'Stock movement recorded successfully',
    data: stockMovement
  }
})
