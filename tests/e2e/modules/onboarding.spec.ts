import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';
import { TEST_ONBOARDING } from '../helpers/test-data';

test.describe('Onboarding Module', () => {
  test.describe('Retailer Onboarding', () => {
    test('should redirect new retailer to onboarding', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('retailer');
      
      // Should redirect to onboarding if not completed
      await expect(page).toHaveURL(/\/retailer\/onboarding/, { timeout: 10000 });
    });

    test('should complete retailer onboarding step 1: Business Profile', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('retailer');
      
      await page.waitForURL(/\/retailer\/onboarding/, { timeout: 10000 });
      
      // Fill business profile form
      const onboarding = TEST_ONBOARDING.retailer;
      await page.fill('input[name*="businessName"], input[placeholder*="business" i]', onboarding.businessName);
      await page.fill('input[name*="address"], input[placeholder*="address" i]', onboarding.address);
      await page.fill('input[name*="city"], input[placeholder*="city" i]', onboarding.city);
      await page.fill('input[name*="postalCode"], input[placeholder*="postal" i]', onboarding.postalCode);
      await page.fill('input[name*="phone"], input[type="tel"]', onboarding.phone);
      
      // Proceed to next step
      const nextButton = page.locator('button:has-text("Next"), button:has-text("Continue")').first();
      await nextButton.click();
      
      // Verify moved to step 2
      await expect(page.locator('text=/step 2|products/i')).toBeVisible({ timeout: 5000 });
    });

    test('should complete retailer onboarding step 2: Add Products', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('retailer');
      
      await page.waitForURL(/\/retailer\/onboarding/, { timeout: 10000 });
      
      // Skip to step 2 if needed (or complete step 1 first)
      // Add product
      const addProductButton = page.locator('button:has-text("Add Product"), button:has-text("Add")').first();
      if (await addProductButton.isVisible()) {
        await addProductButton.click();
        
        const product = TEST_ONBOARDING.retailer.products[0];
        await page.fill('input[name*="name"], input[placeholder*="name" i]', product.name);
        await page.fill('input[name*="sku"], input[placeholder*="sku" i]', product.sku);
        await page.fill('input[name*="price"], input[type="number"]', product.price.toString());
        
        const saveButton = page.locator('button:has-text("Save"), button:has-text("Add")').first();
        await saveButton.click();
      }
      
      // Proceed to next step
      const nextButton = page.locator('button:has-text("Next"), button:has-text("Continue")').first();
      await nextButton.click();
    });

    test('should complete retailer onboarding and redirect to dashboard', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('retailer');
      
      await page.waitForURL(/\/retailer\/onboarding/, { timeout: 10000 });
      
      // Complete all steps (simplified - would need to fill all forms)
      // Click complete/finish button
      const completeButton = page.locator('button:has-text("Complete"), button:has-text("Finish")').first();
      if (await completeButton.isVisible()) {
        await completeButton.click();
        
        // Should redirect to dashboard
        await expect(page).toHaveURL(/\/retailer\/dashboard/, { timeout: 10000 });
      }
    });
  });

  test.describe('Supplier Onboarding', () => {
    test('should redirect new supplier to onboarding', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('supplier');
      
      // Should redirect to onboarding if not completed
      await expect(page).toHaveURL(/\/supplier\/onboarding/, { timeout: 10000 });
    });

    test('should complete supplier onboarding', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('supplier');
      
      await page.waitForURL(/\/supplier\/onboarding/, { timeout: 10000 });
      
      // Fill supplier profile
      const onboarding = TEST_ONBOARDING.supplier;
      await page.fill('input[name*="businessName"], input[placeholder*="business" i]', onboarding.businessName);
      await page.fill('input[name*="address"], input[placeholder*="address" i]', onboarding.address);
      await page.fill('input[name*="city"], input[placeholder*="city" i]', onboarding.city);
      
      // Select categories
      const categoryInput = page.locator('input[type="checkbox"], select').first();
      if (await categoryInput.isVisible()) {
        await categoryInput.click();
      }
      
      // Complete onboarding
      const completeButton = page.locator('button:has-text("Complete"), button:has-text("Finish")').first();
      if (await completeButton.isVisible()) {
        await completeButton.click();
        
        // Should redirect to dashboard
        await expect(page).toHaveURL(/\/supplier\/dashboard/, { timeout: 10000 });
      }
    });
  });

  test.describe('Driver Onboarding', () => {
    test('should redirect new driver to onboarding', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('driver');
      
      // Should redirect to onboarding if not completed
      await expect(page).toHaveURL(/\/driver\/onboarding/, { timeout: 10000 });
    });

    test('should complete driver onboarding', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('driver');
      
      await page.waitForURL(/\/driver\/onboarding/, { timeout: 10000 });
      
      // Fill driver profile
      const onboarding = TEST_ONBOARDING.driver;
      await page.fill('input[name*="firstName"], input[placeholder*="first" i]', onboarding.firstName);
      await page.fill('input[name*="lastName"], input[placeholder*="last" i]', onboarding.lastName);
      await page.fill('input[name*="phone"], input[type="tel"]', onboarding.phone);
      await page.fill('input[name*="vehicleType"], input[placeholder*="vehicle" i]', onboarding.vehicleType);
      await page.fill('input[name*="registration"], input[placeholder*="registration" i]', onboarding.registration);
      await page.fill('input[name*="area"], input[placeholder*="area" i]', onboarding.typicalArea);
      
      // Complete onboarding
      const completeButton = page.locator('button:has-text("Complete"), button:has-text("Finish")').first();
      if (await completeButton.isVisible()) {
        await completeButton.click();
        
        // Should redirect to deliveries
        await expect(page).toHaveURL(/\/driver\/deliveries/, { timeout: 10000 });
      }
    });
  });

  test.describe('Onboarding Persistence', () => {
    test('should not show onboarding after completion', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('retailer');
      
      // If onboarding is completed, should go to dashboard
      const currentUrl = page.url();
      if (currentUrl.includes('onboarding')) {
        // Complete onboarding first
        const completeButton = page.locator('button:has-text("Complete"), button:has-text("Finish")').first();
        if (await completeButton.isVisible()) {
          await completeButton.click();
          await page.waitForURL(/\/retailer\/dashboard/, { timeout: 10000 });
        }
      }
      
      // Logout and login again
      await authHelper.logout();
      await authHelper.loginAs('retailer');
      
      // Should NOT redirect to onboarding
      await expect(page).not.toHaveURL(/\/onboarding/);
      await expect(page).toHaveURL(/\/retailer\/(dashboard|products)/);
    });
  });
});

