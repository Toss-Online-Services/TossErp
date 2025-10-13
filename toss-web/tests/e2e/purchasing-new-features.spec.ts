import { test, expect } from '@playwright/test'

test.describe('Purchasing Module - New Features', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/')
    await page.waitForLoadState('networkidle')
  })

  test.describe('Material Requests', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/material-requests')
      await page.waitForLoadState('networkidle')
    })

    test('should display material requests page title', async ({ page }) => {
      await expect(page).toHaveTitle(/Material Requests - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Material Requests')
      await expect(page.locator('text=Department requisitions for materials and services')).toBeVisible()
    })

    test('should display material request statistics', async ({ page }) => {
      await expect(page.locator('text=Total Requests')).toBeVisible()
      await expect(page.locator('text=145')).toBeVisible()
      await expect(page.locator('text=Pending Approval')).toBeVisible()
      await expect(page.locator('text=15')).toBeVisible()
      await expect(page.locator('text=To Purchase')).toBeVisible()
    })

    test('should have action buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("New Material Request")')).toBeVisible()
      await expect(page.locator('button:has-text("Export")')).toBeVisible()
      await expect(page.locator('button:has-text("Bulk Approve")')).toBeVisible()
      await expect(page.locator('button:has-text("Convert to PR")')).toBeVisible()
    })

    test('should display material requests table', async ({ page }) => {
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('text=MR-2025-001')).toBeVisible()
      await expect(page.locator('text=Thabo Mokoena')).toBeVisible()
      await expect(page.locator('text=Production')).toBeVisible()
    })

    test('should display stock status badges', async ({ page }) => {
      await expect(page.locator('text=Partial').first()).toBeVisible()
      await expect(page.locator('text=Low Stock').first()).toBeVisible()
      await expect(page.locator('text=In Stock').first()).toBeVisible()
    })

    test('should filter by department', async ({ page }) => {
      const departmentFilter = page.locator('select').nth(2)
      await departmentFilter.selectOption('Production')
      await page.waitForTimeout(300)
      await expect(page.locator('text=Production')).toBeVisible()
    })

    test('should search material requests', async ({ page }) => {
      const searchInput = page.locator('input[placeholder*="Search material requests"]')
      await searchInput.fill('Steel')
      await page.waitForTimeout(300)
      await expect(page.locator('text=Steel Sheets')).toBeVisible()
    })
  })

  test.describe('Request for Quotation (RFQ)', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/rfq')
      await page.waitForLoadState('networkidle')
    })

    test('should display RFQ page title', async ({ page }) => {
      await expect(page).toHaveTitle(/Request for Quotation - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Request for Quotation')
      await expect(page.locator('text=Solicit competitive quotes from multiple suppliers')).toBeVisible()
    })

    test('should display RFQ statistics', async ({ page }) => {
      await expect(page.locator('text=Total RFQs')).toBeVisible()
      await expect(page.locator('text=67')).toBeVisible()
      await expect(page.locator('text=Open RFQs')).toBeVisible()
      await expect(page.locator('text=Quotes Received')).toBeVisible()
      await expect(page.locator('text=Avg Savings')).toBeVisible()
      await expect(page.locator('text=14.5%')).toBeVisible()
    })

    test('should have action buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("Create RFQ")')).toBeVisible()
      await expect(page.locator('button:has-text("Export")')).toBeVisible()
    })

    test('should display RFQ cards', async ({ page }) => {
      await expect(page.locator('text=RFQ-2025-001')).toBeVisible()
      await expect(page.locator('text=Office Equipment & Furniture')).toBeVisible()
      await expect(page.locator('text=Suppliers Invited').first()).toBeVisible()
    })

    test('should show supplier response status', async ({ page }) => {
      // Check for supplier badges with checkmark for responded
      await expect(page.locator('text=Tech Solutions Inc').first()).toBeVisible()
      await expect(page.locator('text=Quality Equipment Co').first()).toBeVisible()
    })

    test('should have compare quotes button for RFQs with responses', async ({ page }) => {
      await expect(page.locator('button:has-text("Compare Quotes")').first()).toBeVisible()
    })
  })

  test.describe('Supplier Quotations', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/supplier-quotations')
      await page.waitForLoadState('networkidle')
    })

    test('should display supplier quotations page', async ({ page }) => {
      await expect(page).toHaveTitle(/Supplier Quotations - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Supplier Quotations')
    })

    test('should display quotation statistics', async ({ page }) => {
      await expect(page.locator('text=Total Quotations')).toBeVisible()
      await expect(page.locator('text=156')).toBeVisible()
      await expect(page.locator('text=Under Review')).toBeVisible()
      await expect(page.locator('text=Cost Reduction')).toBeVisible()
    })

    test('should have compare quotes button', async ({ page }) => {
      await expect(page.locator('button:has-text("Compare Quotes")')).toBeVisible()
    })

    test('should display quotation cards with details', async ({ page }) => {
      await expect(page.locator('text=SQ-2025-001')).toBeVisible()
      await expect(page.locator('text=Tech Solutions Inc').first()).toBeVisible()
      // Check price display
      await expect(page.locator('text=R 126,500').first()).toBeVisible()
    })

    test('should show VAT breakdown', async ({ page }) => {
      await expect(page.locator('text=Subtotal:').first()).toBeVisible()
      await expect(page.locator('text=VAT (15%):').first()).toBeVisible()
      await expect(page.locator('text=Total:').first()).toBeVisible()
    })

    test('should have accept and reject buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("Accept & Award")').first()).toBeVisible()
      await expect(page.locator('button:has-text("Reject")').first()).toBeVisible()
    })
  })

  test.describe('Blanket Orders', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/blanket-orders')
      await page.waitForLoadState('networkidle')
    })

    test('should display blanket orders page', async ({ page }) => {
      await expect(page).toHaveTitle(/Blanket Orders - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Blanket Orders')
    })

    test('should display blanket order statistics', async ({ page }) => {
      await expect(page.locator('text=Active Agreements')).toBeVisible()
      await expect(page.locator('text=Scheduled Releases')).toBeVisible()
      await expect(page.locator('text=Utilization Rate')).toBeVisible()
      await expect(page.locator('text=78%').first()).toBeVisible()
    })

    test('should have create blanket order button', async ({ page }) => {
      await expect(page.locator('button:has-text("New Blanket Order")')).toBeVisible()
    })

    test('should display blanket order cards', async ({ page }) => {
      await expect(page.locator('text=BO-2025-001')).toBeVisible()
      await expect(page.locator('text=Raw Materials Corp').first()).toBeVisible()
      await expect(page.locator('text=Steel Sheets Grade A')).toBeVisible()
    })

    test('should show commitment progress bars', async ({ page }) => {
      const progressBars = page.locator('.bg-blue-600.h-2.rounded-full')
      await expect(progressBars.first()).toBeVisible()
    })

    test('should display release history', async ({ page }) => {
      await expect(page.locator('text=Release History').first()).toBeVisible()
      await expect(page.locator('text=delivered').first()).toBeVisible()
      await expect(page.locator('text=scheduled').first()).toBeVisible()
    })
  })

  test.describe('Purchase Analytics', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/purchasing/analytics')
      await page.waitForLoadState('networkidle')
    })

    test('should display analytics page', async ({ page }) => {
      await expect(page).toHaveTitle(/Purchase Analytics - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Purchase Analytics')
    })

    test('should display key metrics', async ({ page }) => {
      await expect(page.locator('text=Total Spend')).toBeVisible()
      await expect(page.locator('text=R 2.45M').first()).toBeVisible()
      await expect(page.locator('text=Cost Savings')).toBeVisible()
      await expect(page.locator('text=Active POs')).toBeVisible()
      await expect(page.locator('text=Supplier Score')).toBeVisible()
    })

    test('should have period selector', async ({ page }) => {
      const periodSelector = page.locator('select').first()
      await expect(periodSelector).toBeVisible()
      await expect(periodSelector.locator('option:has-text("This Month")')).toBeVisible()
    })

    test('should display spend by category', async ({ page }) => {
      await expect(page.locator('text=Spend by Category')).toBeVisible()
      await expect(page.locator('text=Raw Materials').first()).toBeVisible()
      await expect(page.locator('text=Equipment').first()).toBeVisible()
    })

    test('should display top suppliers', async ({ page }) => {
      await expect(page.locator('text=Top Suppliers by Spend')).toBeVisible()
      await expect(page.locator('text=Raw Materials Corp').first()).toBeVisible()
    })

    test('should display KPI sections', async ({ page }) => {
      await expect(page.locator('text=Cycle Time')).toBeVisible()
      await expect(page.locator('text=Compliance')).toBeVisible()
      await expect(page.locator('text=Procurement ROI')).toBeVisible()
    })

    test('should display supplier performance metrics', async ({ page }) => {
      await expect(page.locator('text=Supplier Performance Metrics')).toBeVisible()
      await expect(page.locator('text=On-Time Delivery')).toBeVisible()
      await expect(page.locator('text=Quality Pass Rate')).toBeVisible()
    })
  })

  test.describe('Dashboard Integration', () => {
    test('should navigate to material requests from dashboard', async ({ page }) => {
      await page.goto('/purchasing')
      await page.waitForLoadState('networkidle')
      
      const materialRequestsLink = page.locator('a:has-text("Material Requests")')
      await expect(materialRequestsLink).toBeVisible()
      await materialRequestsLink.click()
      
      await page.waitForLoadState('networkidle')
      await expect(page).toHaveURL(/\/purchasing\/material-requests/)
      await expect(page.locator('h1')).toContainText('Material Requests')
    })

    test('should navigate to RFQ from dashboard', async ({ page }) => {
      await page.goto('/purchasing')
      await page.waitForLoadState('networkidle')
      
      const rfqLink = page.locator('a:has-text("Request for Quotation")')
      await expect(rfqLink).toBeVisible()
      await rfqLink.click()
      
      await page.waitForLoadState('networkidle')
      await expect(page).toHaveURL(/\/purchasing\/rfq/)
      await expect(page.locator('h1')).toContainText('Request for Quotation')
    })

    test('should navigate to supplier quotations from dashboard', async ({ page }) => {
      await page.goto('/purchasing')
      await page.waitForLoadState('networkidle')
      
      const quotationsLink = page.locator('a:has-text("Supplier Quotations")')
      await expect(quotationsLink).toBeVisible()
      await quotationsLink.click()
      
      await page.waitForLoadState('networkidle')
      await expect(page).toHaveURL(/\/purchasing\/supplier-quotations/)
      await expect(page.locator('h1')).toContainText('Supplier Quotations')
    })

    test('should navigate to blanket orders from dashboard', async ({ page }) => {
      await page.goto('/purchasing')
      await page.waitForLoadState('networkidle')
      
      const blanketOrdersLink = page.locator('a:has-text("Blanket Orders")')
      await expect(blanketOrdersLink).toBeVisible()
      await blanketOrdersLink.click()
      
      await page.waitForLoadState('networkidle')
      await expect(page).toHaveURL(/\/purchasing\/blanket-orders/)
      await expect(page.locator('h1')).toContainText('Blanket Orders')
    })

    test('should navigate to analytics from dashboard quick actions', async ({ page }) => {
      await page.goto('/purchasing')
      await page.waitForLoadState('networkidle')
      
      const analyticsButton = page.locator('button:has-text("View Analytics")')
      await expect(analyticsButton).toBeVisible()
      await analyticsButton.click()
      
      await page.waitForLoadState('networkidle')
      await expect(page).toHaveURL(/\/purchasing\/analytics/)
      await expect(page.locator('h1')).toContainText('Purchase Analytics')
    })
  })
})

