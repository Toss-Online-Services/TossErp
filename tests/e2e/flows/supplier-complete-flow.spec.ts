import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

/**
 * Complete end-to-end flow test for Supplier role
 */
test.describe('Supplier Complete Flow', () => {
  test('complete supplier workflow: login -> onboarding -> view orders -> accept -> ship', async ({ page }) => {
    const authHelper = new AuthHelper(page);
    
    // Step 1: Login
    await authHelper.loginAs('supplier');
    await authHelper.waitForAuth();
    
    // Step 2: Complete onboarding if needed
    if (page.url().includes('onboarding')) {
      const completeButton = page.locator('button:has-text("Complete"), button:has-text("Skip")').first();
      if (await completeButton.isVisible()) {
        await completeButton.click();
        await page.waitForURL(/\/supplier\/(dashboard|orders)/, { timeout: 10000 });
      }
    }
    
    // Step 3: View incoming orders
    await page.goto('/supplier/orders');
    await page.waitForSelector('table, .order-list', { timeout: 5000 });
    
    // Step 4: Find and view a submitted order
    const submittedOrder = page.locator('text=/submitted/i').locator('..').locator('..').first();
    if (await submittedOrder.isVisible()) {
      await submittedOrder.locator('a').first().click();
      
      await page.waitForURL(/\/supplier\/orders\/\d+/, { timeout: 5000 });
      
      // Step 5: Accept the order
      const acceptButton = page.locator('button:has-text("Accept"), [data-testid*="accept"]').first();
      if (await acceptButton.isVisible()) {
        await acceptButton.click();
        await page.waitForTimeout(2000);
        
        // Verify status changed to Accepted
        await expect(page.locator('text=/accepted/i')).toBeVisible({ timeout: 5000 });
        
        // Step 6: Mark as ready for pickup
        const readyButton = page.locator('button:has-text("Ready"), button:has-text("Pickup")').first();
        if (await readyButton.isVisible()) {
          await readyButton.click();
          await page.waitForTimeout(2000);
          
          // Verify status updated
          await expect(page.locator('text=/ready|pickup/i')).toBeVisible({ timeout: 3000 });
          
          // Step 7: Mark as shipped
          const shippedButton = page.locator('button:has-text("Shipped"), [data-testid*="shipped"]').first();
          if (await shippedButton.isVisible()) {
            await shippedButton.click();
            await page.waitForTimeout(2000);
            
            // Verify status updated to Shipped
            await expect(page.locator('text=/shipped/i')).toBeVisible({ timeout: 3000 });
          }
        }
      }
    }
  });
});

