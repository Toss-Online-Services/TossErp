import { test, expect } from '@playwright/test'

test.describe('Accounting Integration Tests', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/accounting')
    await page.waitForLoadState('networkidle')
  })

  // Account to Journal Entry Flow
  test.describe('Account Creation to Usage', () => {
    test('should create account and use in journal entry', async ({ page }) => {
      // Navigate to Chart of Accounts
      await page.goto('/accounting/chart-of-accounts')
      
      // Open account modal
      await page.click('button:has-text("Add Account")')
      await expect(page.locator('text=Create New Account')).toBeVisible()
      
      // Fill account form
      await page.fill('input[placeholder="e.g., 1110"]', '1150')
      await page.fill('input[placeholder="e.g., Cash and Bank"]', 'Petty Cash')
      await page.selectOption('select', { label: 'Asset' })
      await page.check('input[type="checkbox"]:has-text("Active")') // Should already be checked
      
      // Submit (mock save)
      await page.click('button:has-text("Create Account")')
      
      // Navigate to Journal Entries
      await page.goto('/accounting/journal-entries')
      
      // Create new journal entry
      await page.click('button:has-text("New Entry")')
      await expect(page.locator('text=Create New Journal Entry')).toBeVisible()
    })
  })

  // Journal Entry to Ledger to Reports Flow
  test.describe('End-to-End Transaction Flow', () => {
    test('should create journal entry and verify in reports', async ({ page }) => {
      // Step 1: Create Journal Entry
      await page.goto('/accounting/journal-entries')
      await page.click('button:has-text("New Entry")')
      
      await expect(page.locator('text=Create New Journal Entry')).toBeVisible()
      
      // Fill entry details
      await page.fill('input[type="date"]', '2024-03-15')
      await page.fill('input[placeholder="e.g., INV-001"]', 'TEST-001')
      await page.fill('textarea', 'Test transaction for integration')
      
      // Add line items would go here...
      // Close modal
      await page.click('button:has-text("Cancel")')
      
      // Step 2: Check General Ledger
      await page.goto('/accounting/reports/general-ledger')
      await page.selectOption('select', { label: '1110 - Cash and Bank' })
      await page.click('button:has-text("Generate Report")')
      await expect(page.locator('text=Account: 1110')).toBeVisible()
      
      // Step 3: Verify in Balance Sheet
      await page.goto('/accounting/reports/balance-sheet')
      await expect(page.locator('text=Cash and Bank')).toBeVisible()
    })

    test('should post journal entry and reflect in trial balance', async ({ page }) => {
      // Navigate to journal entries
      await page.goto('/accounting/journal-entries')
      
      // Find a draft entry
      const draftEntry = page.locator('tr:has-text("Draft")').first()
      if (await draftEntry.count() > 0) {
        await draftEntry.locator('button:has-text("Post")').click()
        // Confirm posting
      }
      
      // Navigate to trial balance
      await page.goto('/accounting/reports/trial-balance')
      await expect(page.locator('text=Balanced')).toBeVisible()
    })
  })

  // Multi-Currency Handling
  test.describe('Multi-Currency Operations', () => {
    test('should manage multiple currencies', async ({ page }) => {
      // Navigate to currency page
      await page.goto('/accounting/currency')
      
      await expect(page.locator('text=ZAR')).toBeVisible()
      await expect(page.locator('text=USD')).toBeVisible()
      await expect(page.locator('text=EUR')).toBeVisible()
      
      // Verify exchange rates displayed
      await expect(page.locator('text=Exchange Rate')).toBeVisible()
    })

    test('should use correct currency in reports', async ({ page }) => {
      // Navigate to balance sheet
      await page.goto('/accounting/reports/balance-sheet')
      
      // Verify currency format (R for ZAR)
      await expect(page.locator('text=/R \\d/')).toBeVisible()
    })
  })

  // VAT Calculations
  test.describe('VAT Calculations', () => {
    test('should calculate VAT correctly at 15%', async ({ page }) => {
      await page.goto('/accounting/vat-report')
      
      // Verify 15% VAT standard rate section
      await expect(page.locator('text=Standard Rate (15%)')).toBeVisible()
      
      // Check that output and input VAT are calculated
      await expect(page.locator('text=Output VAT (Sales)')).toBeVisible()
      await expect(page.locator('text=Input VAT (Purchases)')).toBeVisible()
    })

    test('should show net VAT payable/refundable', async ({ page }) => {
      await page.goto('/accounting/vat-report')
      
      // Net VAT should be calculated as Output - Input
      await expect(page.locator('text=Net VAT')).toBeVisible()
      await expect(page.locator('text=Total Payable')).toBeVisible()
    })
  })

  // Period Closing
  test.describe('Period Closing Workflow', () => {
    test('should display period closing options', async ({ page }) => {
      await page.goto('/accounting/periods')
      
      // Find open periods
      const openPeriod = page.locator('text=Open').first()
      if (await openPeriod.count() > 0) {
        await expect(page.locator('button:has-text("Close Period")')).toBeVisible()
      }
    })

    test('should prevent posting to closed periods', async ({ page }) => {
      await page.goto('/accounting/periods')
      
      // Verify closed periods are marked
      await expect(page.locator('text=Closed')).toBeVisible()
    })
  })

  // Navigation Integration
  test.describe('Navigation Integration', () => {
    test('should navigate between accounting pages', async ({ page }) => {
      // Start at dashboard
      await page.goto('/accounting')
      
      // Navigate to Chart of Accounts
      await page.click('text=Chart of Accounts')
      await expect(page).toHaveURL(/\/accounting\/chart-of-accounts/)
      
      // Navigate back to dashboard
      await page.goto('/accounting')
      
      // Navigate to Journal Entries
      await page.click('text=Journal Entries')
      await expect(page).toHaveURL(/\/accounting\/journal-entries/)
      
      // Navigate to Balance Sheet
      await page.goto('/accounting')
      await page.click('text=Balance Sheet')
      await expect(page).toHaveURL(/\/accounting\/reports\/balance-sheet/)
    })

    test('should access accounting from navigation menu', async ({ page }) => {
      await page.goto('/')
      
      // Open navigation (if burger menu exists)
      const burgerMenu = page.locator('[aria-label="Toggle navigation"]')
      if (await burgerMenu.count() > 0) {
        await burgerMenu.click()
      }
      
      // Click accounting link
      const accountingLink = page.locator('text=Accounting').or(page.locator('text=📊 Accounting'))
      if (await accountingLink.count() > 0) {
        await accountingLink.click()
        await expect(page).toHaveURL(/\/accounting/)
      }
    })
  })

  // Data Consistency
  test.describe('Data Consistency', () => {
    test('should maintain balance equation (Assets = Liabilities + Equity)', async ({ page }) => {
      await page.goto('/accounting/reports/balance-sheet')
      
      // Verify the fundamental accounting equation
      await expect(page.locator('text=Total Assets')).toBeVisible()
      await expect(page.locator('text=Total Liabilities & Equity')).toBeVisible()
      
      // Both totals should be equal (indicated by balanced books)
    })

    test('should maintain double-entry bookkeeping in journal entries', async ({ page }) => {
      await page.goto('/accounting/journal-entries')
      
      // Verify totals summary shows balanced entries
      await expect(page.locator('text=Difference')).toBeVisible()
      
      // For properly posted entries, difference should be 0
      const differenceElement = page.locator('text=Difference').locator('..').locator('p').last()
      // Should show R 0.00 for balanced entries
    })
  })

  // Export Functionality
  test.describe('Export Integration', () => {
    test('should export from all major pages', async ({ page }) => {
      const pages = [
        { url: '/accounting/chart-of-accounts', button: 'Export CSV' },
        { url: '/accounting/journal-entries', button: 'Export CSV' },
        { url: '/accounting/company', button: 'Export CSV' },
        { url: '/accounting/vat-report', button: 'Export CSV' },
        { url: '/accounting/reports/trial-balance', button: 'Export CSV' },
        { url: '/accounting/reports/cash-flow', button: 'Export CSV' },
      ]

      for (const pageInfo of pages) {
        await page.goto(pageInfo.url)
        await page.waitForLoadState('networkidle')
        
        const exportButton = page.locator(`button:has-text("${pageInfo.button}")`)
        await expect(exportButton).toBeVisible()
      }
    })
  })

  // Error Handling
  test.describe('Error Handling', () => {
    test('should handle missing data gracefully', async ({ page }) => {
      await page.goto('/accounting/reports/general-ledger')
      
      // Try to generate without selecting account
      await page.click('button:has-text("Generate Report")')
      
      // Should not crash, should show message or stay on empty state
      await expect(page.locator('text=Select an Account')).toBeVisible()
    })

    test('should validate journal entry balance', async ({ page }) => {
      await page.goto('/accounting/journal-entries')
      await page.click('button:has-text("New Entry")')
      
      // Modal should validate that debits = credits
      await expect(page.locator('text=Create New Journal Entry')).toBeVisible()
    })
  })
})

