# TOSS E2E Testing Suite

Comprehensive end-to-end testing suite for the TOSS ERP application.

## Test Structure

```
tests/e2e/
├── helpers/           # Test utilities and helpers
│   ├── auth.helper.ts      # Authentication helper functions
│   ├── api.helper.ts      # API request helpers
│   └── test-data.ts       # Test data constants
├── modules/           # Module-specific tests
│   ├── auth.spec.ts       # Authentication module tests
│   ├── admin.spec.ts      # Admin module tests
│   ├── retailer.spec.ts   # Retailer module tests
│   ├── supplier.spec.ts   # Supplier module tests
│   ├── driver.spec.ts     # Driver module tests
│   └── onboarding.spec.ts # Onboarding module tests
└── flows/             # Complete workflow tests
    ├── retailer-complete-flow.spec.ts
    ├── supplier-complete-flow.spec.ts
    ├── driver-complete-flow.spec.ts
    ├── admin-complete-flow.spec.ts
    └── complete-order-lifecycle.spec.ts
```

## Running Tests

### Run All Tests
```bash
npm run test:e2e
# or
pnpm test:e2e
```

### Run Specific Test Suite
```bash
# Run authentication tests only
npx playwright test tests/e2e/modules/auth.spec.ts

# Run retailer module tests
npx playwright test tests/e2e/modules/retailer.spec.ts

# Run complete flow tests
npx playwright test tests/e2e/flows/
```

### Run Tests in Specific Browser
```bash
# Chrome only
npx playwright test --project=chromium

# Firefox only
npx playwright test --project=firefox

# Safari only
npx playwright test --project=webkit
```

### Run Tests in UI Mode
```bash
npx playwright test --ui
```

### Run Tests in Debug Mode
```bash
npx playwright test --debug
```

### Run Tests with Specific Tags
```bash
# Run only smoke tests
npx playwright test --grep @smoke

# Run only critical path tests
npx playwright test --grep @critical
```

## Test Coverage

### Authentication Module
- ✅ Login flow (all roles)
- ✅ Logout flow
- ✅ Session management
- ✅ Role-based access control
- ✅ Invalid credentials handling

### Admin Module
- ✅ Dashboard statistics
- ✅ User management (list, search, filter)
- ✅ User activation/deactivation
- ✅ Role assignment
- ✅ Order overview and filtering

### Retailer Module
- ✅ Product management (CRUD)
- ✅ Product search and filtering
- ✅ POS interface
- ✅ Cart management
- ✅ Payment processing
- ✅ Cash rounding
- ✅ Purchase order creation
- ✅ Order tracking
- ✅ Inventory management
- ✅ Stock level viewing
- ✅ Low stock indicators

### Supplier Module
- ✅ Incoming order list
- ✅ Order details view
- ✅ Order acceptance/rejection
- ✅ Mark as ready for pickup
- ✅ Mark as shipped
- ✅ Order status filtering

### Driver Module
- ✅ Delivery list
- ✅ Delivery details
- ✅ Accept delivery
- ✅ Mark as picked up
- ✅ Mark as delivered
- ✅ Delivery confirmation
- ✅ Status filtering

### Onboarding Module
- ✅ Retailer onboarding flow
- ✅ Supplier onboarding flow
- ✅ Driver onboarding flow
- ✅ Onboarding persistence
- ✅ Redirect after completion

### Complete Flows
- ✅ Retailer complete workflow (login → products → POS → inventory)
- ✅ Supplier complete workflow (login → orders → accept → ship)
- ✅ Driver complete workflow (login → deliveries → pickup → deliver)
- ✅ Admin complete workflow (login → dashboard → users → orders)
- ✅ Full order lifecycle (retailer → supplier → driver → retailer)

## Test Data

Test users are defined in `helpers/test-data.ts`:
- **Admin:** `admin@toss.local` / `Admin1!`
- **Retailer:** `storeowner1@toss.local` / `StoreOwner1!`
- **Supplier:** `supplier1@toss.local` / `Supplier1!`
- **Driver:** `driver1@toss.local` / `Driver1!`

## Prerequisites

1. **Backend Server:** Must be running on `http://localhost:5000` or `https://localhost:5001`
2. **Frontend Server:** Must be running on `http://localhost:3001`
3. **Database:** PostgreSQL must be running with seeded data
4. **Test Users:** Users must exist in database (seeded by default)

## Configuration

Test configuration is in `playwright.config.ts`:
- Base URL: `http://localhost:3001`
- Timeout: 60 seconds per test
- Retries: 0 (local), 2 (CI)
- Browsers: Chromium, Firefox, WebKit, Mobile Chrome, Mobile Safari

## Writing New Tests

### Example Test Structure
```typescript
import { test, expect } from '@playwright/test';
import { AuthHelper } from '../helpers/auth.helper';

test.describe('My Module', () => {
  let authHelper: AuthHelper;

  test.beforeEach(async ({ page }) => {
    authHelper = new AuthHelper(page);
    await authHelper.loginAs('retailer');
    await authHelper.waitForAuth();
  });

  test('should do something', async ({ page }) => {
    await page.goto('/my-page');
    // Test implementation
  });
});
```

### Best Practices

1. **Use Helpers:** Always use `AuthHelper` and other helpers for common operations
2. **Wait for Elements:** Always wait for elements before interacting
3. **Use Data Attributes:** Prefer `data-testid` attributes for selectors
4. **Clean Up:** Clean up test data after tests
5. **Isolated Tests:** Each test should be independent
6. **Descriptive Names:** Use descriptive test names that explain what is being tested

## Debugging Tests

### View Test Report
```bash
npx playwright show-report
```

### Screenshots and Videos
Screenshots and videos are automatically saved on test failure in:
- `test-results/` directory

### Trace Viewer
```bash
npx playwright show-trace trace.zip
```

## CI/CD Integration

Tests are configured to run in CI environments:
- Retries: 2
- Workers: 1 (sequential execution)
- HTML report generation
- JSON results output

## Known Issues

1. **Onboarding Tests:** May need to skip onboarding if already completed
2. **Order Creation:** Requires existing products and suppliers in database
3. **Timing:** Some tests may need adjustment based on API response times

## Maintenance

- Update test data in `helpers/test-data.ts` when data models change
- Update selectors when UI changes
- Add new test cases for new features
- Keep test helpers up to date with application changes

