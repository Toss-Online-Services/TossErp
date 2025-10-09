import { test, expect } from '@playwright/test'

test.describe('Export Functionality', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login')
    await page.fill('[data-testid="email-input"]', 'admin@toss.co.za')
    await page.fill('[data-testid="password-input"]', 'password123')
    await page.click('[data-testid="login-button"]')
  })

  test('should export sales orders', async ({ page }) => {
    await page.goto('/sales/orders')
    
    // Click export button
    await page.click('[data-testid="export-button"]')
    
    // Check dropdown is visible
    await expect(page.locator('[data-testid="export-dropdown"]')).toBeVisible()
    
    // Test CSV export
    const downloadPromise = page.waitForEvent('download')
    await page.click('[data-testid="export-csv-option"]')
    const download = await downloadPromise
    
    // Verify download
    expect(download.suggestedFilename()).toMatch(/sales_orders.*\.csv/)
  })

  test('should export manufacturing BOMs', async ({ page }) => {
    await page.goto('/manufacturing/bom')
    
    // Click export button
    await page.click('[data-testid="export-button"]')
    
    // Test Excel export
    const downloadPromise = page.waitForEvent('download')
    await page.click('[data-testid="export-excel-option"]')
    const download = await downloadPromise
    
    // Verify download
    expect(download.suggestedFilename()).toMatch(/bills_of_materials.*\.xlsx/)
  })

  test('should export work orders', async ({ page }) => {
    await page.goto('/manufacturing/work-orders')
    
    // Click export button
    await page.click('[data-testid="export-button"]')
    
    // Test PDF export
    const downloadPromise = page.waitForEvent('download')
    await page.click('[data-testid="export-pdf-option"]')
    const download = await downloadPromise
    
    // Verify download
    expect(download.suggestedFilename()).toMatch(/work_orders.*\.pdf/)
  })

  test('should export quality inspections', async ({ page }) => {
    await page.goto('/manufacturing/quality')
    
    // Click export button
    await page.click('[data-testid="export-button"]')
    
    // Test CSV export
    const downloadPromise = page.waitForEvent('download')
    await page.click('[data-testid="export-csv-option"]')
    const download = await downloadPromise
    
    // Verify download
    expect(download.suggestedFilename()).toMatch(/quality_inspections.*\.csv/)
  })

  test('should handle export errors gracefully', async ({ page }) => {
    await page.goto('/sales/orders')
    
    // Mock network failure
    await page.route('**/api/**', route => route.abort())
    
    // Try to export
    await page.click('[data-testid="export-button"]')
    await page.click('[data-testid="export-csv-option"]')
    
    // Should show error message
    await expect(page.locator('[data-testid="error-message"]')).toBeVisible()
  })

  test('should show loading state during export', async ({ page }) => {
    await page.goto('/sales/orders')
    
    // Click export button
    await page.click('[data-testid="export-button"]')
    
    // Click export option
    await page.click('[data-testid="export-csv-option"]')
    
    // Should show loading state briefly
    await expect(page.locator('[data-testid="export-button"]')).toContainText('Exporting...')
  })

  test('should disable export when no data', async ({ page }) => {
    // Navigate to a page with no data (mock empty state)
    await page.goto('/manufacturing/bom')
    
    // Clear all data (simulate empty state)
    await page.evaluate(() => {
      // This would need to be implemented in the actual component
      window.localStorage.setItem('test-empty-state', 'true')
    })
    
    await page.reload()
    
    // Export button should be disabled
    const exportButton = page.locator('[data-testid="export-button"]')
    await expect(exportButton).toBeDisabled()
  })
})
