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
    await this.page.fill('input[type="email"], input[name="email"]', credentials.email);
    await this.page.fill('input[type="password"], input[name="password"]', credentials.password);
    await this.page.click('button[type="submit"], button:has-text("Login"), button:has-text("Sign in")');
    
    // Wait for navigation after login
    await this.page.waitForURL(/\/(dashboard|retailer|supplier|driver|admin)/, { timeout: 10000 });
    
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

