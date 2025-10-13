import { test, expect } from '@playwright/test'

test.describe('Purchasing Module - Core Features', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/')
    await page.waitForLoadState('networkidle')
  })

  test.describe('Suppliers Management', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/suppliers')
      await page.waitForLoadState('networkidle')
    })

    test('should display suppliers page with stats', async ({ page }) => {
      await expect(page).toHaveTitle(/Suppliers - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Supplier Management')
      
      // Check stats cards
      await expect(page.locator('text=Total Suppliers')).toBeVisible()
      await expect(page.locator('text=Active Suppliers')).toBeVisible()
    })

    test('should have add supplier button', async ({ page }) => {
      await expect(page.locator('button:has-text("Add Supplier")')).toBeVisible()
    })

    test('should have export button', async ({ page }) => {
      await expect(page.locator('button:has-text("Export")')).toBeVisible()
    })

    test('should display suppliers table', async ({ page }) => {
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('text=Tech Solutions Inc')).toBeVisible()
      await expect(page.locator('text=Raw Materials Corp')).toBeVisible()
    })

    test('should filter suppliers by status', async ({ page }) => {
      const statusFilter = page.locator('select').nth(1)
      await statusFilter.selectOption('active')
      await page.waitForTimeout(300)
      await expect(page.locator('text=active').first()).toBeVisible()
    })

    test('should search suppliers', async ({ page }) => {
      const searchInput = page.locator('input[placeholder*="Search"]')
      await searchInput.fill('Tech Solutions')
      await page.waitForTimeout(300)
      await expect(page.locator('text=Tech Solutions Inc')).toBeVisible()
    })
  })

  test.describe('Purchase Requests', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/requests')
      await page.waitForLoadState('networkidle')
    })

    test('should display purchase requests page', async ({ page }) => {
      await expect(page).toHaveTitle(/Purchase Requests - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Purchase Requests')
    })

    test('should have new request and group buy buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("New Request")')).toBeVisible()
      await expect(page.locator('button:has-text("Group Buy")')).toBeVisible()
    })

    test('should display requests table', async ({ page }) => {
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('text=PR-2024-001')).toBeVisible()
    })

    test('should show group buy badge', async ({ page }) => {
      await expect(page.locator('text=GROUP').first()).toBeVisible()
    })
  })

  test.describe('Purchase Orders', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/orders')
      await page.waitForLoadState('networkidle')
    })

    test('should display purchase orders page', async ({ page }) => {
      await expect(page).toHaveTitle(/Purchase Orders - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Purchase Orders')
    })

    test('should display PO stats', async ({ page }) => {
      await expect(page.locator('text=Total POs')).toBeVisible()
      await expect(page.locator('text=Pending')).toBeVisible()
      await expect(page.locator('text=Delivered')).toBeVisible()
    })

    test('should have create PO button', async ({ page }) => {
      await expect(page.locator('button:has-text("Create PO")')).toBeVisible()
    })

    test('should display POs table with progress bars', async ({ page }) => {
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('text=PO-2024-001')).toBeVisible()
      // Check progress bar exists
      const progressBars = page.locator('.bg-blue-600.h-2.rounded-full')
      await expect(progressBars.first()).toBeVisible()
    })
  })

  test.describe('Purchase Receipts', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/receipts')
      await page.waitForLoadState('networkidle')
    })

    test('should display receipts page', async ({ page }) => {
      await expect(page).toHaveTitle(/Purchase Receipts - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Purchase Receipts')
    })

    test('should have record receipt and scan barcode buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("Record Receipt")')).toBeVisible()
      await expect(page.locator('button:has-text("Scan Barcode")')).toBeVisible()
    })

    test('should display receipt stats with quality metrics', async ({ page }) => {
      await expect(page.locator('text=Total Receipts')).toBeVisible()
      await expect(page.locator('text=Pending QC')).toBeVisible()
      await expect(page.locator('text=Accepted')).toBeVisible()
      await expect(page.locator('text=Rejected')).toBeVisible()
    })

    test('should display receipts table', async ({ page }) => {
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('text=RCP-2024-001')).toBeVisible()
    })

    test('should show quality status badges', async ({ page }) => {
      await expect(page.locator('text=passed').first()).toBeVisible()
    })
  })

  test.describe('Purchase Invoices', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/invoices')
      await page.waitForLoadState('networkidle')
    })

    test('should display invoices page', async ({ page }) => {
      await expect(page).toHaveTitle(/TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Purchase Invoices')
    })

    test('should display invoice stats', async ({ page }) => {
      await expect(page.locator('text=Total Invoices')).toBeVisible()
      await expect(page.locator('text=Pending')).toBeVisible()
      await expect(page.locator('text=Overdue')).toBeVisible()
      await expect(page.locator('text=Paid')).toBeVisible()
    })

    test('should have create, import, and export buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("Create Invoice")')).toBeVisible()
      await expect(page.locator('button:has-text("Import")')).toBeVisible()
      await expect(page.locator('button:has-text("Export")')).toBeVisible()
    })

    test('should display invoices table with three-way matching', async ({ page }) => {
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('text=INV-2024-001')).toBeVisible()
      // Check for PO references
      await expect(page.locator('text=PO-2024-001').first()).toBeVisible()
    })

    test('should show invoice status badges', async ({ page }) => {
      await expect(page.locator('text=approved').first()).toBeVisible()
      await expect(page.locator('text=overdue').first()).toBeVisible()
      await expect(page.locator('text=paid').first()).toBeVisible()
    })
  })

  test.describe('Mobile Responsiveness', () => {
    test.use({ viewport: { width: 375, height: 667 } })

    test('should display suppliers page on mobile', async ({ page }) => {
      await page.goto('/purchasing/suppliers')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Supplier Management')
      await expect(page.locator('button:has-text("Add Supplier")')).toBeVisible()
    })

    test('should display purchase requests on mobile', async ({ page }) => {
      await page.goto('/purchasing/requests')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Purchase Requests')
      await expect(page.locator('button:has-text("New Request")')).toBeVisible()
    })
  })

  test.describe('Dark Mode', () => {
    test.beforeEach(async ({ page }) => {
      await page.emulateMedia({ colorScheme: 'dark' })
    })

    test('should display suppliers in dark mode', async ({ page }) => {
      await page.goto('/purchasing/suppliers')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Supplier Management')
      const bgColor = await page.locator('main').evaluate(el => 
        window.getComputedStyle(el).backgroundColor
      )
      expect(bgColor).not.toBe('rgb(255, 255, 255)')
    })

    test('should display purchase orders in dark mode', async ({ page }) => {
      await page.goto('/purchasing/orders')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Purchase Orders')
      const bgColor = await page.locator('main').evaluate(el => 
        window.getComputedStyle(el).backgroundColor
      )
      expect(bgColor).not.toBe('rgb(255, 255, 255)')
    })
  })
})

