import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

/**
 * Complete end-to-end flow test for Driver role
 */
test.describe('Driver Complete Flow', () => {
  test('complete driver workflow: login -> onboarding -> view deliveries -> accept -> pickup -> deliver', async ({ page }) => {
    const authHelper = new AuthHelper(page);
    
    // Step 1: Login
    await authHelper.loginAs('driver');
    await authHelper.waitForAuth();
    
    // Step 2: Complete onboarding if needed
    if (page.url().includes('onboarding')) {
      const completeButton = page.locator('button:has-text("Complete"), button:has-text("Skip")').first();
      if (await completeButton.isVisible()) {
        await completeButton.click();
        await page.waitForURL(/\/driver\/(deliveries|dashboard)/, { timeout: 10000 });
      }
    }
    
    // Step 3: View assigned deliveries
    await page.goto('/driver/deliveries');
    await page.waitForSelector('table, .delivery-list', { timeout: 5000 });
    
    // Step 4: View delivery details
    const firstDelivery = page.locator('table tbody tr a, .delivery-item a').first();
    if (await firstDelivery.isVisible()) {
      await firstDelivery.click();
      
      await page.waitForURL(/\/driver\/deliveries\/\d+/, { timeout: 5000 });
      
      // Verify delivery information is displayed
      await expect(page.locator('text=/pickup|destination|contact|address/i')).toBeVisible();
      
      // Step 5: Accept delivery
      const acceptButton = page.locator('button:has-text("Accept"), [data-testid*="accept"]').first();
      if (await acceptButton.isVisible()) {
        await acceptButton.click();
        await page.waitForTimeout(2000);
        
        // Verify status changed
        await expect(page.locator('text=/accepted/i')).toBeVisible({ timeout: 5000 });
        
        // Step 6: Mark as picked up
        const pickedUpButton = page.locator('button:has-text("Picked Up"), button:has-text("Pickup")').first();
        if (await pickedUpButton.isVisible()) {
          await pickedUpButton.click();
          await page.waitForTimeout(2000);
          
          // Verify status updated
          await expect(page.locator('text=/picked up/i')).toBeVisible({ timeout: 3000 });
          
          // Step 7: Mark as delivered
          const deliveredButton = page.locator('button:has-text("Delivered"), [data-testid*="delivered"]').first();
          if (await deliveredButton.isVisible()) {
            await deliveredButton.click();
            await page.waitForTimeout(1000);
            
            // Fill delivery notes if form appears
            const notesInput = page.locator('textarea, input[placeholder*="note" i]').first();
            if (await notesInput.isVisible()) {
              await notesInput.fill('Delivered successfully to customer');
            }
            
            // Confirm delivery
            const confirmButton = page.locator('button:has-text("Confirm"), button:has-text("Save")').first();
            if (await confirmButton.isVisible()) {
              await confirmButton.click();
            }
            
            // Verify delivery completed
            await expect(page.locator('text=/delivered|success|complete/i')).toBeVisible({ timeout: 5000 });
          }
        }
      }
    }
  });
});

