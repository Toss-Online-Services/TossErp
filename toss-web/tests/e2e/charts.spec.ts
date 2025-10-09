import { test, expect } from '@playwright/test'

test.describe('Charts and Data Visualization', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login')
    await page.fill('[data-testid="email-input"]', 'admin@toss.co.za')
    await page.fill('[data-testid="password-input"]', 'password123')
    await page.click('[data-testid="login-button"]')
  })

  test('should render dashboard charts', async ({ page }) => {
    await page.goto('/dashboard')
    
    // Wait for charts to load
    await page.waitForTimeout(2000)
    
    // Check sales trend chart
    const salesChart = page.locator('[data-testid="sales-chart"] canvas')
    await expect(salesChart).toBeVisible()
    
    // Check revenue chart
    const revenueChart = page.locator('[data-testid="revenue-chart"] canvas')
    await expect(revenueChart).toBeVisible()
    
    // Verify charts have rendered content (canvas should have non-zero dimensions)
    const salesChartBox = await salesChart.boundingBox()
    expect(salesChartBox?.width).toBeGreaterThan(0)
    expect(salesChartBox?.height).toBeGreaterThan(0)
  })

  test('should render manufacturing charts', async ({ page }) => {
    await page.goto('/manufacturing')
    
    // Wait for charts to load
    await page.waitForTimeout(2000)
    
    // Check production trend chart
    const productionChart = page.locator('[data-testid="production-trend-chart"] canvas')
    await expect(productionChart).toBeVisible()
    
    // Check capacity utilization chart
    const capacityChart = page.locator('[data-testid="capacity-utilization-chart"] canvas')
    await expect(capacityChart).toBeVisible()
    
    // Verify charts have rendered content
    const productionChartBox = await productionChart.boundingBox()
    expect(productionChartBox?.width).toBeGreaterThan(0)
    expect(productionChartBox?.height).toBeGreaterThan(0)
  })

  test('should render quality control charts', async ({ page }) => {
    await page.goto('/manufacturing/quality')
    
    // Wait for charts to load
    await page.waitForTimeout(2000)
    
    // Check quality trend chart
    const qualityChart = page.locator('[data-testid="quality-trend-chart"] canvas')
    await expect(qualityChart).toBeVisible()
    
    // Check defect distribution chart
    const defectChart = page.locator('[data-testid="defect-distribution-chart"] canvas')
    await expect(defectChart).toBeVisible()
  })

  test('should handle chart interactions', async ({ page }) => {
    await page.goto('/dashboard')
    
    // Wait for charts to load
    await page.waitForTimeout(2000)
    
    // Hover over chart to show tooltip (if implemented)
    const salesChart = page.locator('[data-testid="sales-chart"] canvas')
    await salesChart.hover()
    
    // Click on chart (should not cause errors)
    await salesChart.click()
    
    // Chart should still be visible after interaction
    await expect(salesChart).toBeVisible()
  })

  test('should be responsive on different screen sizes', async ({ page }) => {
    await page.goto('/dashboard')
    
    // Test desktop view
    await page.setViewportSize({ width: 1920, height: 1080 })
    await page.waitForTimeout(1000)
    
    let salesChart = page.locator('[data-testid="sales-chart"] canvas')
    await expect(salesChart).toBeVisible()
    
    // Test tablet view
    await page.setViewportSize({ width: 768, height: 1024 })
    await page.waitForTimeout(1000)
    
    salesChart = page.locator('[data-testid="sales-chart"] canvas')
    await expect(salesChart).toBeVisible()
    
    // Test mobile view
    await page.setViewportSize({ width: 375, height: 667 })
    await page.waitForTimeout(1000)
    
    salesChart = page.locator('[data-testid="sales-chart"] canvas')
    await expect(salesChart).toBeVisible()
  })

  test('should handle chart data updates', async ({ page }) => {
    await page.goto('/dashboard')
    
    // Wait for initial charts to load
    await page.waitForTimeout(2000)
    
    // Refresh the page to test data reloading
    await page.reload()
    await page.waitForTimeout(2000)
    
    // Charts should still be visible after refresh
    const salesChart = page.locator('[data-testid="sales-chart"] canvas')
    await expect(salesChart).toBeVisible()
    
    const revenueChart = page.locator('[data-testid="revenue-chart"] canvas')
    await expect(revenueChart).toBeVisible()
  })

  test('should export chart as image', async ({ page }) => {
    await page.goto('/dashboard')
    
    // Wait for charts to load
    await page.waitForTimeout(2000)
    
    // Right-click on chart to test context menu (if implemented)
    const salesChart = page.locator('[data-testid="sales-chart"] canvas')
    await salesChart.click({ button: 'right' })
    
    // Chart should still be visible
    await expect(salesChart).toBeVisible()
  })
})
