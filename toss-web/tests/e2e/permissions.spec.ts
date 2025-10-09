import { test, expect } from '@playwright/test'

test.describe('Permissions & RBAC', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForLoadState('networkidle')
  })

  test('should show all modules for admin user', async ({ page }) => {
    // Login as admin
    await page.fill('#email', 'admin@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    
    // Wait for dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    
    // Check that admin can see all navigation items
    const sidebar = page.locator('[data-testid="sidebar"], nav')
    await expect(sidebar.locator('text=Dashboard')).toBeVisible()
    await expect(sidebar.locator('text=Sales')).toBeVisible()
    await expect(sidebar.locator('text=Inventory')).toBeVisible()
    await expect(sidebar.locator('text=Manufacturing')).toBeVisible()
  })

  test('should restrict modules for viewer role', async ({ page }) => {
    // Login as viewer (if we have this role)
    await page.fill('#email', 'viewer@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    
    // Wait for dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    
    // Viewer should see dashboard but not have edit/delete buttons
    await expect(page.locator('text=Dashboard')).toBeVisible()
    
    // Check that edit/delete buttons are not visible
    const editButtons = page.locator('button:has-text("Edit"), button:has-text("Delete")')
    const count = await editButtons.count()
    expect(count).toBe(0)
  })

  test('should allow sales rep to access sales module', async ({ page }) => {
    // Login as sales rep
    await page.fill('#email', 'sales@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    
    // Wait for dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    
    // Navigate to sales
    await page.click('text=Sales')
    await page.waitForURL('**/sales/**', { timeout: 5000 })
    
    // Should be able to view sales page
    await expect(page).toHaveURL(/.*sales/)
  })

  test('should prevent unauthorized access to settings', async ({ page }) => {
    // Login as employee
    await page.fill('#email', 'employee@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    
    // Wait for dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    
    // Try to navigate to settings directly
    await page.goto('http://localhost:3000/settings')
    
    // Should be redirected or show unauthorized
    await page.waitForTimeout(2000)
    const url = page.url()
    expect(url).not.toContain('/settings')
  })

  test('should show permission-based UI elements', async ({ page }) => {
    // Login as manager
    await page.fill('#email', 'manager@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    
    // Wait for dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    
    // Manager should see create buttons but maybe not delete
    await page.goto('http://localhost:3000/sales/orders')
    await page.waitForLoadState('networkidle')
    
    // Check for create button
    const createButton = page.locator('button:has-text("New"), button:has-text("Create")')
    await expect(createButton.first()).toBeVisible()
  })
})

