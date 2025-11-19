import { Page, APIRequestContext } from '@playwright/test';

/**
 * API helper functions for E2E tests
 */
export class ApiHelper {
  constructor(
    private page: Page,
    private apiContext?: APIRequestContext
  ) {}

  /**
   * Get authentication token from localStorage
   */
  async getAuthToken(): Promise<string | null> {
    return await this.page.evaluate(() => {
      return localStorage.getItem('auth_token') || localStorage.getItem('token');
    });
  }

  /**
   * Make authenticated API request
   */
  async apiRequest(method: string, endpoint: string, data?: any) {
    const token = await this.getAuthToken();
    const baseURL = process.env.API_BASE_URL || 'http://localhost:5000';
    
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    };

    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    const response = await fetch(`${baseURL}${endpoint}`, {
      method,
      headers,
      body: data ? JSON.stringify(data) : undefined
    });

    return {
      status: response.status,
      data: await response.json().catch(() => null),
      ok: response.ok
    };
  }

  /**
   * Wait for API call to complete
   */
  async waitForApiCall(urlPattern: string | RegExp, timeout = 10000) {
    await this.page.waitForResponse(
      (response) => {
        const url = response.url();
        if (typeof urlPattern === 'string') {
          return url.includes(urlPattern);
        }
        return urlPattern.test(url);
      },
      { timeout }
    );
  }

  /**
   * Intercept and mock API response
   */
  async mockApiResponse(urlPattern: string | RegExp, mockData: any) {
    await this.page.route(urlPattern, (route) => {
      route.fulfill({
        status: 200,
        contentType: 'application/json',
        body: JSON.stringify(mockData)
      });
    });
  }
}

