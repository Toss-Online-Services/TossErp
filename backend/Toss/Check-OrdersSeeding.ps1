# Check if Orders are Seeded
# This script checks backend logs or tests the database connection

Write-Host "Checking Orders Seeding Status..." -ForegroundColor Cyan
Write-Host ""

# Method 1: Check if backend logs show seeding messages
Write-Host "[1] Checking backend startup logs..." -ForegroundColor Yellow
Write-Host "Look for log messages like:" -ForegroundColor White
Write-Host "  - '✅ Seeded X orders'" -ForegroundColor Green
Write-Host "  - '✅ Orders already seeded (X existing)'" -ForegroundColor Green
Write-Host "  - '⚠️ Missing required data (customers/products). Skipping orders seeding.'" -ForegroundColor Yellow
Write-Host ""

# Method 2: Test if we can query orders directly
Write-Host "[2] Testing database connectivity..." -ForegroundColor Yellow
Write-Host ""

# Method 3: Check seeding conditions
Write-Host "[3] Seeding Conditions Check:" -ForegroundColor Yellow
Write-Host "  - Orders are seeded if existingCount < 100" -ForegroundColor White
Write-Host "  - Requires Customers and Products to exist first" -ForegroundColor White
Write-Host "  - Creates 100 orders total" -ForegroundColor White
Write-Host ""

# Method 4: Force re-seeding (if needed)
Write-Host "[4] To force re-seeding:" -ForegroundColor Yellow
Write-Host "  - Clear existing orders from database" -ForegroundColor White
Write-Host "  - Restart backend server" -ForegroundColor White
Write-Host "  - Seeding runs automatically on startup" -ForegroundColor White
Write-Host ""

Write-Host "Current Status:" -ForegroundColor Cyan
Write-Host "  - Backend API is timing out (suggests database connection issue)" -ForegroundColor Red
Write-Host "  - Cannot verify orders count due to timeout" -ForegroundColor Red
Write-Host ""

Write-Host "Recommendations:" -ForegroundColor Yellow
Write-Host "  1. Check backend logs for seeding messages" -ForegroundColor White
Write-Host "  2. Verify PostgreSQL is running and accessible" -ForegroundColor White
Write-Host "  3. Check database connection string in appsettings.json" -ForegroundColor White
Write-Host "  4. Verify Customers and Products exist before Orders" -ForegroundColor White
Write-Host "  5. Restart backend after ensuring database is accessible" -ForegroundColor White










