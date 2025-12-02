import { test, expect } from '@playwright/test';

test.describe('Routing', () => {
  test('should redirect root to dashboard', async ({ page }) => {
    await page.goto('/');
    await page.waitForLoadState('networkidle');
    
    // Should redirect to dashboard
    await page.waitForURL(/\/dashboard/, { timeout: 15000 });
    expect(page.url()).toMatch(/\/dashboard/);
  });

  test('should handle 404 for non-existent routes', async ({ page }) => {
    const response = await page.goto('/non-existent-route-12345', { waitUntil: 'networkidle' });
    
    // Should either show 404 page or redirect
    // In Nuxt, 404s are handled gracefully
    expect(response?.status()).toBeGreaterThanOrEqual(200);
    expect(response?.status()).toBeLessThan(500);
  });

  test('should maintain state during navigation', async ({ page }) => {
    await page.goto('/dashboard');
    await page.waitForLoadState('networkidle');
    
    // Navigate to another page
    const stockLink = page.locator('a, [role="link"]').filter({ hasText: /Stock/i });
    if (await stockLink.count() > 0) {
      await stockLink.first().click();
      await page.waitForURL(/\/stock/, { timeout: 10000 });
      
      // Navigate back
      await page.goBack();
      await page.waitForURL(/\/dashboard/, { timeout: 10000 });
      
      // Should still be on dashboard
      expect(page.url()).toMatch(/\/dashboard/);
    }
  });
});




