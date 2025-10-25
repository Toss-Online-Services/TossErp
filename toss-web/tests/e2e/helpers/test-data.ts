/**
 * Test Data Generators for TOSS E2E Tests
 */

export const generateShopData = (index: number = 1) => ({
  name: `Spaza Shop ${index}`,
  ownerName: `Owner ${index}`,
  email: `shop${index}@test.toss`,
  password: 'Test123!@#',
  phone: `+27 ${70 + index}1 234 5678`,
  address: `${100 + index} Township Road, Soweto`,
  location: {
    lat: -26.2692 + (index * 0.01),
    lng: 27.8546 + (index * 0.01)
  },
  type: 'spaza',
  businessType: 'Retail'
})

export const generateSupplierData = (index: number = 1) => ({
  name: `Wholesale Supplier ${index}`,
  contactName: `Supplier Contact ${index}`,
  email: `supplier${index}@test.toss`,
  password: 'Supplier123!@#',
  phone: `+27 11 ${400 + index}  ${5000 + index}`,
  address: `${200 + index} Industrial Ave, Johannesburg`,
  type: 'wholesale',
  businessType: 'Wholesale',
  deliveryRadius: 50 // km
})

export const generateDriverData = (index: number = 1) => ({
  name: `Driver ${index}`,
  email: `driver${index}@test.toss`,
  password: 'Driver123!@#',
  phone: `+27 ${80 + index}2 345 6789`,
  vehicleType: index % 2 === 0 ? 'Van' : 'Truck',
  licensePlate: `ABC ${index}23 GP`,
  licenseNumber: `DL${1000 + index}`,
  vehicleCapacity: index % 2 === 0 ? 1000 : 3000 // kg
})

export const generateProductData = (category: string, index: number = 1) => {
  const products: Record<string, any> = {
    Bakery: [
      { name: 'White Bread 700g', price: 12.99, sku: 'BREAD001' },
      { name: 'Brown Bread 700g', price: 14.99, sku: 'BREAD002' },
      { name: 'Rolls 6 pack', price: 8.99, sku: 'ROLLS001' }
    ],
    Dairy: [
      { name: 'Fresh Milk 2L', price: 24.99, sku: 'MILK001' },
      { name: 'Maas 1L', price: 18.99, sku: 'MAAS001' },
      { name: 'Cheese 500g', price: 45.99, sku: 'CHEESE001' }
    ],
    Beverages: [
      { name: 'Coca Cola 2L', price: 18.99, sku: 'COKE001' },
      { name: 'Fanta 2L', price: 18.99, sku: 'FANTA001' },
      { name: 'Water 500ml', price: 8.99, sku: 'WATER001' }
    ],
    Groceries: [
      { name: 'Rice 2kg', price: 45.99, sku: 'RICE001' },
      { name: 'Sugar 2.5kg', price: 32.99, sku: 'SUGAR001' },
      { name: 'Flour 2.5kg', price: 28.99, sku: 'FLOUR001' },
      { name: 'Cooking Oil 2L', price: 54.99, sku: 'OIL001' }
    ]
  }
  
  const categoryProducts = products[category] || products.Groceries
  return categoryProducts[index % categoryProducts.length]
}

export const generateOrderData = (shopId: number, supplierId: number) => ({
  shopId,
  supplierId,
  items: [
    { productId: 1, productName: 'White Bread 700g', quantity: 50, unitPrice: 12.99 },
    { productId: 2, productName: 'Fresh Milk 2L', quantity: 30, unitPrice: 24.99 },
    { productId: 3, productName: 'Coca Cola 2L', quantity: 40, unitPrice: 18.99 }
  ],
  deliveryAddress: '123 Township Road, Soweto',
  deliveryDate: new Date(Date.now() + 86400000).toISOString(), // Tomorrow
  notes: 'Please deliver in the morning'
})

export const generateGroupBuyingPoolData = (productId: number, creatorShopId: number) => ({
  productId,
  creatorShopId,
  productName: 'Sugar 2.5kg',
  targetQuantity: 100,
  currentQuantity: 0,
  unitPrice: 32.99,
  discountedPrice: 29.99, // 10% discount
  minParticipants: 5,
  maxParticipants: 20,
  targetDate: new Date(Date.now() + 172800000).toISOString(), // 2 days
  status: 'open'
})

export const wait = (ms: number) => new Promise(resolve => setTimeout(resolve, ms))

export const formatCurrency = (amount: number) => `R ${amount.toFixed(2)}`

export const getOrderTotal = (items: Array<{quantity: number, unitPrice: number}>) => {
  return items.reduce((total, item) => total + (item.quantity * item.unitPrice), 0)
}

export const mockGPSLocation = () => ({
  lat: -26.2692 + (Math.random() * 0.1 - 0.05),
  lng: 27.8546 + (Math.random() * 0.1 - 0.05),
  timestamp: new Date().toISOString()
})

