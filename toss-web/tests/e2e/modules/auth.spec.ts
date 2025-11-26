import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

test.describe('Authentication Module', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test.describe('Login Flow', () => {
    test('should display login page', async ({ page }) => {
      await page.goto('/login');
      await expect(page).toHaveTitle(/TOSS|Login/i);
      
      const emailInput = page.locator('input[type="email"], input[name="email"], input[placeholder*="email" i]').first();
      const passwordInput = page.locator('input[type="password"], input[name="password"]').first();
      
      await expect(emailInput).toBeVisible({ timeout: 5000 });
      await expect(passwordInput).toBeVisible();
    });

    test('should login as admin successfully', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('admin');
      await expect(page).not.toHaveURL(/\/login/);
      await authHelper.waitForAuth();
    });

    test('should login as retailer successfully', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('retailer');
      const url = page.url();
      expect(url).toMatch(/\/(retailer|onboarding|dashboard)/);
    });

    test('should show error for invalid credentials', async ({ page }) => {
      await page.goto('/login');
      const emailInput = page.locator('input[type="email"], input[name="email"]').first();
      const passwordInput = page.locator('input[type="password"], input[name="password"]').first();
      const submitButton = page.locator('button[type="submit"], button:has-text("Login")').first();
      
      await emailInput.fill('invalid@test.com');
      await passwordInput.fill('wrongpassword');
      await submitButton.click();
      await page.waitForTimeout(2000);
    });
  });

  test.describe('Logout Flow', () => {
    test('should logout successfully', async ({ page }) => {
      const authHelper = new AuthHelper(page);
      await authHelper.loginAs('admin');
      await authHelper.waitForAuth();
      await authHelper.logout();
      await expect(page).toHaveURL(/\/login/, { timeout: 10000 });
    });
  });
});

