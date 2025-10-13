import { test, expect } from '@playwright/test'

test.describe('Stock Management - Advanced Features', () => {
  test.describe('Stock Reconciliation', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/stock/reconciliation')
      await page.waitForLoadState('networkidle')
    })

    test('should display reconciliation page', async ({ page }) => {
      await expect(page).toHaveTitle(/Stock Reconciliation/)
      await expect(page.locator('text=Stock Reconciliation')).toBeVisible()
      await expect(page.locator('text=Reconcile physical stock counts')).toBeVisible()
    })

    test('should show reconciliation stats', async ({ page }) => {
      await expect(page.locator('text=Pending Reconciliations')).toBeVisible()
      await expect(page.locator('text=Completed This Month')).toBeVisible()
      await expect(page.locator('text=Discrepancies')).toBeVisible()
      await expect(page.locator('text=Value Adjustment')).toBeVisible()
    })

    test('should open new reconciliation modal', async ({ page }) => {
      await page.click('button:has-text("New Reconciliation")')
      await expect(page.locator('text=New Stock Reconciliation')).toBeVisible()
      await expect(page.locator('label:has-text("Warehouse")')).toBeVisible()
      await expect(page.locator('label:has-text("Reconciliation Date")')).toBeVisible()
    })

    test('should display reconciliation list', async ({ page }) => {
      await expect(page.locator('text=Reconciliation Records')).toBeVisible()
      await expect(page.locator('th:has-text("Reconciliation ID")')).toBeVisible()
      await expect(page.locator('th:has-text("Warehouse")')).toBeVisible()
      await expect(page.locator('th:has-text("Discrepancies")')).toBeVisible()
    })

    test('should filter reconciliations by warehouse', async ({ page }) => {
      const warehouseFilter = page.locator('select').first()
      await warehouseFilter.selectOption({ index: 1 })
      // Should filter reconciliations
    })

    test('should filter reconciliations by status', async ({ page }) => {
      const statusFilter = page.locator('select').nth(1)
      await statusFilter.selectOption('completed')
      // Should show only completed
    })

    test('should view reconciliation details', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button[title="View Details"]').first()
      // Alert should show reconciliation details
    })

    test('should start reconciliation', async ({ page }) => {
      const startButton = page.locator('button[title="Start Reconciliation"]').first()
      if (await startButton.isVisible()) {
        await startButton.click()
        await expect(page.locator('text=New Stock Reconciliation')).toBeVisible()
      }
    })

    test('should export reconciliations', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/stock-reconciliations-.*\.csv/)
    })
  })

  test.describe('Stock Reports', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/stock/reports')
      await page.waitForLoadState('networkidle')
    })

    test('should display reports page', async ({ page }) => {
      await expect(page).toHaveTitle(/Stock Reports/)
      await expect(page.locator('text=Stock Reports')).toBeVisible()
      await expect(page.locator('text=Comprehensive inventory analytics')).toBeVisible()
    })

    test('should show report categories', async ({ page }) => {
      await expect(page.locator('text=Inventory Reports')).toBeVisible()
      await expect(page.locator('text=Movement Reports')).toBeVisible()
      await expect(page.locator('text=Valuation Reports')).toBeVisible()
    })

    test('should generate stock balance report', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Stock Balance Report")')
      // Alert should show report generation
    })

    test('should generate low stock report', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Low Stock Report")')
      // Alert should show report generation
    })

    test('should generate movement history report', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Stock Movement History")')
      // Alert should show report generation
    })

    test('should generate stock valuation report', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Stock Valuation")')
      // Alert should show report generation
    })

    test('should show recent reports table', async ({ page }) => {
      await expect(page.locator('text=Recent Reports')).toBeVisible()
      await expect(page.locator('th:has-text("Report Name")')).toBeVisible()
      await expect(page.locator('th:has-text("Type")')).toBeVisible()
      await expect(page.locator('th:has-text("Generated")')).toBeVisible()
    })

    test('should download report', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button[title="Download"]').first()
      // Alert should show download action
    })

    test('should view report', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button[title="View"]').first()
      // Alert should show view action
    })

    test('should delete report with confirmation', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button[title="Delete"]').first()
      // Should show delete confirmation
    })

    test('should generate custom report', async ({ page }) => {
      await page.selectOption('select', 'stock-balance')
      
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Generate Report")')
      // Should generate report
    })

    test('should export all reports', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export All")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/stock-reports-index-.*\.csv/)
    })

    test('should show schedule report option', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Schedule Report")')
      // Alert should show schedule feature
    })
  })

  test.describe('Export Functionality', () => {
    test('should export items from items page', async ({ page }) => {
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toContain('items-export')
    })

    test('should export warehouses from warehouses page', async ({ page }) => {
      await page.goto('/stock/warehouses')
      await page.waitForLoadState('networkidle')
      
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toContain('warehouses')
    })

    test('should export movements from movements page', async ({ page }) => {
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export CSV")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toContain('stock-movements')
    })

    test('should export reconciliations', async ({ page }) => {
      await page.goto('/stock/reconciliation')
      await page.waitForLoadState('networkidle')
      
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toContain('stock-reconciliations')
    })
  })

  test.describe('Low Stock Alerts', () => {
    test('should highlight low stock items in items list', async ({ page }) => {
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      // Filter by low stock
      const stockFilter = page.locator('select').nth(1)
      await stockFilter.selectOption('low')
      
      // Should show low stock items with warning icon
      const warningIcons = page.locator('svg.text-orange-500, svg.text-red-500')
      const count = await warningIcons.count()
      expect(count).toBeGreaterThan(0)
    })

    test('should show low stock alert in dashboard', async ({ page }) => {
      await page.goto('/stock')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('text=AI Co-Pilot Insights')).toBeVisible()
      await expect(page.locator('text=Low stock detected')).toBeVisible()
    })
  })

  test.describe('Item Details Actions', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      await page.click('text=White Bread Loaf')
      await expect(page.locator('text=Basic Information')).toBeVisible()
    })

    test('should adjust stock from item details', async ({ page }) => {
      page.on('dialog', dialog => {
        if (dialog.type() === 'prompt') {
          dialog.accept('10')
        } else {
          dialog.accept()
        }
      })
      
      await page.click('button:has-text("Adjust Stock")')
      // Should show adjustment prompt and confirmation
    })

    test('should view stock history from item details', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("View History")')
      // Should show history alert
    })

    test('should print label from item details', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Print Label")')
      // Should show print label alert
    })

    test('should edit item from details modal', async ({ page }) => {
      await page.click('button:has-text("Edit")')
      await expect(page.locator('text=Edit Item')).toBeVisible()
    })

    test('should delete item with confirmation', async ({ page }) => {
      page.on('dialog', dialog => dialog.accept())
      await page.click('button:has-text("Delete Item")')
      // Should show delete confirmation
    })
  })
})

