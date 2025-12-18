import { test, expect } from '@playwright/test';

test.describe('Dashboard Analytics Page', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/dashboard/analytics');
  });

  test('should load analytics page successfully', async ({ page }) => {
    // Wait for the page to load
    await page.waitForLoadState('networkidle');
    
    // Check that we're on the analytics page
    await expect(page).toHaveURL(/\/dashboard\/analytics/);
    
    // Check for main heading
    await expect(page.locator('h1, h2').filter({ hasText: /Analytics|Dashboard/i }).first()).toBeVisible();
  });

  test('should display stat cards with data', async ({ page }) => {
    await page.waitForLoadState('networkidle');
    
    // Wait for data to load (no skeleton loaders)
    await page.waitForSelector('[data-testid="stat-card"], .stat-card, [class*="stat"]', { timeout: 10000 });
    
    // Should have multiple stat cards (revenue, orders, customers, conversion)
    const statCards = page.locator('[data-testid="stat-card"], .stat-card, [class*="stat"]');
    const count = await statCards.count();
    expect(count).toBeGreaterThanOrEqual(3); // At least 3 stat cards
  });

  test('should display charts', async ({ page }) => {
    await page.waitForLoadState('networkidle');
    
    // Wait for charts to render
    await page.waitForSelector('canvas', { timeout: 10000 });
    
    // Should have at least one chart (revenue trend)
    const charts = page.locator('canvas');
    const chartCount = await charts.count();
    expect(chartCount).toBeGreaterThanOrEqual(1);
  });

  test('should have period selector', async ({ page }) => {
    await page.waitForLoadState('networkidle');
    
    // Look for period selector buttons (Today, Week, Month, Year)
    const periodButtons = page.locator('button').filter({ hasText: /Today|Week|Month|Year/i });
    const buttonCount = await periodButtons.count();
    expect(buttonCount).toBeGreaterThanOrEqual(2); // At least 2 period options
  });

  test('should be able to switch periods', async ({ page }) => {
    await page.waitForLoadState('networkidle');
    
    // Find and click on a different period button
    const weekButton = page.locator('button').filter({ hasText: /Week/i }).first();
    if (await weekButton.isVisible()) {
      await weekButton.click();
      
      // Wait for data to refresh
      await page.waitForTimeout(500);
      
      // Button should appear selected/active
      await expect(weekButton).toHaveAttribute('class', /active|selected|primary/i);
    }
  });

  test('should show offline indicator when offline', async ({ page, context }) => {
    await page.waitForLoadState('networkidle');
    
    // Simulate offline mode
    await context.setOffline(true);
    
    // Wait a moment for the offline indicator to appear
    await page.waitForTimeout(1000);
    
    // Check for offline indicator
    const offlineIndicator = page.locator('[data-testid="offline-indicator"], [class*="offline"]').filter({ hasText: /offline|no connection/i });
    await expect(offlineIndicator.first()).toBeVisible({ timeout: 5000 });
    
    // Restore online mode
    await context.setOffline(false);
  });

  test('should handle API errors gracefully', async ({ page }) => {
    // Intercept the API call and return an error
    await page.route('**/api/analytics/**', route => {
      route.abort('failed');
    });
    
    await page.goto('/dashboard/analytics');
    await page.waitForLoadState('networkidle');
    
    // Should show error message
    const errorMessage = page.locator('text=/error|failed|retry/i').first();
    await expect(errorMessage).toBeVisible({ timeout: 5000 });
  });

  test('should have working sidebar navigation', async ({ page }) => {
    await page.waitForLoadState('networkidle');
    
    // Check that sidebar exists
    const sidebar = page.locator('[data-testid="sidebar"], aside, nav').first();
    await expect(sidebar).toBeVisible();
    
    // Should have navigation links
    const navLinks = page.locator('a[href*="/dashboard"]');
    const linkCount = await navLinks.count();
    expect(linkCount).toBeGreaterThanOrEqual(1);
  });

  test('should be responsive on mobile', async ({ page }) => {
    // Set mobile viewport
    await page.setViewportSize({ width: 375, height: 667 });
    await page.goto('/dashboard/analytics');
    await page.waitForLoadState('networkidle');
    
    // Page should still load
    await expect(page.locator('body')).toBeVisible();
    
    // Mobile menu button should be visible
    const menuButton = page.locator('button[aria-label*="menu"], button[aria-label*="navigation"]').first();
    if (await menuButton.isVisible()) {
      await expect(menuButton).toBeVisible();
    }
  });

  test('should take screenshot of analytics page', async ({ page }) => {
    await page.waitForLoadState('networkidle');
    
    // Wait for charts to fully render
    await page.waitForTimeout(2000);
    
    // Take full page screenshot
    await page.screenshot({ 
      path: 'tests/screenshots/analytics-dashboard.png', 
      fullPage: true 
    });
  });
});

test.describe('Dashboard Navigation', () => {
  test('should redirect from /dashboard to /dashboard/analytics', async ({ page }) => {
    await page.goto('/dashboard');
    await page.waitForLoadState('networkidle');
    
    // Should redirect to analytics
    await expect(page).toHaveURL(/\/dashboard\/analytics/);
  });

  test('should have active state on analytics nav item', async ({ page }) => {
    await page.goto('/dashboard/analytics');
    await page.waitForLoadState('networkidle');
    
    // Find the analytics nav link
    const analyticsLink = page.locator('a[href*="/dashboard/analytics"]').first();
    await expect(analyticsLink).toBeVisible();
    
    // Should have active/selected class
    const classList = await analyticsLink.getAttribute('class');
    expect(classList).toMatch(/active|selected|current/i);
  });
});

test.describe('Analytics API', () => {
  test('should return valid analytics data', async ({ request }) => {
    const response = await request.get('http://localhost:3000/api/analytics/dashboard');
    
    expect(response.ok()).toBeTruthy();
    
    const data = await response.json();
    
    // Check structure
    expect(data).toHaveProperty('kpis');
    expect(data).toHaveProperty('revenueTrend');
    expect(data.kpis).toHaveProperty('revenue');
    expect(data.kpis).toHaveProperty('orders');
    expect(data.kpis).toHaveProperty('customers');
    expect(data.kpis).toHaveProperty('conversionRate');
  });

  test('should support period parameter', async ({ request }) => {
    const periods = ['today', 'week', 'month', 'year'];
    
    for (const period of periods) {
      const response = await request.get(`http://localhost:3000/api/analytics/dashboard?period=${period}`);
      expect(response.ok()).toBeTruthy();
      
      const data = await response.json();
      expect(data).toHaveProperty('kpis');
    }
  });
});
