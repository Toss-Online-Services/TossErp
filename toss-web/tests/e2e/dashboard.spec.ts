import { test, expect } from '@playwright/test'

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login')
    await page.fill('[data-testid="email-input"]', 'admin@toss.co.za')
    await page.fill('[data-testid="password-input"]', 'password123')
    await page.click('[data-testid="login-button"]')
    await expect(page).toHaveURL('/dashboard')
  })

  test('should display dashboard metrics', async ({ page }) => {
    // Check KPI cards are visible
    await expect(page.locator('[data-testid="revenue-card"]')).toBeVisible()
    await expect(page.locator('[data-testid="orders-card"]')).toBeVisible()
    await expect(page.locator('[data-testid="customers-card"]')).toBeVisible()
    await expect(page.locator('[data-testid="low-stock-card"]')).toBeVisible()
    
    // Check charts are rendered
    await expect(page.locator('[data-testid="sales-chart"]')).toBeVisible()
    await expect(page.locator('[data-testid="revenue-chart"]')).toBeVisible()
  })

  test('should navigate to different modules', async ({ page }) => {
    // Test CRM navigation
    await page.click('[data-testid="nav-crm"]')
    await expect(page).toHaveURL('/crm')
    
    // Test Sales navigation
    await page.click('[data-testid="nav-sales"]')
    await expect(page).toHaveURL('/sales')
    
    // Test Inventory navigation
    await page.click('[data-testid="nav-inventory"]')
    await expect(page).toHaveURL('/inventory')
    
    // Test Manufacturing navigation
    await page.click('[data-testid="nav-manufacturing"]')
    await expect(page).toHaveURL('/manufacturing')
  })

  test('should display top products table', async ({ page }) => {
    // Check table is visible
    await expect(page.locator('[data-testid="top-products-table"]')).toBeVisible()
    
    // Check table has data
    const rows = page.locator('[data-testid="top-products-table"] tbody tr')
    await expect(rows).toHaveCount(5) // Assuming 5 top products
    
    // Check first row has product data
    const firstRow = rows.first()
    await expect(firstRow.locator('td').first()).toContainText(/\w+/) // Product name
  })

  test('should show quick actions', async ({ page }) => {
    // Check quick action buttons are visible
    await expect(page.locator('[data-testid="quick-action-new-sale"]')).toBeVisible()
    await expect(page.locator('[data-testid="quick-action-new-purchase"]')).toBeVisible()
    await expect(page.locator('[data-testid="quick-action-add-product"]')).toBeVisible()
    await expect(page.locator('[data-testid="quick-action-add-customer"]')).toBeVisible()
  })

  test('should handle responsive design', async ({ page }) => {
    // Test mobile viewport
    await page.setViewportSize({ width: 375, height: 667 })
    
    // Check mobile navigation is visible
    await expect(page.locator('[data-testid="mobile-nav-toggle"]')).toBeVisible()
    
    // Check desktop sidebar is hidden
    await expect(page.locator('[data-testid="desktop-sidebar"]')).toBeHidden()
    
    // Test tablet viewport
    await page.setViewportSize({ width: 768, height: 1024 })
    
    // Check layout adapts
    await expect(page.locator('[data-testid="dashboard-content"]')).toBeVisible()
  })
})
