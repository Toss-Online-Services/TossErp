# Running E2E Tests

## Quick Start

```bash
# Install Playwright browsers (first time only)
npx playwright install

# Run all tests
npm run test:e2e

# Run specific test suite
npx playwright test tests/e2e/modules/auth.spec.ts

# Run with UI (interactive)
npx playwright test --ui

# Run in debug mode
npx playwright test --debug
```

## Test Suites

### Module Tests
- `modules/auth.spec.ts` - Authentication tests
- `modules/admin.spec.ts` - Admin module tests  
- `modules/retailer.spec.ts` - Retailer module tests
- `modules/supplier.spec.ts` - Supplier module tests
- `modules/driver.spec.ts` - Driver module tests
- `modules/onboarding.spec.ts` - Onboarding tests

### Flow Tests
- `flows/retailer-complete-flow.spec.ts` - Complete retailer workflow
- `flows/supplier-complete-flow.spec.ts` - Complete supplier workflow
- `flows/driver-complete-flow.spec.ts` - Complete driver workflow
- `flows/admin-complete-flow.spec.ts` - Complete admin workflow
- `flows/complete-order-lifecycle.spec.ts` - Cross-role integration

### Smoke Tests
- `smoke.spec.ts` - Basic functionality checks

## Prerequisites

1. **Backend running:** `http://localhost:5000` or `https://localhost:5001`
2. **Frontend running:** `http://localhost:3001`
3. **Database seeded:** Test users must exist

## Test Users

- Admin: `admin@toss.local` / `Admin1!`
- Retailer: `storeowner1@toss.local` / `StoreOwner1!`
- Supplier: `supplier1@toss.local` / `Supplier1!`
- Driver: `driver1@toss.local` / `Driver1!`

## Viewing Results

```bash
# Open HTML report
npx playwright show-report

# View trace
npx playwright show-trace trace.zip
```

## Troubleshooting

If tests fail:
1. Check backend is running
2. Check frontend is running
3. Verify test users exist in database
4. Check selectors match current UI
5. Review screenshots in `test-results/` directory

