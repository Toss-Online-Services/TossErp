import { test, expect } from '@playwright/test';

test.describe('Sales Quotations', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/sales/quotations');
  });

  test('should display the quotations list', async ({ page }) => {
    await expect(page.locator('h1')).toHaveText('Sales Quotations');
    await expect(page.locator('table')).toBeVisible();
  });

  test('should navigate to the create quotation page', async ({ page }) => {
    await page.click('text=Create Quotation');
    await expect(page).toHaveURL('/sales/quotations/create');
    await expect(page.locator('h1')).toHaveText('Create Quotation');
  });

  test('should create a new quotation', async ({ page }) => {
    await page.goto('/sales/quotations/create');

    // Fill out the form
    await page.fill('input[name="customerName"]', 'Test Customer');
    await page.fill('input[name="quotationDate"]', '2025-11-12');
    await page.fill('input[name="expiryDate"]', '2025-12-12');

    // Add an item
    await page.click('button:has-text("Add Item")');
    await page.fill('input[name="items.0.productName"]', 'Test Product');
    await page.fill('input[name="items.0.quantity"]', '10');
    await page.fill('input[name="items.0.unitPrice"]', '100');

    // Submit the form
    await page.click('button:has-text("Save Quotation")');

    // Check for successful creation
    await expect(page).toHaveURL(/\/sales\/quotations\/\d+/);
    await expect(page.locator('h1')).toHaveText(/Quotation #/);
  });
});
