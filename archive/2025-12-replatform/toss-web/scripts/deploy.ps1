# TOSS MVP Deployment Script for Windows PowerShell
# Quick deployment to Vercel

Write-Host "🚀 TOSS MVP Deployment Starting..." -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green

# Check if we're in the toss-web directory
if (-not (Test-Path "nuxt.config.ts")) {
    Write-Host "❌ Error: Must run from toss-web directory" -ForegroundColor Red
    exit 1
}

# Check if package.json exists
if (-not (Test-Path "package.json")) {
    Write-Host "❌ Error: package.json not found" -ForegroundColor Red
    exit 1
}

# Install dependencies if needed
if (-not (Test-Path "node_modules")) {
    Write-Host "📦 Installing dependencies..." -ForegroundColor Yellow
    npm install
}

# Run type check (optional, won't fail deployment)
Write-Host "🔍 Type checking..." -ForegroundColor Yellow
npm run typecheck
if ($LASTEXITCODE -ne 0) {
    Write-Host "⚠️ Type check warnings (non-blocking)" -ForegroundColor Yellow
}

# Build the project
Write-Host "🏗️ Building production bundle..." -ForegroundColor Yellow
npm run generate

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed! Fix errors and try again." -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build successful!" -ForegroundColor Green

# Check if Vercel CLI is installed
$vercelExists = Get-Command vercel -ErrorAction SilentlyContinue
if (-not $vercelExists) {
    Write-Host "📥 Installing Vercel CLI..." -ForegroundColor Yellow
    npm install -g vercel
}

# Deploy to Vercel
Write-Host "🌍 Deploying to Vercel..." -ForegroundColor Yellow
vercel --prod

if ($LASTEXITCODE -eq 0) {
    Write-Host "=================================" -ForegroundColor Green
    Write-Host "✅ Deployment successful!" -ForegroundColor Green
    Write-Host "🎉 Your TOSS MVP is now live!" -ForegroundColor Green
    Write-Host "=================================" -ForegroundColor Green
} else {
    Write-Host "❌ Deployment failed!" -ForegroundColor Red
    exit 1
}

