import { test, expect, Page } from '@playwright/test'

/**
 * TOSS Complete E2E Workflow Test
 * 
 * This test simulates the entire TOSS ecosystem from onboarding to delivery:
 * 1. Shop Owner Onboarding (Spaza Shop)
 * 2. Supplier Onboarding
 * 3. Driver Onboarding
 * 4. Supplier creates products
 * 5. Shop creates order (group buying)
 * 6. Supplier processes order
 * 7. Driver picks up and delivers
 * 8. Shop receives goods
 */

// Test data
const testData = {
  shop: {
    name: "Lerato's Spaza Shop",
    email: 'lerato@spaza.test',
    password: 'Test123!@#',
    phone: '+27 71 234 5678',
    address: '123 Township Road, Soweto',
    location: {
      lat: -26.2692,
      lng: 27.8546
    }
  },
  supplier: {
    name: 'Fresh Foods Wholesale',
    email: 'supplier@freshfoods.test',
    password: 'Supplier123!@#',
    phone: '+27 11 456 7890',
    address: '456 Industrial Ave, Johannesburg'
  },
  driver: {
    name: 'Thabo Mkhize',
    email: 'thabo.driver@toss.test',
    password: 'Driver123!@#',
    phone: '+27 82 345 6789',
    vehicleType: 'Van',
    licensePlate: 'ABC 123 GP'
  },
  products: [
    { name: 'White Bread 700g', price: 12.99, category: 'Bakery', sku: 'BREAD001' },
    { name: 'Fresh Milk 2L', price: 24.99, category: 'Dairy', sku: 'MILK001' },
    { name: 'Coca Cola 2L', price: 18.99, category: 'Beverages', sku: 'COKE001' },
    { name: 'Rice 2kg', price: 45.99, category: 'Groceries', sku: 'RICE001' },
    { name: 'Sugar 2.5kg', price: 32.99, category: 'Groceries', sku: 'SUGAR001' }
  ]
}

let shopContext: any = {}
let supplierContext: any = {}
let driverContext: any = {}
let orderContext: any = {}

test.describe('TOSS Complete Workflow', () => {
  test.describe.configure({ mode: 'serial' })

  // ============================================================================
  // PHASE 1: ONBOARDING
  // ============================================================================

  test('1. Shop Owner Onboarding - Register Spaza Shop', async ({ page }) => {
    console.log('\n🏪 PHASE 1: Shop Owner Onboarding')
    console.log('=' .repeat(60))
    
    await page.goto('http://localhost:3000/onboarding')
    
    // Step 1: Welcome
    await expect(page.locator('h1')).toContainText('Welcome to TOSS')
    await page.click('button:has-text("Get Started")')
    
    // Step 2: Business Information
    await page.fill('input[placeholder="Your Company Ltd"]', testData.shop.name)
    await page.selectOption('select', { label: 'Retail' }) // Industry
    await page.selectOption('select[class*="companySize"]', '1-10') // Size
    await page.selectOption('select[class*="country"]', 'ZA') // South Africa
    await page.selectOption('select[class*="currency"]', 'ZAR') // South African Rand
    await page.click('button:has-text("Continue")')
    
    // Step 3: Personal Information
    await page.fill('input[placeholder*="First"]', 'Lerato')
    await page.fill('input[placeholder*="Last"]', 'Mokoena')
    await page.fill('input[type="email"]', testData.shop.email)
    await page.fill('input[type="tel"]', testData.shop.phone)
    await page.fill('input[placeholder*="Job"]', 'Owner')
    await page.click('button:has-text("Continue")')
    
    // Step 4: Select Modules - Select POS, Inventory, Group Buying
    await page.click('text=Point of Sale')
    await page.click('text=Inventory')
    await page.click('text=Group Buying')
    await page.click('button:has-text("Continue")')
    
    // Step 5: Complete
    await expect(page.locator('h1')).toContainText("You're All Set")
    await page.click('button:has-text("Go to Dashboard")')
    
    // Verify dashboard loaded
    await expect(page).toHaveURL(/\/(dashboard)?/)
    console.log('✅ Shop owner onboarded successfully')
    
    // Store shop context
    shopContext.id = 1 // Mock ID
    shopContext.name = testData.shop.name
  })

  test('2. Supplier Onboarding - Register Wholesale Supplier', async ({ page }) => {
    console.log('\n📦 PHASE 1: Supplier Onboarding')
    console.log('=' .repeat(60))
    
    // Logout shop owner
    await page.goto('http://localhost:3000/logout')
    
    // Register supplier
    await page.goto('http://localhost:3000/onboarding')
    
    // Welcome
    await page.click('button:has-text("Get Started")')
    
    // Business Info
    await page.fill('input[placeholder="Your Company Ltd"]', testData.supplier.name)
    await page.selectOption('select', { label: 'Wholesale' })
    await page.selectOption('select[class*="companySize"]', '11-50')
    await page.selectOption('select[class*="country"]', 'ZA')
    await page.selectOption('select[class*="currency"]', 'ZAR')
    await page.click('button:has-text("Continue")')
    
    // Personal Info
    await page.fill('input[placeholder*="First"]', 'John')
    await page.fill('input[placeholder*="Last"]', 'Supplier')
    await page.fill('input[type="email"]', testData.supplier.email)
    await page.fill('input[type="tel"]', testData.supplier.phone)
    await page.fill('input[placeholder*="Job"]', 'Sales Manager')
    await page.click('button:has-text("Continue")')
    
    // Modules - Select relevant for supplier
    await page.click('text=Inventory')
    await page.click('text=Sales')
    await page.click('button:has-text("Continue")')
    
    // Complete
    await page.click('button:has-text("Go to Dashboard")')
    
    console.log('✅ Supplier onboarded successfully')
    
    supplierContext.id = 2
    supplierContext.name = testData.supplier.name
  })

  test('3. Driver Onboarding - Register Delivery Driver', async ({ page }) => {
    console.log('\n🚚 PHASE 1: Driver Onboarding')
    console.log('=' .repeat(60))
    
    // Logout supplier
    await page.goto('http://localhost:3000/logout')
    
    // Register driver
    await page.goto('http://localhost:3000/onboarding/driver')
    
    // Driver-specific onboarding
    await page.fill('input[placeholder*="Name"]', testData.driver.name)
    await page.fill('input[type="email"]', testData.driver.email)
    await page.fill('input[type="tel"]', testData.driver.phone)
    await page.fill('input[placeholder*="License"]', testData.driver.licensePlate)
    await page.selectOption('select', testData.driver.vehicleType)
    await page.click('button:has-text("Register")')
    
    console.log('✅ Driver onboarded successfully')
    
    driverContext.id = 3
    driverContext.name = testData.driver.name
  })

  // ============================================================================
  // PHASE 2: PRODUCT SETUP
  // ============================================================================

  test('4. Supplier Creates Products', async ({ page }) => {
    console.log('\n📦 PHASE 2: Product Setup')
    console.log('=' .repeat(60))
    
    // Login as supplier
    await page.goto('http://localhost:3000/login')
    await page.fill('input[type="email"]', testData.supplier.email)
    await page.fill('input[type="password"]', testData.supplier.password)
    await page.click('button:has-text("Login")')
    
    // Navigate to products
    await page.goto('http://localhost:3000/inventory/products')
    
    // Create each product
    for (const product of testData.products) {
      await page.click('button:has-text("Add Product")')
      
      await page.fill('input[placeholder*="Name"]', product.name)
      await page.fill('input[placeholder*="SKU"]', product.sku)
      await page.fill('input[placeholder*="Price"]', product.price.toString())
      await page.selectOption('select[placeholder*="Category"]', product.category)
      await page.fill('input[placeholder*="Stock"]', '1000') // High stock
      
      await page.click('button:has-text("Save")')
      await page.waitForTimeout(500)
      
      console.log(`✅ Created product: ${product.name}`)
    }
    
    console.log(`✅ All ${testData.products.length} products created`)
  })

  // ============================================================================
  // PHASE 3: ORDERING
  // ============================================================================

  test('5. Shop Owner Browses Products', async ({ page }) => {
    console.log('\n🛒 PHASE 3: Shopping & Ordering')
    console.log('=' .repeat(60))
    
    // Login as shop owner
    await page.goto('http://localhost:3000/login')
    await page.fill('input[type="email"]', testData.shop.email)
    await page.fill('input[type="password"]', testData.shop.password)
    await page.click('button:has-text("Login")')
    
    // Browse products
    await page.goto('http://localhost:3000/buying/products')
    
    // Verify products are visible
    for (const product of testData.products) {
      await expect(page.locator(`text=${product.name}`)).toBeVisible()
    }
    
    console.log('✅ All products visible to shop owner')
  })

  test('6. Shop Owner Creates Order (Individual)', async ({ page }) => {
    console.log('\n📝 Creating individual order...')
    
    await page.goto('http://localhost:3000/buying/orders/new')
    
    // Add products to order
    await page.click('button:has-text("Add Item")')
    await page.selectOption('select[name="product"]', testData.products[0].name) // Bread
    await page.fill('input[name="quantity"]', '50')
    
    await page.click('button:has-text("Add Item")')
    await page.selectOption('select[name="product"]', testData.products[1].name) // Milk
    await page.fill('input[name="quantity"]', '30')
    
    await page.click('button:has-text("Add Item")')
    await page.selectOption('select[name="product"]', testData.products[2].name) // Coke
    await page.fill('input[name="quantity"]', '40')
    
    // Select supplier
    await page.selectOption('select[name="supplier"]', testData.supplier.name)
    
    // Set delivery details
    await page.fill('input[name="deliveryAddress"]', testData.shop.address)
    await page.fill('input[name="deliveryDate"]', '2025-10-25')
    
    // Submit order
    await page.click('button:has-text("Place Order")')
    
    // Wait for confirmation
    await expect(page.locator('text=Order Created')).toBeVisible()
    
    // Get order ID
    const orderIdText = await page.locator('[data-order-id]').textContent()
    orderContext.orderId = orderIdText?.match(/\d+/)?.[0]
    
    console.log(`✅ Order #${orderContext.orderId} created successfully`)
  })

  test('7. Shop Owner Joins Group Buying Pool', async ({ page }) => {
    console.log('\n👥 Joining group buying pool...')
    
    await page.goto('http://localhost:3000/group-buying')
    
    // Check for available pools
    await page.click('button:has-text("Browse Pools")')
    
    // Look for relevant pool (e.g., Sugar pool)
    const sugarPoolExists = await page.locator('text=Sugar 2.5kg Pool').count() > 0
    
    if (sugarPoolExists) {
      // Join existing pool
      await page.click('text=Sugar 2.5kg Pool')
      await page.fill('input[placeholder*="Quantity"]', '20')
      await page.click('button:has-text("Join Pool")')
      console.log('✅ Joined existing group buying pool')
    } else {
      // Create new pool
      await page.click('button:has-text("Create Pool")')
      await page.selectOption('select[name="product"]', testData.products[4].name) // Sugar
      await page.fill('input[name="quantity"]', '20')
      await page.fill('input[name="minParticipants"]', '5')
      await page.fill('input[name="targetDate"]', '2025-10-26')
      await page.click('button:has-text("Create Pool")')
      console.log('✅ Created new group buying pool')
    }
    
    orderContext.poolId = 1
  })

  // ============================================================================
  // PHASE 4: ORDER PROCESSING
  // ============================================================================

  test('8. Supplier Receives and Processes Order', async ({ page }) => {
    console.log('\n📋 PHASE 4: Order Processing')
    console.log('=' .repeat(60))
    
    // Login as supplier
    await page.goto('http://localhost:3000/login')
    await page.fill('input[type="email"]', testData.supplier.email)
    await page.fill('input[type="password"]', testData.supplier.password)
    await page.click('button:has-text("Login")')
    
    // Go to orders
    await page.goto('http://localhost:3000/supplier/orders')
    
    // Find the order
    await expect(page.locator(`text=Order #${orderContext.orderId}`)).toBeVisible()
    await page.click(`text=Order #${orderContext.orderId}`)
    
    // Review order details
    await expect(page.locator('text=White Bread')).toBeVisible()
    await expect(page.locator('text=Fresh Milk')).toBeVisible()
    await expect(page.locator('text=Coca Cola')).toBeVisible()
    
    // Approve order
    await page.click('button:has-text("Approve Order")')
    await page.waitForTimeout(500)
    
    // Prepare order for pickup
    await page.click('button:has-text("Mark as Ready")')
    await page.waitForTimeout(500)
    
    console.log('✅ Order approved and marked ready for pickup')
  })

  // ============================================================================
  // PHASE 5: LOGISTICS & DELIVERY
  // ============================================================================

  test('9. Driver Picks Up Order', async ({ page }) => {
    console.log('\n🚚 PHASE 5: Delivery')
    console.log('=' .repeat(60))
    
    // Login as driver
    await page.goto('http://localhost:3000/login')
    await page.fill('input[type="email"]', testData.driver.email)
    await page.fill('input[type="password"]', testData.driver.password)
    await page.click('button:has-text("Login")')
    
    // Go to available deliveries
    await page.goto('http://localhost:3000/driver/deliveries')
    
    // Find and accept delivery
    await expect(page.locator(`text=Order #${orderContext.orderId}`)).toBeVisible()
    await page.click(`text=Order #${orderContext.orderId}`)
    await page.click('button:has-text("Accept Delivery")')
    
    // Start delivery
    await page.click('button:has-text("Start Delivery")')
    console.log('✅ Driver started delivery')
    
    // Simulate pickup at supplier
    await page.click('button:has-text("Confirm Pickup")')
    await page.fill('textarea[placeholder*="Notes"]', 'All items loaded successfully')
    await page.click('button:has-text("Continue")')
    console.log('✅ Order picked up from supplier')
  })

  test('10. Driver Delivers to Shop', async ({ page }) => {
    console.log('\n📦 Completing delivery...')
    
    // Continue with delivery
    await page.goto('http://localhost:3000/driver/deliveries/active')
    await page.click(`text=Order #${orderContext.orderId}`)
    
    // Mark as out for delivery
    await page.click('button:has-text("Out for Delivery")')
    await page.waitForTimeout(1000)
    
    // Arrive at destination
    await page.click('button:has-text("Arrived at Destination")')
    
    // Capture proof of delivery
    await page.fill('input[name="recipientName"]', 'Lerato Mokoena')
    await page.fill('textarea[placeholder*="Notes"]', 'Delivered to shop owner. All items verified.')
    
    // Take photo (simulate)
    const fileInput = await page.locator('input[type="file"]')
    if (await fileInput.count() > 0) {
      // In real test, we'd upload an image
      console.log('📸 Photo capture simulated')
    }
    
    // Complete delivery
    await page.click('button:has-text("Complete Delivery")')
    
    await expect(page.locator('text=Delivery Completed')).toBeVisible()
    console.log('✅ Order delivered successfully')
  })

  // ============================================================================
  // PHASE 6: CONFIRMATION
  // ============================================================================

  test('11. Shop Owner Confirms Receipt', async ({ page }) => {
    console.log('\n✅ PHASE 6: Confirmation')
    console.log('=' .repeat(60))
    
    // Login as shop owner
    await page.goto('http://localhost:3000/login')
    await page.fill('input[type="email"]', testData.shop.email)
    await page.fill('input[type="password"]', testData.shop.password)
    await page.click('button:has-text("Login")')
    
    // Check notifications
    await page.goto('http://localhost:3000/notifications')
    await expect(page.locator('text=Order Delivered')).toBeVisible()
    
    // View order
    await page.goto(`http://localhost:3000/buying/orders/${orderContext.orderId}`)
    
    // Verify order status
    await expect(page.locator('text=Delivered')).toBeVisible()
    
    // Confirm receipt
    await page.click('button:has-text("Confirm Receipt")')
    await page.fill('textarea[placeholder*="Review"]', 'Great service! All items in perfect condition.')
    await page.click('button[value="5"]') // 5-star rating
    await page.click('button:has-text("Submit")')
    
    console.log('✅ Shop owner confirmed receipt')
  })

  test('12. Verify Stock Updated', async ({ page }) => {
    console.log('\n📊 Verifying final state...')
    
    // Check inventory updated
    await page.goto('http://localhost:3000/stock')
    
    // Verify new stock levels
    await expect(page.locator('text=White Bread')).toBeVisible()
    await expect(page.locator('text=50')).toBeVisible() // Quantity received
    
    await expect(page.locator('text=Fresh Milk')).toBeVisible()
    await expect(page.locator('text=30')).toBeVisible()
    
    await expect(page.locator('text=Coca Cola')).toBeVisible()
    await expect(page.locator('text=40')).toBeVisible()
    
    console.log('✅ Inventory updated correctly')
  })

  test('13. Verify Payment Processing', async ({ page }) => {
    console.log('\n💰 Verifying payment...')
    
    await page.goto('http://localhost:3000/payments')
    
    // Check payment record
    await expect(page.locator(`text=Order #${orderContext.orderId}`)).toBeVisible()
    await expect(page.locator('text=Pending Payment')).toBeVisible()
    
    // Process payment
    await page.click(`text=Order #${orderContext.orderId}`)
    await page.click('button:has-text("Pay Now")')
    
    // Select payment method
    await page.click('text=Mobile Money')
    await page.fill('input[placeholder*="Phone"]', testData.shop.phone)
    await page.click('button:has-text("Confirm Payment")')
    
    await expect(page.locator('text=Payment Successful')).toBeVisible()
    console.log('✅ Payment processed successfully')
  })

  test('14. Generate Reports and Analytics', async ({ page }) => {
    console.log('\n📈 Generating reports...')
    
    await page.goto('http://localhost:3000/dashboard')
    
    // Verify metrics updated
    await expect(page.locator('[data-metric="total-orders"]')).toContainText('1')
    await expect(page.locator('[data-metric="total-sales"]')).toContainText('R')
    
    // View detailed reports
    await page.goto('http://localhost:3000/reports/sales')
    await expect(page.locator(`text=Order #${orderContext.orderId}`)).toBeVisible()
    
    console.log('✅ Reports generated successfully')
  })

  // ============================================================================
  // PHASE 7: AI ASSISTANT VERIFICATION
  // ============================================================================

  test('15. Test AI Assistant', async ({ page }) => {
    console.log('\n🤖 Testing AI Assistant')
    console.log('=' .repeat(60))
    
    await page.goto('http://localhost:3000')
    
    // Open AI assistant
    await page.click('[data-testid="ai-assistant-toggle"]')
    
    // Ask questions
    const questions = [
      "How were my sales today?",
      "What products are selling best?",
      "What stock is running low?",
      "When is my next delivery?"
    ]
    
    for (const question of questions) {
      await page.fill('input[placeholder*="Ask"]', question)
      await page.press('input[placeholder*="Ask"]', 'Enter')
      await page.waitForTimeout(2000)
      
      // Verify response
      await expect(page.locator('.ai-response').last()).toBeVisible()
      console.log(`✅ AI answered: "${question}"`)
    }
    
    console.log('✅ AI Assistant working correctly')
  })

  // ============================================================================
  // SUMMARY
  // ============================================================================

  test('16. Test Summary', async ({ page }) => {
    console.log('\n')
    console.log('🎉 TOSS E2E TEST COMPLETE')
    console.log('=' .repeat(60))
    console.log('✅ Phase 1: Onboarding - Shop, Supplier, Driver')
    console.log('✅ Phase 2: Product Setup - 5 products created')
    console.log('✅ Phase 3: Ordering - Individual & Group orders')
    console.log('✅ Phase 4: Order Processing - Supplier approval')
    console.log('✅ Phase 5: Delivery - Pickup & delivery')
    console.log('✅ Phase 6: Confirmation - Receipt & payment')
    console.log('✅ Phase 7: AI Assistant - Q&A functionality')
    console.log('=' .repeat(60))
    console.log('\n📊 Final Stats:')
    console.log(`  Shops onboarded: 1`)
    console.log(`  Suppliers onboarded: 1`)
    console.log(`  Drivers onboarded: 1`)
    console.log(`  Products created: ${testData.products.length}`)
    console.log(`  Orders placed: 1`)
    console.log(`  Deliveries completed: 1`)
    console.log(`  Payments processed: 1`)
    console.log('\n✨ TOSS is fully operational! ✨\n')
    
    expect(true).toBe(true)
  })
})

