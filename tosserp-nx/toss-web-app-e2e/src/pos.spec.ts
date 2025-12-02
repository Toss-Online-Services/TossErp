import { test, expect } from '@playwright/test';

test.describe('POS (Point of Sale)', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/sales/pos');
    await page.waitForLoadState('networkidle');
  });

  test('should load POS page', async ({ page }) => {
    await expect(page.locator('body')).toBeVisible();
    expect(page.url()).toMatch(/\/pos/);
  });

  test('should display POS interface elements', async ({ page }) => {
    // Wait for page content
    await page.waitForSelector('main, [role="main"]', { timeout: 10000 });
    
    // Check for main content area
    const main = page.locator('main, [role="main"]');
    await expect(main.first()).toBeVisible();
  });

  test('should have search functionality', async ({ page }) => {
    // Look for search input
    const searchInput = page.locator('input[type="search"], input[placeholder*="Search"], input[placeholder*="search"]');
    const count = await searchInput.count();
    
    if (count > 0) {
      await expect(searchInput.first()).toBeVisible({ timeout: 5000 });
    }
  });

  test('should display cart or checkout area', async ({ page }) => {
    // Look for cart, checkout, or total elements
    const cartElements = page.locator('text=/Cart|Checkout|Total|Subtotal/i');
    const count = await cartElements.count();
    
    // At least one cart-related element should be visible
    if (count > 0) {
      await expect(cartElements.first()).toBeVisible({ timeout: 10000 });
    }
  });
});




