# TOSS ERP-III Frontend Deployment Script (PowerShell)
# This script builds and prepares the Nuxt application for deployment

$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting TOSS ERP-III Frontend Deployment..." -ForegroundColor Cyan

# Check if we're in the right directory
if (-not (Test-Path "nuxt.config.ts")) {
    Write-Host "Error: nuxt.config.ts not found. Please run this script from the toss-web-app directory." -ForegroundColor Red
    exit 1
}

# Step 1: Install dependencies
Write-Host "📦 Installing dependencies..." -ForegroundColor Yellow
pnpm install --frozen-lockfile
if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to install dependencies." -ForegroundColor Red
    exit 1
}

# Step 2: Run type checking
Write-Host "🔍 Running type check..." -ForegroundColor Yellow
pnpm run typecheck
if ($LASTEXITCODE -ne 0) {
    Write-Host "Type check failed. Please fix errors before deploying." -ForegroundColor Red
    exit 1
}

# Step 3: Run linter
Write-Host "🧹 Running linter..." -ForegroundColor Yellow
pnpm run lint
if ($LASTEXITCODE -ne 0) {
    Write-Host "Linter found issues. Continuing anyway..." -ForegroundColor Yellow
}

# Step 4: Run tests
Write-Host "🧪 Running tests..." -ForegroundColor Yellow
pnpm run test
if ($LASTEXITCODE -ne 0) {
    Write-Host "Some tests failed. Continuing anyway..." -ForegroundColor Yellow
}

# Step 5: Build for production
Write-Host "🏗️  Building for production..." -ForegroundColor Yellow
pnpm run build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed." -ForegroundColor Red
    exit 1
}

# Step 6: Verify build output
if (-not (Test-Path "dist/toss-web-app")) {
    Write-Host "Error: Build output not found. Build may have failed." -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build completed successfully!" -ForegroundColor Green
Write-Host "📁 Build output: dist/toss-web-app" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Review the build output in dist/toss-web-app"
Write-Host "2. Deploy to your hosting platform (Vercel, Netlify, etc.)"
Write-Host "3. Ensure environment variables are set:"
Write-Host "   - NUXT_PUBLIC_API_BASE (backend API URL)"
Write-Host "   - Other required environment variables"

