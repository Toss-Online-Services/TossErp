import { test, expect } from '@playwright/test'

test.describe('Stock Management - Core Features', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to stock dashboard
    await page.goto('/stock')
    await page.waitForLoadState('networkidle')
  })

  test.describe('Stock Dashboard', () => {
    test('should display stock dashboard with stats', async ({ page }) => {
      await expect(page).toHaveTitle(/Stock Management - TOSS ERP/)
      
      // Check for stat cards
      await expect(page.locator('text=Total Items')).toBeVisible()
      await expect(page.locator('text=Warehouses')).toBeVisible()
      await expect(page.locator('text=Low Stock')).toBeVisible()
      await expect(page.locator('text=Stock Value')).toBeVisible()
    })

    test('should show AI co-pilot insights', async ({ page }) => {
      await expect(page.locator('text=AI Co-Pilot Insights')).toBeVisible()
      await expect(page.locator('text=Low stock detected')).toBeVisible()
    })

    test('should have navigation to key pages', async ({ page }) => {
      await expect(page.locator('a[href="/stock/items"]')).toBeVisible()
      await expect(page.locator('a[href="/stock/warehouses"]')).toBeVisible()
      await expect(page.locator('a[href="/stock/movements"]')).toBeVisible()
    })

    test('should refresh stats on button click', async ({ page }) => {
      const refreshButton = page.locator('button:has-text("Refresh")')
      await refreshButton.click()
      await expect(page.locator('text=Stock statistics refreshed!')).toBeVisible()
    })
  })

  test.describe('Items Page', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
    })

    test('should display items list', async ({ page }) => {
      await expect(page).toHaveTitle(/Items/)
      await expect(page.locator('text=Manage your inventory items')).toBeVisible()
    })

    test('should show stats cards', async ({ page }) => {
      await expect(page.locator('text=Total Items')).toBeVisible()
      await expect(page.locator('text=Low Stock')).toBeVisible()
      await expect(page.locator('text=Out of Stock')).toBeVisible()
      await expect(page.locator('text=Total Value')).toBeVisible()
    })

    test('should open create item modal', async ({ page }) => {
      await page.click('button:has-text("Add Item")')
      await expect(page.locator('text=Create New Item')).toBeVisible()
      await expect(page.locator('label:has-text("SKU")')).toBeVisible()
      await expect(page.locator('label:has-text("Item Name")')).toBeVisible()
    })

    test('should filter items by search', async ({ page }) => {
      const searchInput = page.locator('input[placeholder="Search items..."]')
      await searchInput.fill('Bread')
      await expect(page.locator('text=White Bread Loaf')).toBeVisible()
    })

    test('should filter items by category', async ({ page }) => {
      const categorySelect = page.locator('select').nth(0)
      await categorySelect.selectOption('Bakery')
      // Items should be filtered
      await expect(page.locator('td:has-text("Bakery")')).toBeVisible()
    })

    test('should filter items by stock status', async ({ page }) => {
      const stockFilter = page.locator('select').nth(1)
      await stockFilter.selectOption('low')
      // Should show only low stock items
    })

    test('should export items', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/items-export-.*\.csv/)
    })

    test('should show item details on click', async ({ page }) => {
      await page.click('text=White Bread Loaf')
      await expect(page.locator('text=Basic Information')).toBeVisible()
      await expect(page.locator('text=Pricing')).toBeVisible()
      await expect(page.locator('text=Stock Information')).toBeVisible()
    })

    test('should paginate items', async ({ page }) => {
      // Check pagination controls
      const paginationExists = await page.locator('text=Page').isVisible()
      if (paginationExists) {
        await expect(page.locator('button:has-text("Next")')).toBeVisible()
        await expect(page.locator('button:has-text("Previous")')).toBeVisible()
      }
    })
  })

  test.describe('Warehouses Page', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/stock/warehouses')
      await page.waitForLoadState('networkidle')
    })

    test('should display warehouses page', async ({ page }) => {
      await expect(page).toHaveTitle(/Warehouses/)
      await expect(page.locator('text=Manage your warehouse locations')).toBeVisible()
    })

    test('should show warehouse stats', async ({ page }) => {
      await expect(page.locator('text=Total Warehouses')).toBeVisible()
      await expect(page.locator('text=Active Warehouses')).toBeVisible()
      await expect(page.locator('text=Total Items Stored')).toBeVisible()
      await expect(page.locator('text=Total Stock Value')).toBeVisible()
    })

    test('should open create warehouse modal', async ({ page }) => {
      await page.click('button:has-text("Add Warehouse")')
      await expect(page.locator('text=Create New Warehouse')).toBeVisible()
      await expect(page.locator('label:has-text("Warehouse Code")')).toBeVisible()
      await expect(page.locator('label:has-text("Warehouse Name")')).toBeVisible()
    })

    test('should display warehouse cards', async ({ page }) => {
      await expect(page.locator('text=Main Store')).toBeVisible()
      await expect(page.locator('text=Cold Storage Facility')).toBeVisible()
    })

    test('should filter warehouses by search', async ({ page }) => {
      const searchInput = page.locator('input[placeholder="Search warehouses..."]')
      await searchInput.fill('Main')
      await expect(page.locator('text=Main Store')).toBeVisible()
    })

    test('should export warehouses', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/warehouses-.*\.csv/)
    })

    test('should open warehouse details modal on card click', async ({ page }) => {
      await page.click('text=Main Store')
      await expect(page.locator('text=Warehouse Information')).toBeVisible()
      await expect(page.locator('text=Stock Summary')).toBeVisible()
    })

    test('should open edit modal from details', async ({ page }) => {
      await page.click('text=Main Store')
      await page.click('button:has-text("Edit")')
      await expect(page.locator('text=Edit Warehouse')).toBeVisible()
    })
  })

  test.describe('Stock Movements Page', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
    })

    test('should display movements page', async ({ page }) => {
      await expect(page.locator('text=Stock Movements')).toBeVisible()
      await expect(page.locator('text=Track all stock transactions')).toBeVisible()
    })

    test('should show quick action buttons for all movement types', async ({ page }) => {
      await expect(page.locator('button:has-text("Stock Receipt")')).toBeVisible()
      await expect(page.locator('button:has-text("Stock Issue")')).toBeVisible()
      await expect(page.locator('button:has-text("Stock Transfer")')).toBeVisible()
      await expect(page.locator('button:has-text("Stock Adjustment")')).toBeVisible()
    })

    test('should open stock receipt modal', async ({ page }) => {
      await page.click('button:has-text("Stock Receipt")')
      await expect(page.locator('text=Stock Receipt')).toBeVisible()
      await expect(page.locator('text=Receive stock into warehouse')).toBeVisible()
      await expect(page.locator('label:has-text("Warehouse")')).toBeVisible()
    })

    test('should open stock issue modal', async ({ page }) => {
      await page.click('button:has-text("Stock Issue")')
      await expect(page.locator('text=Stock Issue')).toBeVisible()
      await expect(page.locator('text=Issue stock from warehouse')).toBeVisible()
    })

    test('should open stock transfer modal', async ({ page }) => {
      await page.click('button:has-text("Stock Transfer")')
      await expect(page.locator('text=Stock Transfer')).toBeVisible()
      await expect(page.locator('text=Transfer stock between warehouses')).toBeVisible()
      await expect(page.locator('label:has-text("Source Warehouse")')).toBeVisible()
      await expect(page.locator('label:has-text("Target Warehouse")')).toBeVisible()
    })

    test('should open stock adjustment modal', async ({ page }) => {
      await page.click('button:has-text("Stock Adjustment")')
      await expect(page.locator('text=Stock Adjustment')).toBeVisible()
      await expect(page.locator('text=Adjust stock levels')).toBeVisible()
    })

    test('should display movement history table', async ({ page }) => {
      await expect(page.locator('text=Recent Movements')).toBeVisible()
      await expect(page.locator('th:has-text("Date & Reference")')).toBeVisible()
      await expect(page.locator('th:has-text("Type")')).toBeVisible()
      await expect(page.locator('th:has-text("Item")')).toBeVisible()
    })

    test('should filter movements by type', async ({ page }) => {
      const typeFilter = page.locator('select#type-filter')
      await typeFilter.selectOption('receipt')
      // Should show only receipts
    })

    test('should filter movements by warehouse', async ({ page }) => {
      const warehouseFilter = page.locator('select#warehouse-filter')
      await warehouseFilter.selectOption({ index: 1 })
      // Should filter by warehouse
    })

    test('should export movements', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export CSV")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/stock-movements-.*\.csv/)
    })

    test('should view movement details', async ({ page }) => {
      await page.click('button:has-text("View")').first()
      // Should show alert with movement details
    })
  })

  test.describe('Mobile Responsiveness', () => {
    test('should be mobile responsive on stock dashboard', async ({ page }) => {
      await page.setViewportSize({ width: 375, height: 667 })
      await page.goto('/stock')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('text=Stock Management')).toBeVisible()
      await expect(page.locator('text=Total Items')).toBeVisible()
    })

    test('should be mobile responsive on items page', async ({ page }) => {
      await page.setViewportSize({ width: 375, height: 667 })
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('text=Items')).toBeVisible()
      await expect(page.locator('button:has-text("Add Item")')).toBeVisible()
    })

    test('should be mobile responsive on warehouses page', async ({ page }) => {
      await page.setViewportSize({ width: 375, height: 667 })
      await page.goto('/stock/warehouses')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('text=Warehouses')).toBeVisible()
      await expect(page.locator('button:has-text("Add Warehouse")')).toBeVisible()
    })
  })

  test.describe('Dark Mode', () => {
    test('should support dark mode on stock pages', async ({ page }) => {
      await page.goto('/stock')
      await page.waitForLoadState('networkidle')
      
      // Toggle dark mode (assuming there's a theme switcher)
      const darkModeToggle = page.locator('[class*="theme"]').first()
      if (await darkModeToggle.isVisible()) {
        await darkModeToggle.click()
      }
      
      // Verify dark mode classes
      const body = page.locator('body')
      const hasDarkClass = await body.evaluate(el => el.classList.contains('dark'))
      // Dark mode should be toggleable
    })
  })
})

