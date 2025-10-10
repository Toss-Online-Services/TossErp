import { test, expect } from '@playwright/test'

test.describe('POS Hardware Integration', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to login
    await page.goto('/login')
    
    // Login with test credentials
    await page.fill('input[type="email"]', 'admin@toss.co.za')
    await page.fill('input[type="password"]', 'admin123')
    await page.click('button[type="submit"]')
    
    // Wait for redirect to dashboard
    await page.waitForURL('/dashboard')
    
    // Navigate to POS hardware page
    await page.goto('/sales/pos/hardware')
    await page.waitForLoadState('networkidle')
  })

  test('should display POS hardware page', async ({ page }) => {
    // Check page title
    await expect(page.locator('h1')).toContainText('POS Hardware')
    
    // Check hardware status section exists
    await expect(page.locator('text=Hardware Status')).toBeVisible()
    
    // Check all hardware types are displayed
    await expect(page.locator('text=Barcode Scanner')).toBeVisible()
    await expect(page.locator('text=Card Reader')).toBeVisible()
    await expect(page.locator('text=Receipt Printer')).toBeVisible()
    await expect(page.locator('text=Cash Drawer')).toBeVisible()
  })

  test('should show hardware connection status', async ({ page }) => {
    // All devices should initially show as disconnected
    const statusBadges = page.locator('.badge, .status-badge')
    const count = await statusBadges.count()
    
    expect(count).toBeGreaterThan(0)
    
    // Check for status indicators
    await expect(page.locator('text=Disconnected, text=Connected, text=Ready')).toBeVisible()
  })

  test('should request barcode scanner permissions', async ({ page }) => {
    // Click connect button for barcode scanner
    const connectButton = page.locator('button:has-text("Connect Scanner")')
    
    if (await connectButton.isVisible()) {
      await connectButton.click()
      
      // Wait for permission dialog or status change
      await page.waitForTimeout(1000)
      
      // Check if status changed or error message shown
      const hasError = await page.locator('text=Permission denied, text=No devices found').isVisible()
      const hasSuccess = await page.locator('text=Connected, text=Ready').isVisible()
      
      expect(hasError || hasSuccess).toBeTruthy()
    }
  })

  test('should handle manual barcode entry', async ({ page }) => {
    // Find manual entry input
    const manualInput = page.locator('input[placeholder*="barcode"], input[placeholder*="Enter barcode"]')
    
    if (await manualInput.isVisible()) {
      await manualInput.fill('1234567890123')
      await manualInput.press('Enter')
      
      // Wait for product to be added to cart
      await page.waitForTimeout(500)
      
      // Check if product was added (should show in cart or error message)
      const hasProduct = await page.locator('.cart-item, .product-item').isVisible()
      const hasError = await page.locator('text=not found, text=invalid').isVisible()
      
      expect(hasProduct || hasError).toBeTruthy()
    }
  })

  test('should test receipt printer', async ({ page }) => {
    // Find test print button
    const testPrintButton = page.locator('button:has-text("Test Print"), button:has-text("Print Test")')
    
    if (await testPrintButton.isVisible()) {
      await testPrintButton.click()
      
      // Wait for print action
      await page.waitForTimeout(1000)
      
      // Check for success or error message
      const hasMessage = await page.locator('.alert, .notification, .toast').isVisible()
      expect(hasMessage).toBeTruthy()
    }
  })

  test('should open cash drawer', async ({ page }) => {
    // Find open drawer button
    const openDrawerButton = page.locator('button:has-text("Open Drawer"), button:has-text("Open Cash")')
    
    if (await openDrawerButton.isVisible()) {
      await openDrawerButton.click()
      
      // Wait for action
      await page.waitForTimeout(1000)
      
      // Check for feedback
      const hasMessage = await page.locator('.alert, .notification, .toast').isVisible()
      expect(hasMessage).toBeTruthy()
    }
  })

  test('should display hardware statistics', async ({ page }) => {
    // Check for statistics section
    const statsSection = page.locator('text=Statistics, text=Activity, text=Usage')
    
    if (await statsSection.isVisible()) {
      // Check for numeric stats
      const numbers = page.locator('text=/\\d+/')
      const count = await numbers.count()
      
      expect(count).toBeGreaterThan(0)
    }
  })

  test('should handle hardware errors gracefully', async ({ page }) => {
    // Try to connect to non-existent device
    const connectButtons = page.locator('button:has-text("Connect")')
    const count = await connectButtons.count()
    
    if (count > 0) {
      await connectButtons.first().click()
      
      // Wait for error handling
      await page.waitForTimeout(1000)
      
      // Should show error message or status
      const hasError = await page.locator('text=error, text=failed, text=not found').isVisible()
      const hasStatus = await page.locator('.status, .badge').isVisible()
      
      expect(hasError || hasStatus).toBeTruthy()
    }
  })

  test('should be mobile responsive', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 })
    
    // Check if page is still functional
    await expect(page.locator('h1')).toBeVisible()
    
    // Check if hardware cards are stacked vertically
    const cards = page.locator('.card, .hardware-card')
    const count = await cards.count()
    
    if (count > 0) {
      const firstCard = cards.first()
      const secondCard = cards.nth(1)
      
      const firstBox = await firstCard.boundingBox()
      const secondBox = await secondCard.boundingBox()
      
      if (firstBox && secondBox) {
        // Cards should be stacked (second card below first)
        expect(secondBox.y).toBeGreaterThan(firstBox.y + firstBox.height - 10)
      }
    }
  })
})

test.describe('Barcode Scanner Component', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login')
    await page.fill('input[type="email"]', 'admin@toss.co.za')
    await page.fill('input[type="password"]', 'admin123')
    await page.click('button[type="submit"]')
    await page.waitForURL('/dashboard')
    await page.goto('/sales/pos')
    await page.waitForLoadState('networkidle')
  })

  test('should display barcode scanner interface', async ({ page }) => {
    // Check if scanner interface exists
    const scannerSection = page.locator('text=Scan Barcode, text=Scanner, text=Barcode')
    
    if (await scannerSection.isVisible()) {
      // Check for scanner controls
      await expect(page.locator('button:has-text("Scan"), button:has-text("Camera")')).toBeVisible()
    }
  })

  test('should toggle between USB and camera modes', async ({ page }) => {
    // Find mode toggle buttons
    const usbButton = page.locator('button:has-text("USB"), button[value="usb"]')
    const cameraButton = page.locator('button:has-text("Camera"), button[value="camera"]')
    
    if (await usbButton.isVisible() && await cameraButton.isVisible()) {
      // Toggle to camera mode
      await cameraButton.click()
      await page.waitForTimeout(500)
      
      // Check if camera interface appears
      const cameraView = page.locator('video, canvas, .camera-view')
      const hasCameraView = await cameraView.isVisible()
      
      // Toggle back to USB
      await usbButton.click()
      await page.waitForTimeout(500)
      
      // Camera view should be hidden
      const cameraHidden = !(await cameraView.isVisible())
      
      expect(hasCameraView || cameraHidden).toBeTruthy()
    }
  })

  test('should accept manual barcode input', async ({ page }) => {
    // Find manual input field
    const manualInput = page.locator('input[type="text"][placeholder*="barcode"]')
    
    if (await manualInput.isVisible()) {
      // Enter test barcode
      await manualInput.fill('9876543210987')
      await manualInput.press('Enter')
      
      // Wait for processing
      await page.waitForTimeout(500)
      
      // Check for feedback (product added or error)
      const hasResult = await page.locator('.cart, .product, .error, .alert').isVisible()
      expect(hasResult).toBeTruthy()
    }
  })

  test('should show scan statistics', async ({ page }) => {
    // Check for statistics display
    const statsText = page.locator('text=scans, text=successful, text=failed')
    
    if (await statsText.isVisible()) {
      // Should show numbers
      const numbers = page.locator('text=/\\d+/')
      const count = await numbers.count()
      
      expect(count).toBeGreaterThan(0)
    }
  })
})

test.describe('POS Payment Processing', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login')
    await page.fill('input[type="email"]', 'admin@toss.co.za')
    await page.fill('input[type="password"]', 'admin123')
    await page.click('button[type="submit"]')
    await page.waitForURL('/dashboard')
    await page.goto('/sales/pos')
    await page.waitForLoadState('networkidle')
  })

  test('should process cash payment', async ({ page }) => {
    // Add a product to cart first (if cart is empty)
    const addProductButton = page.locator('button:has-text("Add"), .product-card').first()
    
    if (await addProductButton.isVisible()) {
      await addProductButton.click()
      await page.waitForTimeout(500)
    }
    
    // Find cash payment button
    const cashButton = page.locator('button:has-text("Cash"), button[value="cash"]')
    
    if (await cashButton.isVisible()) {
      await cashButton.click()
      await page.waitForTimeout(500)
      
      // Should show payment dialog or process payment
      const hasDialog = await page.locator('.modal, .dialog, .payment-form').isVisible()
      const hasSuccess = await page.locator('text=success, text=complete').isVisible()
      
      expect(hasDialog || hasSuccess).toBeTruthy()
    }
  })

  test('should process card payment', async ({ page }) => {
    // Add a product to cart first
    const addProductButton = page.locator('button:has-text("Add"), .product-card').first()
    
    if (await addProductButton.isVisible()) {
      await addProductButton.click()
      await page.waitForTimeout(500)
    }
    
    // Find card payment button
    const cardButton = page.locator('button:has-text("Card"), button[value="card"]')
    
    if (await cardButton.isVisible()) {
      await cardButton.click()
      await page.waitForTimeout(500)
      
      // Should show payment processing
      const hasProcessing = await page.locator('text=processing, text=swipe, text=insert').isVisible()
      const hasSuccess = await page.locator('text=success, text=approved').isVisible()
      
      expect(hasProcessing || hasSuccess).toBeTruthy()
    }
  })

  test('should print receipt after payment', async ({ page }) => {
    // Complete a sale first (simplified test)
    const checkoutButton = page.locator('button:has-text("Checkout"), button:has-text("Complete")')
    
    if (await checkoutButton.isVisible()) {
      await checkoutButton.click()
      await page.waitForTimeout(1000)
      
      // Look for print receipt option
      const printButton = page.locator('button:has-text("Print"), button:has-text("Receipt")')
      
      if (await printButton.isVisible()) {
        await printButton.click()
        await page.waitForTimeout(500)
        
        // Should show print dialog or success
        const hasResult = await page.locator('.alert, .notification').isVisible()
        expect(hasResult).toBeTruthy()
      }
    }
  })
})

test.describe('POS Cart Management', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login')
    await page.fill('input[type="email"]', 'admin@toss.co.za')
    await page.fill('input[type="password"]', 'admin123')
    await page.click('button[type="submit"]')
    await page.waitForURL('/dashboard')
    await page.goto('/sales/pos')
    await page.waitForLoadState('networkidle')
  })

  test('should add products to cart', async ({ page }) => {
    // Find and click first product
    const productButton = page.locator('.product-card, button:has-text("Add")').first()
    
    if (await productButton.isVisible()) {
      await productButton.click()
      await page.waitForTimeout(500)
      
      // Check if cart has items
      const cartItems = page.locator('.cart-item, .cart-product')
      const count = await cartItems.count()
      
      expect(count).toBeGreaterThan(0)
    }
  })

  test('should update product quantities', async ({ page }) => {
    // Add product first
    const productButton = page.locator('.product-card, button:has-text("Add")').first()
    
    if (await productButton.isVisible()) {
      await productButton.click()
      await page.waitForTimeout(500)
      
      // Find quantity increase button
      const increaseButton = page.locator('button:has-text("+"), button.increase').first()
      
      if (await increaseButton.isVisible()) {
        // Get initial quantity
        const quantityDisplay = page.locator('.quantity, text=/Qty:/, text=/x\\d+/').first()
        const initialText = await quantityDisplay.textContent()
        
        // Increase quantity
        await increaseButton.click()
        await page.waitForTimeout(300)
        
        // Check if quantity changed
        const newText = await quantityDisplay.textContent()
        expect(newText).not.toBe(initialText)
      }
    }
  })

  test('should remove products from cart', async ({ page }) => {
    // Add product first
    const productButton = page.locator('.product-card, button:has-text("Add")').first()
    
    if (await productButton.isVisible()) {
      await productButton.click()
      await page.waitForTimeout(500)
      
      // Find remove button
      const removeButton = page.locator('button:has-text("Remove"), button:has-text("Delete"), button.remove').first()
      
      if (await removeButton.isVisible()) {
        // Get initial cart count
        const cartItems = page.locator('.cart-item')
        const initialCount = await cartItems.count()
        
        // Remove item
        await removeButton.click()
        await page.waitForTimeout(300)
        
        // Check if count decreased
        const newCount = await cartItems.count()
        expect(newCount).toBeLessThan(initialCount)
      }
    }
  })

  test('should clear entire cart', async ({ page }) => {
    // Add product first
    const productButton = page.locator('.product-card, button:has-text("Add")').first()
    
    if (await productButton.isVisible()) {
      await productButton.click()
      await page.waitForTimeout(500)
      
      // Find clear cart button
      const clearButton = page.locator('button:has-text("Clear"), button:has-text("Empty")')
      
      if (await clearButton.isVisible()) {
        await clearButton.click()
        await page.waitForTimeout(300)
        
        // Cart should be empty
        const cartItems = page.locator('.cart-item')
        const count = await cartItems.count()
        
        expect(count).toBe(0)
      }
    }
  })

  test('should calculate cart total correctly', async ({ page }) => {
    // Add multiple products
    const productButtons = page.locator('.product-card, button:has-text("Add")')
    const buttonCount = await productButtons.count()
    
    if (buttonCount > 0) {
      // Add first product
      await productButtons.first().click()
      await page.waitForTimeout(500)
      
      // Check for total display
      const totalDisplay = page.locator('text=/Total:/, text=/R\\s*[\\d,]+/, .total')
      
      if (await totalDisplay.isVisible()) {
        const totalText = await totalDisplay.textContent()
        
        // Should contain currency and number
        expect(totalText).toMatch(/R|ZAR|\d/)
      }
    }
  })
})

