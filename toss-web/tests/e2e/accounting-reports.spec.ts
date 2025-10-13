import { test, expect } from '@playwright/test'

test.describe('Accounting Reports', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/accounting')
    await page.waitForLoadState('networkidle')
  })

  // Balance Sheet Tests
  test.describe('Balance Sheet Report', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/accounting/reports/balance-sheet')
      await page.waitForLoadState('networkidle')
    })

    test('should display balance sheet page', async ({ page }) => {
      await expect(page).toHaveTitle(/Balance Sheet - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Balance Sheet')
    })

    test('should load balance sheet data', async ({ page }) => {
      await expect(page.locator('text=Assets')).toBeVisible()
      await expect(page.locator('text=Liabilities')).toBeVisible()
      await expect(page.locator('text=Equity')).toBeVisible()
    })

    test('should display current assets', async ({ page }) => {
      await expect(page.locator('text=Current Assets')).toBeVisible()
      await expect(page.locator('text=Cash and Bank')).toBeVisible()
      await expect(page.locator('text=Accounts Receivable')).toBeVisible()
      await expect(page.locator('text=Inventory')).toBeVisible()
    })

    test('should display fixed assets', async ({ page }) => {
      await expect(page.locator('text=Fixed Assets')).toBeVisible()
      await expect(page.locator('text=Equipment')).toBeVisible()
      await expect(page.locator('text=Vehicles')).toBeVisible()
    })

    test('should calculate totals correctly', async ({ page }) => {
      await expect(page.locator('text=Total Assets')).toBeVisible()
      await expect(page.locator('text=Total Liabilities & Equity')).toBeVisible()
    })

    test('should allow date selection', async ({ page }) => {
      const dateInput = page.locator('input[type="date"]')
      await expect(dateInput).toBeVisible()
    })

    test('should refresh report', async ({ page }) => {
      await page.click('button:has-text("Refresh")')
      await expect(page.locator('text=Assets')).toBeVisible()
    })
  })

  // Profit & Loss Tests
  test.describe('Profit & Loss Report', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/accounting/reports/profit-loss')
      await page.waitForLoadState('networkidle')
    })

    test('should display profit & loss page', async ({ page }) => {
      await expect(page).toHaveTitle(/Profit & Loss - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Profit & Loss')
    })

    test('should display summary cards', async ({ page }) => {
      await expect(page.locator('text=Total Revenue')).toBeVisible()
      await expect(page.locator('text=Total Expenses')).toBeVisible()
      await expect(page.locator('text=Net Profit')).toBeVisible()
      await expect(page.locator('text=Profit Margin')).toBeVisible()
    })

    test('should display revenue breakdown', async ({ page }) => {
      await expect(page.locator('text=Revenue')).toBeVisible()
      await expect(page.locator('text=Operating Revenue')).toBeVisible()
      await expect(page.locator('text=Sales Revenue')).toBeVisible()
    })

    test('should display expense breakdown', async ({ page }) => {
      await expect(page.locator('text=Expenses')).toBeVisible()
      await expect(page.locator('text=Operating Expenses')).toBeVisible()
      await expect(page.locator('text=Cost of Goods Sold')).toBeVisible()
    })

    test('should allow date range selection', async ({ page }) => {
      const startDateInput = page.locator('input[type="date"]').first()
      const endDateInput = page.locator('input[type="date"]').last()
      await expect(startDateInput).toBeVisible()
      await expect(endDateInput).toBeVisible()
    })

    test('should generate report', async ({ page }) => {
      await page.click('button:has-text("Generate Report")')
      await expect(page.locator('text=Revenue')).toBeVisible()
    })
  })

  // Cash Flow Tests
  test.describe('Cash Flow Statement', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/accounting/reports/cash-flow')
      await page.waitForLoadState('networkidle')
    })

    test('should display cash flow page', async ({ page }) => {
      await expect(page).toHaveTitle(/Cash Flow Statement - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Cash Flow Statement')
    })

    test('should display summary cards', async ({ page }) => {
      await expect(page.locator('text=Operating Activities')).toBeVisible()
      await expect(page.locator('text=Investing Activities')).toBeVisible()
      await expect(page.locator('text=Financing Activities')).toBeVisible()
      await expect(page.locator('text=Net Cash Flow')).toBeVisible()
    })

    test('should display operating activities', async ({ page }) => {
      await expect(page.locator('text=Net Profit')).toBeVisible()
      await expect(page.locator('text=Depreciation')).toBeVisible()
    })

    test('should display cash balance changes', async ({ page }) => {
      await expect(page.locator('text=Opening Cash Balance')).toBeVisible()
      await expect(page.locator('text=Closing Cash Balance')).toBeVisible()
    })

    test('should allow date range selection', async ({ page }) => {
      const dateInputs = page.locator('input[type="date"]')
      await expect(dateInputs.first()).toBeVisible()
    })

    test('should export cash flow to CSV', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export CSV")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/cash-flow.*\.csv/)
    })
  })

  // Trial Balance Tests
  test.describe('Trial Balance Report', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/accounting/reports/trial-balance')
      await page.waitForLoadState('networkidle')
    })

    test('should display trial balance page', async ({ page }) => {
      await expect(page).toHaveTitle(/Trial Balance - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Trial Balance')
    })

    test('should show balance status', async ({ page }) => {
      await expect(page.locator('text=Trial Balance Status')).toBeVisible()
      await expect(page.locator('text=Balanced')).toBeVisible()
    })

    test('should display accounts with debits and credits', async ({ page }) => {
      await expect(page.locator('text=Account Code')).toBeVisible()
      await expect(page.locator('text=Account Name')).toBeVisible()
      await expect(page.locator('text=Debit (R)')).toBeVisible()
      await expect(page.locator('text=Credit (R)')).toBeVisible()
    })

    test('should display totals', async ({ page }) => {
      await expect(page.locator('text=Total Debits')).toBeVisible()
      await expect(page.locator('text=Total Credits')).toBeVisible()
    })

    test('should export trial balance', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export CSV")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/trial-balance.*\.csv/)
    })

    test('should refresh report', async ({ page }) => {
      await page.click('button:has-text("Refresh")')
      await expect(page.locator('text=Trial Balance Status')).toBeVisible()
    })
  })

  // General Ledger Tests
  test.describe('General Ledger Report', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/accounting/reports/general-ledger')
      await page.waitForLoadState('networkidle')
    })

    test('should display general ledger page', async ({ page }) => {
      await expect(page).toHaveTitle(/General Ledger - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('General Ledger')
    })

    test('should show account selection', async ({ page }) => {
      await expect(page.locator('text=Select Account')).toBeVisible()
    })

    test('should show empty state when no account selected', async ({ page }) => {
      await expect(page.locator('text=Select an Account')).toBeVisible()
    })

    test('should load ledger entries after account selection', async ({ page }) => {
      await page.selectOption('select', { label: '1110 - Cash and Bank' })
      await page.click('button:has-text("Generate Report")')
      await expect(page.locator('text=Account: 1110')).toBeVisible()
    })

    test('should display ledger columns', async ({ page }) => {
      await page.selectOption('select', { label: '1110 - Cash and Bank' })
      await page.click('button:has-text("Generate Report")')
      await expect(page.locator('text=Date')).toBeVisible()
      await expect(page.locator('text=Reference')).toBeVisible()
      await expect(page.locator('text=Debit (R)')).toBeVisible()
      await expect(page.locator('text=Credit (R)')).toBeVisible()
      await expect(page.locator('text=Balance (R)')).toBeVisible()
    })
  })

  // VAT Report Tests
  test.describe('VAT Report', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/accounting/vat-report')
      await page.waitForLoadState('networkidle')
    })

    test('should display VAT report page', async ({ page }) => {
      await expect(page).toHaveTitle(/VAT Report - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('VAT Report')
    })

    test('should display summary cards', async ({ page }) => {
      await expect(page.locator('text=Output VAT (Sales)')).toBeVisible()
      await expect(page.locator('text=Input VAT (Purchases)')).toBeVisible()
      await expect(page.locator('text=Net VAT')).toBeVisible()
      await expect(page.locator('text=Total Payable')).toBeVisible()
    })

    test('should display sales VAT breakdown', async ({ page }) => {
      await expect(page.locator('text=Standard Rate (15%)')).toBeVisible()
      await expect(page.locator('text=Zero-Rated (0%)')).toBeVisible()
      await expect(page.locator('text=Exempt')).toBeVisible()
    })

    test('should display purchases VAT breakdown', async ({ page }) => {
      await expect(page.locator('text=Input VAT (Purchases)')).toBeVisible()
    })

    test('should export VAT report to CSV', async ({ page }) => {
      const downloadPromise = page.waitForEvent('download')
      await page.click('button:has-text("Export CSV")')
      const download = await downloadPromise
      expect(download.suggestedFilename()).toMatch(/vat-report.*\.csv/)
    })
  })
})

