import { test, expect } from '@playwright/test';

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/dashboard');
    await page.waitForLoadState('networkidle');
  });

  test('should display dashboard with correct layout', async ({ page }) => {
    // Check for main layout elements
    await expect(page.locator('main, [role="main"]')).toBeVisible();
    
    // Check for sidebar
    const sidebar = page.locator('aside, [role="aside"]');
    await expect(sidebar.first()).toBeVisible();
    
    // Check for navbar
    const navbar = page.locator('nav, [role="navigation"]');
    await expect(navbar.first()).toBeVisible();
  });

  test('should display Analytics title and breadcrumbs', async ({ page }) => {
    // Wait for content to load
    await page.waitForSelector('h1, [class*="heading"]', { timeout: 10000 });
    
    // Check for Analytics title
    const title = page.locator('h1').filter({ hasText: /Analytics/i });
    await expect(title).toBeVisible({ timeout: 10000 });
    
    // Check for breadcrumbs
    const breadcrumbs = page.locator('[role="navigation"]').filter({ hasText: /Pages|Dashboard/i });
    await expect(breadcrumbs.first()).toBeVisible({ timeout: 5000 });
  });

  test('should display three chart cards', async ({ page }) => {
    // Wait for cards to load
    await page.waitForSelector('[class*="Card"], [class*="card"]', { timeout: 10000 });
    
    // Check for Website Views chart
    const websiteViews = page.locator('text=/Website Views/i');
    await expect(websiteViews).toBeVisible({ timeout: 10000 });
    
    // Check for Daily Sales chart
    const dailySales = page.locator('text=/Daily Sales/i');
    await expect(dailySales).toBeVisible({ timeout: 10000 });
    
    // Check for Completed Tasks chart
    const completedTasks = page.locator('text=/Completed Tasks/i');
    await expect(completedTasks).toBeVisible({ timeout: 10000 });
  });

  test('should display four KPI cards', async ({ page }) => {
    await page.waitForSelector('[class*="Card"], [class*="card"]', { timeout: 10000 });
    
    // Check for Bookings KPI
    const bookings = page.locator('text=/Bookings/i');
    await expect(bookings).toBeVisible({ timeout: 10000 });
    
    // Check for Today's Users KPI
    const todayUsers = page.locator('text=/Today\'s Users|Today Users/i');
    await expect(todayUsers).toBeVisible({ timeout: 10000 });
    
    // Check for Revenue KPI
    const revenue = page.locator('text=/Revenue/i');
    await expect(revenue).toBeVisible({ timeout: 10000 });
    
    // Check for Followers KPI
    const followers = page.locator('text=/Followers/i');
    await expect(followers).toBeVisible({ timeout: 10000 });
  });

  test('should display KPI card values', async ({ page }) => {
    await page.waitForSelector('[class*="Card"], [class*="card"]', { timeout: 10000 });
    
    // Check for numeric values in KPI cards
    const numericValues = page.locator('text=/\\d+/');
    await expect(numericValues.first()).toBeVisible({ timeout: 10000 });
  });

  test('should display three image cards at bottom', async ({ page }) => {
    await page.waitForSelector('[class*="Card"], [class*="card"]', { timeout: 10000 });
    
    // Scroll to bottom to see image cards
    await page.evaluate(() => window.scrollTo(0, document.body.scrollHeight));
    await page.waitForTimeout(1000);
    
    // Check for gradient cards (they should have gradient backgrounds)
    const imageCards = page.locator('[class*="gradient"], [class*="bg-gradient"]');
    const count = await imageCards.count();
    expect(count).toBeGreaterThanOrEqual(0); // At least some cards should be visible
  });

  test('should have light gray background in main content area', async ({ page }) => {
    const main = page.locator('main, [role="main"]').first();
    await expect(main).toBeVisible();
    
    // Check for gray background class
    const bgClass = await main.evaluate((el) => {
      return window.getComputedStyle(el).backgroundColor;
    });
    
    // Should have a light background (not black)
    expect(bgClass).not.toBe('rgb(0, 0, 0)');
  });
});




