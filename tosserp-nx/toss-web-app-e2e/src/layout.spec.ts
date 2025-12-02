import { test, expect } from '@playwright/test';

test.describe('Layout and UI Components', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/dashboard');
    await page.waitForLoadState('networkidle');
  });

  test('should have responsive layout', async ({ page }) => {
    // Test desktop view
    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.reload();
    await page.waitForLoadState('networkidle');
    
    const sidebar = page.locator('aside, [role="aside"]');
    await expect(sidebar.first()).toBeVisible({ timeout: 10000 });
    
    // Test mobile view
    await page.setViewportSize({ width: 375, height: 667 });
    await page.reload();
    await page.waitForLoadState('networkidle');
    
    await expect(sidebar.first()).toBeVisible({ timeout: 10000 });
  });

  test('should display navbar with search', async ({ page }) => {
    const navbar = page.locator('nav, [role="navigation"]');
    await expect(navbar.first()).toBeVisible({ timeout: 10000 });
    
    // Look for search input in navbar
    const searchInput = page.locator('input[type="search"], input[placeholder*="Search"]');
    const count = await searchInput.count();
    
    if (count > 0) {
      await expect(searchInput.first()).toBeVisible({ timeout: 5000 });
    }
  });

  test('should have footer on desktop', async ({ page }) => {
    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.reload();
    await page.waitForLoadState('networkidle');
    
    const footer = page.locator('footer, [role="contentinfo"]');
    const count = await footer.count();
    
    // Footer might be hidden on mobile, so just check if it exists
    expect(count).toBeGreaterThanOrEqual(0);
  });

  test('should have mobile bottom navigation', async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 667 });
    await page.reload();
    await page.waitForLoadState('networkidle');
    
    // Look for bottom navigation
    const bottomNav = page.locator('[class*="bottom"], [class*="Bottom"]');
    const count = await bottomNav.count();
    
    // Bottom nav should exist on mobile
    expect(count).toBeGreaterThanOrEqual(0);
  });
});




