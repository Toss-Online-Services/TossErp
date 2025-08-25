export default defineEventHandler(async (event) => {
  const body = await readBody(event)
  
  // Validate required fields
  const { name, sku, category, unitCost, sellingPrice, minimumStock, maximumStock } = body
  
  if (!name || !sku || !category || !unitCost || !sellingPrice) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Missing required fields: name, sku, category, unitCost, sellingPrice'
    })
  }

  // Validate business rules
  if (unitCost < 0 || sellingPrice < 0) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Cost and selling price must be positive numbers'
    })
  }

  if (sellingPrice <= unitCost) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Selling price must be greater than unit cost'
    })
  }

  if (minimumStock && maximumStock && minimumStock >= maximumStock) {
    throw createError({
      statusCode: 400,
      statusMessage: 'Minimum stock must be less than maximum stock'
    })
  }

  // Generate new ID (in real app, this would be handled by database)
  const newId = Math.random().toString(36).substr(2, 9)
  
  const newItem = {
    id: newId,
    name: name.trim(),
    sku: sku.trim().toUpperCase(),
    category: category.trim(),
    brand: body.brand?.trim() || '',
    description: body.description?.trim() || '',
    currentStock: Number(body.currentStock) || 0,
    minimumStock: Number(minimumStock) || 0,
    maximumStock: Number(maximumStock) || 100,
    unitCost: Number(unitCost),
    sellingPrice: Number(sellingPrice),
    supplier: body.supplier?.trim() || '',
    location: body.location?.trim() || '',
    expiryDate: body.expiryDate || null,
    status: 'active',
    images: body.images || [],
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString()
  }

  // In real app: Save to database
  // await database.inventory.create(newItem)

  return {
    success: true,
    message: 'Inventory item created successfully',
    data: newItem
  }
})
