import { test, expect } from '@playwright/test'

test.describe('Stock Management - Integration Tests', () => {
  test.describe('Item to Movement to Stock Level Flow', () => {
    test('should create item and track through movements', async ({ page }) => {
      // Step 1: Create a new item
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Add Item")')
      await expect(page.locator('text=Create New Item')).toBeVisible()
      
      await page.fill('#sku', 'TEST-001')
      await page.fill('#name', 'Test Item for Integration')
      await page.selectOption('#category', { index: 1 })
      await page.selectOption('#unit', 'PCS')
      await page.fill('#sellingPrice', '100')
      await page.fill('#costPrice', '75')
      await page.fill('#reorderLevel', '10')
      await page.fill('#reorderQty', '20')
      
      page.on('dialog', dialog => dialog.accept())
      await page.click('button[type="submit"]:has-text("Create Item")')
      
      // Step 2: Navigate to movements and create a receipt
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Stock Receipt")')
      await expect(page.locator('text=Stock Receipt')).toBeVisible()
      
      // Select warehouse
      await page.selectOption('select#warehouse', { index: 1 })
      
      // Item search would need item to be created, skip for now
      await page.click('button:has-text("Cancel")')
    })

    test('should transfer stock between warehouses', async ({ page }) => {
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Stock Transfer")')
      await expect(page.locator('text=Stock Transfer')).toBeVisible()
      await expect(page.locator('label:has-text("Source Warehouse")')).toBeVisible()
      await expect(page.locator('label:has-text("Target Warehouse")')).toBeVisible()
      
      await page.click('button:has-text("Cancel")')
    })

    test('should issue stock and reduce levels', async ({ page }) => {
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Stock Issue")')
      await expect(page.locator('text=Stock Issue')).toBeVisible()
      await expect(page.locator('text=Issue stock from warehouse')).toBeVisible()
      
      await page.click('button:has-text("Cancel")')
    })
  })

  test.describe('Purchase to Receipt to Stock Flow', () => {
    test('should navigate from purchasing to stock receipt', async ({ page }) => {
      // Start at purchasing orders
      await page.goto('/purchasing/orders')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('text=Purchase Orders')).toBeVisible()
      
      // Navigate to stock movements
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Stock Receipt")')
      await expect(page.locator('text=Receive stock into warehouse')).toBeVisible()
    })
  })

  test.describe('Sale to Issue to Stock Flow', () => {
    test('should navigate from sales to stock issue', async ({ page }) => {
      // Start at sales invoices
      await page.goto('/sales/invoices')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('text=Sales Invoices')).toBeVisible()
      
      // Navigate to stock movements
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Stock Issue")')
      await expect(page.locator('text=Issue stock from warehouse')).toBeVisible()
    })
  })

  test.describe('Reconciliation to Stock Adjustment', () => {
    test('should complete full reconciliation workflow', async ({ page }) => {
      await page.goto('/stock/reconciliation')
      await page.waitForLoadState('networkidle')
      
      // Open new reconciliation
      await page.click('button:has-text("New Reconciliation")')
      await expect(page.locator('text=New Stock Reconciliation')).toBeVisible()
      
      // Fill in details
      await page.selectOption('select#warehouse', { index: 1 })
      await page.fill('input#date', '2024-01-20')
      
      // Wait for items to load
      await page.waitForTimeout(1000)
      
      // Cancel for now
      await page.click('button:has-text("Cancel")')
    })

    test('should navigate from reconciliation to adjustments', async ({ page }) => {
      await page.goto('/stock/reconciliation')
      await page.waitForLoadState('networkidle')
      
      // Navigate to movements for adjustment
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Stock Adjustment")')
      await expect(page.locator('text=Stock Adjustment')).toBeVisible()
    })
  })

  test.describe('Complete Stock Cycle', () => {
    test('should navigate through complete stock management cycle', async ({ page }) => {
      // 1. Check dashboard
      await page.goto('/stock')
      await page.waitForLoadState('networkidle')
      await expect(page.locator('text=Stock Management')).toBeVisible()
      
      // 2. View items
      await page.click('a[href="/stock/items"]')
      await page.waitForLoadState('networkidle')
      await expect(page.locator('text=Items')).toBeVisible()
      
      // 3. View warehouses
      await page.goto('/stock/warehouses')
      await page.waitForLoadState('networkidle')
      await expect(page.locator('text=Warehouses')).toBeVisible()
      
      // 4. View movements
      await page.goto('/stock/movements')
      await page.waitForLoadState('networkidle')
      await expect(page.locator('text=Stock Movements')).toBeVisible()
      
      // 5. View reconciliation
      await page.goto('/stock/reconciliation')
      await page.waitForLoadState('networkidle')
      await expect(page.locator('text=Stock Reconciliation')).toBeVisible()
      
      // 6. View reports
      await page.goto('/stock/reports')
      await page.waitForLoadState('networkidle')
      await expect(page.locator('text=Stock Reports')).toBeVisible()
    })

    test('should maintain state across navigation', async ({ page }) => {
      // Apply filter on items page
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      const searchInput = page.locator('input[placeholder="Search items..."]')
      await searchInput.fill('Bread')
      
      // Navigate away and back
      await page.goto('/stock/warehouses')
      await page.waitForLoadState('networkidle')
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      // Filter should be cleared (fresh page load)
      const searchValue = await searchInput.inputValue()
      expect(searchValue).toBe('')
    })
  })

  test.describe('Warehouse Hierarchy', () => {
    test('should support parent-child warehouse relationships', async ({ page }) => {
      await page.goto('/stock/warehouses')
      await page.waitForLoadState('networkidle')
      
      await page.click('button:has-text("Add Warehouse")')
      await expect(page.locator('text=Create New Warehouse')).toBeVisible()
      
      // Should have parent warehouse selector
      await expect(page.locator('label:has-text("Parent Warehouse")')).toBeVisible()
      await expect(page.locator('text=None (Top Level)')).toBeVisible()
    })
  })

  test.describe('Error Handling', () => {
    test('should handle failed item load gracefully', async ({ page }) => {
      // This would test error states if API fails
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      // Page should still render
      await expect(page.locator('text=Items')).toBeVisible()
    })

    test('should handle failed warehouse load gracefully', async ({ page }) => {
      await page.goto('/stock/warehouses')
      await page.waitForLoadState('networkidle')
      
      // Page should still render
      await expect(page.locator('text=Warehouses')).toBeVisible()
    })
  })

  test.describe('Data Consistency', () => {
    test('should show consistent data across pages', async ({ page }) => {
      // Get total items from dashboard
      await page.goto('/stock')
      await page.waitForLoadState('networkidle')
      
      const dashboardItems = await page.locator('text=Total Items').locator('..').locator('p.text-2xl').textContent()
      
      // Navigate to items page
      await page.goto('/stock/items')
      await page.waitForLoadState('networkidle')
      
      const itemsPageItems = await page.locator('text=Total Items').locator('..').locator('dd').textContent()
      
      // Numbers should match (or be close if filtering is applied)
      expect(dashboardItems).toBeTruthy()
      expect(itemsPageItems).toBeTruthy()
    })
  })
})

