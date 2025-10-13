import { test, expect } from '@playwright/test'

test.describe('Sales Module - New Features', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/')
    await page.waitForLoadState('networkidle')
  })

  test.describe('Delivery Notes', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/sales/delivery-notes')
      await page.waitForLoadState('networkidle')
    })

    test('should display delivery notes page title and description', async ({ page }) => {
      await expect(page).toHaveTitle(/Delivery Notes - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Delivery Notes')
      await expect(page.locator('text=Track order fulfillment and deliveries')).toBeVisible()
    })

    test('should display delivery statistics cards', async ({ page }) => {
      // Total Deliveries
      await expect(page.locator('text=Total Deliveries')).toBeVisible()
      await expect(page.locator('text=234')).toBeVisible()
      await expect(page.locator('text=12 today')).toBeVisible()

      // In Transit
      await expect(page.locator('text=In Transit')).toBeVisible()
      await expect(page.locator('text=18')).toBeVisible()
      await expect(page.locator('text=5 pending')).toBeVisible()

      // Delivered
      await expect(page.locator('text=Delivered')).toBeVisible()
      await expect(page.locator('text=216')).toBeVisible()
      await expect(page.locator('text=94% on time')).toBeVisible()

      // Avg Delivery Time
      await expect(page.locator('text=Avg Delivery Time')).toBeVisible()
      await expect(page.locator('text=4.5h')).toBeVisible()
    })

    test('should display action buttons', async ({ page }) => {
      await expect(page.locator('button:has-text("New Delivery")')).toBeVisible()
      await expect(page.locator('button:has-text("Export")')).toBeVisible()
    })

    test('should display search and filter controls', async ({ page }) => {
      await expect(page.locator('input[placeholder*="Search deliveries"]')).toBeVisible()
      await expect(page.locator('select').first()).toBeVisible() // Status filter
      await expect(page.locator('select').nth(1)).toBeVisible() // Time filter
    })

    test('should display recent deliveries list', async ({ page }) => {
      await expect(page.locator('h3:has-text("Recent Deliveries")')).toBeVisible()
      
      // Check first delivery note
      await expect(page.locator('text=DN-2025-001')).toBeVisible()
      await expect(page.locator('text=Nomsa Community Kitchen')).toBeVisible()
      await expect(page.locator('text=123 Community Street, Soweto, 1818')).toBeVisible()
      await expect(page.locator('text=8 items')).toBeVisible()
    })

    test('should filter deliveries by status', async ({ page }) => {
      const statusFilter = page.locator('select').first()
      await statusFilter.selectOption('In Transit')
      await page.waitForTimeout(500) // Wait for filter to apply
      
      // Verify filtered results
      await expect(page.locator('text=in-transit')).toBeVisible()
    })

    test('should filter deliveries by time period', async ({ page }) => {
      const timeFilter = page.locator('select').nth(1)
      await timeFilter.selectOption('Today')
      await page.waitForTimeout(500) // Wait for filter to apply
    })

    test('should search deliveries', async ({ page }) => {
      const searchInput = page.locator('input[placeholder*="Search deliveries"]')
      await searchInput.fill('Nomsa')
      await page.waitForTimeout(500) // Wait for search to apply
      
      await expect(page.locator('text=Nomsa Community Kitchen')).toBeVisible()
    })

    test('should have view, print, and track actions for each delivery', async ({ page }) => {
      // Check that action buttons exist for the first delivery
      const firstDelivery = page.locator('.space-y-4 > div').first()
      const actionButtons = firstDelivery.locator('button')
      
      await expect(actionButtons).toHaveCount(3) // View, Print, Track
    })

    test('should display delivery status badges', async ({ page }) => {
      await expect(page.locator('text=in-transit').first()).toBeVisible()
      await expect(page.locator('text=delivered').first()).toBeVisible()
      await expect(page.locator('text=ready').first()).toBeVisible()
    })
  })

  test.describe('Pricing Rules', () => {
    test.beforeEach(async ({ page }) => {
      await page.goto('/sales/pricing-rules')
      await page.waitForLoadState('networkidle')
    })

    test('should display pricing rules page title and description', async ({ page }) => {
      await expect(page).toHaveTitle(/Pricing Rules - TOSS ERP/)
      await expect(page.locator('h1')).toContainText('Pricing Rules')
      await expect(page.locator('text=Automate discounts and special pricing')).toBeVisible()
    })

    test('should display pricing statistics cards', async ({ page }) => {
      // Active Rules
      await expect(page.locator('text=Active Rules')).toBeVisible()
      await expect(page.locator('text=8').first()).toBeVisible()
      await expect(page.locator('text=12 total')).toBeVisible()

      // Discounts Applied
      await expect(page.locator('text=Discounts Applied')).toBeVisible()
      await expect(page.locator('text=234')).toBeVisible()
      await expect(page.locator('text=This month')).toBeVisible()

      // Total Savings
      await expect(page.locator('text=Total Savings')).toBeVisible()
      await expect(page.locator('text=R 12,450')).toBeVisible()
      await expect(page.locator('text=Customer savings')).toBeVisible()

      // Avg Discount
      await expect(page.locator('text=Avg Discount')).toBeVisible()
      await expect(page.locator('text=8.5%')).toBeVisible()
    })

    test('should display new rule button', async ({ page }) => {
      await expect(page.locator('button:has-text("New Rule")')).toBeVisible()
    })

    test('should display search and filter controls', async ({ page }) => {
      await expect(page.locator('input[placeholder*="Search pricing rules"]')).toBeVisible()
      await expect(page.locator('select').first()).toBeVisible() // Type filter
      await expect(page.locator('select').nth(1)).toBeVisible() // Status filter
    })

    test('should display pricing rules list', async ({ page }) => {
      await expect(page.locator('h3:has-text("Pricing Rules")')).toBeVisible()
      
      // Check first rule
      await expect(page.locator('text=Weekend Special')).toBeVisible()
      await expect(page.locator('text=10% off all purchases on weekends')).toBeVisible()
      await expect(page.locator('text=Valid: 01 Jan 2025 - 31 Dec 2025')).toBeVisible()
      await expect(page.locator('text=10% off').first()).toBeVisible()
      await expect(page.locator('text=89 times')).toBeVisible()
    })

    test('should display different rule types', async ({ page }) => {
      // Percentage discount
      await expect(page.locator('text=Weekend Special')).toBeVisible()
      await expect(page.locator('text=10% off').first()).toBeVisible()

      // Fixed amount discount
      await expect(page.locator('text=Bulk Buy Discount')).toBeVisible()
      await expect(page.locator('text=R50 off')).toBeVisible()

      // Buy X Get Y
      await expect(page.locator('text=Buy 2 Get 1 Free')).toBeVisible()
      await expect(page.locator('text=Buy 2 Get 1')).toBeVisible()
    })

    test('should filter rules by type', async ({ page }) => {
      const typeFilter = page.locator('select').first()
      await typeFilter.selectOption('Percentage')
      await page.waitForTimeout(500) // Wait for filter to apply
      
      await expect(page.locator('text=Weekend Special')).toBeVisible()
    })

    test('should filter rules by status', async ({ page }) => {
      const statusFilter = page.locator('select').nth(1)
      await statusFilter.selectOption('Active')
      await page.waitForTimeout(500) // Wait for filter to apply
      
      await expect(page.locator('text=active').first()).toBeVisible()
    })

    test('should search pricing rules', async ({ page }) => {
      const searchInput = page.locator('input[placeholder*="Search pricing rules"]')
      await searchInput.fill('Weekend')
      await page.waitForTimeout(500) // Wait for search to apply
      
      await expect(page.locator('text=Weekend Special')).toBeVisible()
    })

    test('should have edit and delete actions for each rule', async ({ page }) => {
      // Check that action buttons exist for the first rule
      const firstRule = page.locator('.space-y-4 > div').first()
      const actionButtons = firstRule.locator('button')
      
      await expect(actionButtons).toHaveCount(3) // Edit, Toggle, Delete
    })

    test('should display rule status badges', async ({ page }) => {
      const activeBadges = page.locator('text=active')
      await expect(activeBadges.first()).toBeVisible()
    })

    test('should display rule usage statistics', async ({ page }) => {
      // Check that each rule shows usage count
      await expect(page.locator('text=89 times')).toBeVisible()
      await expect(page.locator('text=45 times')).toBeVisible()
      await expect(page.locator('text=67 times')).toBeVisible()
    })
  })

  test.describe('Sales Dashboard Integration', () => {
    test('should navigate to delivery notes from sales dashboard', async ({ page }) => {
      await page.goto('/sales')
      await page.waitForLoadState('networkidle')
      
      const deliveryNotesLink = page.locator('a:has-text("Delivery Notes")')
      await expect(deliveryNotesLink).toBeVisible()
      await deliveryNotesLink.click()
      
      await page.waitForLoadState('networkidle')
      await expect(page).toHaveURL(/\/sales\/delivery-notes/)
      await expect(page.locator('h1')).toContainText('Delivery Notes')
    })

    test('should navigate to pricing rules from sales dashboard', async ({ page }) => {
      await page.goto('/sales')
      await page.waitForLoadState('networkidle')
      
      const pricingRulesLink = page.locator('a:has-text("Pricing Rules")')
      await expect(pricingRulesLink).toBeVisible()
      await pricingRulesLink.click()
      
      await page.waitForLoadState('networkidle')
      await expect(page).toHaveURL(/\/sales\/pricing-rules/)
      await expect(page.locator('h1')).toContainText('Pricing Rules')
    })

    test('should display new features in core sales features section', async ({ page }) => {
      await page.goto('/sales')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h3:has-text("Core Sales Features")')).toBeVisible()
      await expect(page.locator('a:has-text("Delivery Notes")')).toBeVisible()
      await expect(page.locator('text=Track order fulfillment')).toBeVisible()
    })

    test('should display pricing rules in advanced features section', async ({ page }) => {
      await page.goto('/sales')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h3:has-text("Advanced Features")')).toBeVisible()
      await expect(page.locator('a:has-text("Pricing Rules")')).toBeVisible()
      await expect(page.locator('text=Automate discounts')).toBeVisible()
    })
  })

  test.describe('Mobile Responsiveness', () => {
    test.use({ viewport: { width: 375, height: 667 } }) // iPhone SE size

    test('should display delivery notes page correctly on mobile', async ({ page }) => {
      await page.goto('/sales/delivery-notes')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Delivery Notes')
      await expect(page.locator('button:has-text("New Delivery")')).toBeVisible()
      await expect(page.locator('text=Total Deliveries')).toBeVisible()
    })

    test('should display pricing rules page correctly on mobile', async ({ page }) => {
      await page.goto('/sales/pricing-rules')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Pricing Rules')
      await expect(page.locator('button:has-text("New Rule")')).toBeVisible()
      await expect(page.locator('text=Active Rules')).toBeVisible()
    })
  })

  test.describe('Dark Mode', () => {
    test.beforeEach(async ({ page }) => {
      // Enable dark mode
      await page.emulateMedia({ colorScheme: 'dark' })
    })

    test('should display delivery notes page in dark mode', async ({ page }) => {
      await page.goto('/sales/delivery-notes')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Delivery Notes')
      // Verify dark mode is applied by checking background color
      const bgColor = await page.locator('main').evaluate(el => 
        window.getComputedStyle(el).backgroundColor
      )
      // Dark mode background should be dark (not white)
      expect(bgColor).not.toBe('rgb(255, 255, 255)')
    })

    test('should display pricing rules page in dark mode', async ({ page }) => {
      await page.goto('/sales/pricing-rules')
      await page.waitForLoadState('networkidle')
      
      await expect(page.locator('h1')).toContainText('Pricing Rules')
      // Verify dark mode is applied
      const bgColor = await page.locator('main').evaluate(el => 
        window.getComputedStyle(el).backgroundColor
      )
      expect(bgColor).not.toBe('rgb(255, 255, 255)')
    })
  })
})

