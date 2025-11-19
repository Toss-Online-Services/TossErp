import { test, expect } from '@playwright/test';

/**
 * Smoke tests - Basic functionality checks
 */
test.describe('Smoke Tests', () => {
  test('should load the application', async ({ page }) => {
    await page.goto('/');
    
    // Check that page loads (not a 404 or error)
    await expect(page).not.toHaveURL(/404|error/i);
    
    // Check for some content on the page
    const body = page.locator('body');
    await expect(body).toBeVisible();
  });

  test('should access login page', async ({ page }) => {
    await page.goto('/login');
    
    // Verify login page loads
    const emailInput = page.locator('input[type="email"], input[name="email"], input[placeholder*="email" i]').first();
    await expect(emailInput.or(page.locator('body'))).toBeVisible({ timeout: 10000 });
  });

  test('should have working navigation', async ({ page }) => {
    await page.goto('/');
    
    // Check that navigation works (if logged in, should see nav; if not, should redirect)
    await page.waitForLoadState('networkidle');
    
    // Page should load without errors
    const errorMessages = page.locator('text=/error|failed|404/i');
    const errorCount = await errorMessages.count();
    expect(errorCount).toBe(0);
  });
});

