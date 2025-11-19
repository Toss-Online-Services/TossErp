/**
 * Test data constants for E2E tests
 */

export const TEST_USERS = {
  admin: {
    email: 'admin@toss.local',
    password: 'Admin1!',
    role: 'Administrator'
  },
  retailer: {
    email: 'storeowner1@toss.local',
    password: 'StoreOwner1!',
    role: 'StoreOwner'
  },
  supplier: {
    email: 'supplier1@toss.local',
    password: 'Supplier1!',
    role: 'Supplier'
  },
  driver: {
    email: 'driver1@toss.local',
    password: 'Driver1!',
    role: 'Driver'
  }
};

export const TEST_PRODUCTS = {
  valid: {
    name: 'Test Product',
    sku: `TEST-${Date.now()}`,
    basePrice: 99.99,
    categoryId: 1,
    unit: 'each',
    minimumStockLevel: 10,
    isTaxable: true
  },
  updated: {
    name: 'Updated Test Product',
    basePrice: 149.99,
    minimumStockLevel: 20
  }
};

export const TEST_CATEGORIES = {
  valid: {
    name: 'Test Category',
    description: 'Test category description'
  }
};

export const TEST_PURCHASE_ORDER = {
  valid: {
    supplierId: 1,
    expectedDeliveryDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString().split('T')[0],
    notes: 'Test purchase order',
    items: [
      {
        productId: 1,
        quantity: 10,
        unitPrice: 50.00
      }
    ]
  }
};

export const TEST_SALE = {
  valid: {
    items: [
      {
        productId: 1,
        quantity: 2,
        unitPrice: 99.99
      }
    ],
    paymentMethod: 'Cash',
    totalAmount: 199.98
  }
};

export const TEST_ONBOARDING = {
  retailer: {
    businessName: 'Test Retail Store',
    businessType: 'Retail',
    address: '123 Test Street',
    city: 'Cape Town',
    postalCode: '8000',
    phone: '0211234567',
    products: [
      {
        name: 'Test Product 1',
        sku: 'TEST-001',
        price: 50.00
      }
    ]
  },
  supplier: {
    businessName: 'Test Supplier Co',
    businessType: 'Wholesale',
    address: '456 Supplier Ave',
    city: 'Johannesburg',
    postalCode: '2000',
    phone: '0119876543',
    categories: ['Electronics', 'Clothing']
  },
  driver: {
    firstName: 'Test',
    lastName: 'Driver',
    phone: '0821234567',
    vehicleType: 'Van',
    registration: 'ABC123GP',
    typicalArea: 'Gauteng'
  }
};

export const WAIT_TIMEOUTS = {
  short: 2000,
  medium: 5000,
  long: 10000,
  veryLong: 30000
};

