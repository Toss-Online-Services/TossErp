import { test, expect } from '@playwright/test';

test.describe('Navigation', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/dashboard');
    await page.waitForLoadState('networkidle');
  });

  test('should have sidebar navigation', async ({ page }) => {
    const sidebar = page.locator('aside, [role="aside"]');
    await expect(sidebar.first()).toBeVisible({ timeout: 10000 });
  });

  test('should navigate to Sales page', async ({ page }) => {
    // Find and click Sales link
    const salesLink = page.locator('a, [role="link"]').filter({ hasText: /Sales|Sale/i });
    await expect(salesLink.first()).toBeVisible({ timeout: 10000 });
    await salesLink.first().click();
    
    // Wait for navigation
    await page.waitForURL(/\/sales|\/pos/, { timeout: 10000 });
    await page.waitForLoadState('networkidle');
    
    // Verify we're on a sales-related page
    expect(page.url()).toMatch(/\/sales|\/pos/);
  });

  test('should navigate to Stock page', async ({ page }) => {
    const stockLink = page.locator('a, [role="link"]').filter({ hasText: /Stock/i });
    await expect(stockLink.first()).toBeVisible({ timeout: 10000 });
    await stockLink.first().click();
    
    await page.waitForURL(/\/stock/, { timeout: 10000 });
    await page.waitForLoadState('networkidle');
    expect(page.url()).toMatch(/\/stock/);
  });

  test('should navigate to Money page', async ({ page }) => {
    const moneyLink = page.locator('a, [role="link"]').filter({ hasText: /Money/i });
    await expect(moneyLink.first()).toBeVisible({ timeout: 10000 });
    await moneyLink.first().click();
    
    await page.waitForURL(/\/money/, { timeout: 10000 });
    await page.waitForLoadState('networkidle');
    expect(page.url()).toMatch(/\/money/);
  });

  test('should navigate to People page', async ({ page }) => {
    const peopleLink = page.locator('a, [role="link"]').filter({ hasText: /People/i });
    await expect(peopleLink.first()).toBeVisible({ timeout: 10000 });
    await peopleLink.first().click();
    
    await page.waitForURL(/\/people/, { timeout: 10000 });
    await page.waitForLoadState('networkidle');
    expect(page.url()).toMatch(/\/people/);
  });

  test('should navigate to Jobs page', async ({ page }) => {
    const jobsLink = page.locator('a, [role="link"]').filter({ hasText: /Jobs|Job/i });
    await expect(jobsLink.first()).toBeVisible({ timeout: 10000 });
    await jobsLink.first().click();
    
    await page.waitForURL(/\/jobs/, { timeout: 10000 });
    await page.waitForLoadState('networkidle');
    expect(page.url()).toMatch(/\/jobs/);
  });

  test('should navigate back to Dashboard', async ({ page }) => {
    // Navigate away first
    const stockLink = page.locator('a, [role="link"]').filter({ hasText: /Stock/i });
    await stockLink.first().click();
    await page.waitForURL(/\/stock/, { timeout: 10000 });
    
    // Navigate back to dashboard
    const homeLink = page.locator('a, [role="link"]').filter({ hasText: /Home|Dashboard/i });
    await expect(homeLink.first()).toBeVisible({ timeout: 10000 });
    await homeLink.first().click();
    
    await page.waitForURL(/\/dashboard/, { timeout: 10000 });
    expect(page.url()).toMatch(/\/dashboard/);
  });

  test('should toggle sidebar on mobile', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.reload();
    await page.waitForLoadState('networkidle');
    
    // Look for toggle button
    const toggleButton = page.locator('button').filter({ hasText: /Toggle|Menu/i });
    if (await toggleButton.count() > 0) {
      await toggleButton.first().click();
      await page.waitForTimeout(500);
      
      // Sidebar should be visible after toggle
      const sidebar = page.locator('aside, [role="aside"]');
      await expect(sidebar.first()).toBeVisible({ timeout: 5000 });
    }
  });
});




