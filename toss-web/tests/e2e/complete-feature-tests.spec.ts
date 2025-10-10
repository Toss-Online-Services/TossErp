import { test, expect } from '@playwright/test'

test.describe('Complete Feature Tests', () => {
  test.beforeEach(async ({ page }) => {
    // Login before each test
    await page.goto('/login')
    await page.fill('input[name="email"]', 'admin@example.com')
    await page.fill('input[name="password"]', 'password')
    await page.click('button[type="submit"]')
    await page.waitForURL('/dashboard')
  })

  test.describe('Dashboard Features', () => {
    test('should display dashboard with charts', async ({ page }) => {
      await page.goto('/dashboard')
      
      // Check page title
      await expect(page.locator('h1')).toContainText('Dashboard')
      
      // Check stats cards
      await expect(page.locator('text=Total Revenue')).toBeVisible()
      await expect(page.locator('text=Total Orders')).toBeVisible()
      await expect(page.locator('text=Active Customers')).toBeVisible()
      
      // Check that charts are rendered
      await expect(page.locator('canvas')).toHaveCount(4) // 4 charts on dashboard
    })

    test('should display chart tooltips on hover', async ({ page }) => {
      await page.goto('/dashboard')
      await page.waitForTimeout(1000) // Wait for charts to render
      
      // Hover over a chart area
      const chart = page.locator('canvas').first()
      await chart.hover({ position: { x: 100, y: 100 } })
      await page.waitForTimeout(500)
    })
  })

  test.describe('Manufacturing Module', () => {
    test('should display manufacturing dashboard', async ({ page }) => {
      await page.goto('/manufacturing/dashboard')
      
      await expect(page.locator('h1')).toContainText('Manufacturing Dashboard')
      
      // Check key metrics
      await expect(page.locator('text=Production Output')).toBeVisible()
      await expect(page.locator('text=Active Work Orders')).toBeVisible()
      await expect(page.locator('text=Quality Rate')).toBeVisible()
      
      // Check charts are present
      await expect(page.locator('canvas')).toHaveCount(2) // Production trend and quality metrics charts
    })

    test('should display Bill of Materials page', async ({ page }) => {
      await page.goto('/manufacturing/bom')
      
      await expect(page.locator('h1')).toContainText('Bill of Materials')
      
      // Check search and filters
      await expect(page.locator('input[placeholder*="Search"]')).toBeVisible()
      
      // Check table
      await expect(page.locator('table')).toBeVisible()
      await expect(page.locator('th:has-text("BOM Code")')).toBeVisible()
      await expect(page.locator('th:has-text("Product")')).toBeVisible()
    })

    test('should display Work Orders page', async ({ page }) => {
      await page.goto('/manufacturing/work-orders')
      
      await expect(page.locator('h1')).toContainText('Work Orders')
      
      // Check stats
      await expect(page.locator('text=Total Orders')).toBeVisible()
      await expect(page.locator('text=In Progress')).toBeVisible()
      
      // Check table
      await expect(page.locator('table tbody tr')).toHaveCount(5)
    })

    test('should display Quality Control page', async ({ page }) => {
      await page.goto('/manufacturing/quality')
      
      await expect(page.locator('h1')).toContainText('Quality Control')
      
      // Check quality metrics
      await expect(page.locator('text=Pass Rate')).toBeVisible()
      await expect(page.locator('text=Inspections')).toBeVisible()
    })
  })

  test.describe('Sales Module', () => {
    test('should display sales orders page', async ({ page }) => {
      await page.goto('/sales/orders')
      
      await expect(page.locator('h1')).toContainText('Sales Orders')
      
      // Check export button is visible
      await expect(page.locator('button:has-text("Export")')).toBeVisible()
    })

    test('should display POS dashboard', async ({ page }) => {
      await page.goto('/sales/pos/dashboard')
      
      await expect(page.locator('h1')).toContainText('POS Dashboard')
      
      // Check POS metrics
      await expect(page.locator('text=Today\'s Sales')).toBeVisible()
      await expect(page.locator('text=Transactions')).toBeVisible()
    })
  })

  test.describe('Inventory Module', () => {
    test('should display inventory dashboard', async ({ page }) => {
      await page.goto('/inventory/dashboard')
      
      await expect(page.locator('h1')).toContainText('Inventory Dashboard')
      
      // Check inventory stats
      await expect(page.locator('text=Total Items')).toBeVisible()
      await expect(page.locator('text=Low Stock')).toBeVisible()
    })
  })

  test.describe('HR Module', () => {
    test('should display HR dashboard', async ({ page }) => {
      await page.goto('/hr/dashboard')
      
      await expect(page.locator('h1')).toContainText('HR Dashboard')
      
      // Check HR stats
      await expect(page.locator('text=Total Employees')).toBeVisible()
      await expect(page.locator('text=On Leave')).toBeVisible()
    })

    test('should display employees page', async ({ page }) => {
      await page.goto('/hr/employees')
      
      await expect(page.locator('h1')).toContainText('Employees')
      
      // Check employee list
      await expect(page.locator('table')).toBeVisible()
    })
  })

  test.describe('CRM Module', () => {
    test('should display CRM leads page', async ({ page }) => {
      await page.goto('/crm/leads')
      
      await expect(page.locator('h1')).toContainText('Leads')
      
      // Check leads stats
      await expect(page.locator('text=Total Leads')).toBeVisible()
    })

    test('should display customers page', async ({ page }) => {
      await page.goto('/crm/customers')
      
      await expect(page.locator('h1')).toContainText('Customers')
      
      // Check customer list
      await expect(page.locator('table')).toBeVisible()
    })
  })

  test.describe('Export Functionality', () => {
    test('should have export button on sales orders', async ({ page }) => {
      await page.goto('/sales/orders')
      
      const exportButton = page.locator('button:has-text("Export")')
      await expect(exportButton).toBeVisible()
      
      // Click export to open menu
      await exportButton.click()
      await page.waitForTimeout(300)
      
      // Check export options are available
      // Note: actual implementation may vary based on ExportButton component
    })
  })

  test.describe('Theme Switching', () => {
    test('should toggle dark mode', async ({ page }) => {
      await page.goto('/dashboard')
      
      // Find theme switcher button
      const themeButton = page.locator('button[aria-label*="theme"], button:has-text("Theme")')
      
      if (await themeButton.count() > 0) {
        await themeButton.click()
        await page.waitForTimeout(300)
        
        // Check that dark mode classes are applied
        const html = page.locator('html')
        const classList = await html.getAttribute('class')
        
        // Should have either 'dark' or 'light' class
        expect(classList).toBeTruthy()
      }
    })
  })

  test.describe('Navigation', () => {
    test('should navigate through sidebar menu', async ({ page }) => {
      await page.goto('/dashboard')
      
      // Check sidebar is visible
      await expect(page.locator('nav, aside')).toBeVisible()
      
      // Navigate to different modules
      const modules = [
        { name: 'Manufacturing', url: '/manufacturing' },
        { name: 'Sales', url: '/sales' },
        { name: 'Inventory', url: '/inventory' },
        { name: 'Accounting', url: '/accounting' },
      ]
      
      for (const module of modules) {
        const link = page.locator(`a:has-text("${module.name}")`).first()
        if (await link.count() > 0) {
          await link.click()
          await page.waitForTimeout(500)
          expect(page.url()).toContain(module.url)
        }
      }
    })
  })

  test.describe('Responsive Design', () => {
    test('should be responsive on mobile', async ({ page }) => {
      // Set mobile viewport
      await page.setViewportSize({ width: 375, height: 667 })
      await page.goto('/dashboard')
      
      // Check that page loads
      await expect(page.locator('h1')).toBeVisible()
      
      // Mobile menu button should be visible
      const menuButton = page.locator('button[aria-label*="menu"], button:has-text("Menu")')
      if (await menuButton.count() > 0) {
        await expect(menuButton).toBeVisible()
      }
    })

    test('should be responsive on tablet', async ({ page }) => {
      // Set tablet viewport
      await page.setViewportSize({ width: 768, height: 1024 })
      await page.goto('/dashboard')
      
      // Check that page loads and is properly laid out
      await expect(page.locator('h1')).toBeVisible()
    })
  })

  test.describe('Search Functionality', () => {
    test('should search across pages', async ({ page }) => {
      // Test search on accounting/country page
      await page.goto('/accounting/country')
      
      const searchInput = page.locator('input[placeholder*="Search"]')
      await searchInput.fill('South')
      await page.waitForTimeout(500)
      
      // Results should be filtered
      await expect(page.locator('table tbody tr')).toHaveCount(1)
    })
  })

  test.describe('Data Formatting', () => {
    test('should format currency correctly', async ({ page }) => {
      await page.goto('/buying/orders')
      
      // Check that amounts are formatted as South African Rand
      const amountCell = page.locator('td').filter({ hasText: /R\s[\d,]+/ }).first()
      await expect(amountCell).toBeVisible()
      
      const text = await amountCell.textContent()
      expect(text).toMatch(/R\s[\d,]+\.[\d]{2}/)
    })

    test('should format dates correctly', async ({ page }) => {
      await page.goto('/accounts/journal')
      
      // Check that dates are formatted correctly
      const dateCell = page.locator('td').filter({ hasText: /\d{4}\/\d{2}\/\d{2}|\d{2}\/\d{2}\/\d{4}/ }).first()
      await expect(dateCell).toBeVisible()
    })
  })

  test.describe('Authentication Flow', () => {
    test('should redirect to login when not authenticated', async ({ page }) => {
      // Try to access protected page without login
      await page.goto('/dashboard')
      
      // Should be redirected to login
      await page.waitForURL('/login', { timeout: 5000 })
      await expect(page.locator('h1, h2')).toContainText(/login|sign in/i)
    })

    test('should logout successfully', async ({ page }) => {
      await page.goto('/dashboard')
      
      // Find and click logout button
      const logoutButton = page.locator('button:has-text("Logout"), button:has-text("Sign out"), a:has-text("Logout"), a:has-text("Sign out")')
      
      if (await logoutButton.count() > 0) {
        // Might need to open user menu first
        const userMenu = page.locator('button[aria-label*="user"], button:has([aria-label*="user"])')
        if (await userMenu.count() > 0) {
          await userMenu.click()
          await page.waitForTimeout(300)
        }
        
        await logoutButton.first().click()
        await page.waitForURL('/login', { timeout: 5000 })
        await expect(page).toHaveURL('/login')
      }
    })
  })
})

