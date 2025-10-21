# 🧪 Stock Module Test Runner Guide

Quick reference for running tests on the TOSS ERP Stock Module.

---

## 🚀 QUICK START

### **Install Dependencies:**
```bash
pnpm install
```

### **Run All Unit Tests:**
```bash
pnpm test
```

### **Run All E2E Tests:**
```bash
pnpm test:e2e
```

---

## 📋 SPECIFIC TEST FILES

### **Items Page Tests:**
```bash
# Unit tests
pnpm test test/stock/items.test.ts

# E2E tests (Items section)
pnpm test:e2e tests/e2e/stock.spec.ts -g "Items Page"
```

### **Movements Page Tests:**
```bash
# Unit tests
pnpm test test/stock/movements.test.ts

# E2E tests (Movements section)
pnpm test:e2e tests/e2e/stock.spec.ts -g "Stock Movements Page"
```

### **Stock Movement Modal Tests:**
```bash
# Unit tests
pnpm test test/stock/StockMovementModal.test.ts

# E2E tests (Modal section)
pnpm test:e2e tests/e2e/stock.spec.ts -g "Stock Movement Modal"
```

---

## 🎯 TEST CATEGORIES

### **Mobile Layout Tests:**
```bash
pnpm test:e2e tests/e2e/stock.spec.ts -g "Mobile Layout"
```

### **Accessibility Tests:**
```bash
pnpm test:e2e tests/e2e/stock.spec.ts -g "Accessibility"
```

### **Performance Tests:**
```bash
pnpm test:e2e tests/e2e/stock.spec.ts -g "Performance"
```

---

## 🔍 ADVANCED OPTIONS

### **Watch Mode (Auto-rerun on changes):**
```bash
pnpm test --watch
```

### **Coverage Report:**
```bash
pnpm test --coverage
```

### **Verbose Output:**
```bash
pnpm test --verbose
```

### **Run Tests in Specific Browser (E2E):**
```bash
# Chrome
pnpm test:e2e --project=chromium

# Firefox
pnpm test:e2e --project=firefox

# Safari
pnpm test:e2e --project=webkit
```

### **Headed Mode (See browser):**
```bash
pnpm test:e2e --headed
```

### **Debug Mode (Playwright):**
```bash
pnpm test:e2e --debug
```

### **UI Mode (Interactive):**
```bash
pnpm test:e2e --ui
```

---

## 📊 GENERATE REPORTS

### **HTML Coverage Report:**
```bash
pnpm test --coverage --reporter=html
```

### **Playwright HTML Report:**
```bash
pnpm test:e2e --reporter=html
```

---

## 🐛 DEBUGGING FAILED TESTS

### **Run Only Failed Tests:**
```bash
pnpm test --run --reporter=verbose --bail
```

### **Run Single Test:**
```bash
pnpm test test/stock/items.test.ts -t "should display page header"
```

### **Show Console Logs:**
```bash
pnpm test --reporter=verbose
```

### **Playwright Trace Viewer:**
```bash
# Generate trace
pnpm test:e2e --trace on

# View trace
npx playwright show-trace trace.zip
```

---

## 🔄 CI/CD INTEGRATION

### **GitHub Actions Example:**
```yaml
name: Stock Module Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: pnpm/action-setup@v2
      - uses: actions/setup-node@v3
        with:
          node-version: '18'
          cache: 'pnpm'
      
      - name: Install dependencies
        run: pnpm install
      
      - name: Run unit tests
        run: pnpm test --coverage
      
      - name: Install Playwright
        run: pnpm exec playwright install --with-deps
      
      - name: Run E2E tests
        run: pnpm test:e2e
      
      - name: Upload coverage
        uses: codecov/codecov-action@v3
```

---

## 🎨 TEST CONFIGURATION

### **Vitest Config (`vitest.config.ts`):**
```typescript
import { defineConfig } from 'vitest/config'
import vue from '@vitejs/plugin-vue'
import path from 'path'

export default defineConfig({
  plugins: [vue()],
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: ['./test/setup.ts'],
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      include: ['pages/stock/**', 'components/stock/**'],
      exclude: ['**/*.test.ts', '**/*.spec.ts']
    }
  },
  resolve: {
    alias: {
      '~': path.resolve(__dirname, './'),
      '@': path.resolve(__dirname, './')
    }
  }
})
```

### **Playwright Config (`playwright.config.ts`):**
```typescript
import { defineConfig, devices } from '@playwright/test'

export default defineConfig({
  testDir: './tests/e2e',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  use: {
    baseURL: 'http://localhost:3001',
    trace: 'on-first-retry',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
    {
      name: 'Mobile Chrome',
      use: { ...devices['Pixel 5'] },
    },
    {
      name: 'Mobile Safari',
      use: { ...devices['iPhone 12'] },
    },
  ],
  webServer: {
    command: 'pnpm dev',
    url: 'http://localhost:3001',
    reuseExistingServer: !process.env.CI,
  },
})
```

---

## 📈 EXPECTED OUTPUT

### **Successful Test Run:**
```
✓ test/stock/items.test.ts (50 tests) 1234ms
  ✓ Page Layout (5 tests)
  ✓ Stats Cards (4 tests)
  ✓ Search and Filters (5 tests)
  ...

✓ test/stock/movements.test.ts (45 tests) 987ms
  ✓ Page Layout (2 tests)
  ✓ Quick Action Buttons (8 tests)
  ...

Test Files  3 passed (3)
     Tests  135 passed (135)
  Start at  10:30:45
  Duration  3.21s
```

### **E2E Test Output:**
```
Running 45 tests using 4 workers

  ✓ [chromium] › stock.spec.ts:20:3 › Mobile Layout › should display mobile header correctly (2.3s)
  ✓ [chromium] › stock.spec.ts:30:3 › Mobile Layout › should display bottom navigation on mobile (1.8s)
  ...

  45 passed (1.2m)
```

---

## 🎯 COVERAGE GOALS

- **Lines:** > 80%
- **Functions:** > 80%
- **Branches:** > 75%
- **Statements:** > 80%

---

## 📞 TROUBLESHOOTING

### **Tests Failing Locally:**
1. Clear cache: `pnpm test --clearCache`
2. Reinstall: `rm -rf node_modules && pnpm install`
3. Check Node version: `node -v` (should be v18+)

### **E2E Tests Hanging:**
1. Check dev server is running: `pnpm dev`
2. Clear Playwright cache: `pnpm exec playwright install`
3. Run in headed mode: `pnpm test:e2e --headed`

### **Coverage Not Generating:**
1. Check coverage config in `vitest.config.ts`
2. Run with verbose: `pnpm test --coverage --reporter=verbose`

---

## ✅ PRE-COMMIT CHECKLIST

Before committing code, run:

```bash
# 1. Lint
pnpm lint

# 2. Type check
pnpm typecheck

# 3. Run unit tests
pnpm test

# 4. Run E2E tests
pnpm test:e2e

# 5. Build
pnpm build
```

---

**Happy Testing! 🎉**

