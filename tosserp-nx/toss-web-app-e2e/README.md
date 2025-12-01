# E2E Tests for TOSS Web App

End-to-end tests using Playwright for the TOSS ERP-III frontend.

## Prerequisites

1. **Start the dev server** before running tests:
   ```bash
   nx serve toss-web-app
   ```
   The server should be running on `http://localhost:4200`

2. **Install Playwright browsers** (if not already installed):
   ```bash
   npx playwright install
   ```

## Running Tests

### Using Nx (Recommended)

```bash
# Run all E2E tests
nx e2e toss-web-app-e2e

# Run with specific browser
nx e2e toss-web-app-e2e --project=chromium
```

### Using Playwright Directly

```bash
# Set BASE_URL environment variable
$env:BASE_URL='http://localhost:4200'

# Run tests
npx playwright test

# Run specific test file
npx playwright test src/example.spec.ts

# Run in headed mode (see browser)
npx playwright test --headed

# Run in debug mode
npx playwright test --debug
```

## Test Structure

Tests are located in `src/` directory:
- `example.spec.ts` - Basic smoke tests for app loading and navigation

## Configuration

- **Config file**: `playwright.config.ts`
- **Base URL**: `http://localhost:4200` (configurable via `BASE_URL` env var)
- **Browsers**: Chromium, Firefox, WebKit (Safari)
- **Timeout**: 30 seconds per test

## Troubleshooting

### Server not starting
- Ensure port 4200 is available
- Check that `nx serve toss-web-app` works independently
- Verify Nuxt app builds successfully: `nx build toss-web-app`

### Tests timing out
- Ensure dev server is fully started before running tests
- Increase timeout in `playwright.config.ts` if needed
- Check browser console for errors

### Port conflicts
- If port 4200 is in use, the server will try 4201
- Update `BASE_URL` environment variable to match the actual port
- Or kill processes using the port: `netstat -ano | findstr :4200`

## CI/CD

For CI environments, set `CI=true` environment variable. The webServer will:
- Not reuse existing servers
- Start a fresh server for each test run
- Have extended timeouts

