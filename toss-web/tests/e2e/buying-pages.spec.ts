import { test, expect } from '@playwright/test'

test.describe('Buying Module Pages', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login')
    await page.fill('input[name="email"]', 'admin@example.com')
    await page.fill('input[name="password"]', 'password')
    await page.click('button[type="submit"]')
    await page.waitForURL('/dashboard')
  })

  test('should display Purchase Orders page', async ({ page }) => {
    await page.goto('/buying/orders')
    
    // Check page title
    await expect(page.locator('h1')).toContainText('Purchase Orders')
    
    // Check stats cards
    await expect(page.locator('text=Total Orders')).toBeVisible()
    await expect(page.locator('text=Pending')).toBeVisible()
    await expect(page.locator('text=Total Value')).toBeVisible()
    await expect(page.locator('text=This Month')).toBeVisible()
    
    // Check search and filter
    await expect(page.locator('input[placeholder*="Search orders"]')).toBeVisible()
    await expect(page.locator('select:has-text("All Status")')).toBeVisible()
    
    // Check table columns
    await expect(page.locator('th:has-text("PO Number")')).toBeVisible()
    await expect(page.locator('th:has-text("Supplier")')).toBeVisible()
    await expect(page.locator('th:has-text("Date")')).toBeVisible()
    await expect(page.locator('th:has-text("Amount")')).toBeVisible()
    
    // Check orders are displayed
    await expect(page.locator('table tbody tr')).toHaveCount(5)
  })

  test('should filter purchase orders by status', async ({ page }) => {
    await page.goto('/buying/orders')
    
    // Filter by Confirmed status
    const statusFilter = page.locator('select:has-text("All Status")')
    await statusFilter.selectOption('Confirmed')
    await page.waitForTimeout(300)
    
    // Verify filtered results
    const confirmedBadges = page.locator('.bg-blue-100.text-blue-800:has-text("Confirmed")')
    const confirmedCount = await confirmedBadges.count()
    expect(confirmedCount).toBeGreaterThan(0)
    
    // Filter by Draft
    await statusFilter.selectOption('Draft')
    await page.waitForTimeout(300)
    await expect(page.locator('.bg-gray-100.text-gray-800:has-text("Draft")')).toBeVisible()
  })

  test('should search purchase orders', async ({ page }) => {
    await page.goto('/buying/orders')
    
    // Search for specific supplier
    const searchInput = page.locator('input[placeholder*="Search orders"]')
    await searchInput.fill('ABC Suppliers')
    await page.waitForTimeout(500)
    
    // Verify filtered results
    await expect(page.locator('text=ABC Suppliers Ltd')).toBeVisible()
    
    // Clear search
    await searchInput.clear()
    await page.waitForTimeout(300)
    await expect(page.locator('table tbody tr')).toHaveCount(5)
  })

  test('should display stats correctly on Purchase Orders', async ({ page }) => {
    await page.goto('/buying/orders')
    
    // Check that stats cards show numbers
    const totalOrders = page.locator('text=Total Orders').locator('..').locator('.text-2xl')
    await expect(totalOrders).toContainText('5')
    
    const totalValue = page.locator('text=Total Value').locator('..').locator('.text-2xl')
    const valueText = await totalValue.textContent()
    expect(valueText).toContain('R')
  })

  test('should display Purchase Invoices page', async ({ page }) => {
    await page.goto('/buying/invoices')
    
    // Check page title
    await expect(page.locator('h1')).toContainText('Purchase Invoices')
    
    // Check stats cards
    await expect(page.locator('text=Total Invoices')).toBeVisible()
    await expect(page.locator('text=Outstanding')).toBeVisible()
    await expect(page.locator('text=Paid This Month')).toBeVisible()
    await expect(page.locator('text=Overdue')).toBeVisible()
    
    // Check table columns
    await expect(page.locator('th:has-text("Invoice #")')).toBeVisible()
    await expect(page.locator('th:has-text("Supplier")')).toBeVisible()
    await expect(page.locator('th:has-text("PO Reference")')).toBeVisible()
    await expect(page.locator('th:has-text("Due Date")')).toBeVisible()
    await expect(page.locator('th:has-text("Outstanding")')).toBeVisible()
    
    // Check invoices are displayed
    await expect(page.locator('table tbody tr')).toHaveCount(5)
  })

  test('should filter invoices by status', async ({ page }) => {
    await page.goto('/buying/invoices')
    
    // Filter by Paid status
    const statusFilter = page.locator('select:has-text("All Status")')
    await statusFilter.selectOption('Paid')
    await page.waitForTimeout(300)
    
    // Verify only paid invoices shown
    const paidBadges = page.locator('.bg-green-100.text-green-800:has-text("Paid")')
    const paidCount = await paidBadges.count()
    const totalRows = await page.locator('table tbody tr').count()
    expect(paidCount).toBe(totalRows)
  })

  test('should highlight overdue invoices', async ({ page }) => {
    await page.goto('/buying/invoices')
    
    // Filter by Overdue
    const statusFilter = page.locator('select:has-text("All Status")')
    await statusFilter.selectOption('Overdue')
    await page.waitForTimeout(300)
    
    // Check overdue badge
    await expect(page.locator('.bg-red-100.text-red-800:has-text("Overdue")')).toBeVisible()
    
    // Verify due date is highlighted
    const overdueCell = page.locator('td.text-red-600.font-medium')
    await expect(overdueCell).toBeVisible()
  })

  test('should search invoices by invoice number', async ({ page }) => {
    await page.goto('/buying/invoices')
    
    const searchInput = page.locator('input[placeholder*="Search invoices"]')
    await searchInput.fill('PINV-2024-001')
    await page.waitForTimeout(500)
    
    await expect(page.locator('table tbody tr')).toHaveCount(1)
    await expect(page.locator('text=PINV-2024-001')).toBeVisible()
  })

  test('should display Purchase Requests page', async ({ page }) => {
    await page.goto('/buying/requests')
    
    // Check page title
    await expect(page.locator('h1')).toContainText('Purchase Requests')
    
    // Check stats cards
    await expect(page.locator('text=Total Requests')).toBeVisible()
    await expect(page.locator('text=Pending Approval')).toBeVisible()
    await expect(page.locator('text=Approved')).toBeVisible()
    
    // Check filters
    await expect(page.locator('input[placeholder*="Search requests"]')).toBeVisible()
    await expect(page.locator('select:has-text("All Status")')).toBeVisible()
    await expect(page.locator('select:has-text("All Departments")')).toBeVisible()
    
    // Check table columns
    await expect(page.locator('th:has-text("Request #")')).toBeVisible()
    await expect(page.locator('th:has-text("Requestor")')).toBeVisible()
    await expect(page.locator('th:has-text("Department")')).toBeVisible()
    await expect(page.locator('th:has-text("Priority")')).toBeVisible()
    
    // Check requests are displayed
    await expect(page.locator('table tbody tr')).toHaveCount(5)
  })

  test('should filter requests by status', async ({ page }) => {
    await page.goto('/buying/requests')
    
    // Filter by Pending
    const statusFilter = page.locator('select:has-text("All Status")')
    await statusFilter.selectOption('Pending')
    await page.waitForTimeout(300)
    
    // Verify pending requests
    const pendingBadges = page.locator('.bg-yellow-100.text-yellow-800:has-text("Pending")')
    const pendingCount = await pendingBadges.count()
    expect(pendingCount).toBeGreaterThan(0)
  })

  test('should filter requests by department', async ({ page }) => {
    await page.goto('/buying/requests')
    
    // Filter by Manufacturing department
    const deptFilter = page.locator('select:has-text("All Departments")')
    await deptFilter.selectOption('Manufacturing')
    await page.waitForTimeout(300)
    
    // Verify all results are from Manufacturing
    await expect(page.locator('td:has-text("Manufacturing")')).toHaveCount(2)
  })

  test('should display priority badges correctly', async ({ page }) => {
    await page.goto('/buying/requests')
    
    // Check for different priority levels
    await expect(page.locator('.bg-red-100.text-red-800:has-text("High")')).toBeVisible()
    await expect(page.locator('.bg-yellow-100.text-yellow-800:has-text("Medium")')).toBeVisible()
    await expect(page.locator('.bg-blue-100.text-blue-800:has-text("Low")')).toBeVisible()
  })

  test('should show approve button for pending requests', async ({ page }) => {
    await page.goto('/buying/requests')
    
    // Filter by Pending
    const statusFilter = page.locator('select:has-text("All Status")')
    await statusFilter.selectOption('Pending')
    await page.waitForTimeout(300)
    
    // Check for Approve button
    const approveButton = page.locator('button:has-text("Approve")').first()
    await expect(approveButton).toBeVisible()
  })

  test('should show Create PO button for approved requests', async ({ page }) => {
    await page.goto('/buying/requests')
    
    // Filter by Approved
    const statusFilter = page.locator('select:has-text("All Status")')
    await statusFilter.selectOption('Approved')
    await page.waitForTimeout(300)
    
    // Check for Create PO button
    const createPOButton = page.locator('button:has-text("Create PO")').first()
    await expect(createPOButton).toBeVisible()
  })

  test('should search requests by requestor name', async ({ page }) => {
    await page.goto('/buying/requests')
    
    const searchInput = page.locator('input[placeholder*="Search requests"]')
    await searchInput.fill('John Smith')
    await page.waitForTimeout(500)
    
    await expect(page.locator('text=John Smith')).toBeVisible()
  })

  test('should calculate and display total estimated cost', async ({ page }) => {
    await page.goto('/buying/requests')
    
    // Check that Total Value stat shows a currency amount
    const totalValue = page.locator('text=Total Value').locator('..').locator('.text-2xl')
    const valueText = await totalValue.textContent()
    expect(valueText).toContain('R')
    
    // Verify it's a formatted currency
    expect(valueText).toMatch(/R\s[\d,]+/)
  })
})

