import { Page, expect } from '@playwright/test';

/**
 * Helper to save screenshots to a consistent folder
 */
export async function snap(page: Page, name: string) {
  await page.screenshot({ 
    path: `tests/screenshots/${name}.png`, 
    fullPage: true 
  });
}

/**
 * Wait for analytics data to load
 */
export async function waitForAnalyticsData(page: Page) {
  // Wait for network to be idle
  await page.waitForLoadState('networkidle');
  
  // Wait for loading indicators to disappear
  await page.waitForSelector('[data-testid="loading"], .loading', { 
    state: 'hidden', 
    timeout: 10000 
  }).catch(() => {
    // If no loading indicator found, that's fine
  });
  
  // Wait for stat cards to appear
  await page.waitForSelector('[data-testid="stat-card"], .stat-card', { 
    timeout: 10000 
  }).catch(() => {
    // Fallback to checking for any data content
  });
}

/**
 * Check if element has active/selected styling
 */
export async function isActive(element: any): Promise<boolean> {
  const classList = await element.getAttribute('class') || '';
  return /active|selected|current|primary/i.test(classList);
}

/**
 * Get stat card by title
 */
export function getStatCard(page: Page, title: string) {
  return page.locator('[data-testid="stat-card"], .stat-card').filter({ 
    hasText: new RegExp(title, 'i') 
  }).first();
}

/**
 * Get chart by title or index
 */
export function getChart(page: Page, index: number = 0) {
  return page.locator('canvas').nth(index);
}

/**
 * Check if offline indicator is visible
 */
export async function hasOfflineIndicator(page: Page): Promise<boolean> {
  const indicator = page.locator('[data-testid="offline-indicator"], [class*="offline"]')
    .filter({ hasText: /offline|no connection/i })
    .first();
  
  try {
    await indicator.waitFor({ state: 'visible', timeout: 2000 });
    return true;
  } catch {
    return false;
  }
}

/**
 * Navigate to analytics with period
 */
export async function navigateToAnalytics(page: Page, period?: string) {
  const url = period 
    ? `/dashboard/analytics?period=${period}` 
    : '/dashboard/analytics';
  await page.goto(url);
  await waitForAnalyticsData(page);
}

/**
 * Click period button and wait for data refresh
 */
export async function selectPeriod(page: Page, period: 'today' | 'week' | 'month' | 'year') {
  const button = page.locator('button').filter({ 
    hasText: new RegExp(period, 'i') 
  }).first();
  
  await button.click();
  await page.waitForTimeout(500); // Wait for data refresh
  await waitForAnalyticsData(page);
}

/**
 * Get all visible stat values
 */
export async function getStatValues(page: Page): Promise<Record<string, string>> {
  const statCards = page.locator('[data-testid="stat-card"], .stat-card');
  const count = await statCards.count();
  const values: Record<string, string> = {};
  
  for (let i = 0; i < count; i++) {
    const card = statCards.nth(i);
    const title = await card.locator('h3, .title, [class*="title"]').first().textContent() || `stat-${i}`;
    const value = await card.locator('.value, [class*="value"], [class*="amount"]').first().textContent() || '0';
    values[title.trim()] = value.trim();
  }
  
  return values;
}

/**
 * Mock analytics API response
 */
export async function mockAnalyticsAPI(page: Page, data?: any) {
  await page.route('**/api/analytics/**', route => {
    route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify(data || {
        kpis: {
          revenue: { value: 50000, change: 12.5, trend: 'up' },
          orders: { value: 1234, change: 8.3, trend: 'up' },
          customers: { value: 856, change: -2.1, trend: 'down' },
          conversionRate: { value: 3.2, change: 1.5, trend: 'up' }
        },
        revenueTrend: {
          labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri'],
          data: [12000, 15000, 13500, 18000, 16500]
        }
      })
    });
  });
}
