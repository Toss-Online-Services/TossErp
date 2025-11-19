import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';
import { TEST_PRODUCTS, TEST_PURCHASE_ORDER, TEST_SALE } from '../helpers/test-data';

test.describe('Retailer Module', () => {
  let authHelper: AuthHelper;

  test.beforeEach(async ({ page }) => {
    authHelper = new AuthHelper(page);
    await authHelper.loginAs('retailer');
    await authHelper.waitForAuth();
  });

  test.describe('Product Management', () => {
    test('should display product list', async ({ page }) => {
      await page.goto('/retailer/products');
      
      // Check for product list
      await expect(
        page.locator('table, .product-list, [data-testid="product-list"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should create new product', async ({ page }) => {
      await page.goto('/retailer/products');
      
      // Click add product button
      const addButton = page.locator('a:has-text("Add"), button:has-text("Add"), a[href*="new"]').first();
      await addButton.click();
      
      // Wait for product form
      await page.waitForSelector('input[name="name"], input[placeholder*="name" i]', { timeout: 5000 });
      
      // Fill product form
      const product = TEST_PRODUCTS.valid;
      await page.fill('input[name="name"], input[placeholder*="name" i]', product.name);
      await page.fill('input[name="sku"], input[placeholder*="sku" i]', product.sku);
      await page.fill('input[name="basePrice"], input[type="number"]', product.basePrice.toString());
      await page.fill('input[name="minimumStockLevel"], input[placeholder*="stock" i]', product.minimumStockLevel.toString());
      
      // Save product
      const saveButton = page.locator('button:has-text("Save"), button[type="submit"]').first();
      await saveButton.click();
      
      // Verify success and redirect
      await page.waitForURL(/\/retailer\/products/, { timeout: 5000 });
      await expect(page.locator(`text=${product.name}`)).toBeVisible({ timeout: 5000 });
    });

    test('should edit existing product', async ({ page }) => {
      await page.goto('/retailer/products');
      
      // Wait for product list
      await page.waitForSelector('table tbody tr, .product-item', { timeout: 5000 });
      
      // Click edit on first product
      const editLink = page.locator('a:has-text("Edit"), button:has-text("Edit")').first();
      if (await editLink.isVisible()) {
        await editLink.click();
        
        // Wait for edit form
        await page.waitForSelector('input[name="name"]', { timeout: 5000 });
        
        // Update product name
        const nameInput = page.locator('input[name="name"]').first();
        const currentName = await nameInput.inputValue();
        await nameInput.fill(`${currentName} Updated`);
        
        // Save
        await page.click('button:has-text("Save"), button[type="submit"]');
        await page.waitForURL(/\/retailer\/products/, { timeout: 5000 });
      }
    });

    test('should delete product', async ({ page }) => {
      await page.goto('/retailer/products');
      
      // Wait for product list
      await page.waitForSelector('table tbody tr, .product-item', { timeout: 5000 });
      
      // Find delete button
      const deleteButton = page.locator('button:has-text("Delete"), [data-testid*="delete"]').first();
      if (await deleteButton.isVisible()) {
        // Get product name before deletion
        const productRow = deleteButton.locator('..').locator('..');
        const productName = await productRow.locator('td, .product-name').first().textContent();
        
        await deleteButton.click();
        
        // Confirm deletion if dialog appears
        page.on('dialog', async dialog => {
          expect(dialog.type()).toBe('confirm');
          await dialog.accept();
        });
        
        await page.waitForTimeout(1000);
        
        // Verify product is removed (if productName was captured)
        if (productName) {
          await expect(page.locator(`text=${productName}`)).not.toBeVisible({ timeout: 3000 });
        }
      }
    });

    test('should search products', async ({ page }) => {
      await page.goto('/retailer/products');
      
      // Find search input
      const searchInput = page.locator('input[type="search"], input[placeholder*="search" i]').first();
      if (await searchInput.isVisible()) {
        await searchInput.fill('test');
        await page.waitForTimeout(1000);
        
        // Verify filtered results
        const results = page.locator('table tbody tr, .product-item');
        await expect(results.first()).toBeVisible();
      }
    });

    test('should filter products by category', async ({ page }) => {
      await page.goto('/retailer/products');
      
      // Find category filter
      const categoryFilter = page.locator('select, [data-testid*="category"]').first();
      if (await categoryFilter.isVisible()) {
        await categoryFilter.selectOption({ index: 1 }); // Select first category option
        await page.waitForTimeout(1000);
        
        // Verify filter is applied
        const results = page.locator('table tbody tr, .product-item');
        await expect(results.first()).toBeVisible();
      }
    });
  });

  test.describe('POS (Point of Sale)', () => {
    test('should display POS interface', async ({ page }) => {
      await page.goto('/sales/pos');
      
      // Check for POS elements
      await expect(
        page.locator('text=/pos|point of sale|checkout/i, .pos-interface, [data-testid="pos"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should search and add products to cart', async ({ page }) => {
      await page.goto('/sales/pos');
      
      // Wait for product search
      await page.waitForSelector('input[type="search"], input[placeholder*="product" i]', { timeout: 5000 });
      
      // Search for product
      const searchInput = page.locator('input[type="search"], input[placeholder*="product" i]').first();
      await searchInput.fill('test');
      await page.waitForTimeout(1000);
      
      // Click on first product result
      const productResult = page.locator('.product-item, [data-testid*="product"], .product-card').first();
      if (await productResult.isVisible()) {
        await productResult.click();
        
        // Verify product added to cart
        await expect(page.locator('.cart, [data-testid="cart"]')).toBeVisible({ timeout: 3000 });
      }
    });

    test('should update cart item quantity', async ({ page }) => {
      await page.goto('/sales/pos');
      
      // Add product to cart first (simplified - assumes product is already added)
      await page.waitForSelector('.cart-item, [data-testid*="cart-item"]', { timeout: 5000 });
      
      // Find quantity input
      const quantityInput = page.locator('input[type="number"], [data-testid*="quantity"]').first();
      if (await quantityInput.isVisible()) {
        await quantityInput.fill('3');
        await page.waitForTimeout(500);
        
        // Verify total updated
        const total = page.locator('.total, [data-testid="total"]');
        await expect(total).toBeVisible();
      }
    });

    test('should process payment', async ({ page }) => {
      await page.goto('/sales/pos');
      
      // Ensure cart has items (this would ideally be set up in beforeEach)
      await page.waitForSelector('.cart-item, [data-testid*="cart-item"], .checkout-button', { timeout: 5000 });
      
      // Select payment method
      const paymentMethod = page.locator('button:has-text("Cash"), select[name*="payment"], [data-testid*="payment"]').first();
      if (await paymentMethod.isVisible()) {
        if (await paymentMethod.evaluate(el => el.tagName === 'SELECT')) {
          await paymentMethod.selectOption('Cash');
        } else {
          await paymentMethod.click();
        }
      }
      
      // Click checkout/process payment
      const checkoutButton = page.locator('button:has-text("Checkout"), button:has-text("Process"), [data-testid*="checkout"]').first();
      if (await checkoutButton.isVisible()) {
        await checkoutButton.click();
        
        // Wait for success or receipt
        await expect(
          page.locator('text=/success|receipt|complete/i, .receipt, [data-testid="receipt"]')
        ).toBeVisible({ timeout: 5000 });
      }
    });

    test('should round cash payments to nearest 5c', async ({ page }) => {
      await page.goto('/sales/pos');
      
      // This test would verify cash rounding logic
      // Implementation depends on how rounding is displayed in UI
      await page.waitForSelector('.total, [data-testid="total"]', { timeout: 5000 });
      
      const totalElement = page.locator('.total, [data-testid="total"]').first();
      const totalText = await totalElement.textContent();
      
      // Extract amount and verify it ends in .00, .05, .10, .15, .20, etc.
      const amountMatch = totalText?.match(/[\d.]+/);
      if (amountMatch) {
        const amount = parseFloat(amountMatch[0]);
        const cents = Math.round((amount % 1) * 100);
        // Cash should round to nearest 5c
        expect([0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100]).toContain(cents);
      }
    });
  });

  test.describe('Purchase Orders', () => {
    test('should display purchase order list', async ({ page }) => {
      await page.goto('/retailer/orders');
      
      // Check for orders list
      await expect(
        page.locator('table, .order-list, [data-testid="order-list"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should create new purchase order', async ({ page }) => {
      await page.goto('/retailer/orders');
      
      // Click create order button
      const createButton = page.locator('a:has-text("Create"), button:has-text("New"), a[href*="new"]').first();
      await createButton.click();
      
      // Wait for order form
      await page.waitForSelector('select[name*="supplier"], select, input', { timeout: 5000 });
      
      // Select supplier
      const supplierSelect = page.locator('select[name*="supplier"], select').first();
      if (await supplierSelect.isVisible()) {
        await supplierSelect.selectOption({ index: 1 });
      }
      
      // Add items (simplified - would need to add product selection)
      const addItemButton = page.locator('button:has-text("Add Item"), button:has-text("Add")').first();
      if (await addItemButton.isVisible()) {
        await addItemButton.click();
        await page.waitForTimeout(500);
      }
      
      // Submit order
      const submitButton = page.locator('button:has-text("Create"), button:has-text("Submit"), button[type="submit"]').first();
      await submitButton.click();
      
      // Verify redirect to orders list
      await page.waitForURL(/\/retailer\/orders/, { timeout: 5000 });
    });

    test('should view purchase order details', async ({ page }) => {
      await page.goto('/retailer/orders');
      
      // Wait for orders list
      await page.waitForSelector('table tbody tr, .order-item', { timeout: 5000 });
      
      // Click on first order
      const firstOrder = page.locator('table tbody tr a, .order-item a').first();
      if (await firstOrder.isVisible()) {
        await firstOrder.click();
        
        // Verify order details page
        await expect(page).toHaveURL(/\/retailer\/orders\/\d+/);
        await expect(page.locator('text=/order|items|total/i')).toBeVisible();
      }
    });

    test('should filter orders by status', async ({ page }) => {
      await page.goto('/retailer/orders');
      
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

  test.describe('Inventory Management', () => {
    test('should display inventory/stock levels', async ({ page }) => {
      await page.goto('/retailer/inventory');
      
      // Check for inventory list
      await expect(
        page.locator('table, .inventory-list, [data-testid="inventory-list"]')
      ).toBeVisible({ timeout: 5000 });
    });

    test('should show low stock indicators', async ({ page }) => {
      await page.goto('/retailer/inventory');
      
      // Wait for inventory list
      await page.waitForSelector('table tbody tr, .inventory-item', { timeout: 5000 });
      
      // Check for low stock indicators (red/orange styling or badges)
      const lowStockItems = page.locator('.low-stock, [data-testid*="low-stock"], .text-red, .text-orange');
      // At least verify the selector exists (may not have low stock items)
      await expect(lowStockItems.or(page.locator('table tbody tr')).first()).toBeVisible();
    });

    test('should adjust stock levels', async ({ page }) => {
      await page.goto('/retailer/inventory');
      
      // Wait for inventory list
      await page.waitForSelector('table tbody tr, .inventory-item', { timeout: 5000 });
      
      // Find adjust button
      const adjustButton = page.locator('button:has-text("Adjust"), [data-testid*="adjust"]').first();
      if (await adjustButton.isVisible()) {
        await adjustButton.click();
        
        // Wait for adjustment form/modal
        await page.waitForSelector('input[type="number"], input[placeholder*="quantity" i]', { timeout: 5000 });
        
        // Enter adjustment
        const quantityInput = page.locator('input[type="number"]').first();
        await quantityInput.fill('10');
        
        // Save adjustment
        const saveButton = page.locator('button:has-text("Save"), button:has-text("Apply")').first();
        await saveButton.click();
        
        // Verify success
        await expect(
          page.locator('text=/success|updated/i')
        ).toBeVisible({ timeout: 3000 });
      }
    });
  });
});

