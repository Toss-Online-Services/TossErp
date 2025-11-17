// Mock Products Data for Testing
// South African township spaza shop products with realistic ZAR pricing

export interface MockProduct {
  id: number
  name: string
  sku: string
  barcode: string | null
  basePrice: number
  categoryId: number
  categoryName: string
  availableStock: number
  unit: string
  isTaxable: boolean
  imageUrl: string | null
  reorderPoint: number
  isActive: boolean
}

export interface MockCategory {
  id: number
  name: string
}

// Realistic South African spaza shop categories
export const mockCategories: MockCategory[] = [
  { id: 1, name: 'Groceries' },
  { id: 2, name: 'Beverages' },
  { id: 3, name: 'Snacks' },
  { id: 4, name: 'Airtime & Electricity' },
  { id: 5, name: 'Household' },
  { id: 6, name: 'Bread & Bakery' },
  { id: 7, name: 'Frozen Foods' },
]

// Comprehensive mock products with South African context
export const mockProducts: MockProduct[] = [
  // Groceries
  {
    id: 1,
    name: 'White Star Maize Meal 5kg',
    sku: 'GROC-001',
    barcode: '6001087232456',
    basePrice: 89.99,
    categoryId: 1,
    categoryName: 'Groceries',
    availableStock: 45,
    unit: 'bag',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 10,
    isActive: true,
  },
  {
    id: 2,
    name: 'Tastic Rice 2kg',
    sku: 'GROC-002',
    barcode: '6001087234567',
    basePrice: 54.99,
    categoryId: 1,
    categoryName: 'Groceries',
    availableStock: 32,
    unit: 'bag',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 8,
    isActive: true,
  },
  {
    id: 3,
    name: 'Sunflower Oil 2L',
    sku: 'GROC-003',
    barcode: '6001087235678',
    basePrice: 52.99,
    categoryId: 1,
    categoryName: 'Groceries',
    availableStock: 28,
    unit: 'bottle',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 6,
    isActive: true,
  },
  {
    id: 4,
    name: 'White Sugar 2.5kg',
    sku: 'GROC-004',
    barcode: '6001087236789',
    basePrice: 42.99,
    categoryId: 1,
    categoryName: 'Groceries',
    availableStock: 38,
    unit: 'bag',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 8,
    isActive: true,
  },
  {
    id: 5,
    name: 'Salt 1kg',
    sku: 'GROC-005',
    barcode: '6001087237890',
    basePrice: 12.99,
    categoryId: 1,
    categoryName: 'Groceries',
    availableStock: 52,
    unit: 'bag',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 10,
    isActive: true,
  },
  
  // Beverages
  {
    id: 6,
    name: 'Coca-Cola 2L',
    sku: 'BEV-001',
    barcode: '5449000000996',
    basePrice: 24.99,
    categoryId: 2,
    categoryName: 'Beverages',
    availableStock: 64,
    unit: 'bottle',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 15,
    isActive: true,
  },
  {
    id: 7,
    name: 'Coca-Cola 500ml',
    sku: 'BEV-002',
    barcode: '5449000000989',
    basePrice: 12.99,
    categoryId: 2,
    categoryName: 'Beverages',
    availableStock: 120,
    unit: 'bottle',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 30,
    isActive: true,
  },
  {
    id: 8,
    name: 'Fanta Orange 2L',
    sku: 'BEV-003',
    barcode: '5449000001016',
    basePrice: 23.99,
    categoryId: 2,
    categoryName: 'Beverages',
    availableStock: 48,
    unit: 'bottle',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 12,
    isActive: true,
  },
  {
    id: 9,
    name: 'Ricoffy 250g',
    sku: 'BEV-004',
    barcode: '6001087238901',
    basePrice: 34.99,
    categoryId: 2,
    categoryName: 'Beverages',
    availableStock: 22,
    unit: 'tin',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 5,
    isActive: true,
  },
  {
    id: 10,
    name: 'Joko Tea 100 Bags',
    sku: 'BEV-005',
    barcode: '6001087239012',
    basePrice: 28.99,
    categoryId: 2,
    categoryName: 'Beverages',
    availableStock: 35,
    unit: 'box',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 8,
    isActive: true,
  },
  
  // Snacks
  {
    id: 11,
    name: 'Simba Chips 120g',
    sku: 'SNACK-001',
    barcode: '6001087240123',
    basePrice: 17.99,
    categoryId: 3,
    categoryName: 'Snacks',
    availableStock: 85,
    unit: 'packet',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 20,
    isActive: true,
  },
  {
    id: 12,
    name: 'Ghost Pops 80g',
    sku: 'SNACK-002',
    barcode: '6001087241234',
    basePrice: 14.99,
    categoryId: 3,
    categoryName: 'Snacks',
    availableStock: 92,
    unit: 'packet',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 25,
    isActive: true,
  },
  {
    id: 13,
    name: 'Albany Brown Bread',
    sku: 'BREAD-001',
    barcode: '6001087242345',
    basePrice: 15.99,
    categoryId: 6,
    categoryName: 'Bread & Bakery',
    availableStock: 24,
    unit: 'loaf',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 10,
    isActive: true,
  },
  {
    id: 14,
    name: 'Albany White Bread',
    sku: 'BREAD-002',
    barcode: '6001087243456',
    basePrice: 15.99,
    categoryId: 6,
    categoryName: 'Bread & Bakery',
    availableStock: 18,
    unit: 'loaf',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 10,
    isActive: true,
  },
  
  // Airtime & Electricity
  {
    id: 15,
    name: 'Vodacom Airtime R10',
    sku: 'AIR-001',
    barcode: null,
    basePrice: 10.00,
    categoryId: 4,
    categoryName: 'Airtime & Electricity',
    availableStock: 999,
    unit: 'voucher',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 0,
    isActive: true,
  },
  {
    id: 16,
    name: 'MTN Airtime R20',
    sku: 'AIR-002',
    barcode: null,
    basePrice: 20.00,
    categoryId: 4,
    categoryName: 'Airtime & Electricity',
    availableStock: 999,
    unit: 'voucher',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 0,
    isActive: true,
  },
  {
    id: 17,
    name: 'Electricity R50',
    sku: 'ELEC-001',
    barcode: null,
    basePrice: 50.00,
    categoryId: 4,
    categoryName: 'Airtime & Electricity',
    availableStock: 999,
    unit: 'voucher',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 0,
    isActive: true,
  },
  
  // Household
  {
    id: 18,
    name: 'Sunlight Soap 500g',
    sku: 'HOUSE-001',
    barcode: '6001087244567',
    basePrice: 18.99,
    categoryId: 5,
    categoryName: 'Household',
    availableStock: 42,
    unit: 'bar',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 10,
    isActive: true,
  },
  {
    id: 19,
    name: 'Handy Andy 750ml',
    sku: 'HOUSE-002',
    barcode: '6001087245678',
    basePrice: 22.99,
    categoryId: 5,
    categoryName: 'Household',
    availableStock: 36,
    unit: 'bottle',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 8,
    isActive: true,
  },
  {
    id: 20,
    name: 'Toilet Paper 9 Pack',
    sku: 'HOUSE-003',
    barcode: '6001087246789',
    basePrice: 32.99,
    categoryId: 5,
    categoryName: 'Household',
    availableStock: 28,
    unit: 'pack',
    isTaxable: true,
    imageUrl: null,
    reorderPoint: 6,
    isActive: true,
  },
  
  // Frozen Foods
  {
    id: 21,
    name: 'Chicken Feet 1kg',
    sku: 'FROZEN-001',
    barcode: '6001087247890',
    basePrice: 45.99,
    categoryId: 7,
    categoryName: 'Frozen Foods',
    availableStock: 15,
    unit: 'kg',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 5,
    isActive: true,
  },
  {
    id: 22,
    name: 'Polony 1kg',
    sku: 'FROZEN-002',
    barcode: '6001087248901',
    basePrice: 52.99,
    categoryId: 7,
    categoryName: 'Frozen Foods',
    availableStock: 12,
    unit: 'kg',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 4,
    isActive: true,
  },
  
  // Low stock items for testing
  {
    id: 23,
    name: 'Peanut Butter 400g',
    sku: 'GROC-006',
    barcode: '6001087249012',
    basePrice: 38.99,
    categoryId: 1,
    categoryName: 'Groceries',
    availableStock: 3, // Low stock!
    unit: 'jar',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 8,
    isActive: true,
  },
  {
    id: 24,
    name: 'Jam 450g',
    sku: 'GROC-007',
    barcode: '6001087250123',
    basePrice: 29.99,
    categoryId: 1,
    categoryName: 'Groceries',
    availableStock: 2, // Low stock!
    unit: 'jar',
    isTaxable: false,
    imageUrl: null,
    reorderPoint: 6,
    isActive: true,
  },
]

// Helper functions for mock data
export const getMockProductById = (id: number): MockProduct | undefined => {
  return mockProducts.find(p => p.id === id)
}

export const getMockProductByBarcode = (barcode: string): MockProduct | undefined => {
  return mockProducts.find(p => p.barcode === barcode)
}

export const getMockProductsByCategoryId = (categoryId: number): MockProduct[] => {
  return mockProducts.filter(p => p.categoryId === categoryId)
}

export const searchMockProducts = (searchTerm: string): MockProduct[] => {
  const term = searchTerm.toLowerCase()
  return mockProducts.filter(p => 
    p.name.toLowerCase().includes(term) || 
    p.sku.toLowerCase().includes(term) ||
    (p.barcode && p.barcode.includes(term))
  )
}

export const getMockLowStockProducts = (): MockProduct[] => {
  return mockProducts.filter(p => p.availableStock <= p.reorderPoint)
}
