import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

/**
 * Complete integration test: Order lifecycle across all roles
 * Tests: Retailer creates PO -> Supplier accepts -> Driver delivers -> Retailer receives
 */
test.describe('Complete Order Lifecycle Integration', () => {
  test('full order lifecycle: retailer creates PO -> supplier accepts -> driver delivers', async ({ browser }) => {
    // Create separate contexts for each role
    const retailerContext = await browser.newContext();
    const supplierContext = await browser.newContext();
    const driverContext = await browser.newContext();
    const adminContext = await browser.newContext();

    const retailerPage = await retailerContext.newPage();
    const supplierPage = await supplierContext.newPage();
    const driverPage = await driverContext.newPage();
    const adminPage = await adminContext.newPage();

    try {
      // Step 1: Retailer creates purchase order
      const retailerAuth = new AuthHelper(retailerPage);
      await retailerAuth.loginAs('retailer');
      await retailerAuth.waitForAuth();

      // Skip onboarding if needed
      if (retailerPage.url().includes('onboarding')) {
        const skipButton = retailerPage.locator('button:has-text("Skip"), button:has-text("Complete")').first();
        if (await skipButton.isVisible()) {
          await skipButton.click();
          await retailerPage.waitForURL(/\/retailer\/(dashboard|products|orders)/, { timeout: 10000 });
        }
      }

      await retailerPage.goto('/retailer/orders');
      await retailerPage.waitForSelector('table, .order-list', { timeout: 5000 });

      // Create new order
      const createButton = retailerPage.locator('a:has-text("Create"), button:has-text("New")').first();
      await createButton.click();

      await retailerPage.waitForSelector('select[name*="supplier"], select', { timeout: 5000 });

      // Select supplier and add items
      const supplierSelect = retailerPage.locator('select[name*="supplier"], select').first();
      if (await supplierSelect.isVisible()) {
        await supplierSelect.selectOption({ index: 1 });
      }

      // Add item
      const addItemButton = retailerPage.locator('button:has-text("Add Item"), button:has-text("Add")').first();
      if (await addItemButton.isVisible()) {
        await addItemButton.click();
        await retailerPage.waitForTimeout(500);

        const productSelect = retailerPage.locator('select').nth(1);
        if (await productSelect.isVisible()) {
          await productSelect.selectOption({ index: 1 });
        }

        const quantityInput = retailerPage.locator('input[type="number"]').first();
        if (await quantityInput.isVisible()) {
          await quantityInput.fill('5');
        }
      }

      // Create and submit order
      const submitButton = retailerPage.locator('button:has-text("Create"), button:has-text("Submit")').first();
      await submitButton.click();
      await retailerPage.waitForURL(/\/retailer\/orders/, { timeout: 5000 });

      // Get order number/ID for tracking
      const orderLink = retailerPage.locator('table tbody tr a, .order-item a').first();
      let orderId = '';
      if (await orderLink.isVisible()) {
        await orderLink.click();
        await retailerPage.waitForURL(/\/retailer\/orders\/\d+/, { timeout: 5000 });
        orderId = retailerPage.url().split('/').pop() || '';

        // Submit order if in draft
        const submitOrderButton = retailerPage.locator('button:has-text("Submit Order")').first();
        if (await submitOrderButton.isVisible()) {
          await submitOrderButton.click();
          await retailerPage.waitForTimeout(2000);
        }
      }

      // Step 2: Supplier views and accepts order
      const supplierAuth = new AuthHelper(supplierPage);
      await supplierAuth.loginAs('supplier');
      await supplierAuth.waitForAuth();

      if (supplierPage.url().includes('onboarding')) {
        const skipButton = supplierPage.locator('button:has-text("Skip"), button:has-text("Complete")').first();
        if (await skipButton.isVisible()) {
          await skipButton.click();
          await supplierPage.waitForURL(/\/supplier\/(dashboard|orders)/, { timeout: 10000 });
        }
      }

      await supplierPage.goto('/supplier/orders');
      await supplierPage.waitForSelector('table, .order-list', { timeout: 5000 });

      // Find the submitted order
      const submittedOrder = supplierPage.locator('text=/submitted/i').locator('..').locator('..').first();
      if (await submittedOrder.isVisible()) {
        await submittedOrder.locator('a').first().click();
        await supplierPage.waitForURL(/\/supplier\/orders\/\d+/, { timeout: 5000 });

        // Accept order
        const acceptButton = supplierPage.locator('button:has-text("Accept"), [data-testid*="accept"]').first();
        if (await acceptButton.isVisible()) {
          await acceptButton.click();
          await supplierPage.waitForTimeout(2000);
          await expect(supplierPage.locator('text=/accepted/i')).toBeVisible({ timeout: 5000 });

          // Mark as ready for pickup
          const readyButton = supplierPage.locator('button:has-text("Ready"), button:has-text("Pickup")').first();
          if (await readyButton.isVisible()) {
            await readyButton.click();
            await supplierPage.waitForTimeout(2000);
          }
        }
      }

      // Step 3: Driver views and delivers
      const driverAuth = new AuthHelper(driverPage);
      await driverAuth.loginAs('driver');
      await driverAuth.waitForAuth();

      if (driverPage.url().includes('onboarding')) {
        const skipButton = driverPage.locator('button:has-text("Skip"), button:has-text("Complete")').first();
        if (await skipButton.isVisible()) {
          await skipButton.click();
          await driverPage.waitForURL(/\/driver\/(deliveries|dashboard)/, { timeout: 10000 });
        }
      }

      await driverPage.goto('/driver/deliveries');
      await driverPage.waitForSelector('table, .delivery-list', { timeout: 5000 });

      // Find delivery
      const deliveryLink = driverPage.locator('table tbody tr a, .delivery-item a').first();
      if (await deliveryLink.isVisible()) {
        await deliveryLink.click();
        await driverPage.waitForURL(/\/driver\/deliveries\/\d+/, { timeout: 5000 });

        // Accept delivery
        const acceptDeliveryButton = driverPage.locator('button:has-text("Accept"), [data-testid*="accept"]').first();
        if (await acceptDeliveryButton.isVisible()) {
          await acceptDeliveryButton.click();
          await driverPage.waitForTimeout(2000);
        }

        // Mark as picked up
        const pickedUpButton = driverPage.locator('button:has-text("Picked Up"), button:has-text("Pickup")').first();
        if (await pickedUpButton.isVisible()) {
          await pickedUpButton.click();
          await driverPage.waitForTimeout(2000);
        }

        // Mark as delivered
        const deliveredButton = driverPage.locator('button:has-text("Delivered"), [data-testid*="delivered"]').first();
        if (await deliveredButton.isVisible()) {
          await deliveredButton.click();
          await driverPage.waitForTimeout(1000);

          const notesInput = driverPage.locator('textarea, input[placeholder*="note" i]').first();
          if (await notesInput.isVisible()) {
            await notesInput.fill('Delivered successfully');
          }

          const confirmButton = driverPage.locator('button:has-text("Confirm"), button:has-text("Save")').first();
          if (await confirmButton.isVisible()) {
            await confirmButton.click();
            await driverPage.waitForTimeout(2000);
          }
        }
      }

      // Step 4: Retailer receives goods
      await retailerPage.goto(`/retailer/orders/${orderId}`);
      await retailerPage.waitForSelector('text=/order|items/i', { timeout: 5000 });

      // Verify order status is delivered or ready to receive
      await expect(retailerPage.locator('text=/delivered|received|ready/i')).toBeVisible({ timeout: 5000 });

      // Step 5: Admin views order in dashboard
      const adminAuth = new AuthHelper(adminPage);
      await adminAuth.loginAs('admin');
      await adminAuth.waitForAuth();

      await adminPage.goto('/admin/orders');
      await adminPage.waitForSelector('table, .order-list', { timeout: 5000 });

      // Filter by delivered status
      const statusFilter = adminPage.locator('select, [data-testid*="status"]').first();
      if (await statusFilter.isVisible()) {
        await statusFilter.selectOption('Delivered');
        await adminPage.waitForTimeout(1000);
      }

      // Verify order appears in admin view
      await expect(adminPage.locator('table tbody tr, .order-item').first()).toBeVisible();

    } finally {
      // Cleanup
      await retailerContext.close();
      await supplierContext.close();
      await driverContext.close();
      await adminContext.close();
    }
  });
});

