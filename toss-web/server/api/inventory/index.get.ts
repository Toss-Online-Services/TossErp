export default defineEventHandler(async (event) => {
  // Get query parameters for filtering and pagination
  const query = getQuery(event)
  const { page = 1, limit = 20, search = '', category = '', lowStock = false } = query

  // Demo inventory data - replace with database calls
  const demoInventory = [
    {
      id: '1',
      name: 'Maize Meal (1kg)',
      sku: 'MM001',
      category: 'Food & Beverages',
      brand: 'ACE',
      description: 'High quality maize meal for cooking',
      currentStock: 45,
      minimumStock: 20,
      maximumStock: 100,
      unitCost: 15.50,
      sellingPrice: 22.99,
      supplier: 'Local Grain Supplier',
      location: 'Shelf A1',
      lastRestocked: '2024-08-20T10:00:00Z',
      expiryDate: '2025-02-15T00:00:00Z',
      status: 'active',
      images: ['/images/products/maize-meal.jpg'],
      createdAt: '2024-01-15T09:00:00Z',
      updatedAt: '2024-08-20T10:00:00Z'
    },
    {
      id: '2',
      name: 'Sunflower Oil (750ml)',
      sku: 'SO002',
      category: 'Food & Beverages',
      brand: 'Sunfoil',
      description: 'Pure sunflower cooking oil',
      currentStock: 8,
      minimumStock: 15,
      maximumStock: 50,
      unitCost: 28.00,
      sellingPrice: 39.99,
      supplier: 'Oil Distributors SA',
      location: 'Shelf B2',
      lastRestocked: '2024-08-18T14:30:00Z',
      expiryDate: '2025-06-30T00:00:00Z',
      status: 'low_stock',
      images: ['/images/products/sunflower-oil.jpg'],
      createdAt: '2024-01-20T11:00:00Z',
      updatedAt: '2024-08-18T14:30:00Z'
    },
    {
      id: '3',
      name: 'Bread (White Loaf)',
      sku: 'BR003',
      category: 'Bakery',
      brand: 'Albany',
      description: 'Fresh white bread loaf',
      currentStock: 12,
      minimumStock: 10,
      maximumStock: 30,
      unitCost: 12.00,
      sellingPrice: 16.99,
      supplier: 'Albany Bakeries',
      location: 'Bread Section',
      lastRestocked: '2024-08-24T08:00:00Z',
      expiryDate: '2024-08-26T00:00:00Z',
      status: 'active',
      images: ['/images/products/white-bread.jpg'],
      createdAt: '2024-02-01T08:00:00Z',
      updatedAt: '2024-08-24T08:00:00Z'
    },
    {
      id: '4',
      name: 'Coca Cola (500ml)',
      sku: 'CC004',
      category: 'Beverages',
      brand: 'Coca Cola',
      description: 'Classic Coca Cola soft drink',
      currentStock: 24,
      minimumStock: 12,
      maximumStock: 48,
      unitCost: 8.50,
      sellingPrice: 14.99,
      supplier: 'Coca Cola Distributors',
      location: 'Fridge A',
      lastRestocked: '2024-08-23T16:00:00Z',
      expiryDate: '2025-01-15T00:00:00Z',
      status: 'active',
      images: ['/images/products/coca-cola.jpg'],
      createdAt: '2024-01-25T10:00:00Z',
      updatedAt: '2024-08-23T16:00:00Z'
    },
    {
      id: '5',
      name: 'Rice (2kg)',
      sku: 'RC005',
      category: 'Food & Beverages',
      brand: 'Tastic',
      description: 'Premium long grain white rice',
      currentStock: 3,
      minimumStock: 10,
      maximumStock: 40,
      unitCost: 45.00,
      sellingPrice: 65.99,
      supplier: 'Rice Importers CC',
      location: 'Shelf C1',
      lastRestocked: '2024-08-15T12:00:00Z',
      expiryDate: '2025-08-01T00:00:00Z',
      status: 'critical_stock',
      images: ['/images/products/rice.jpg'],
      createdAt: '2024-01-30T13:00:00Z',
      updatedAt: '2024-08-15T12:00:00Z'
    }
  ]

  // Filter inventory based on query parameters
  let filteredInventory = demoInventory

  if (search) {
    const searchLower = search.toString().toLowerCase()
    filteredInventory = filteredInventory.filter(item => 
      item.name.toLowerCase().includes(searchLower) ||
      item.sku.toLowerCase().includes(searchLower) ||
      item.brand.toLowerCase().includes(searchLower) ||
      item.category.toLowerCase().includes(searchLower)
    )
  }

  if (category) {
    filteredInventory = filteredInventory.filter(item => 
      item.category.toLowerCase() === category.toString().toLowerCase()
    )
  }

  if (lowStock === 'true') {
    filteredInventory = filteredInventory.filter(item => 
      item.currentStock <= item.minimumStock
    )
  }

  // Calculate pagination
  const totalItems = filteredInventory.length
  const totalPages = Math.ceil(totalItems / Number(limit))
  const startIndex = (Number(page) - 1) * Number(limit)
  const endIndex = startIndex + Number(limit)
  const paginatedItems = filteredInventory.slice(startIndex, endIndex)

  // Calculate summary statistics
  const totalValue = filteredInventory.reduce((sum, item) => sum + (item.currentStock * item.unitCost), 0)
  const lowStockItems = filteredInventory.filter(item => item.currentStock <= item.minimumStock).length
  const outOfStockItems = filteredInventory.filter(item => item.currentStock === 0).length
  const categories = [...new Set(filteredInventory.map(item => item.category))]

  return {
    success: true,
    data: {
      items: paginatedItems,
      pagination: {
        currentPage: Number(page),
        totalPages,
        totalItems,
        itemsPerPage: Number(limit),
        hasNextPage: Number(page) < totalPages,
        hasPreviousPage: Number(page) > 1
      },
      summary: {
        totalItems: filteredInventory.length,
        totalValue,
        lowStockItems,
        outOfStockItems,
        categories
      }
    }
  }
})
