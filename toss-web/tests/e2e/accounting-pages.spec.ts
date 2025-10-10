import { test, expect } from '@playwright/test'

test.describe('Accounting Module Pages', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login')
    await page.fill('input[name="email"]', 'admin@example.com')
    await page.fill('input[name="password"]', 'password')
    await page.click('button[type="submit"]')
    await page.waitForURL('/dashboard')
  })

  test('should navigate to and display Country Management page', async ({ page }) => {
    await page.goto('/accounting/country')
    
    // Check page title
    await expect(page.locator('h1')).toContainText('Country Management')
    
    // Check search functionality
    const searchInput = page.locator('input[placeholder*="Search"]')
    await expect(searchInput).toBeVisible()
    
    // Check table headers
    await expect(page.locator('th:has-text("Country Name")')).toBeVisible()
    await expect(page.locator('th:has-text("Code")')).toBeVisible()
    
    // Check that countries are displayed
    await expect(page.locator('table tbody tr')).toHaveCount(5)
    
    // Test search functionality
    await searchInput.fill('South Africa')
    await page.waitForTimeout(500)
    await expect(page.locator('table tbody tr')).toHaveCount(1)
  })

  test('should navigate to and display Currency Management page', async ({ page }) => {
    await page.goto('/accounting/currency')
    
    await expect(page.locator('h1')).toContainText('Currency Management')
    
    // Check currency cards are displayed
    await expect(page.locator('.bg-white.dark\\:bg-gray-800')).toHaveCount(5) // 5 currency cards
    
    // Check search functionality
    const searchInput = page.locator('input[placeholder*="Search"]')
    await searchInput.fill('USD')
    await page.waitForTimeout(500)
    
    // Verify USD is visible
    await expect(page.locator('text=US Dollar')).toBeVisible()
  })

  test('should navigate to and display Finance Book page', async ({ page }) => {
    await page.goto('/accounting/finance-book')
    
    await expect(page.locator('h1')).toContainText('Finance Books')
    
    // Check finance book cards
    const financeBooks = page.locator('.bg-white.dark\\:bg-gray-800.rounded-lg')
    await expect(financeBooks).toHaveCount(3)
    
    // Check status filters
    const statusFilter = page.locator('select:has-text("All Status")')
    await expect(statusFilter).toBeVisible()
    
    // Filter by active
    await statusFilter.selectOption('Active')
    await page.waitForTimeout(300)
  })

  test('should navigate to and display Fiscal Year page', async ({ page }) => {
    await page.goto('/accounting/fiscal-year')
    
    await expect(page.locator('h1')).toContainText('Fiscal Year Management')
    
    // Check fiscal year cards
    const fiscalYearCards = page.locator('.bg-white.dark\\:bg-gray-800.rounded-lg')
    await expect(fiscalYearCards.first()).toBeVisible()
    
    // Check for periods within fiscal year
    await expect(page.locator('text=Q1')).toBeVisible()
    await expect(page.locator('text=Q2')).toBeVisible()
  })

  test('should navigate to and display Loyalty Program page', async ({ page }) => {
    await page.goto('/accounting/loyalty-program')
    
    await expect(page.locator('h1')).toContainText('Loyalty Programs')
    
    // Check program cards
    const programCards = page.locator('.bg-white.dark\\:bg-gray-800.rounded-lg')
    await expect(programCards).toHaveCount(3)
    
    // Verify program details are visible
    await expect(page.locator('text=VIP Rewards')).toBeVisible()
    await expect(page.locator('text=members')).toBeVisible()
  })

  test('should navigate to and display Mode of Payment page', async ({ page }) => {
    await page.goto('/accounting/mode-of-payment')
    
    await expect(page.locator('h1')).toContainText('Payment Methods')
    
    // Check payment method cards
    const paymentCards = page.locator('.bg-white.dark\\:bg-gray-800.rounded-lg')
    await expect(paymentCards).toHaveCount(5)
    
    // Verify payment methods
    await expect(page.locator('text=Cash')).toBeVisible()
    await expect(page.locator('text=Credit Card')).toBeVisible()
    await expect(page.locator('text=Bank Transfer')).toBeVisible()
  })

  test('should navigate to and display Payment Terms page', async ({ page }) => {
    await page.goto('/accounting/payment-terms')
    
    await expect(page.locator('h1')).toContainText('Payment Terms')
    
    // Check table
    await expect(page.locator('table')).toBeVisible()
    
    // Check payment terms are listed
    await expect(page.locator('text=Net 30')).toBeVisible()
    await expect(page.locator('text=2/10 Net 30')).toBeVisible()
    
    // Check usage count column
    await expect(page.locator('th:has-text("Usage Count")')).toBeVisible()
  })

  test('should navigate to and display Accounting Periods page', async ({ page }) => {
    await page.goto('/accounting/periods')
    
    await expect(page.locator('h1')).toContainText('Accounting Periods')
    
    // Check fiscal year dropdown
    const fiscalYearSelect = page.locator('select:has-text("Select Fiscal Year")')
    await expect(fiscalYearSelect).toBeVisible()
    
    // Check period cards
    const periodCards = page.locator('.bg-white.dark\\:bg-gray-800.rounded-lg')
    await expect(periodCards).toHaveCount(6)
    
    // Verify period statuses
    await expect(page.locator('text=Open')).toBeVisible()
    await expect(page.locator('text=Closed')).toBeVisible()
  })

  test('should navigate to Balance Sheet report', async ({ page }) => {
    await page.goto('/accounting/reports/balance-sheet')
    
    await expect(page.locator('h1')).toContainText('Balance Sheet')
    
    // Check date picker
    const datePicker = page.locator('input[type="date"]')
    await expect(datePicker).toBeVisible()
    
    // Check currency selector
    const currencySelect = page.locator('select#currency')
    await expect(currencySelect).toBeVisible()
    
    // Generate report
    await page.click('button:has-text("Generate Report")')
    await page.waitForTimeout(1000)
    
    // Verify report sections
    await expect(page.locator('h3:has-text("Assets")')).toBeVisible()
    await expect(page.locator('h3:has-text("Liabilities")')).toBeVisible()
    await expect(page.locator('text=Total Assets')).toBeVisible()
  })

  test('should navigate to Profit & Loss report', async ({ page }) => {
    await page.goto('/accounting/reports/profit-loss')
    
    await expect(page.locator('h1')).toContainText('Profit & Loss Statement')
    
    // Check date range pickers
    const startDate = page.locator('input#startDate')
    const endDate = page.locator('input#endDate')
    await expect(startDate).toBeVisible()
    await expect(endDate).toBeVisible()
    
    // Generate report
    await page.click('button:has-text("Generate Report")')
    await page.waitForTimeout(1000)
    
    // Verify report sections
    await expect(page.locator('h3:has-text("Revenue")')).toBeVisible()
    await expect(page.locator('h3:has-text("Operating Expenses")')).toBeVisible()
    await expect(page.locator('text=Net Profit')).toBeVisible()
  })

  test('should navigate to VAT Report', async ({ page }) => {
    await page.goto('/accounting/vat-report')
    
    await expect(page.locator('h1')).toContainText('VAT Report')
    
    // Check date range pickers
    await expect(page.locator('input#startDate')).toBeVisible()
    await expect(page.locator('input#endDate')).toBeVisible()
    
    // Generate report
    await page.click('button:has-text("Generate Report")')
    await page.waitForTimeout(1000)
    
    // Verify VAT summary
    await expect(page.locator('text=Output VAT')).toBeVisible()
    await expect(page.locator('text=Input VAT')).toBeVisible()
    await expect(page.locator('text=Net VAT Payable')).toBeVisible()
  })
})

