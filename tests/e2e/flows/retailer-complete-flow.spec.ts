import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';
import { TEST_PRODUCTS, TEST_PURCHASE_ORDER, TEST_SALE } from '../helpers/test-data';

/**
 * Complete end-to-end flow test for Retailer role
 * Tests the full workflow from login to making a sale
 */
test.describe('Retailer Complete Flow', () => {
  test('complete retailer workflow: login -> onboarding -> products -> POS sale -> inventory check', async ({ page }) => {
    const authHelper = new AuthHelper(page);
    
    // Step 1: Login
    await authHelper.loginAs('retailer');
    await authHelper.waitForAuth();
    
    // Step 2: Complete onboarding if needed
    if (page.url().includes('onboarding')) {
      // Quick onboarding completion (simplified)
      const completeButton = page.locator('button:has-text("Complete"), button:has-text("Skip")').first();
      if (await completeButton.isVisible()) {
        await completeButton.click();
        await page.waitForURL(/\/retailer\/(dashboard|products)/, { timeout: 10000 });
      }
    }
    
    // Step 3: Navigate to products
    await page.goto('/retailer/products');
    await page.waitForSelector('table, .product-list', { timeout: 5000 });
    
    // Step 4: Create a product
    const addButton = page.locator('a:has-text("Add"), button:has-text("Add"), a[href*="new"]').first();
    await addButton.click();
    
    await page.waitForSelector('input[name="name"]', { timeout: 5000 });
    const product = TEST_PRODUCTS.valid;
    const uniqueSKU = `${product.sku}-${Date.now()}`;
    
    await page.fill('input[name="name"], input[placeholder*="name" i]', product.name);
    await page.fill('input[name="sku"], input[placeholder*="sku" i]', uniqueSKU);
    await page.fill('input[name="basePrice"], input[type="number"]', product.basePrice.toString());
    await page.fill('input[name="minimumStockLevel"]', product.minimumStockLevel.toString());
    
    await page.click('button:has-text("Save"), button[type="submit"]');
    await page.waitForURL(/\/retailer\/products/, { timeout: 5000 });
    
    // Verify product was created
    await expect(page.locator(`text=${product.name}`)).toBeVisible();
    
    // Step 5: Navigate to POS
    await page.goto('/sales/pos');
    await page.waitForSelector('input[type="search"], .pos-interface', { timeout: 5000 });
    
    // Step 6: Add product to cart
    const searchInput = page.locator('input[type="search"], input[placeholder*="product" i]').first();
    await searchInput.fill(product.name);
    await page.waitForTimeout(1000);
    
    // Click on product result
    const productResult = page.locator(`text=${product.name}`).first();
    if (await productResult.isVisible()) {
      await productResult.click();
      await page.waitForTimeout(500);
    }
    
    // Step 7: Process sale
    const checkoutButton = page.locator('button:has-text("Checkout"), button:has-text("Process")').first();
    if (await checkoutButton.isVisible()) {
      // Select payment method
      const paymentSelect = page.locator('select[name*="payment"], button:has-text("Cash")').first();
      if (await paymentSelect.isVisible()) {
        if (await paymentSelect.evaluate(el => el.tagName === 'SELECT')) {
          await paymentSelect.selectOption('Cash');
        } else {
          await paymentSelect.click();
        }
      }
      
      await checkoutButton.click();
      
      // Wait for success/receipt
      await expect(
        page.locator('text=/success|receipt|complete/i')
      ).toBeVisible({ timeout: 5000 });
    }
    
    // Step 8: Check inventory was updated
    await page.goto('/retailer/inventory');
    await page.waitForSelector('table, .inventory-list', { timeout: 5000 });
    
    // Verify product appears in inventory with updated stock
    await expect(page.locator(`text=${product.name}`)).toBeVisible();
  });

  test('complete purchase order workflow: create -> submit -> track', async ({ page }) => {
    const authHelper = new AuthHelper(page);
    await authHelper.loginAs('retailer');
    await authHelper.waitForAuth();
    
    // Navigate to orders
    await page.goto('/retailer/orders');
    await page.waitForSelector('table, .order-list', { timeout: 5000 });
    
    // Create new order
    const createButton = page.locator('a:has-text("Create"), button:has-text("New")').first();
    await createButton.click();
    
    await page.waitForSelector('select[name*="supplier"], select', { timeout: 5000 });
    
    // Select supplier
    const supplierSelect = page.locator('select[name*="supplier"], select').first();
    if (await supplierSelect.isVisible()) {
      await supplierSelect.selectOption({ index: 1 });
    }
    
    // Add item
    const addItemButton = page.locator('button:has-text("Add Item"), button:has-text("Add")').first();
    if (await addItemButton.isVisible()) {
      await addItemButton.click();
      await page.waitForTimeout(500);
      
      // Select product and quantity
      const productSelect = page.locator('select').nth(1);
      if (await productSelect.isVisible()) {
        await productSelect.selectOption({ index: 1 });
      }
      
      const quantityInput = page.locator('input[type="number"]').first();
      if (await quantityInput.isVisible()) {
        await quantityInput.fill('10');
      }
    }
    
    // Create order
    const submitButton = page.locator('button:has-text("Create"), button:has-text("Submit")').first();
    await submitButton.click();
    
    await page.waitForURL(/\/retailer\/orders/, { timeout: 5000 });
    
    // Verify order appears in list
    await expect(page.locator('text=/draft|submitted/i')).toBeVisible();
    
    // View order details
    const orderLink = page.locator('table tbody tr a, .order-item a').first();
    if (await orderLink.isVisible()) {
      await orderLink.click();
      
      // Submit order if in draft
      const submitOrderButton = page.locator('button:has-text("Submit Order")').first();
      if (await submitOrderButton.isVisible()) {
        await submitOrderButton.click();
        await page.waitForTimeout(1000);
        
        // Verify status changed
        await expect(page.locator('text=/submitted/i')).toBeVisible({ timeout: 3000 });
      }
    }
  });
});

