#!/bin/bash

# TOSS MVP Deployment Script
# Quick deployment to Vercel

echo "🚀 TOSS MVP Deployment Starting..."
echo "================================="

# Check if we're in the toss-web directory
if [ ! -f "nuxt.config.ts" ]; then
    echo "❌ Error: Must run from toss-web directory"
    exit 1
fi

# Check if package.json exists
if [ ! -f "package.json" ]; then
    echo "❌ Error: package.json not found"
    exit 1
fi

# Install dependencies if needed
if [ ! -d "node_modules" ]; then
    echo "📦 Installing dependencies..."
    npm install
fi

# Run type check (optional, won't fail deployment)
echo "🔍 Type checking..."
npm run typecheck || echo "⚠️ Type check warnings (non-blocking)"

# Build the project
echo "🏗️ Building production bundle..."
npm run generate

if [ $? -ne 0 ]; then
    echo "❌ Build failed! Fix errors and try again."
    exit 1
fi

echo "✅ Build successful!"

# Check if Vercel CLI is installed
if ! command -v vercel &> /dev/null; then
    echo "📥 Installing Vercel CLI..."
    npm install -g vercel
fi

# Deploy to Vercel
echo "🌍 Deploying to Vercel..."
vercel --prod

if [ $? -eq 0 ]; then
    echo "================================="
    echo "✅ Deployment successful!"
    echo "🎉 Your TOSS MVP is now live!"
    echo "================================="
else
    echo "❌ Deployment failed!"
    exit 1
fi

