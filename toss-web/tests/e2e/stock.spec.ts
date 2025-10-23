import { test, expect } from '@playwright/test'

test.describe('Stock Module - E2E Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to the stock items page
    await page.goto('http://localhost:3001/stock/items')
    await page.waitForLoadState('networkidle')
  })

  test.describe('Mobile Layout', () => {
    test.use({ viewport: { width: 375, height: 667 } })

    test('should display mobile header correctly', async ({ page }) => {
      // Check for hamburger menu
      const hamburger = page.locator('button').first()
      await expect(hamburger).toBeVisible()

      // Check for TOSS ERP logo
      await expect(page.locator('text=TOSS ERP')).toBeVisible()

      // Check for notification bell
      const bellIcon = page.locator('[role="button"]:has-text("notification")')
      await expect(bellIcon).toBeVisible()
    })

    test('should display bottom navigation on mobile', async ({ page }) => {
      // Check for bottom nav items
      await expect(page.locator('text=Home')).toBeVisible()
      await expect(page.locator('text=Group Buying')).toBeVisible()
    })

    test('should stack stat cards vertically on mobile', async ({ page }) => {
      const statsContainer = page.locator('.grid').first()
      await expect(statsContainer).toBeVisible()

      // Stats should be visible
      await expect(page.locator('text=Total Items')).toBeVisible()
      await expect(page.locator('text=Low Stock')).toBeVisible()
      await expect(page.locator('text=Out of Stock')).toBeVisible()
      await expect(page.locator('text=Total Value')).toBeVisible()
    })

    test('should make table scrollable horizontally on mobile', async ({ page }) => {
      const table = page.locator('table')
      await expect(table).toBeVisible()

      // Table should have overflow styling
      const tableContainer = page.locator('table').locator('..')
      await expect(tableContainer).toBeVisible()
    })

    test('should display action buttons in grid on mobile', async ({ page }) => {
      await page.goto('http://localhost:3001/stock/movements')
      await page.waitForLoadState('networkidle')

      // Quick action buttons should be visible
      await expect(page.locator('text=Stock IN ↓')).toBeVisible()
      await expect(page.locator('text=Stock OUT ↑')).toBeVisible()
      await expect(page.locator('text=Stock MOVED →')).toBeVisible()
      await expect(page.locator('text=Stock FIXED ⇌')).toBeVisible()
    })
  })

  test.describe('Items Page - Desktop', () => {
    test.use({ viewport: { width: 1280, height: 720 } })

    test('should display page header and stats', async ({ page }) => {
      await expect(page.locator('h1:has-text("Items Management")')).toBeVisible()
      await expect(page.locator('text=Manage inventory items')).toBeVisible()

      // Stats should be visible
      await expect(page.locator('text=Total Items')).toBeVisible()
      await expect(page.locator('text=Low Stock')).toBeVisible()
    })

    test('should display Add Item button', async ({ page }) => {
      const addButton = page.locator('button:has-text("Add Item")')
      await expect(addButton).toBeVisible()
      await expect(addButton).toBeEnabled()
    })

    test('should open Add Item modal when button is clicked', async ({ page }) => {
      await page.click('button:has-text("Add Item")')

      // Modal should be visible
      await expect(page.locator('text=Create New Item')).toBeVisible()
      await expect(page.locator('input#name')).toBeVisible()
      await expect(page.locator('input#sku')).toBeVisible()
    })

    test('should display items in table', async ({ page }) => {
      // Check table headers
      await expect(page.locator('th:has-text("Item Details")')).toBeVisible()
      await expect(page.locator('th:has-text("SKU / Barcode")')).toBeVisible()
      await expect(page.locator('th:has-text("Stock Level")')).toBeVisible()

      // Check for sample items (demo data)
      await expect(page.locator('text=White Bread Loaf')).toBeVisible()
      await expect(page.locator('text=BREAD-001')).toBeVisible()
    })

    test('should filter items by search query', async ({ page }) => {
      const searchInput = page.locator('input[placeholder*="Search"]')
      await searchInput.fill('Bread')
      await page.waitForTimeout(500) // Wait for debounce

      // Should show only bread items
      await expect(page.locator('text=White Bread Loaf')).toBeVisible()
    })

    test('should filter items by category', async ({ page }) => {
      const categorySelect = page.locator('select').first()
      await categorySelect.selectOption('Bakery')

      // Should show only bakery items
      await expect(page.locator('text=Bakery')).toBeVisible()
    })

    test('should display low stock warning icon', async ({ page }) => {
      // Look for warning icons (Fresh Milk is low stock)
      const warningIcon = page.locator('[class*="text-orange"]').first()
      await expect(warningIcon).toBeVisible()
    })

    test('should have working pagination', async ({ page }) => {
      // Check pagination info
      await expect(page.locator('text=/Showing \\d+ to \\d+ of \\d+ results/')).toBeVisible()

      // Check pagination buttons
      const prevButton = page.locator('button:has-text("Previous")')
      const nextButton = page.locator('button:has-text("Next")')

      await expect(prevButton).toBeVisible()
      await expect(nextButton).toBeVisible()
    })

    test('should export data when export button is clicked', async ({ page }) => {
      const exportButton = page.locator('button:has-text("Export")')
      await expect(exportButton).toBeVisible()

      // Click export (should trigger download in real scenario)
      await exportButton.click()
    })
  })

  test.describe('Stock Movements Page - Desktop', () => {
    test.use({ viewport: { width: 1280, height: 720 } })

    test.beforeEach(async ({ page }) => {
      await page.goto('http://localhost:3001/stock/movements')
      await page.waitForLoadState('networkidle')
    })

    test('should display page header and quick actions', async ({ page }) => {
      await expect(page.locator('h1:has-text("Stock Movements")')).toBeVisible()
      await expect(page.locator('text=Track all stock transactions')).toBeVisible()
    })

    test('should display all quick action buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("Stock IN ↓")')).toBeVisible()
      await expect(page.locator('button:has-text("Stock OUT ↑")')).toBeVisible()
      await expect(page.locator('button:has-text("Stock MOVED →")')).toBeVisible()
      await expect(page.locator('button:has-text("Stock FIXED ⇌")')).toBeVisible()
    })

    test('should open modal when Stock IN is clicked', async ({ page }) => {
      await page.click('button:has-text("Stock IN ↓")')

      // Modal should open
      await expect(page.locator('text=Stock IN ↓')).toBeVisible()
      await expect(page.locator('text=Record new inventory received')).toBeVisible()
      await expect(page.locator('select#item')).toBeVisible()
    })

    test('should open modal when Stock OUT is clicked', async ({ page }) => {
      await page.click('button:has-text("Stock OUT ↑")')

      await expect(page.locator('text=Record inventory removed or sold')).toBeVisible()
    })

    test('should open modal when Stock MOVED is clicked', async ({ page }) => {
      await page.click('button:has-text("Stock MOVED →")')

      await expect(page.locator('text=Move stock between locations')).toBeVisible()
      // Transfer-specific fields
      await expect(page.locator('select#fromLocation')).toBeVisible()
      await expect(page.locator('select#toLocation')).toBeVisible()
    })

    test('should open modal when Stock FIXED is clicked', async ({ page }) => {
      await page.click('button:has-text("Stock FIXED ⇌")')

      await expect(page.locator('text=Correct inventory discrepancies')).toBeVisible()
      // Notes should be required for adjustment
      const notesTextarea = page.locator('textarea#notes')
      await expect(notesTextarea).toHaveAttribute('required')
    })

    test('should display movements table with data', async ({ page }) => {
      // Check table headers
      await expect(page.locator('th:has-text("Date & Reference")')).toBeVisible()
      await expect(page.locator('th:has-text("Type")')).toBeVisible()
      await expect(page.locator('th:has-text("Item")')).toBeVisible()
      await expect(page.locator('th:has-text("Quantity")')).toBeVisible()

      // Check for sample movements
      await expect(page.locator('text=RCP-20240115')).toBeVisible()
      await expect(page.locator('text=ISS-20240114')).toBeVisible()
    })

    test('should filter movements by type', async ({ page }) => {
      const typeSelect = page.locator('select').nth(1) // Second select is type filter
      await typeSelect.selectOption('receipt')

      // Should show only receipt movements
      await expect(page.locator('text=receipt')).toBeVisible()
    })

    test('should filter movements by search query', async ({ page }) => {
      const searchInput = page.locator('input[placeholder*="Search"]')
      await searchInput.fill('Bread')
      await page.waitForTimeout(500)

      // Should show only bread-related movements
      await expect(page.locator('text=White Bread Loaf')).toBeVisible()
    })

    test('should clear all filters', async ({ page }) => {
      // Apply filters
      const searchInput = page.locator('input[placeholder*="Search"]')
      await searchInput.fill('Test')

      const typeSelect = page.locator('select').nth(1)
      await typeSelect.selectOption('receipt')

      // Clear filters
      await page.click('button:has-text("Clear")')

      // Filters should be reset
      await expect(searchInput).toHaveValue('')
    })

    test('should export movements to CSV', async ({ page }) => {
      const exportButton = page.locator('button:has-text("Export CSV")')
      await expect(exportButton).toBeVisible()
      await exportButton.click()
    })
  })

  test.describe('Stock Movement Modal - Form Validation', () => {
    test.use({ viewport: { width: 1280, height: 720 } })

    test.beforeEach(async ({ page }) => {
      await page.goto('http://localhost:3001/stock/movements')
      await page.waitForLoadState('networkidle')
    })

    test('should validate required fields for receipt', async ({ page }) => {
      await page.click('button:has-text("Stock IN ↓")')

      // Try to submit without filling required fields
      await page.click('button:has-text("Record Receipt")')

      // Should show validation errors or not submit
      const itemSelect = page.locator('select#item')
      await expect(itemSelect).toHaveAttribute('required')

      const quantityInput = page.locator('input#quantity')
      await expect(quantityInput).toHaveAttribute('required')
    })

    test('should submit receipt form with valid data', async ({ page }) => {
      await page.click('button:has-text("Stock IN ↓")')

      // Fill in the form
      await page.selectOption('select#item', { index: 1 }) // Select first item
      await page.fill('input#quantity', '50')
      await page.fill('input#reference', 'PO-001')
      await page.fill('textarea#notes', 'New stock delivery')

      // Submit
      await page.click('button:has-text("Record Receipt")')

      // Should close modal and show success (in real app)
      await page.waitForTimeout(500)
    })

    test('should validate transfer requires both locations', async ({ page }) => {
      await page.click('button:has-text("Stock MOVED →")')

      const fromLocation = page.locator('select#fromLocation')
      const toLocation = page.locator('select#toLocation')

      await expect(fromLocation).toHaveAttribute('required')
      await expect(toLocation).toHaveAttribute('required')
    })

    test('should validate adjustment requires notes', async ({ page }) => {
      await page.click('button:has-text("Stock FIXED ⇌")')

      const notesTextarea = page.locator('textarea#notes')
      await expect(notesTextarea).toHaveAttribute('required')
    })

    test('should close modal when Cancel is clicked', async ({ page }) => {
      await page.click('button:has-text("Stock IN ↓")')

      // Modal should be open
      await expect(page.locator('text=Record new inventory received')).toBeVisible()

      // Click cancel
      await page.click('button:has-text("Cancel")')

      // Modal should close
      await page.waitForTimeout(300)
      await expect(page.locator('text=Record new inventory received')).not.toBeVisible()
    })

    test('should close modal when X button is clicked', async ({ page }) => {
      await page.click('button:has-text("Stock IN ↓")')

      // Find and click the X (close) button
      const closeButton = page.locator('button').first() // First button in modal is usually close
      await closeButton.click()

      // Modal should close
      await page.waitForTimeout(300)
      await expect(page.locator('text=Record new inventory received')).not.toBeVisible()
    })
  })

  test.describe('Accessibility', () => {
    test.use({ viewport: { width: 1280, height: 720 } })

    test('should have proper ARIA labels', async ({ page }) => {
      // Check for aria-label on buttons
      const editButtons = page.locator('[aria-label="Edit"]')
      const deleteButtons = page.locator('[aria-label="Delete"]')

      expect(await editButtons.count()).toBeGreaterThan(0)
      expect(await deleteButtons.count()).toBeGreaterThan(0)
    })

    test('should be keyboard navigable', async ({ page }) => {
      // Tab through elements
      await page.keyboard.press('Tab')
      await page.keyboard.press('Tab')
      await page.keyboard.press('Tab')

      // Focus should be visible
      const focusedElement = page.locator(':focus')
      await expect(focusedElement).toBeVisible()
    })

    test('should have proper heading hierarchy', async ({ page }) => {
      const h1 = page.locator('h1')
      await expect(h1).toBeVisible()

      const headingText = await h1.textContent()
      expect(headingText).toBeTruthy()
    })
  })

  test.describe('Performance', () => {
    test.use({ viewport: { width: 1280, height: 720 } })

    test('should load items page within acceptable time', async ({ page }) => {
      const startTime = Date.now()
      await page.goto('http://localhost:3001/stock/items')
      await page.waitForLoadState('networkidle')
      const loadTime = Date.now() - startTime

      // Should load in under 3 seconds
      expect(loadTime).toBeLessThan(3000)
    })

    test('should load movements page within acceptable time', async ({ page }) => {
      const startTime = Date.now()
      await page.goto('http://localhost:3001/stock/movements')
      await page.waitForLoadState('networkidle')
      const loadTime = Date.now() - startTime

      // Should load in under 3 seconds
      expect(loadTime).toBeLessThan(3000)
    })
  })
})

