import { test, expect } from '@playwright/test'

test.describe('Security & Audit', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForLoadState('networkidle')
  })

  test('should log successful login attempts', async ({ page, context }) => {
    // Monitor network requests
    const auditLogs: any[] = []
    page.on('request', request => {
      if (request.url().includes('/api/audit/log')) {
        auditLogs.push(request.postDataJSON())
      }
    })

    // Login
    await page.fill('#email', 'admin@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    
    // Wait a bit for audit log to be sent
    await page.waitForTimeout(2000)
    
    // Check that login was logged
    const loginLogs = auditLogs.filter(log => log.action === 'auth.login')
    expect(loginLogs.length).toBeGreaterThan(0)
  })

  test('should log failed login attempts', async ({ page }) => {
    // Monitor network requests
    const auditLogs: any[] = []
    page.on('request', request => {
      if (request.url().includes('/api/audit/log')) {
        auditLogs.push(request.postDataJSON())
      }
    })

    // Try to login with wrong credentials
    await page.fill('#email', 'wrong@email.com')
    await page.fill('#password', 'wrongpassword')
    await page.click('button[type="submit"]')
    
    // Wait for error message
    await page.waitForTimeout(2000)
    
    // Check that failed login was logged
    const failedLoginLogs = auditLogs.filter(log => log.action === 'auth.login_failed')
    expect(failedLoginLogs.length).toBeGreaterThan(0)
  })

  test('should log logout events', async ({ page }) => {
    // Login first
    await page.fill('#email', 'admin@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    await page.waitForURL('**/dashboard', { timeout: 10000 })

    // Monitor network requests
    const auditLogs: any[] = []
    page.on('request', request => {
      if (request.url().includes('/api/audit/log')) {
        auditLogs.push(request.postDataJSON())
      }
    })

    // Logout
    await page.click('[data-testid="user-menu"], button:has-text("Logout")')
    await page.waitForTimeout(2000)
    
    // Check that logout was logged
    const logoutLogs = auditLogs.filter(log => log.action === 'auth.logout')
    expect(logoutLogs.length).toBeGreaterThan(0)
  })

  test('should manage session with activity tracking', async ({ page }) => {
    // Login
    await page.fill('#email', 'admin@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    await page.waitForURL('**/dashboard', { timeout: 10000 })

    // Check that session is initialized
    await page.waitForTimeout(2000)
    
    // Perform some actions to trigger activity updates
    await page.click('text=Sales')
    await page.waitForTimeout(1000)
    await page.click('text=Dashboard')
    await page.waitForTimeout(1000)
    
    // Session should still be active
    const url = page.url()
    expect(url).toContain('/dashboard')
  })

  test('should handle session expiry', async ({ page, context }) => {
    // This test would require manipulating time or using a short-lived token
    // For now, we'll just verify the session validation endpoint exists
    
    // Login
    await page.fill('#email', 'admin@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    await page.waitForURL('**/dashboard', { timeout: 10000 })

    // Make a request to session validation endpoint
    const response = await page.evaluate(async () => {
      const res = await fetch('/api/auth/session/validate')
      return res.json()
    })

    expect(response).toHaveProperty('valid')
  })

  test('should prevent unauthorized access', async ({ page }) => {
    // Try to access protected route without login
    await page.goto('http://localhost:3000/settings')
    
    // Should be redirected to login
    await page.waitForTimeout(2000)
    const url = page.url()
    expect(url).toContain('/login')
  })

  test('should validate token on API requests', async ({ page }) => {
    // Login
    await page.fill('#email', 'admin@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.click('button[type="submit"]')
    await page.waitForURL('**/dashboard', { timeout: 10000 })

    // Make an API request
    const response = await page.evaluate(async () => {
      const res = await fetch('/api/auth/session')
      return {
        status: res.status,
        data: await res.json()
      }
    })

    expect(response.status).toBe(200)
    expect(response.data).toHaveProperty('sessionId')
  })
})

