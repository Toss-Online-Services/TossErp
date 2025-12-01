import { test, expect } from '@playwright/test';

test('app loads and redirects to dashboard', async ({ page }) => {
  await page.goto('/');
  
  // Wait for navigation to complete (index redirects to /dashboard)
  await page.waitForURL('**/dashboard', { timeout: 10000 });
  
  // Check that the page loaded (look for TOSS branding or layout elements)
  const body = page.locator('body');
  await expect(body).toBeVisible();
  
  // Check for main layout elements
  const mainContent = page.locator('main, [role="main"]');
  await expect(mainContent.first()).toBeVisible({ timeout: 5000 });
});

test('dashboard page is accessible', async ({ page }) => {
  await page.goto('/dashboard');
  
  // Wait for page to be fully loaded
  await page.waitForLoadState('networkidle', { timeout: 10000 });
  
  // Verify page structure
  const body = page.locator('body');
  await expect(body).toBeVisible();
});
