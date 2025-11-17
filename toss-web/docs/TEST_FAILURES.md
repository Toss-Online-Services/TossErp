# Test Failures Catalog (Vitest Run)

Date: 2025-11-10
Command: `pnpm test --reporter verbose`

## Summary

- Test files: 19 total (16 failed, 3 passed)
- Tests: 35 total (18 failed, 17 passed)

## Categories

### 1) E2E tests executed under Vitest (should be Playwright)
Errors like: “Playwright Test did not expect test.describe() to be called here.”

- tests/auth.e2e.test.ts
- tests/e2e/registration.e2e.test.ts
- tests/e2e/sales-documents-comprehensive.spec.ts
- tests/e2e/stock.spec.ts
- tests/e2e/toss-complete-flow.e2e.test.ts
- tests/e2e/toss-complete-workflow.e2e.test.ts

Notes:
- These belong to the Playwright runner (`pnpm test:e2e`), not Vitest.
- Consider moving E2E specs under a dedicated e2e folder with its own config or filter them out of Vitest.

### 2) Vue SFC transform/import errors
Errors like: “Failed to parse source for import analysis… Install @vitejs/plugin-vue to handle .vue files.”

- tests/buying/pages/dashboard.test.ts
- tests/buying/pages/orders.test.ts
- test/stock/items.test.ts

Notes:
- Vitest/Vite config likely missing `@vitejs/plugin-vue` and/or proper test environment config.
- Ensure `vitest.config.ts` includes `plugins: [vue()]` and `environment: 'happy-dom'` or jsdom.

### 3) Missing component file / incorrect path
Error: “Failed to resolve import …/components/stock/StockMovementModal.vue”

- test/stock/StockMovementModal.test.ts

Notes:
- Verify the component path or add the missing SFC.

### 4) Composable not defined / mocking gaps
Error: `ReferenceError: useApi is not defined`

- tests/buying/composables/useBuyingAPI.test.ts (multiple spec failures)

Notes:
- Add a test double for `useApi()` or import the actual composable used by `useBuyingAPI`.

### 5) API base URL assertion mismatch
Assertion comparing relative vs absolute URL in `$fetch` mock

- tests/sales/composables/useProductsAPI.test.ts

Notes:
- The code calls `$fetch` with an absolute URL (`https://localhost:5001/api/Inventory/products`) while the test expects a relative path.
- Align tests with runtime config or update code to use URL join logic.

## Immediate Triage Recommendations

1. Exclude E2E Playwright specs from Vitest by glob or move them under a separate folder ignored by Vitest.
2. Add `@vitejs/plugin-vue` to the Vitest config and ensure SFC transform is active in tests.
3. Fix missing component path for `StockMovementModal.vue` or provide a mock.
4. Mock `useApi()` in composable tests or adapt implementation to inject dependencies.
5. Harmonize `$fetch` URL expectations between tests and implementation.

---
This file will be updated as fixes land to keep the failure inventory current.