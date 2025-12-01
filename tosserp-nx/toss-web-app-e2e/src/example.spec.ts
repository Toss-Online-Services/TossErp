import { test, expect } from '@playwright/test';

const baseURL = process.env.BASE_URL || 'http://localhost:4200';

test('app loads and redirects to dashboard', async ({ page }) => {
  await page.goto(`${baseURL}/`, { waitUntil: 'domcontentloaded' });
  
  // Wait for navigation to complete (index redirects to /dashboard)
  // Use a more flexible URL pattern
  await page.waitForURL(url => url.pathname === '/dashboard', { timeout: 15000 });
  
  // Check that the page loaded
  await expect(page.locator('body')).toBeVisible({ timeout: 5000 });
  
  // Check for main layout elements (more flexible)
  const mainContent = page.locator('main, [role="main"], .flex-1');
  await expect(mainContent.first()).toBeVisible({ timeout: 5000 });
});

test('dashboard page is accessible', async ({ page }) => {
  await page.goto(`${baseURL}/dashboard`, { waitUntil: 'domcontentloaded' });
  
  // Wait for page to be fully loaded
  await page.waitForLoadState('domcontentloaded', { timeout: 10000 });
  
  // Verify page structure
  await expect(page.locator('body')).toBeVisible({ timeout: 5000 });
  
  // Check for dashboard content (more flexible)
  const dashboardContent = page.locator('h1, [class*="dashboard"], main');
  await expect(dashboardContent.first()).toBeVisible({ timeout: 5000 });
});
