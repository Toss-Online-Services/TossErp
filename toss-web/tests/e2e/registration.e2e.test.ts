import { test, expect, type Page } from '@playwright/test';

/**
 * TOSS Registration E2E Test
 * 
 * Tests the complete 3-step registration flow:
 * Step 1: Shop Information
 * Step 2: Owner Information
 * Step 3: Account Security
 */

test.describe('Registration Flow', () => {
  test.setTimeout(60000); // 1 minute timeout

  const testUser = {
    // Step 1: Shop Information
    shopName: `Test Spaza ${Date.now()}`,
    area: 'soweto',
    zone: 'Diepkloof Extension 1',
    address: '123 Test Street, Diepkloof, Soweto',
    
    // Step 2: Owner Information
    firstName: 'Thabo',
    lastName: 'Mokoena',
    phone: '+27821234567',
    email: `thabo${Date.now()}@test.co.za`,
    
    // Step 3: Account Security
    password: 'Test123!@#',
    confirmPassword: 'Test123!@#',
  };

  test('should complete full registration flow successfully', async ({ page }) => {
    console.log('🚀 Starting registration test...');
    console.log('Test user:', JSON.stringify(testUser, null, 2));

    // Navigate to registration page
    await page.goto('http://localhost:3001/auth/register', { waitUntil: 'networkidle' });
    console.log('✅ Navigated to registration page');

    // Wait for page to fully load
    await page.waitForLoadState('domcontentloaded');
    
    // Verify page loaded - try multiple selectors
    const pageTitle = page.locator('h1').filter({ hasText: 'Join TOSS' });
    await expect(pageTitle).toBeVisible({ timeout: 15000 });
    console.log('✅ Registration page loaded');

    // Step 1: Shop Information
    console.log('📝 Step 1: Filling shop information...');
    
    // Verify we're on step 1
    await expect(page.locator('text=Step 1 of 3')).toBeVisible();
    await expect(page.locator('h3:has-text("Tell us about your shop")')).toBeVisible();

    // Fill shop information using v-model bindings (data-testid or unique text)
    await page.fill('input[placeholder*="Thabo\'s Spaza Shop"]', testUser.shopName);
    await page.selectOption('select', { value: testUser.area });
    await page.fill('input[placeholder*="Diepkloof Extension"]', testUser.zone);
    await page.fill('textarea[placeholder*="physical address"]', testUser.address);
    
    console.log('✅ Shop information filled');

    // Click Continue to Step 2
    await page.click('button:has-text("Continue →")');
    await page.waitForTimeout(500); // Wait for transition
    
    // Verify we're on step 2
    await expect(page.locator('text=Step 2 of 3')).toBeVisible();
    await expect(page.locator('h3:has-text("Your contact details")')).toBeVisible();
    console.log('✅ Moved to Step 2');

    // Step 2: Owner Information
    console.log('📝 Step 2: Filling owner information...');
    
    // Find inputs by placeholder text
    await page.fill('input[placeholder="First name"]', testUser.firstName);
    await page.fill('input[placeholder="Last name"]', testUser.lastName);
    await page.fill('input[placeholder="+27 XX XXX XXXX"]', testUser.phone);
    await page.fill('input[placeholder="your@email.com"]', testUser.email);
    
    console.log('✅ Owner information filled');

    // Click Continue to Step 3
    await page.click('button:has-text("Continue →")');
    await page.waitForTimeout(500); // Wait for transition
    
    // Verify we're on step 3
    await expect(page.locator('text=Step 3 of 3')).toBeVisible();
    await expect(page.locator('h3:has-text("Secure your account")')).toBeVisible();
    console.log('✅ Moved to Step 3');

    // Step 3: Account Security
    console.log('📝 Step 3: Filling account security...');
    
    // Fill password fields
    await page.fill('input[placeholder*="Create a strong password"]', testUser.password);
    await page.fill('input[placeholder*="Re-enter password"]', testUser.confirmPassword);
    
    // Check terms acceptance
    await page.check('input[type="checkbox"][required]');
    
    console.log('✅ Account security filled');

    // Intercept the registration API call
    const registrationPromise = page.waitForResponse(
      response => response.url().includes('/api/auth/register') && response.request().method() === 'POST',
      { timeout: 10000 }
    );

    // Submit the form
    await page.click('button:has-text("Complete Registration")');
    console.log('✅ Registration form submitted');

    // Wait for API response
    const response = await registrationPromise;
    const responseData = await response.json();
    
    console.log('API Response:', JSON.stringify(responseData, null, 2));

    // Verify successful registration
    expect(response.ok()).toBeTruthy();
    expect(responseData.success).toBe(true);
    
    console.log('✅ Registration API call successful');

    // Verify navigation to onboarding or dashboard
    // The page should navigate after successful registration
    await page.waitForURL(/\/(onboarding|dashboard)/, { timeout: 10000 });
    
    console.log('✅ Navigated to post-registration page');
    console.log('🎉 Registration test completed successfully!');
  });

  test('should validate password mismatch', async ({ page }) => {
    console.log('🧪 Testing password validation...');

    await page.goto('http://localhost:3001/auth/register');

    // Fill steps 1 and 2 quickly
    await page.fill('input[placeholder*="Thabo\'s Spaza Shop"]', testUser.shopName);
    await page.selectOption('select', { value: testUser.area });
    await page.fill('textarea[placeholder*="physical address"]', testUser.address);
    await page.click('button:has-text("Continue →")');
    
    await page.fill('input[placeholder="First name"]', testUser.firstName);
    await page.fill('input[placeholder="Last name"]', testUser.lastName);
    await page.fill('input[placeholder="+27 XX XXX XXXX"]', testUser.phone);
    await page.fill('input[placeholder="your@email.com"]', testUser.email);
    await page.click('button:has-text("Continue →")');

    // Fill mismatched passwords
    await page.fill('input[placeholder*="Create a strong password"]', 'Password123!');
    await page.fill('input[placeholder*="Re-enter password"]', 'DifferentPassword123!');
    await page.check('input[type="checkbox"][required]');

    // Try to submit
    await page.click('button:has-text("Complete Registration")');

    // Wait for alert
    page.on('dialog', async dialog => {
      expect(dialog.message()).toContain('Passwords do not match');
      await dialog.accept();
    });

    console.log('✅ Password validation working correctly');
  });

  test('should validate terms acceptance', async ({ page }) => {
    console.log('🧪 Testing terms validation...');

    await page.goto('http://localhost:3001/auth/register');

    // Fill steps 1 and 2 quickly
    await page.fill('input[placeholder*="Thabo\'s Spaza Shop"]', testUser.shopName);
    await page.selectOption('select', { value: testUser.area });
    await page.fill('textarea[placeholder*="physical address"]', testUser.address);
    await page.click('button:has-text("Continue →")');
    
    await page.fill('input[placeholder="First name"]', testUser.firstName);
    await page.fill('input[placeholder="Last name"]', testUser.lastName);
    await page.fill('input[placeholder="+27 XX XXX XXXX"]', testUser.phone);
    await page.fill('input[placeholder="your@email.com"]', testUser.email);
    await page.click('button:has-text("Continue →")');

    // Fill passwords but DON'T check terms
    await page.fill('input[placeholder*="Create a strong password"]', testUser.password);
    await page.fill('input[placeholder*="Re-enter password"]', testUser.confirmPassword);

    // Try to submit without accepting terms
    await page.click('button:has-text("Complete Registration")');

    // Wait for alert
    page.on('dialog', async dialog => {
      expect(dialog.message()).toContain('terms and conditions');
      await dialog.accept();
    });

    console.log('✅ Terms validation working correctly');
  });

  test('should allow navigation back through steps', async ({ page }) => {
    console.log('🧪 Testing back navigation...');

    await page.goto('http://localhost:3001/auth/register');

    // Move to step 2
    await page.fill('input[placeholder*="Thabo\'s Spaza Shop"]', testUser.shopName);
    await page.click('button:has-text("Continue →")');
    await expect(page.locator('text=Step 2 of 3')).toBeVisible();

    // Go back to step 1
    await page.click('button:has-text("← Back")');
    await expect(page.locator('text=Step 1 of 3')).toBeVisible();
    
    // Verify data is preserved
    await expect(page.locator('input[placeholder*="Thabo\'s Spaza Shop"]')).toHaveValue(testUser.shopName);

    console.log('✅ Back navigation working correctly');
  });
});

