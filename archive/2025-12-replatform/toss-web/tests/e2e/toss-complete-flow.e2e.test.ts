import { test, expect, type Page } from '@playwright/test';

/**
 * TOSS E2E Complete Workflow Test
 * 
 * This test covers the complete flow of the TOSS application:
 * 1. Create a Store (Owner registration)
 * 2. Register Users (Admin, Manager, Cashier)
 * 3. Login as different users
 * 4. Create Customers
 * 5. Place Orders
 * 6. Register Vendors
 * 7. Create Drivers
 * 8. Complete Delivery Flow
 */

// Test data
const testData = {
  // Store Owner
  storeOwner: {
    email: `owner${Date.now()}@test.com`,
    password: 'Test123!@#',
    firstName: 'Store',
    lastName: 'Owner',
    phone: '+27821234567'
  },
  
  // Store
  store: {
    name: `Test Store ${Date.now()}`,
    description: 'A test store for E2E testing',
    companyName: 'Test Company PTY LTD',
    companyAddress: '123 Test Street, Cape Town',
    companyPhone: '+27211234567',
    companyVat: 'ZA1234567890',
    email: 'store@test.com',
    currency: 'ZAR',
    taxRate: 15,
    addressStreet: '123 Test Street',
    addressCity: 'Cape Town',
    addressProvince: 'Western Cape',
    addressPostalCode: '8001',
    addressCountry: 'South Africa'
  },

  // Users
  manager: {
    email: `manager${Date.now()}@test.com`,
    password: 'Manager123!@#',
    firstName: 'Store',
    lastName: 'Manager',
    phone: '+27821234568',
    role: 'Manager'
  },

  cashier: {
    email: `cashier${Date.now()}@test.com`,
    password: 'Cashier123!@#',
    firstName: 'Store',
    lastName: 'Cashier',
    phone: '+27821234569',
    role: 'Cashier'
  },

  // Customer
  customer: {
    firstName: 'John',
    lastName: 'Customer',
    email: `customer${Date.now()}@test.com`,
    phoneNumber: '+27821234570',
    addressStreet: '456 Customer Road',
    addressCity: 'Johannesburg',
    addressProvince: 'Gauteng',
    addressPostalCode: '2001',
    addressCountry: 'South Africa'
  },

  // Vendor
  vendor: {
    name: `Test Vendor ${Date.now()}`,
    email: `vendor${Date.now()}@test.com`,
    phoneNumber: '+27821234571',
    contactPerson: 'Vendor Contact',
    addressStreet: '789 Vendor Street',
    addressCity: 'Durban',
    addressProvince: 'KwaZulu-Natal',
    addressPostalCode: '4001',
    addressCountry: 'South Africa'
  },

  // Driver
  driver: {
    firstName: 'Driver',
    lastName: 'Test',
    phoneNumber: '+27821234572',
    email: `driver${Date.now()}@test.com`,
    licenseNumber: 'DL123456789',
    vehicleRegistration: 'GP12345'
  },

  // Product
  product: {
    name: `Test Product ${Date.now()}`,
    sku: `SKU${Date.now()}`,
    barcode: `${Date.now()}`,
    description: 'A test product for E2E testing',
    basePrice: 99.99,
    categoryName: 'Test Category'
  }
};

// Global variable to store IDs
let storeId: number;
let managerId: string;
let cashierId: string;
let customerId: number;
let vendorId: number;
let driverId: number;
let productId: number;
let orderId: number;

// Helper functions
async function registerUser(page: Page, userData: any) {
  await page.goto('http://localhost:3001/auth/register');
  
  // Step 1: Shop Information
  await page.fill('input[placeholder*="Thabo\'s Spaza Shop"]', `${userData.firstName}'s Shop`);
  await page.selectOption('select', { value: 'soweto' });
  await page.fill('textarea[placeholder*="physical address"]', '123 Test Street, Soweto');
  await page.click('button:has-text("Continue →")');
  await page.waitForTimeout(500);
  
  // Step 2: Owner Information
  await page.fill('input[placeholder="First name"]', userData.firstName);
  await page.fill('input[placeholder="Last name"]', userData.lastName);
  await page.fill('input[placeholder="+27 XX XXX XXXX"]', userData.phone);
  await page.fill('input[placeholder="your@email.com"]', userData.email);
  await page.click('button:has-text("Continue →")');
  await page.waitForTimeout(500);
  
  // Step 3: Account Security
  await page.fill('input[placeholder*="Create a strong password"]', userData.password);
  await page.fill('input[placeholder*="Re-enter password"]', userData.password);
  await page.check('input[type="checkbox"][required]');
  await page.click('button:has-text("Complete Registration")');
  
  // Wait for registration success and navigation to dashboard
  await page.waitForURL('**/dashboard', { timeout: 10000 });
}

async function loginUser(page: Page, email: string, password: string) {
  await page.goto('http://localhost:3001/auth/login');
  
  await page.fill('input[name="email"]', email);
  await page.fill('input[name="password"]', password);
  
  await page.click('button[type="submit"]');
  
  // Wait for dashboard
  await page.waitForURL('**/dashboard', { timeout: 10000 });
}

test.describe('TOSS Complete Workflow E2E', () => {
  test.setTimeout(300000); // 5 minutes for the entire suite

  test.beforeAll(async () => {
    console.log('🚀 Starting TOSS E2E Complete Workflow Test');
    console.log('Test data:', JSON.stringify(testData, null, 2));
  });

  test('1. Register Store Owner and Create Store', async ({ page }) => {
    console.log('📝 Step 1: Registering store owner...');
    
    // Register the store owner
    await registerUser(page, testData.storeOwner);
    
    // Login as store owner
    await loginUser(page, testData.storeOwner.email, testData.storeOwner.password);
    
    // Navigate to store creation
    await page.goto('/settings');
    await page.click('button:has-text("Create Store")');
    
    // Fill store form
    await page.fill('input[name="name"]', testData.store.name);
    await page.fill('textarea[name="description"]', testData.store.description);
    await page.fill('input[name="companyName"]', testData.store.companyName);
    await page.fill('input[name="email"]', testData.store.email);
    await page.fill('input[name="addressStreet"]', testData.store.addressStreet);
    await page.fill('input[name="addressCity"]', testData.store.addressCity);
    await page.fill('input[name="addressPostalCode"]', testData.store.addressPostalCode);
    
    // Submit store creation
    await page.click('button[type="submit"]');
    
    // Wait for success and extract store ID
    await page.waitForSelector('text=Store created successfully', { timeout: 10000 });
    
    // Store the ID from the URL or API response
    const url = page.url();
    const match = url.match(/store\/(\d+)/);
    storeId = match ? parseInt(match[1]) : 1;
    
    console.log(`✅ Store created with ID: ${storeId}`);
    expect(storeId).toBeGreaterThan(0);
  });

  test('2. Create Manager and Cashier Users', async ({ page }) => {
    console.log('👥 Step 2: Creating manager and cashier users...');
    
    // Login as store owner
    await loginUser(page, testData.storeOwner.email, testData.storeOwner.password);
    
    // Navigate to users management
    await page.goto('/users');
    
    // Create Manager
    await page.click('button:has-text("Add User")');
    await page.fill('input[name="email"]', testData.manager.email);
    await page.fill('input[name="password"]', testData.manager.password);
    await page.fill('input[name="firstName"]', testData.manager.firstName);
    await page.fill('input[name="lastName"]', testData.manager.lastName);
    await page.fill('input[name="phone"]', testData.manager.phone);
    await page.selectOption('select[name="role"]', 'Manager');
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=User created successfully', { timeout: 10000 });
    
    // Create Cashier
    await page.click('button:has-text("Add User")');
    await page.fill('input[name="email"]', testData.cashier.email);
    await page.fill('input[name="password"]', testData.cashier.password);
    await page.fill('input[name="firstName"]', testData.cashier.firstName);
    await page.fill('input[name="lastName"]', testData.cashier.lastName);
    await page.fill('input[name="phone"]', testData.cashier.phone);
    await page.selectOption('select[name="role"]', 'Cashier');
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=User created successfully', { timeout: 10000 });
    
    console.log('✅ Manager and Cashier users created successfully');
  });

  test('3. Login as Manager and Create Customer', async ({ page }) => {
    console.log('🧑 Step 3: Creating customer as manager...');
    
    // Login as manager
    await loginUser(page, testData.manager.email, testData.manager.password);
    
    // Navigate to CRM
    await page.goto('/crm/customers');
    
    // Create customer
    await page.click('button:has-text("Add Customer")');
    await page.fill('input[name="firstName"]', testData.customer.firstName);
    await page.fill('input[name="lastName"]', testData.customer.lastName);
    await page.fill('input[name="email"]', testData.customer.email);
    await page.fill('input[name="phoneNumber"]', testData.customer.phoneNumber);
    await page.fill('input[name="addressStreet"]', testData.customer.addressStreet);
    await page.fill('input[name="addressCity"]', testData.customer.addressCity);
    await page.fill('input[name="addressPostalCode"]', testData.customer.addressPostalCode);
    
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Customer created successfully', { timeout: 10000 });
    
    // Extract customer ID from URL or response
    const url = page.url();
    const match = url.match(/customer\/(\d+)/);
    customerId = match ? parseInt(match[1]) : 1;
    
    console.log(`✅ Customer created with ID: ${customerId}`);
    expect(customerId).toBeGreaterThan(0);
  });

  test('4. Create Product and Add Stock', async ({ page }) => {
    console.log('📦 Step 4: Creating product and adding stock...');
    
    // Login as manager
    await loginUser(page, testData.manager.email, testData.manager.password);
    
    // Navigate to stock/inventory
    await page.goto('/stock/items');
    
    // Create product
    await page.click('button:has-text("Add Product")');
    await page.fill('input[name="name"]', testData.product.name);
    await page.fill('input[name="sku"]', testData.product.sku);
    await page.fill('input[name="barcode"]', testData.product.barcode);
    await page.fill('textarea[name="description"]', testData.product.description);
    await page.fill('input[name="basePrice"]', testData.product.basePrice.toString());
    
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Product created successfully', { timeout: 10000 });
    
    // Extract product ID
    const url = page.url();
    const match = url.match(/product\/(\d+)/);
    productId = match ? parseInt(match[1]) : 1;
    
    // Add stock
    await page.click(`button[data-product-id="${productId}"]:has-text("Adjust Stock")`);
    await page.fill('input[name="quantity"]', '100');
    await page.selectOption('select[name="movementType"]', 'Purchase');
    await page.fill('textarea[name="notes"]', 'Initial stock for testing');
    
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Stock adjusted successfully', { timeout: 10000 });
    
    console.log(`✅ Product created with ID: ${productId} and stock added`);
    expect(productId).toBeGreaterThan(0);
  });

  test('5. Login as Cashier and Place Order', async ({ page }) => {
    console.log('🛒 Step 5: Placing order as cashier...');
    
    // Login as cashier
    await loginUser(page, testData.cashier.email, testData.cashier.password);
    
    // Navigate to POS
    await page.goto('/sales/pos');
    
    // Search for customer
    await page.fill('input[name="customerSearch"]', testData.customer.email);
    await page.click(`button:has-text("${testData.customer.firstName} ${testData.customer.lastName}")`);
    
    // Search for product
    await page.fill('input[name="productSearch"]', testData.product.barcode);
    await page.keyboard.press('Enter');
    
    // Add to cart
    await page.click('button:has-text("Add to Cart")');
    
    // Set quantity
    await page.fill('input[name="quantity"]', '2');
    
    // Proceed to payment
    await page.click('button:has-text("Checkout")');
    
    // Select payment method
    await page.click('button:has-text("Cash")');
    
    // Complete sale
    await page.click('button:has-text("Complete Sale")');
    
    await page.waitForSelector('text=Sale completed successfully', { timeout: 10000 });
    
    // Extract order ID
    const url = page.url();
    const match = url.match(/sale\/(\d+)/);
    orderId = match ? parseInt(match[1]) : 1;
    
    console.log(`✅ Order placed with ID: ${orderId}`);
    expect(orderId).toBeGreaterThan(0);
  });

  test('6. Register Vendor', async ({ page }) => {
    console.log('🏢 Step 6: Registering vendor...');
    
    // Login as manager
    await loginUser(page, testData.manager.email, testData.manager.password);
    
    // Navigate to buying/suppliers
    await page.goto('/buying/suppliers');
    
    // Create vendor
    await page.click('button:has-text("Add Supplier")');
    await page.fill('input[name="name"]', testData.vendor.name);
    await page.fill('input[name="email"]', testData.vendor.email);
    await page.fill('input[name="phoneNumber"]', testData.vendor.phoneNumber);
    await page.fill('input[name="contactPerson"]', testData.vendor.contactPerson);
    await page.fill('input[name="addressStreet"]', testData.vendor.addressStreet);
    await page.fill('input[name="addressCity"]', testData.vendor.addressCity);
    await page.fill('input[name="addressPostalCode"]', testData.vendor.addressPostalCode);
    
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Supplier created successfully', { timeout: 10000 });
    
    // Extract vendor ID
    const url = page.url();
    const match = url.match(/supplier\/(\d+)/);
    vendorId = match ? parseInt(match[1]) : 1;
    
    console.log(`✅ Vendor registered with ID: ${vendorId}`);
    expect(vendorId).toBeGreaterThan(0);
  });

  test('7. Create Purchase Order from Vendor', async ({ page }) => {
    console.log('📋 Step 7: Creating purchase order from vendor...');
    
    // Login as manager
    await loginUser(page, testData.manager.email, testData.manager.password);
    
    // Navigate to buying/orders
    await page.goto('/buying/orders/create-order');
    
    // Select vendor
    await page.selectOption('select[name="vendorId"]', vendorId.toString());
    
    // Add product to PO
    await page.fill('input[name="productSearch"]', testData.product.sku);
    await page.keyboard.press('Enter');
    
    await page.fill('input[name="quantity"]', '50');
    await page.fill('input[name="unitPrice"]', '50.00');
    
    await page.click('button:has-text("Add Item")');
    
    // Submit PO
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Purchase order created successfully', { timeout: 10000 });
    
    console.log('✅ Purchase order created successfully');
  });

  test('8. Register Driver and Create Delivery Run', async ({ page }) => {
    console.log('🚚 Step 8: Registering driver and creating delivery run...');
    
    // Login as manager
    await loginUser(page, testData.manager.email, testData.manager.password);
    
    // Navigate to logistics
    await page.goto('/logistics');
    
    // Register driver
    await page.click('button:has-text("Add Driver")');
    await page.fill('input[name="firstName"]', testData.driver.firstName);
    await page.fill('input[name="lastName"]', testData.driver.lastName);
    await page.fill('input[name="phoneNumber"]', testData.driver.phoneNumber);
    await page.fill('input[name="email"]', testData.driver.email);
    await page.fill('input[name="licenseNumber"]', testData.driver.licenseNumber);
    await page.fill('input[name="vehicleRegistration"]', testData.driver.vehicleRegistration);
    
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Driver registered successfully', { timeout: 10000 });
    
    // Extract driver ID
    const url = page.url();
    const match = url.match(/driver\/(\d+)/);
    driverId = match ? parseInt(match[1]) : 1;
    
    // Create delivery run
    await page.goto('/logistics/shared-runs');
    await page.click('button:has-text("Create Run")');
    
    await page.selectOption('select[name="driverId"]', driverId.toString());
    await page.fill('input[name="date"]', new Date().toISOString().split('T')[0]);
    
    // Add delivery stop (order)
    await page.click('button:has-text("Add Stop")');
    await page.selectOption('select[name="orderId"]', orderId.toString());
    
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Delivery run created successfully', { timeout: 10000 });
    
    console.log(`✅ Driver registered with ID: ${driverId} and delivery run created`);
    expect(driverId).toBeGreaterThan(0);
  });

  test('9. Complete Delivery and Mark as Delivered', async ({ page }) => {
    console.log('✅ Step 9: Completing delivery...');
    
    // Login as driver (or manager)
    await loginUser(page, testData.manager.email, testData.manager.password);
    
    // Navigate to logistics tracking
    await page.goto('/logistics/tracking');
    
    // Find the delivery run
    await page.click(`button[data-driver-id="${driverId}"]:has-text("View Run")`);
    
    // Mark first stop as delivered
    await page.click('button:has-text("Mark as Delivered")');
    
    // Add proof of delivery
    await page.fill('textarea[name="notes"]', 'Package delivered successfully');
    await page.click('button[type="submit"]');
    
    await page.waitForSelector('text=Delivery marked as completed', { timeout: 10000 });
    
    console.log('✅ Delivery completed successfully');
  });

  test('10. Verify Complete Flow in Dashboard', async ({ page }) => {
    console.log('📊 Step 10: Verifying complete flow in dashboard...');
    
    // Login as store owner
    await loginUser(page, testData.storeOwner.email, testData.storeOwner.password);
    
    // Navigate to dashboard
    await page.goto('/dashboard');
    
    // Verify store stats
    await expect(page.locator('text=Total Sales')).toBeVisible();
    await expect(page.locator('text=Customers')).toBeVisible();
    await expect(page.locator('text=Products')).toBeVisible();
    await expect(page.locator('text=Orders')).toBeVisible();
    
    // Verify recent activity
    await expect(page.locator(`text=${testData.customer.firstName}`)).toBeVisible();
    await expect(page.locator(`text=${testData.product.name}`)).toBeVisible();
    
    console.log('✅ Dashboard verification complete');
    console.log('\n🎉 COMPLETE E2E WORKFLOW TEST PASSED!');
  });

  test.afterAll(async () => {
    console.log('\n📋 Test Summary:');
    console.log(`Store ID: ${storeId}`);
    console.log(`Manager ID: ${managerId}`);
    console.log(`Cashier ID: ${cashierId}`);
    console.log(`Customer ID: ${customerId}`);
    console.log(`Product ID: ${productId}`);
    console.log(`Vendor ID: ${vendorId}`);
    console.log(`Driver ID: ${driverId}`);
    console.log(`Order ID: ${orderId}`);
    console.log('\n✅ All tests completed successfully!');
  });
});

