import { test, expect } from '@playwright/test';

test.describe('Sales Invoices', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/sales/invoices');
  });

  test('should display the sales invoices list', async ({ page }) => {
    await expect(page.locator('h1')).toHaveText('Sales Invoices');
    await expect(page.locator('table')).toBeVisible();
  });

  test('should navigate to the create sales invoice page', async ({ page }) => {
    await page.click('text=Create Invoice');
    await expect(page).toHaveURL('/sales/invoices/create');
    await expect(page.locator('h1')).toHaveText('Create Invoice');
  });

  test('should create a new sales invoice', async ({ page }) => {
    await page.goto('/sales/invoices/create');

    // Fill out the form
    await page.fill('input[name="customerName"]', 'Test Customer');
    await page.fill('input[name="invoiceDate"]', '2025-11-12');
    await page.fill('input[name="dueDate"]', '2025-12-12');

    // Add an item
    await page.click('button:has-text("Add Item")');
    await page.fill('input[name="items.0.productName"]', 'Another Test Product');
    await page.fill('input[name="items.0.quantity"]', '2');
    await page.fill('input[name="items.0.unitPrice"]', '50');

    // Submit the form
    await page.click('button:has-text("Save Invoice")');

    // Check for successful creation
    await expect(page).toHaveURL(/\/sales\/invoices\/\d+/);
    await expect(page.locator('h1')).toHaveText(/Invoice #/);
  });
});
