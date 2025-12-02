# Sales and Sales Documents - Comprehensive E2E Test Suite

## Overview

This comprehensive test suite validates the complete sales and invoice workflow, from sale creation through document generation and UI verification.

## Test File Location

`tests/e2e/sales-documents-comprehensive.spec.ts`

## Test Coverage

### ✅ API Integration Tests (4 tests)

1. **Sale Creation and Verification**
   - Creates a sale via API
   - Verifies sale exists and can be retrieved
   - Validates sale data structure

2. **Invoice Creation from Sale**
   - Creates a sale
   - Generates an invoice from the sale
   - Verifies invoice document exists in API
   - Validates invoice details (number, customer, total, dates)

3. **POS Sale and Receipt Generation**
   - Creates a POS sale (Cash payment)
   - Verifies receipt document generation
   - Tests receipt retrieval

4. **Sales Documents API**
   - Fetches invoices with pagination
   - Fetches receipts
   - Verifies document structure and required fields

### ✅ UI Verification Tests (6 tests)

1. **Page Display**
   - Verifies Sales Invoices page loads correctly
   - Checks all UI components (header, buttons, stats, filters)
   - Validates responsive design handling

2. **Create Sale and Invoice Flow**
   - Creates sale and invoice via API
   - Verifies invoice appears in UI
   - Tests search functionality

3. **New Invoice Modal**
   - Opens modal when button clicked
   - Verifies form fields (Invoice Number, Due Date, Customer, etc.)
   - Validates action buttons

4. **Filtering and Search**
   - Tests status filter dropdown
   - Tests time period filter
   - Tests search input functionality
   - Handles responsive design (hidden elements)

5. **Statistics Display**
   - Verifies invoice statistics cards
   - Checks counts display correctly

### ✅ Complete End-to-End Flow (1 test)

**Full Sales Flow: Sale → Invoice → UI Verification**
- Step 1: Create sale via API
- Step 2: Verify sale exists
- Step 3: Create invoice from sale
- Step 4: Verify invoice via API
- Step 5: Navigate to invoices page
- Step 6: Verify invoice appears in UI
- Step 7: Search for invoice

## Test Results

**Status:** ✅ All 13 tests passing

**Execution Time:** ~3 minutes (Chromium)

**Test Data Created:**
- Sales: Multiple test sales created
- Invoices: Multiple test invoices created
- Receipts: POS sales for receipt testing

## Key Features

### Robust Error Handling
- Handles both camelCase and PascalCase API responses
- Gracefully handles hidden elements (responsive design)
- Provides detailed logging for debugging

### API Response Handling
The tests handle both response formats:
- `Id` (PascalCase) and `id` (camelCase)
- `DocumentNumber` and `documentNumber`
- `TotalAmount` and `totalAmount`
- etc.

### UI Element Detection
- Waits for elements to be visible before interaction
- Handles responsive design where elements may be hidden
- Multiple selector fallbacks for reliability

### Timeout Management
- 120-second timeout per test
- Appropriate waits for API processing
- DOM content loaded vs network idle strategies

## Running the Tests

### Run All Tests
```bash
npx playwright test tests/e2e/sales-documents-comprehensive.spec.ts --project=chromium
```

### Run Specific Test Suite
```bash
# API tests only
npx playwright test tests/e2e/sales-documents-comprehensive.spec.ts --project=chromium --grep="API Integration"

# UI tests only
npx playwright test tests/e2e/sales-documents-comprehensive.spec.ts --project=chromium --grep="UI Verification"

# Complete flow
npx playwright test tests/e2e/sales-documents-comprehensive.spec.ts --project=chromium --grep="complete full sales flow"
```

### With Custom Timeout
```bash
npx playwright test tests/e2e/sales-documents-comprehensive.spec.ts --project=chromium --timeout=120000
```

## Test Helpers

Located in `tests/e2e/helpers/sales.helper.ts`:

- `createSaleAPI()` - Create a sale
- `createInvoiceAPI()` - Create an invoice from a sale
- `getSalesDocumentsAPI()` - Fetch sales documents (invoices/receipts)
- `getSaleAPI()` - Get a specific sale by ID
- `generateSaleData()` - Generate test sale data
- `generateInvoiceData()` - Generate test invoice data

## Configuration

- **Base URL:** `http://localhost:3001` (configurable via `BASE_URL` env var)
- **API Base:** `https://localhost:5001/api` (configurable via `API_BASE_URL` env var)
- **Shop ID:** 1 (default test shop)
- **Timeout:** 120 seconds per test

## Notes

- Test data is created but not automatically cleaned up (backend handles cleanup)
- Some UI elements may be hidden on mobile/responsive views (tests handle this gracefully)
- Invoice visibility in UI may require page refresh (tests handle this)
- Receipts may be auto-generated asynchronously (tests account for this)

## Cleanup

Old test files have been removed:
- ✅ Deleted: `tests/e2e/sales-invoices.spec.ts` (replaced by comprehensive suite)
- ✅ Cleaned: `test-results/` directory
- ✅ Cleaned: `playwright-report/` directory

