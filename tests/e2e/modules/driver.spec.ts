import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

test.describe('Driver Module', () => {
  let authHelper: AuthHelper;

  test.beforeEach(async ({ page }) => {
    authHelper = new AuthHelper(page);
    await authHelper.loginAs('driver');
    await authHelper.waitForAuth();
  });

  test.describe('Delivery Management', () => {
    test('should display assigned deliveries', async ({ page }) => {
      await page.goto('/driver/deliveries');
      
      // Check for deliveries list
      await expect(
        page.locator('table, .delivery-list, [data-testid="delivery-list"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should view delivery details', async ({ page }) => {
      await page.goto('/driver/deliveries');
      
      // Wait for deliveries list
      await page.waitForSelector('table tbody tr, .delivery-item', { timeout: 5000 });
      
      // Click on first delivery
      const firstDelivery = page.locator('table tbody tr a, .delivery-item a').first();
      if (await firstDelivery.isVisible()) {
        await firstDelivery.click();
        
        // Verify delivery details page
        await expect(page).toHaveURL(/\/driver\/deliveries\/\d+/);
        await expect(page.locator('text=/delivery|pickup|destination/i')).toBeVisible();
      }
    });

    test('should accept delivery assignment', async ({ page }) => {
      await page.goto('/driver/deliveries');
      
      // Wait for deliveries list
      await page.waitForSelector('table tbody tr, .delivery-item', { timeout: 5000 });
      
      // Find delivery with "Assigned" or "Pending" status
      const assignedDelivery = page.locator('text=/assigned|pending/i').locator('..').locator('..').first();
      if (await assignedDelivery.isVisible()) {
        await assignedDelivery.locator('a').first().click();
        
        // Wait for delivery details
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

    test('should mark delivery as picked up', async ({ page }) => {
      await page.goto('/driver/deliveries');
      
      // Find accepted delivery
      const acceptedDelivery = page.locator('text=/accepted/i').locator('..').locator('..').first();
      if (await acceptedDelivery.isVisible()) {
        await acceptedDelivery.locator('a').first().click();
        
        // Wait for delivery details
        await page.waitForSelector('button:has-text("Picked Up"), button:has-text("Pickup"), [data-testid*="picked"]', { timeout: 5000 });
        
        // Click picked up button
        const pickedUpButton = page.locator('button:has-text("Picked Up"), button:has-text("Pickup")').first();
        if (await pickedUpButton.isVisible()) {
          await pickedUpButton.click();
          
          // Verify status updated
          await expect(
            page.locator('text=/picked up|success/i')
          ).toBeVisible({ timeout: 5000 });
        }
      }
    });

    test('should mark delivery as delivered', async ({ page }) => {
      await page.goto('/driver/deliveries');
      
      // Find picked up delivery
      const pickedUpDelivery = page.locator('text=/picked up/i').locator('..').locator('..').first();
      if (await pickedUpDelivery.isVisible()) {
        await pickedUpDelivery.locator('a').first().click();
        
        // Wait for delivery details
        await page.waitForSelector('button:has-text("Delivered"), [data-testid*="delivered"]', { timeout: 5000 });
        
        // Click delivered button
        const deliveredButton = page.locator('button:has-text("Delivered"), [data-testid*="delivered"]').first();
        if (await deliveredButton.isVisible()) {
          await deliveredButton.click();
          
          // Wait for delivery confirmation form if it appears
          await page.waitForTimeout(1000);
          
          // Fill delivery notes if form appears
          const notesInput = page.locator('textarea, input[placeholder*="note" i]').first();
          if (await notesInput.isVisible()) {
            await notesInput.fill('Delivery completed successfully');
          }
          
          // Confirm delivery
          const confirmButton = page.locator('button:has-text("Confirm"), button:has-text("Save")').first();
          if (await confirmButton.isVisible()) {
            await confirmButton.click();
          }
          
          // Verify status updated
          await expect(
            page.locator('text=/delivered|success|complete/i')
          ).toBeVisible({ timeout: 5000 });
        }
      }
    });

    test('should display delivery location information', async ({ page }) => {
      await page.goto('/driver/deliveries');
      
      // Wait for deliveries list
      await page.waitForSelector('table tbody tr, .delivery-item', { timeout: 5000 });
      
      // Click on first delivery
      const firstDelivery = page.locator('table tbody tr a, .delivery-item a').first();
      if (await firstDelivery.isVisible()) {
        await firstDelivery.click();
        
        // Verify location details are displayed
        await expect(
          page.locator('text=/address|location|pickup|destination|contact/i')
        ).toBeVisible({ timeout: 5000 });
      }
    });

    test('should filter deliveries by status', async ({ page }) => {
      await page.goto('/driver/deliveries');
      
      // Find status filter
      const statusFilter = page.locator('select, [data-testid*="status"]').first();
      if (await statusFilter.isVisible()) {
        await statusFilter.selectOption('Accepted');
        await page.waitForTimeout(1000);
        
        // Verify filtered results
        const results = page.locator('table tbody tr, .delivery-item');
        await expect(results.first()).toBeVisible();
      }
    });
  });
});

