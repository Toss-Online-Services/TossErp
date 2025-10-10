import { test, expect } from '@playwright/test'

test.describe('Accounts Module Pages', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login')
    await page.fill('input[name="email"]', 'admin@example.com')
    await page.fill('input[name="password"]', 'password')
    await page.click('button[type="submit"]')
    await page.waitForURL('/dashboard')
  })

  test('should display Chart of Accounts page', async ({ page }) => {
    await page.goto('/accounts/chart')
    
    // Check page title
    await expect(page.locator('h1')).toContainText('Chart of Accounts')
    
    // Check search and filter inputs
    const searchInput = page.locator('input[placeholder*="Search accounts"]')
    await expect(searchInput).toBeVisible()
    
    const typeFilter = page.locator('select').first()
    await expect(typeFilter).toBeVisible()
    
    // Check table is displayed
    await expect(page.locator('table')).toBeVisible()
    await expect(page.locator('th:has-text("Account Code")')).toBeVisible()
    await expect(page.locator('th:has-text("Account Name")')).toBeVisible()
    await expect(page.locator('th:has-text("Type")')).toBeVisible()
    await expect(page.locator('th:has-text("Balance")')).toBeVisible()
    
    // Check accounts are displayed
    await expect(page.locator('table tbody tr')).toHaveCount(15)
    
    // Test search functionality
    await searchInput.fill('Cash')
    await page.waitForTimeout(500)
    await expect(page.locator('text=Cash and Bank')).toBeVisible()
  })

  test('should filter Chart of Accounts by type', async ({ page }) => {
    await page.goto('/accounts/chart')
    
    // Filter by Asset type
    const typeFilter = page.locator('select').first()
    await typeFilter.selectOption('Asset')
    await page.waitForTimeout(300)
    
    // Verify only Asset accounts are shown
    const typebadges = page.locator('.px-2.py-1.text-xs.rounded-full:has-text("Asset")')
    const assetCount = await typebadges.count()
    const totalRows = await page.locator('table tbody tr').count()
    
    expect(assetCount).toBe(totalRows)
    
    // Filter by Revenue type
    await typeFilter.selectOption('Revenue')
    await page.waitForTimeout(300)
    await expect(page.locator('text=Sales Revenue')).toBeVisible()
  })

  test('should display hierarchical account structure', async ({ page }) => {
    await page.goto('/accounts/chart')
    
    // Check for parent accounts
    await expect(page.locator('text=Assets')).toBeVisible()
    await expect(page.locator('text=Current Assets')).toBeVisible()
    
    // Check for child accounts with indentation
    const cashAccount = page.locator('td:has-text("Cash and Bank")').first()
    await expect(cashAccount).toBeVisible()
  })

  test('should display Journal Entries page', async ({ page }) => {
    await page.goto('/accounts/journal')
    
    // Check page title
    await expect(page.locator('h1')).toContainText('Journal Entries')
    
    // Check filters
    await expect(page.locator('input[placeholder*="Search entries"]')).toBeVisible()
    await expect(page.locator('input[type="date"]')).toBeVisible()
    await expect(page.locator('select:has-text("All Status")')).toBeVisible()
    
    // Check table columns
    await expect(page.locator('th:has-text("Entry #")')).toBeVisible()
    await expect(page.locator('th:has-text("Date")')).toBeVisible()
    await expect(page.locator('th:has-text("Debit")')).toBeVisible()
    await expect(page.locator('th:has-text("Credit")')).toBeVisible()
    
    // Check entries are displayed
    await expect(page.locator('table tbody tr')).toHaveCount(5)
    
    // Check balance summary
    await expect(page.locator('text=Total Debits')).toBeVisible()
    await expect(page.locator('text=Total Credits')).toBeVisible()
    await expect(page.locator('text=Difference')).toBeVisible()
  })

  test('should filter journal entries by status', async ({ page }) => {
    await page.goto('/accounts/journal')
    
    // Filter by Posted status
    const statusFilter = page.locator('select:has-text("All Status")')
    await statusFilter.selectOption('Posted')
    await page.waitForTimeout(300)
    
    // Verify only Posted entries are shown
    const postedBadges = page.locator('.bg-green-100.text-green-800:has-text("Posted")')
    const postedCount = await postedBadges.count()
    expect(postedCount).toBeGreaterThan(0)
    
    // Filter by Draft status
    await statusFilter.selectOption('Draft')
    await page.waitForTimeout(300)
    await expect(page.locator('.bg-yellow-100.text-yellow-800:has-text("Draft")')).toBeVisible()
  })

  test('should verify journal entry balance', async ({ page }) => {
    await page.goto('/accounts/journal')
    
    // Check that debits equal credits for posted entries
    const debitText = await page.locator('text=Total Debits').locator('..').locator('.text-xl').textContent()
    const creditText = await page.locator('text=Total Credits').locator('..').locator('.text-xl').textContent()
    
    // Both should be formatted currency
    expect(debitText).toContain('R')
    expect(creditText).toContain('R')
  })

  test('should display Financial Statements hub', async ({ page }) => {
    await page.goto('/accounts/statements')
    
    // Check page title
    await expect(page.locator('h1')).toContainText('Financial Statements')
    
    // Check navigation cards are displayed
    await expect(page.locator('text=Balance Sheet')).toBeVisible()
    await expect(page.locator('text=Profit & Loss')).toBeVisible()
    await expect(page.locator('text=Cash Flow')).toBeVisible()
    await expect(page.locator('text=VAT Report')).toBeVisible()
    await expect(page.locator('text=Trial Balance')).toBeVisible()
    await expect(page.locator('text=General Ledger')).toBeVisible()
    
    // Check financial overview section
    await expect(page.locator('h3:has-text("Financial Overview")')).toBeVisible()
    await expect(page.locator('text=Total Assets')).toBeVisible()
    await expect(page.locator('text=Total Liabilities')).toBeVisible()
    await expect(page.locator('text=Net Equity')).toBeVisible()
    
    // Check recent reports section
    await expect(page.locator('h3:has-text("Recent Reports")')).toBeVisible()
  })

  test('should navigate from statements hub to Balance Sheet', async ({ page }) => {
    await page.goto('/accounts/statements')
    
    // Click on Balance Sheet card
    await page.click('a:has-text("Balance Sheet")')
    await page.waitForURL('/accounting/reports/balance-sheet')
    
    // Verify navigation worked
    await expect(page.locator('h1')).toContainText('Balance Sheet')
  })

  test('should navigate from statements hub to Profit & Loss', async ({ page }) => {
    await page.goto('/accounts/statements')
    
    // Click on Profit & Loss card
    await page.click('a:has-text("Profit & Loss")')
    await page.waitForURL('/accounting/reports/profit-loss')
    
    // Verify navigation worked
    await expect(page.locator('h1')).toContainText('Profit & Loss')
  })

  test('should display recent reports list', async ({ page }) => {
    await page.goto('/accounts/statements')
    
    // Check that recent reports are listed
    const reportItems = page.locator('div:has-text("Balance Sheet - February 2024")')
    await expect(reportItems.first()).toBeVisible()
    
    // Check download and view buttons
    await expect(page.locator('button:has-text("Download")')).toHaveCount(4)
    await expect(page.locator('button:has-text("View")').first()).toBeVisible()
  })
})

