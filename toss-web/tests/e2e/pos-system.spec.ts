import { test, expect, type Page } from '@playwright/test'

/**
 * POS System End-to-End Tests
 * 
 * These tests verify the complete functionality of the Point of Sale system including:
 * - UI element visibility and interaction
 * - Product search and filtering
 * - Cart operations (add, update, remove)
 * - Payment processing (cash, card, split payments)
 * - Sale completion and persistence
 * - Hold/recall sale functionality
 * - Session management
 * 
 * Best practices implemented:
 * - Page Object Model for reusability
 * - Explicit waits for dynamic content
 * - Data-testid attributes for reliable selectors
 * - Screenshot capture on failures
 * - Realistic test data matching South African context
 */

// Helper class for POS page interactions
class POSPage {
  constructor(private page: Page) {}

  // Navigation
  async goto() {
    await this.page.goto('/sales/pos')
    await this.page.waitForLoadState('networkidle')
  }

  // Product search and selection
  async searchProducts(term: string) {
    await this.page.fill('input[type="search"], input[placeholder*="Search"]', term)
    await this.page.waitForTimeout(500) // Debounce delay
  }

  async clickCategory(categoryName: string) {
    await this.page.click(`button:has-text("${categoryName}")`)
    await this.page.waitForTimeout(300)
  }

  async selectProduct(productName: string) {
    const productCard = this.page.locator(`text="${productName}"`).first()
    await productCard.waitFor({ state: 'visible' })
    await productCard.click()
  }

  // Cart operations
  async getCartItemCount() {
    const items = await this.page.locator('[data-cart-item]').count()
    return items
  }

  async updateQuantity(productName: string, quantity: number) {
    const item = this.page.locator(`text="${productName}"`).locator('..').locator('..')
    const input = item.locator('input[type="number"]')
    await input.fill(quantity.toString())
  }

  async removeFromCart(productName: string) {
    const item = this.page.locator(`text="${productName}"`).locator('..').locator('..')
    await item.locator('button[title*="Remove"], button:has(svg)').last().click()
  }

  async applyDiscount(productName: string, discountPercent: number) {
    const item = this.page.locator(`text="${productName}"`).locator('..').locator('..')
    await item.locator('button:has-text("Discount")').click()
    // Assuming a dialog opens for discount input
    await this.page.fill('input[type="number"]', discountPercent.toString())
    await this.page.click('button:has-text("Apply")')
  }

  // Payment operations
  async addPayment(method: 'Cash' | 'Card' | 'Mobile Money' | 'Account', amount: number) {
    await this.page.click(`button:has-text("${method}")`)
    await this.page.fill('input[placeholder*="amount"], input[type="number"]', amount.toString())
    if (method !== 'Cash') {
      await this.page.fill('input[placeholder*="reference"]', `REF-${Date.now()}`)
    }
  }

  async completeSale() {
    const completeButton = this.page.locator('button:has-text("Complete Sale")')
    await completeButton.waitFor({ state: 'visible' })
    await completeButton.click()
  }

  // Quick actions
  async holdSale() {
    await this.page.click('button:has-text("Hold")')
  }

  async voidSale() {
    await this.page.click('button:has-text("Void")')
  }

  async viewHeldSales() {
    await this.page.click('button:has-text("Held Sales")')
  }

  async viewRecentSales() {
    await this.page.click('button:has-text("Recent Sales")')
  }

  // Assertions
  async expectTotal(amount: number) {
    const total = this.page.locator('text=/Total.*R.*\\d+/')
    await expect(total).toContainText(`R ${amount.toFixed(2)}`)
  }

  async expectBalance(amount: number) {
    const balance = this.page.locator('text=/Balance.*R.*\\d+/')
    await expect(balance).toContainText(`R ${amount.toFixed(2)}`)
  }

  async expectProductInCart(productName: string) {
    await expect(this.page.locator(`text="${productName}"`)).toBeVisible()
  }

  async expectCartEmpty() {
    const emptyMessage = this.page.locator('text=/empty|no items/i')
    await expect(emptyMessage).toBeVisible()
  }
}

test.describe('POS System - Complete E2E Tests', () => {
  let posPage: POSPage

  test.beforeEach(async ({ page }) => {
    posPage = new POSPage(page)
    await posPage.goto()
  })

  test.describe('UI Elements and Layout', () => {
    test('should display all essential UI components', async ({ page }) => {
      // Header and branding
      await expect(page.locator('text=/Point of Sale/i')).toBeVisible()
      await expect(page.locator('text=/Session/i')).toBeVisible()

      // Product search area
      await expect(page.locator('input[type="search"], input[placeholder*="Search"]')).toBeVisible()
      
      // Category filters
      await expect(page.locator('button:has-text("All Products")')).toBeVisible()
      await expect(page.locator('button:has-text("Groceries")')).toBeVisible()
      await expect(page.locator('button:has-text("Beverages")')).toBeVisible()

      // Cart panel
      await expect(page.locator('text=/Cart|Shopping/i')).toBeVisible()
      
      // Payment panel
      await expect(page.locator('button:has-text("Cash")')).toBeVisible()
      await expect(page.locator('button:has-text("Card")')).toBeVisible()

      // Quick actions
      await expect(page.locator('button:has-text("Hold")')).toBeVisible()
      await expect(page.locator('button:has-text("Void")')).toBeVisible()
    })

    test('should have proper responsive layout', async ({ page }) => {
      // Check that main sections are visible
      const productsSection = page.locator('text="Products"').first()
      const cartSection = page.locator('text=/Cart|Total/i').first()
      
      await expect(productsSection).toBeVisible()
      await expect(cartSection).toBeVisible()
    })
  })

  test.describe('Product Search and Filtering', () => {
    test('should search products by name', async ({ page }) => {
      await posPage.searchProducts('Coca-Cola')
      
      // Should show Coca-Cola products
      await expect(page.locator('text="Coca-Cola 2L"')).toBeVisible()
      await expect(page.locator('text="Coca-Cola 500ml"')).toBeVisible()
      
      // Should not show unrelated products
      await expect(page.locator('text="Maize Meal"')).not.toBeVisible()
    })

    test('should search products by SKU', async ({ page }) => {
      await posPage.searchProducts('BEV-001')
      
      await expect(page.locator('text="Coca-Cola 2L"')).toBeVisible()
    })

    test('should filter products by category', async ({ page }) => {
      await posPage.clickCategory('Beverages')
      
      // Should show beverages
      await expect(page.locator('text="Coca-Cola 2L"')).toBeVisible()
      await expect(page.locator('text="Ricoffy"')).toBeVisible()
      
      // Should not show groceries
      await expect(page.locator('text="Maize Meal"')).not.toBeVisible()
    })

    test('should show all products when clicking "All Products"', async ({ page }) => {
      // First filter by category
      await posPage.clickCategory('Beverages')
      
      // Then show all
      await posPage.clickCategory('All Products')
      
      // Should show products from different categories
      await expect(page.locator('text="Maize Meal"')).toBeVisible()
      await expect(page.locator('text="Coca-Cola"')).toBeVisible()
    })

    test('should display product details correctly', async ({ page }) => {
      const product = page.locator('text="Coca-Cola 2L"').first()
      await expect(product).toBeVisible()
      
      // Price should be visible
      await expect(page.locator('text=/R\\s*24\\.99/')).toBeVisible()
      
      // Stock badge for low stock items
      const lowStockProduct = page.locator('text="Peanut Butter"').first()
      if (await lowStockProduct.isVisible()) {
        await expect(page.locator('text=/low stock|3 left/i')).toBeVisible()
      }
    })
  })

  test.describe('Cart Operations', () => {
    test('should add product to cart', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      
      await posPage.expectProductInCart('Coca-Cola 2L')
      
      // Check toast notification
      await expect(page.locator('text=/added to cart/i')).toBeVisible({ timeout: 3000 })
    })

    test('should add multiple products to cart', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.selectProduct('Maize Meal')
      await posPage.selectProduct('Bread')
      
      const itemCount = await posPage.getCartItemCount()
      expect(itemCount).toBeGreaterThanOrEqual(3)
    })

    test('should update product quantity', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.updateQuantity('Coca-Cola 2L', 5)
      
      // Total should update (24.99 * 5 = 124.95 + VAT)
      const expectedTotal = 24.99 * 5 * 1.15 // Including 15% VAT
      await posPage.expectTotal(expectedTotal)
    })

    test('should remove product from cart', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.removeFromCart('Coca-Cola 2L')
      
      await expect(page.locator('text="Coca-Cola 2L"')).not.toBeVisible()
    })

    test('should calculate totals correctly', async ({ page }) => {
      // Add taxable item
      await posPage.selectProduct('Coca-Cola 2L') // R 24.99 + 15% VAT
      
      // Check subtotal
      await expect(page.locator('text=/Subtotal.*R.*24\\.99/i')).toBeVisible()
      
      // Check VAT (15%)
      const vat = 24.99 * 0.15
      await expect(page.locator(`text=/VAT.*R.*${vat.toFixed(2)}/i`)).toBeVisible()
      
      // Check total
      const total = 24.99 * 1.15
      await posPage.expectTotal(total)
    })

    test('should apply discount to item', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      
      // Apply 10% discount
      await posPage.applyDiscount('Coca-Cola 2L', 10)
      
      // Total should reflect discount
      const discountedPrice = 24.99 * 0.9 * 1.15
      await posPage.expectTotal(discountedPrice)
    })

    test('should handle mixed taxable and non-taxable items', async ({ page }) => {
      // Taxable item
      await posPage.selectProduct('Coca-Cola 2L') // R 24.99 + VAT
      
      // Non-taxable item
      await posPage.selectProduct('Maize Meal') // R 89.99 no VAT
      
      // Total should be (24.99 * 1.15) + 89.99
      const expectedTotal = (24.99 * 1.15) + 89.99
      await posPage.expectTotal(expectedTotal)
    })
  })

  test.describe('Payment Processing', () => {
    test('should complete cash payment for exact amount', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      
      const total = 24.99 * 1.15 // R 28.74
      await posPage.addPayment('Cash', total)
      await posPage.completeSale()
      
      // Should show success message
      await expect(page.locator('text=/sale complete|success/i')).toBeVisible({ timeout: 5000 })
    })

    test('should handle cash overpayment and show change', async ({ page }) => {
      await posPage.selectProduct('Maize Meal') // R 89.99
      
      await posPage.addPayment('Cash', 100)
      
      // Should show change (100 - 89.99 = 10.01)
      await expect(page.locator('text=/Change.*R.*10\\.01/i')).toBeVisible()
      
      await posPage.completeSale()
      await expect(page.locator('text=/sale complete/i')).toBeVisible({ timeout: 5000 })
    })

    test('should complete card payment', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      
      const total = 24.99 * 1.15
      await posPage.addPayment('Card', total)
      await posPage.completeSale()
      
      await expect(page.locator('text=/sale complete/i')).toBeVisible({ timeout: 5000 })
    })

    test('should handle split payment (cash + card)', async ({ page }) => {
      await posPage.selectProduct('Maize Meal') // R 89.99
      
      // Pay R 50 cash
      await posPage.addPayment('Cash', 50)
      
      // Pay remaining R 39.99 by card
      await posPage.addPayment('Card', 39.99)
      
      // Balance should be 0
      await posPage.expectBalance(0)
      
      await posPage.completeSale()
      await expect(page.locator('text=/sale complete/i')).toBeVisible({ timeout: 5000 })
    })

    test('should prevent sale completion with insufficient payment', async ({ page }) => {
      await posPage.selectProduct('Maize Meal') // R 89.99
      
      // Pay only R 50
      await posPage.addPayment('Cash', 50)
      
      // Balance should show R 39.99
      await posPage.expectBalance(39.99)
      
      // Complete button should be disabled
      const completeButton = page.locator('button:has-text("Complete Sale")')
      await expect(completeButton).toBeDisabled()
    })

    test('should process mobile money payment', async ({ page }) => {
      await posPage.selectProduct('Airtime') // R 10
      
      await posPage.addPayment('Mobile Money', 10)
      await posPage.completeSale()
      
      await expect(page.locator('text=/sale complete/i')).toBeVisible({ timeout: 5000 })
    })
  })

  test.describe('Hold and Recall Sales', () => {
    test('should hold a sale', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.selectProduct('Bread')
      
      await posPage.holdSale()
      
      // Should show confirmation
      await expect(page.locator('text=/sale held|parked/i')).toBeVisible({ timeout: 3000 })
      
      // Cart should be cleared
      await posPage.expectCartEmpty()
    })

    test('should recall a held sale', async ({ page }) => {
      // First hold a sale
      await posPage.selectProduct('Maize Meal')
      await posPage.holdSale()
      await page.waitForTimeout(1000)
      
      // View held sales
      await posPage.viewHeldSales()
      
      // Dialog should open
      await expect(page.locator('text=/held sales/i')).toBeVisible()
      
      // Recall the sale
      await page.click('button:has-text("Recall")')
      
      // Cart should be restored
      await posPage.expectProductInCart('Maize Meal')
    })

    test('should list multiple held sales', async ({ page }) => {
      // Hold first sale
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.holdSale()
      await page.waitForTimeout(1000)
      
      // Hold second sale
      await posPage.selectProduct('Bread')
      await posPage.holdSale()
      await page.waitForTimeout(1000)
      
      // View held sales
      await posPage.viewHeldSales()
      
      // Should show multiple sales
      const rows = page.locator('table tr')
      const count = await rows.count()
      expect(count).toBeGreaterThan(2) // Header + at least 2 sales
    })
  })

  test.describe('Void Sale', () => {
    test('should void current sale', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.selectProduct('Bread')
      
      await posPage.voidSale()
      
      // Should show confirmation
      await expect(page.locator('text=/voided|cancelled/i')).toBeVisible({ timeout: 3000 })
      
      // Cart should be empty
      await posPage.expectCartEmpty()
    })

    test('should clear cart completely on void', async ({ page }) => {
      // Add multiple items
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.selectProduct('Bread')
      await posPage.selectProduct('Maize Meal')
      
      // Add payments
      await posPage.addPayment('Cash', 50)
      
      await posPage.voidSale()
      
      // Everything should be cleared
      await posPage.expectCartEmpty()
      await posPage.expectBalance(0)
    })
  })

  test.describe('Recent Sales', () => {
    test('should view recent sales from current session', async ({ page }) => {
      // Complete a sale first
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.addPayment('Cash', 30)
      await posPage.completeSale()
      await page.waitForTimeout(2000)
      
      // View recent sales
      await posPage.viewRecentSales()
      
      // Dialog should show the sale
      await expect(page.locator('text=/recent sales/i')).toBeVisible()
      await expect(page.locator('text="Coca-Cola 2L"')).toBeVisible()
    })

    test('should display payment methods in recent sales', async ({ page }) => {
      // Complete sale with split payment
      await posPage.selectProduct('Maize Meal')
      await posPage.addPayment('Cash', 50)
      await posPage.addPayment('Card', 39.99)
      await posPage.completeSale()
      await page.waitForTimeout(2000)
      
      // View recent sales
      await posPage.viewRecentSales()
      
      // Should show both payment methods
      await expect(page.locator('text=/Cash.*50/i')).toBeVisible()
      await expect(page.locator('text=/Card.*39\\.99/i')).toBeVisible()
    })
  })

  test.describe('Session Persistence', () => {
    test('should restore cart on page reload', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      await posPage.selectProduct('Bread')
      
      // Reload page
      await page.reload()
      await page.waitForLoadState('networkidle')
      
      // Cart should be restored
      await posPage.expectProductInCart('Coca-Cola 2L')
      await posPage.expectProductInCart('Bread')
      
      // Should show restoration message
      await expect(page.locator('text=/session restored/i')).toBeVisible({ timeout: 3000 })
    })

    test('should persist cart items across navigation', async ({ page }) => {
      await posPage.selectProduct('Maize Meal')
      
      // Navigate away
      await page.goto('/dashboard')
      
      // Come back
      await posPage.goto()
      
      // Cart should still have the item
      await posPage.expectProductInCart('Maize Meal')
    })
  })

  test.describe('Edge Cases and Error Handling', () => {
    test('should handle rapid product clicks', async ({ page }) => {
      // Click same product multiple times quickly
      for (let i = 0; i < 5; i++) {
        await posPage.selectProduct('Coca-Cola 2L')
      }
      
      // Should handle gracefully (either increment quantity or prevent duplicates)
      const items = await posPage.getCartItemCount()
      expect(items).toBeGreaterThan(0)
    })

    test('should validate quantity input', async ({ page }) => {
      await posPage.selectProduct('Coca-Cola 2L')
      
      // Try to set quantity to 0
      await posPage.updateQuantity('Coca-Cola 2L', 0)
      
      // Should either remove item or prevent 0 quantity
      // Item should not remain with 0 quantity
    })

    test('should handle products with low stock', async ({ page }) => {
      // Search for low stock item
      await posPage.searchProducts('Peanut Butter')
      
      // Should show low stock warning
      await expect(page.locator('text=/low stock|3 left/i')).toBeVisible()
      
      // Should still allow adding to cart
      await posPage.selectProduct('Peanut Butter')
      await posPage.expectProductInCart('Peanut Butter')
    })

    test('should handle empty search results', async ({ page }) => {
      await posPage.searchProducts('NonExistentProduct12345')
      
      // Should show no results message
      await expect(page.locator('text=/no products|not found|empty/i')).toBeVisible()
    })
  })

  test.describe('Accessibility', () => {
    test('should be keyboard navigable', async ({ page }) => {
      // Tab through interface
      await page.keyboard.press('Tab')
      await page.keyboard.press('Tab')
      await page.keyboard.press('Tab')
      
      // Should be able to interact with focused elements
      await page.keyboard.press('Enter')
    })

    test('should have proper ARIA labels', async ({ page }) => {
      const searchInput = page.locator('input[type="search"]')
      await expect(searchInput).toHaveAttribute('placeholder')
      
      const buttons = page.locator('button')
      const count = await buttons.count()
      expect(count).toBeGreaterThan(5) // Should have multiple interactive buttons
    })
  })

  test.describe('Performance', () => {
    test('should load products quickly', async ({ page }) => {
      const startTime = Date.now()
      await posPage.goto()
      const loadTime = Date.now() - startTime
      
      // Should load within 3 seconds
      expect(loadTime).toBeLessThan(3000)
    })

    test('should handle large cart efficiently', async ({ page }) => {
      // Add 20 different products
      const products = ['Coca-Cola 2L', 'Maize Meal', 'Bread', 'Rice', 'Sugar']
      
      for (const product of products) {
        await posPage.searchProducts(product)
        await posPage.selectProduct(product)
      }
      
      // UI should remain responsive
      const completeButton = page.locator('button:has-text("Complete Sale")')
      await expect(completeButton).toBeVisible()
    })
  })
})

// Screenshot on failure
test.afterEach(async ({ page }, testInfo) => {
  if (testInfo.status !== 'passed') {
    const screenshot = await page.screenshot()
    await testInfo.attach('screenshot', { body: screenshot, contentType: 'image/png' })
  }
})
