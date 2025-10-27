import { test, expect } from '@playwright/test';

const baseUrl = process.env.BASE_URL || 'http://localhost:3001';
const testUser = {
  username: 'testuser',
  password: 'TestPassword123!'
};

test.describe('Authentication Flow', () => {
  test('Login, refresh, logout, verify', async ({ request }) => {
    // Login
    const loginRes = await request.post(`${baseUrl}/api/auth/login`, {
      data: testUser
    });
    expect(loginRes.ok()).toBeTruthy();
    const loginData = await loginRes.json();
    expect(loginData).toHaveProperty('accessToken');
    expect(loginData).toHaveProperty('refreshToken');

    // Verify
    const verifyRes = await request.get(`${baseUrl}/api/auth/verify`, {
      headers: { Authorization: `Bearer ${loginData.accessToken}` }
    });
    expect(verifyRes.status()).toBe(200);

    // Refresh
    const refreshRes = await request.post(`${baseUrl}/api/auth/refresh`, {
      data: { refreshToken: loginData.refreshToken }
    });
    expect(refreshRes.ok()).toBeTruthy();
    const refreshData = await refreshRes.json();
    expect(refreshData).toHaveProperty('accessToken');
    expect(refreshData).toHaveProperty('refreshToken');

    // Logout
    const logoutRes = await request.post(`${baseUrl}/api/auth/logout`, {
      data: { refreshToken: loginData.refreshToken }
    });
    expect(logoutRes.ok()).toBeTruthy();

    // Verify after logout (should fail)
    const verifyRes2 = await request.get(`${baseUrl}/api/auth/verify`, {
      headers: { Authorization: `Bearer ${loginData.accessToken}` }
    });
    expect(verifyRes2.status()).toBe(401);
  });
});
