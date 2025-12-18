import { test, expect } from '@playwright/test';

// Helper to save screenshots to a consistent folder
async function snap(page: any, name: string) {
  await page.screenshot({ path: `screenshots/${name}.png`, fullPage: true });
}

test.describe('UI snapshots', () => {
  test('home page', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');
    await snap(page, 'home');
  });

  test('dashboard layout', async ({ page }) => {
    await page.goto('/dashboard');
    await page.waitForLoadState('networkidle');
    await snap(page, 'dashboard');
  });

  test('selling orders (if accessible)', async ({ page }) => {
    await page.goto('/selling/orders');
    await page.waitForLoadState('networkidle');
    await snap(page, 'selling-orders');
  });
});
