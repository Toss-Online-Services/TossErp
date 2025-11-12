import { test, expect } from '@playwright/test';

test.describe('Sales Orders', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/sales/orders');
  });

  test('should display the sales orders list', async ({ page }) => {
    await expect(page.locator('h1')).toHaveText('Sales Orders');
    await expect(page.locator('table')).toBeVisible();
  });

  test('should navigate to the create sales order page', async ({ page }) => {
    await page.click('text=Create Sales Order');
    await expect(page).toHaveURL('/sales/orders/create');
    await expect(page.locator('h1')).toHaveText('Create Sales Order');
  });

  test('should create a new sales order', async ({ page }) => {
    await page.goto('/sales/orders/create');

    // Fill out the form
    await page.fill('input[name="customerName"]', 'Test Customer');
    await page.fill('input[name="orderDate"]', '2025-11-12');

    // Add an item
    await page.click('button:has-text("Add Item")');
    await page.fill('input[name="items.0.productName"]', 'Test Product');
    await page.fill('input[name="items.0.quantity"]', '5');
    await page.fill('input[name="items.0.unitPrice"]', '200');

    // Submit the form
    await page.click('button:has-text("Save Order")');

    // Check for successful creation
    await expect(page).toHaveURL(/\/sales\/orders\/\d+/);
    await expect(page.locator('h1')).toHaveText(/Sales Order #/);
  });
});
