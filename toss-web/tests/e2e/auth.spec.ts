import { test, expect } from '@playwright/test'

test.describe('Authentication', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('http://localhost:3000/')
  })

  test('should redirect to login when not authenticated', async ({ page }) => {
    // Try to access a protected route (dashboard)
    await page.goto('http://localhost:3000/dashboard')
    
    // Should redirect to login
    await page.waitForURL('**/login', { timeout: 10000 })
    await expect(page).toHaveURL(/.*login/)
  })

  test('should login successfully with valid credentials', async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForLoadState('networkidle')
    
    // Fill login form using input IDs
    await page.fill('#email', 'owner@demo.toss.co.za')
    await page.fill('#password', 'password123')
    
    // Submit form
    await page.click('button[type="submit"]')
    
    // Should redirect to dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    await expect(page).toHaveURL(/.*dashboard/)
  })

  test('should use demo credentials button', async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForLoadState('networkidle')
    
    // Click demo credentials button
    await page.click('text=Use demo credentials')
    
    // Verify fields are filled
    const emailValue = await page.inputValue('#email')
    const passwordValue = await page.inputValue('#password')
    
    expect(emailValue).toBe('owner@demo.toss.co.za')
    expect(passwordValue).toBe('password123')
    
    // Submit form
    await page.click('button[type="submit"]')
    
    // Should redirect to dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    await expect(page).toHaveURL(/.*dashboard/)
  })

  test('should remember login with remember me checkbox', async ({ page }) => {
    await page.goto('http://localhost:3000/login')
    await page.waitForLoadState('networkidle')
    
    // Fill login form and check remember me
    await page.fill('#email', 'owner@demo.toss.co.za')
    await page.fill('#password', 'password123')
    await page.check('#remember-me')
    await page.click('button[type="submit"]')
    
    // Wait for dashboard
    await page.waitForURL('**/dashboard', { timeout: 10000 })
    await expect(page).toHaveURL(/.*dashboard/)
  })
})
