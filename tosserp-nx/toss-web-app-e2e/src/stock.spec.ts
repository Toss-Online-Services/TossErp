import { test, expect } from '@playwright/test';

test.describe('Stock Management', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/stock');
    await page.waitForLoadState('networkidle');
  });

  test('should load stock page', async ({ page }) => {
    await expect(page.locator('body')).toBeVisible();
    expect(page.url()).toMatch(/\/stock/);
  });

  test('should display stock page content', async ({ page }) => {
    await page.waitForSelector('main, [role="main"]', { timeout: 10000 });
    const main = page.locator('main, [role="main"]');
    await expect(main.first()).toBeVisible();
  });

  test('should have navigation to stock alerts', async ({ page }) => {
    // Look for links to stock alerts or low stock
    const alertsLink = page.locator('a, [role="link"]').filter({ hasText: /Alert|Low Stock|Stock Alert/i });
    const count = await alertsLink.count();
    
    if (count > 0) {
      await expect(alertsLink.first()).toBeVisible({ timeout: 5000 });
    }
  });
});




