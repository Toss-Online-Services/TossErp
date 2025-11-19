import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';
import { TEST_USERS } from '../helpers/test-data';

test.describe('Admin Module', () => {
  let authHelper: AuthHelper;

  test.beforeEach(async ({ page }) => {
    authHelper = new AuthHelper(page);
    await authHelper.loginAs('admin');
    await authHelper.waitForAuth();
  });

  test.describe('Dashboard', () => {
    test('should display admin dashboard', async ({ page }) => {
      await page.goto('/admin/dashboard');
      
      // Check for dashboard elements
      await expect(page.locator('text=/dashboard|overview|statistics/i')).toBeVisible();
      
      // Check for key metrics
      const metrics = ['retailers', 'suppliers', 'drivers', 'orders', 'sales'];
      for (const metric of metrics) {
        const metricElement = page.locator(`text=/${metric}/i`).first();
        await expect(metricElement).toBeVisible({ timeout: 5000 });
      }
    });

    test('should display statistics cards', async ({ page }) => {
      await page.goto('/admin/dashboard');
      
      // Wait for stats to load
      await page.waitForSelector('.stat-card, [data-testid*="stat"], .metric', { timeout: 10000 });
      
      // Verify at least one stat card is visible
      const statCards = page.locator('.stat-card, [data-testid*="stat"], .metric');
      await expect(statCards.first()).toBeVisible();
    });
  });

  test.describe('User Management', () => {
    test('should display user list', async ({ page }) => {
      await page.goto('/admin/users');
      
      // Check for user list table or grid
      await expect(
        page.locator('table, .user-list, [data-testid="user-list"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should filter users by role', async ({ page }) => {
      await page.goto('/admin/users');
      
      // Wait for page to load
      await page.waitForSelector('select, [data-testid*="filter"], .filter', { timeout: 5000 });
      
      // Find and select role filter
      const roleFilter = page.locator('select, [data-testid*="role-filter"]').first();
      if (await roleFilter.isVisible()) {
        await roleFilter.selectOption('Retailer');
        
        // Wait for filtered results
        await page.waitForTimeout(1000);
        
        // Verify filter is applied (check URL or visible users)
        const url = page.url();
        expect(url).toMatch(/role|filter/i);
      }
    });

    test('should search users', async ({ page }) => {
      await page.goto('/admin/users');
      
      // Find search input
      const searchInput = page.locator('input[type="search"], input[placeholder*="search" i], [data-testid*="search"]').first();
      if (await searchInput.isVisible()) {
        await searchInput.fill('admin');
        await page.waitForTimeout(1000);
        
        // Verify search results
        const results = page.locator('table tbody tr, .user-item, [data-testid*="user"]');
        await expect(results.first()).toBeVisible();
      }
    });

    test('should activate/deactivate users', async ({ page }) => {
      await page.goto('/admin/users');
      
      // Wait for user list
      await page.waitForSelector('table, .user-list', { timeout: 5000 });
      
      // Find activate/deactivate button for first user
      const toggleButton = page.locator('button:has-text("Activate"), button:has-text("Deactivate"), [data-testid*="toggle"]').first();
      if (await toggleButton.isVisible()) {
        const buttonText = await toggleButton.textContent();
        await toggleButton.click();
        
        // Wait for update
        await page.waitForTimeout(1000);
        
        // Verify button text changed or status updated
        if (buttonText?.includes('Activate')) {
          await expect(page.locator('button:has-text("Deactivate")').first()).toBeVisible({ timeout: 2000 });
        }
      }
    });

    test('should edit user roles', async ({ page }) => {
      await page.goto('/admin/users');
      
      // Wait for user list
      await page.waitForSelector('table, .user-list', { timeout: 5000 });
      
      // Find edit button for first user
      const editButton = page.locator('button:has-text("Edit"), a:has-text("Edit"), [data-testid*="edit"]').first();
      if (await editButton.isVisible()) {
        await editButton.click();
        
        // Wait for edit modal or page
        await page.waitForSelector('select, [data-testid*="role"], .role-selector', { timeout: 5000 });
        
        // Select a role
        const roleSelect = page.locator('select, [data-testid*="role"]').first();
        if (await roleSelect.isVisible()) {
          await roleSelect.selectOption('Supplier');
          
          // Save changes
          const saveButton = page.locator('button:has-text("Save"), button:has-text("Update")').first();
          if (await saveButton.isVisible()) {
            await saveButton.click();
            await page.waitForTimeout(1000);
            
            // Verify success message or redirect
            await expect(
              page.locator('text=/success|updated|saved/i')
            ).toBeVisible({ timeout: 3000 });
          }
        }
      }
    });
  });

  test.describe('Order Management', () => {
    test('should display all orders', async ({ page }) => {
      await page.goto('/admin/orders');
      
      // Check for orders list
      await expect(
        page.locator('table, .order-list, [data-testid="order-list"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should filter orders by status', async ({ page }) => {
      await page.goto('/admin/orders');
      
      // Wait for page to load
      await page.waitForSelector('select, [data-testid*="filter"]', { timeout: 5000 });
      
      // Find status filter
      const statusFilter = page.locator('select, [data-testid*="status"]').first();
      if (await statusFilter.isVisible()) {
        await statusFilter.selectOption('Submitted');
        await page.waitForTimeout(1000);
        
        // Verify filter is applied
        const url = page.url();
        expect(url).toMatch(/status|filter/i);
      }
    });

    test('should view order details', async ({ page }) => {
      await page.goto('/admin/orders');
      
      // Wait for orders list
      await page.waitForSelector('table tbody tr, .order-item', { timeout: 5000 });
      
      // Click on first order
      const firstOrder = page.locator('table tbody tr a, .order-item a, [data-testid*="order"]').first();
      if (await firstOrder.isVisible()) {
        await firstOrder.click();
        
        // Verify order details page
        await expect(page).toHaveURL(/\/admin\/orders\/\d+/);
        await expect(page.locator('text=/order|details|items/i')).toBeVisible();
      }
    });
  });
});

