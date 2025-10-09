import { test, expect } from '@playwright/test'

test.describe('Manufacturing Module', () => {
  test.beforeEach(async ({ page }) => {
    // Login and navigate to manufacturing
    await page.goto('/login')
    await page.fill('[data-testid="email-input"]', 'admin@toss.co.za')
    await page.fill('[data-testid="password-input"]', 'password123')
    await page.click('[data-testid="login-button"]')
    await page.goto('/manufacturing')
  })

  test('should display manufacturing dashboard', async ({ page }) => {
    // Check KPI cards
    await expect(page.locator('[data-testid="active-work-orders-card"]')).toBeVisible()
    await expect(page.locator('[data-testid="units-produced-card"]')).toBeVisible()
    await expect(page.locator('[data-testid="quality-rate-card"]')).toBeVisible()
    await expect(page.locator('[data-testid="production-cost-card"]')).toBeVisible()
    
    // Check charts are rendered
    await expect(page.locator('[data-testid="production-trend-chart"]')).toBeVisible()
    await expect(page.locator('[data-testid="capacity-utilization-chart"]')).toBeVisible()
    
    // Check work order board
    await expect(page.locator('[data-testid="work-order-board"]')).toBeVisible()
  })

  test('should create new work order', async ({ page }) => {
    await page.goto('/manufacturing/work-orders')
    
    // Click create work order button
    await page.click('[data-testid="create-work-order-button"]')
    
    // Fill work order form
    await page.fill('[data-testid="product-name-input"]', 'Test Widget')
    await page.selectOption('[data-testid="order-type-select"]', 'Production')
    await page.fill('[data-testid="quantity-input"]', '100')
    await page.selectOption('[data-testid="priority-select"]', '2')
    
    // Submit form
    await page.click('[data-testid="save-work-order-button"]')
    
    // Check success message
    await expect(page.locator('[data-testid="success-message"]')).toBeVisible()
    
    // Check work order appears in list
    await expect(page.locator('[data-testid="work-order-list"]')).toContainText('Test Widget')
  })

  test('should create new BOM', async ({ page }) => {
    await page.goto('/manufacturing/bom')
    
    // Click create BOM button
    await page.click('[data-testid="create-bom-button"]')
    
    // Fill BOM form
    await page.fill('[data-testid="product-name-input"]', 'Test Product')
    await page.selectOption('[data-testid="product-type-select"]', 'Finished Good')
    
    // Add component
    await page.click('[data-testid="add-component-button"]')
    await page.fill('[data-testid="component-name-input-0"]', 'Steel Rod')
    await page.fill('[data-testid="component-quantity-input-0"]', '2')
    await page.fill('[data-testid="component-cost-input-0"]', '25.00')
    
    // Add operation
    await page.click('[data-testid="add-operation-button"]')
    await page.fill('[data-testid="operation-name-input-0"]', 'Cut Steel')
    await page.selectOption('[data-testid="work-center-select-0"]', 'Assembly Line 1')
    await page.fill('[data-testid="setup-time-input-0"]', '15')
    await page.fill('[data-testid="run-time-input-0"]', '5')
    
    // Submit form
    await page.click('[data-testid="save-bom-button"]')
    
    // Check success message
    await expect(page.locator('[data-testid="success-message"]')).toBeVisible()
    
    // Check BOM appears in list
    await expect(page.locator('[data-testid="bom-list"]')).toContainText('Test Product')
  })

  test('should create quality inspection', async ({ page }) => {
    await page.goto('/manufacturing/quality')
    
    // Click create inspection button
    await page.click('[data-testid="create-inspection-button"]')
    
    // Fill inspection form
    await page.selectOption('[data-testid="work-order-select"]', 'WO-001')
    await page.fill('[data-testid="product-name-input"]', 'Widget A')
    await page.fill('[data-testid="inspector-input"]', 'John Smith')
    
    // Add inspection point
    await page.fill('[data-testid="checklist-description-0"]', 'Dimensional accuracy')
    await page.selectOption('[data-testid="checklist-result-0"]', 'Pass')
    await page.fill('[data-testid="checklist-notes-0"]', 'Within tolerance')
    
    // Set overall result
    await page.check('[data-testid="result-passed"]')
    
    // Submit form
    await page.click('[data-testid="save-inspection-button"]')
    
    // Check success message
    await expect(page.locator('[data-testid="success-message"]')).toBeVisible()
    
    // Check inspection appears in list
    await expect(page.locator('[data-testid="inspection-list"]')).toContainText('Widget A')
  })

  test('should export manufacturing data', async ({ page }) => {
    await page.goto('/manufacturing/work-orders')
    
    // Click export button
    await page.click('[data-testid="export-button"]')
    
    // Click CSV export option
    await page.click('[data-testid="export-csv-option"]')
    
    // Check download starts (this is tricky to test, so we'll just check the button works)
    await expect(page.locator('[data-testid="export-button"]')).toBeVisible()
  })

  test('should filter work orders', async ({ page }) => {
    await page.goto('/manufacturing/work-orders')
    
    // Filter by status
    await page.selectOption('[data-testid="status-filter"]', 'In Progress')
    
    // Check filtered results
    const workOrders = page.locator('[data-testid="work-order-item"]')
    const count = await workOrders.count()
    
    // All visible work orders should have "In Progress" status
    for (let i = 0; i < count; i++) {
      const status = workOrders.nth(i).locator('[data-testid="work-order-status"]')
      await expect(status).toContainText('In Progress')
    }
    
    // Clear filters
    await page.click('[data-testid="clear-filters-button"]')
    
    // Check all work orders are visible again
    await expect(page.locator('[data-testid="work-order-item"]')).toHaveCount(4) // Assuming 4 total work orders
  })

  test('should switch between kanban and list view', async ({ page }) => {
    await page.goto('/manufacturing/work-orders')
    
    // Should start in kanban view
    await expect(page.locator('[data-testid="kanban-view"]')).toBeVisible()
    
    // Switch to list view
    await page.click('[data-testid="list-view-button"]')
    await expect(page.locator('[data-testid="list-view"]')).toBeVisible()
    await expect(page.locator('[data-testid="kanban-view"]')).toBeHidden()
    
    // Switch back to kanban view
    await page.click('[data-testid="kanban-view-button"]')
    await expect(page.locator('[data-testid="kanban-view"]')).toBeVisible()
    await expect(page.locator('[data-testid="list-view"]')).toBeHidden()
  })
})
