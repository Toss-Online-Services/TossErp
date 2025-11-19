import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

/**
 * Complete end-to-end flow test for Admin role
 */
test.describe('Admin Complete Flow', () => {
  test('complete admin workflow: login -> dashboard -> user management -> order overview', async ({ page }) => {
    const authHelper = new AuthHelper(page);
    
    // Step 1: Login
    await authHelper.loginAs('admin');
    await authHelper.waitForAuth();
    
    // Step 2: View dashboard
    await page.goto('/admin/dashboard');
    await page.waitForSelector('.dashboard, [data-testid="dashboard"]', { timeout: 5000 });
    
    // Verify dashboard statistics
    const metrics = ['retailers', 'suppliers', 'drivers', 'orders', 'sales'];
    for (const metric of metrics) {
      const metricElement = page.locator(`text=/${metric}/i`).first();
      await expect(metricElement).toBeVisible({ timeout: 5000 });
    }
    
    // Step 3: Manage users
    await page.goto('/admin/users');
    await page.waitForSelector('table, .user-list', { timeout: 5000 });
    
    // Search for users
    const searchInput = page.locator('input[type="search"], input[placeholder*="search" i]').first();
    if (await searchInput.isVisible()) {
      await searchInput.fill('admin');
      await page.waitForTimeout(1000);
      
      // Verify search results
      const results = page.locator('table tbody tr, .user-item');
      await expect(results.first()).toBeVisible();
    }
    
    // Filter by role
    const roleFilter = page.locator('select, [data-testid*="role"]').first();
    if (await roleFilter.isVisible()) {
      await roleFilter.selectOption('Retailer');
      await page.waitForTimeout(1000);
    }
    
    // Step 4: View all orders
    await page.goto('/admin/orders');
    await page.waitForSelector('table, .order-list', { timeout: 5000 });
    
    // Filter orders by status
    const statusFilter = page.locator('select, [data-testid*="status"]').first();
    if (await statusFilter.isVisible()) {
      await statusFilter.selectOption('Submitted');
      await page.waitForTimeout(1000);
    }
    
    // View order details
    const firstOrder = page.locator('table tbody tr a, .order-item a').first();
    if (await firstOrder.isVisible()) {
      await firstOrder.click();
      
      await page.waitForURL(/\/admin\/orders\/\d+/, { timeout: 5000 });
      
      // Verify order details
      await expect(page.locator('text=/order|items|total|retailer|supplier/i')).toBeVisible();
    }
  });

  test('admin user management: activate/deactivate and role assignment', async ({ page }) => {
    const authHelper = new AuthHelper(page);
    await authHelper.loginAs('admin');
    await authHelper.waitForAuth();
    
    await page.goto('/admin/users');
    await page.waitForSelector('table, .user-list', { timeout: 5000 });
    
    // Find a user to manage
    const firstUserRow = page.locator('table tbody tr, .user-item').first();
    if (await firstUserRow.isVisible()) {
      // Test activate/deactivate
      const toggleButton = page.locator('button:has-text("Activate"), button:has-text("Deactivate")').first();
      if (await toggleButton.isVisible()) {
        const initialText = await toggleButton.textContent();
        await toggleButton.click();
        await page.waitForTimeout(1000);
        
        // Verify button text changed
        const newText = await toggleButton.textContent();
        expect(newText).not.toBe(initialText);
      }
      
      // Test role editing
      const editButton = page.locator('button:has-text("Edit"), a:has-text("Edit")').first();
      if (await editButton.isVisible()) {
        await editButton.click();
        await page.waitForTimeout(1000);
        
        // Change role if role selector is visible
        const roleSelect = page.locator('select, [data-testid*="role"]').first();
        if (await roleSelect.isVisible()) {
          await roleSelect.selectOption('Supplier');
          
          const saveButton = page.locator('button:has-text("Save"), button:has-text("Update")').first();
          if (await saveButton.isVisible()) {
            await saveButton.click();
            await page.waitForTimeout(1000);
            
            // Verify success
            await expect(page.locator('text=/success|updated/i')).toBeVisible({ timeout: 3000 });
          }
        }
      }
    }
  });
});

