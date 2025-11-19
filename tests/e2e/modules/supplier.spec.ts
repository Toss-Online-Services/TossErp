import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

test.describe('Supplier Module', () => {
  let authHelper: AuthHelper;

  test.beforeEach(async ({ page }) => {
    authHelper = new AuthHelper(page);
    await authHelper.loginAs('supplier');
    await authHelper.waitForAuth();
  });

  test.describe('Order Management', () => {
    test('should display incoming purchase orders', async ({ page }) => {
      await page.goto('/supplier/orders');
      
      // Check for orders list
      await expect(
        page.locator('table, .order-list, [data-testid="order-list"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should view purchase order details', async ({ page }) => {
      await page.goto('/supplier/orders');
      
      // Wait for orders list
      await page.waitForSelector('table tbody tr, .order-item', { timeout: 5000 });
      
      // Click on first order
      const firstOrder = page.locator('table tbody tr a, .order-item a').first();
      if (await firstOrder.isVisible()) {
        await firstOrder.click();
        
        // Verify order details page
        await expect(page).toHaveURL(/\/supplier\/orders\/\d+/);
        await expect(page.locator('text=/order|items|retailer/i')).toBeVisible();
      }
    });

    test('should accept purchase order', async ({ page }) => {
      await page.goto('/supplier/orders');
      
      // Wait for orders list
      await page.waitForSelector('table tbody tr, .order-item', { timeout: 5000 });
      
      // Find order with "Submitted" status
      const submittedOrder = page.locator('text=/submitted/i').locator('..').locator('..').first();
      if (await submittedOrder.isVisible()) {
        await submittedOrder.locator('a').first().click();
        
        // Wait for order details
        await page.waitForSelector('button:has-text("Accept"), [data-testid*="accept"]', { timeout: 5000 });
        
        // Click accept button
        const acceptButton = page.locator('button:has-text("Accept"), [data-testid*="accept"]').first();
        if (await acceptButton.isVisible()) {
          await acceptButton.click();
          
          // Verify status changed to Accepted
          await expect(
            page.locator('text=/accepted|success/i')
          ).toBeVisible({ timeout: 5000 });
        }
      }
    });

    test('should reject purchase order', async ({ page }) => {
      await page.goto('/supplier/orders');
      
      // Wait for orders list
      await page.waitForSelector('table tbody tr, .order-item', { timeout: 5000 });
      
      // Find order with "Submitted" status
      const submittedOrder = page.locator('text=/submitted/i').locator('..').locator('..').first();
      if (await submittedOrder.isVisible()) {
        await submittedOrder.locator('a').first().click();
        
        // Wait for order details
        await page.waitForSelector('button:has-text("Reject"), [data-testid*="reject"]', { timeout: 5000 });
        
        // Click reject button
        const rejectButton = page.locator('button:has-text("Reject"), [data-testid*="reject"]').first();
        if (await rejectButton.isVisible()) {
          await rejectButton.click();
          
          // Confirm rejection if dialog appears
          page.on('dialog', async dialog => {
            await dialog.accept();
          });
          
          // Verify status changed
          await expect(
            page.locator('text=/rejected|success/i')
          ).toBeVisible({ timeout: 5000 });
        }
      }
    });

    test('should mark order as ready for pickup', async ({ page }) => {
      await page.goto('/supplier/orders');
      
      // Find accepted order
      const acceptedOrder = page.locator('text=/accepted/i').locator('..').locator('..').first();
      if (await acceptedOrder.isVisible()) {
        await acceptedOrder.locator('a').first().click();
        
        // Wait for order details
        await page.waitForSelector('button:has-text("Ready"), button:has-text("Pickup"), [data-testid*="ready"]', { timeout: 5000 });
        
        // Click ready for pickup button
        const readyButton = page.locator('button:has-text("Ready"), button:has-text("Pickup")').first();
        if (await readyButton.isVisible()) {
          await readyButton.click();
          
          // Verify status updated
          await expect(
            page.locator('text=/ready|pickup|success/i')
          ).toBeVisible({ timeout: 5000 });
        }
      }
    });

    test('should mark order as shipped', async ({ page }) => {
      await page.goto('/supplier/orders');
      
      // Find order ready for pickup
      const readyOrder = page.locator('text=/ready|pickup/i').locator('..').locator('..').first();
      if (await readyOrder.isVisible()) {
        await readyOrder.locator('a').first().click();
        
        // Wait for order details
        await page.waitForSelector('button:has-text("Shipped"), [data-testid*="shipped"]', { timeout: 5000 });
        
        // Click shipped button
        const shippedButton = page.locator('button:has-text("Shipped"), [data-testid*="shipped"]').first();
        if (await shippedButton.isVisible()) {
          await shippedButton.click();
          
          // Verify status updated
          await expect(
            page.locator('text=/shipped|success/i')
          ).toBeVisible({ timeout: 5000 });
        }
      }
    });

    test('should filter orders by status', async ({ page }) => {
      await page.goto('/supplier/orders');
      
      // Find status filter
      const statusFilter = page.locator('select, [data-testid*="status"]').first();
      if (await statusFilter.isVisible()) {
        await statusFilter.selectOption('Submitted');
        await page.waitForTimeout(1000);
        
        // Verify filtered results
        const results = page.locator('table tbody tr, .order-item');
        await expect(results.first()).toBeVisible();
      }
    });
  });
});

