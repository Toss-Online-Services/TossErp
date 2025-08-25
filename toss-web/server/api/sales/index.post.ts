export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  // Validate required fields
  const { customerId, items } = body
  
  if (!customerId || !items || !Array.isArray(items) || items.length === 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Missing required fields: customerId and items array'
    })
  }

  // Validate items
  for (const item of items) {
    if (!item.itemId || !item.quantity || !item.unitPrice) {
      throw createError({
        statusCode: 400,
        statusMessage: 'Each item must have itemId, quantity, and unitPrice'
      })
    }
    
    if (item.quantity <= 0 || item.unitPrice <= 0) {
      throw createError({
        statusCode: 400,
        statusMessage: 'Item quantity and unitPrice must be positive numbers'
      })
    }
  }

  // Generate invoice number
  const invoiceNumber = `INV-${new Date().getFullYear()}-${String(Math.floor(Math.random() * 9999) + 1).padStart(3, '0')}`
  
  // Calculate totals
  let subtotal = 0
  const processedItems = items.map(item => {
    const totalPrice = Number(item.quantity) * Number(item.unitPrice)
    subtotal += totalPrice
    
    return {
      itemId: item.itemId,
      name: item.name || 'Unknown Item',
      sku: item.sku || '',
      quantity: Number(item.quantity),
      unitPrice: Number(item.unitPrice),
      totalPrice,
      discount: Number(item.discount) || 0
    }
  })

  const discountAmount = Number(body.discountAmount) || 0
  const taxRate = Number(body.taxRate) || 0.15 // Default 15% VAT
  const taxAmount = (subtotal - discountAmount) * taxRate
  const totalAmount = subtotal - discountAmount + taxAmount

  const newSale = {
    id: Math.random().toString(36).substr(2, 9),
    invoiceNumber,
    customerId,
    customerName: body.customerName || '',
    customerEmail: body.customerEmail || '',
    customerPhone: body.customerPhone || '',
    saleDate: new Date().toISOString(),
    dueDate: body.dueDate || new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(), // 30 days from now
    status: 'pending',
    paymentMethod: body.paymentMethod || 'cash',
    paymentStatus: 'pending',
    subtotal,
    taxAmount,
    discountAmount,
    totalAmount,
    paidAmount: 0,
    balanceAmount: totalAmount,
    items: processedItems,
    notes: body.notes?.trim() || '',
    salesPersonId: body.salesPersonId || 'system',
    salesPersonName: body.salesPersonName || 'System',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }

  // In real app:
  // 1. Start transaction
  // 2. Create sale record
  // 3. Update inventory stock levels
  // 4. Create stock movements
  // 5. Create accounting entries
  // 6. Send invoice/receipt to customer
  // 7. Commit transaction

  return {
    success: true,
    message: 'Sale created successfully',
    data: newSale
  }
})
