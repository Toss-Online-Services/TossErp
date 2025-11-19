import { Page } from '@playwright/test';

/**
 * Authentication helper functions for E2E tests
 */
export class AuthHelper {
  constructor(private page: Page) {}

  /**
   * Login as a specific user role
   */
  async loginAs(role: 'admin' | 'retailer' | 'supplier' | 'driver', email?: string, password?: string) {
    const credentials = this.getCredentials(role, email, password);
    
    await this.page.goto('/login');
    
    // Wait for page to load
    await this.page.waitForLoadState('networkidle', { timeout: 10000 });
    
    // Try multiple selector patterns for email input
    const emailSelectors = [
      'input[type="email"]',
      'input[name="email"]',
      'input[placeholder*="email" i]',
      'input[placeholder*="Email" i]',
      'input#email',
      'input.v-field__input',
      'input.form-input'
    ];
    
    let emailInput = null;
    for (const selector of emailSelectors) {
      emailInput = this.page.locator(selector).first();
      if (await emailInput.isVisible({ timeout: 2000 }).catch(() => false)) {
        break;
      }
    }
    
    if (!emailInput || !(await emailInput.isVisible({ timeout: 1000 }).catch(() => false))) {
      // Fallback: try to find any input and fill it
      const allInputs = this.page.locator('input').all();
      const inputs = await allInputs;
      if (inputs.length >= 2) {
        await inputs[0].fill(credentials.email);
        await inputs[1].fill(credentials.password);
      } else {
        throw new Error('Could not find login form inputs');
      }
    } else {
      await emailInput.fill(credentials.email);
      
      // Try multiple selector patterns for password input
      const passwordSelectors = [
        'input[type="password"]',
        'input[name="password"]',
        'input[placeholder*="password" i]',
        'input[placeholder*="Password" i]',
        'input#password'
      ];
      
      let passwordInput = null;
      for (const selector of passwordSelectors) {
        passwordInput = this.page.locator(selector).first();
        if (await passwordInput.isVisible({ timeout: 2000 }).catch(() => false)) {
          break;
        }
      }
      
      if (passwordInput && await passwordInput.isVisible({ timeout: 1000 }).catch(() => false)) {
        await passwordInput.fill(credentials.password);
      } else {
        // Fallback: find password input after email
        const allInputs = this.page.locator('input').all();
        const inputs = await allInputs;
        if (inputs.length >= 2) {
          await inputs[1].fill(credentials.password);
        }
      }
    }
    
    // Click submit button
    const submitSelectors = [
      'button[type="submit"]',
      'button:has-text("Login")',
      'button:has-text("Sign in")',
      'button:has-text("Log in")',
      'button.btn-primary',
      'button.primary'
    ];
    
    let submitted = false;
    for (const selector of submitSelectors) {
      const button = this.page.locator(selector).first();
      if (await button.isVisible({ timeout: 2000 }).catch(() => false)) {
        await button.click();
        submitted = true;
        break;
      }
    }
    
    if (!submitted) {
      // Fallback: press Enter on password field
      await this.page.keyboard.press('Enter');
    }
    
    // Wait for navigation after login (with longer timeout)
    try {
      await this.page.waitForURL(/\/(dashboard|retailer|supplier|driver|admin|onboarding)/, { timeout: 15000 });
    } catch (e) {
      // If still on login page, check for error messages
      const currentUrl = this.page.url();
      if (currentUrl.includes('/login')) {
        const errorMsg = await this.page.locator('text=/error|invalid|incorrect/i').first().textContent().catch(() => null);
        throw new Error(`Login failed. Still on login page. Error: ${errorMsg || 'Unknown'}`);
      }
    }
    
    return credentials;
  }

  /**
   * Logout current user
   */
  async logout() {
    // Try multiple logout button locations
    const logoutSelectors = [
      'button:has-text("Logout")',
      'button:has-text("Sign out")',
      '[data-testid="logout-button"]',
      'a:has-text("Logout")',
      '.logout-button'
    ];

    for (const selector of logoutSelectors) {
      const button = await this.page.$(selector);
      if (button) {
        await button.click();
        await this.page.waitForURL('/login', { timeout: 5000 });
        return;
      }
    }

    // Fallback: navigate to logout endpoint
    await this.page.goto('/logout');
    await this.page.waitForURL('/login', { timeout: 5000 });
  }

  /**
   * Get credentials for a role
   */
  private getCredentials(role: 'admin' | 'retailer' | 'supplier' | 'driver', email?: string, password?: string) {
    const defaultCredentials = {
      admin: { email: 'admin@toss.local', password: 'Admin1!' },
      retailer: { email: 'storeowner1@toss.local', password: 'StoreOwner1!' },
      supplier: { email: 'supplier1@toss.local', password: 'Supplier1!' },
      driver: { email: 'driver1@toss.local', password: 'Driver1!' }
    };

    return {
      email: email || defaultCredentials[role].email,
      password: password || defaultCredentials[role].password
    };
  }

  /**
   * Check if user is logged in
   */
  async isLoggedIn(): Promise<boolean> {
    const currentUrl = this.page.url();
    return !currentUrl.includes('/login') && !currentUrl.includes('/register');
  }

  /**
   * Wait for authentication to complete
   */
  async waitForAuth() {
    // Wait for user data to be loaded (check for user menu or dashboard)
    await this.page.waitForSelector('nav, [data-testid="user-menu"], .dashboard, .sidebar', { timeout: 10000 });
  }
}

