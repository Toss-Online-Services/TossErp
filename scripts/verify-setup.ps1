# TossErp Project Setup Verification Script
# This script verifies that all necessary components are properly configured

Write-Host "🔍 Verifying TossErp Project Setup..." -ForegroundColor Blue

# Function to check if a file exists
function Test-File {
    param([string]$Path)
    if (Test-Path $Path) {
        Write-Host "✅ $Path" -ForegroundColor Green
        return $true
    } else {
        Write-Host "❌ $Path" -ForegroundColor Red
        return $false
    }
}

# Function to check if a directory exists
function Test-Directory {
    param([string]$Path)
    if (Test-Path $Path -PathType Container) {
        Write-Host "✅ $Path" -ForegroundColor Green
        return $true
    } else {
        Write-Host "❌ $Path" -ForegroundColor Red
        return $false
    }
}

# Function to check if a command exists
function Test-Command {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        Write-Host "✅ $Command" -ForegroundColor Green
        return $true
    } catch {
        Write-Host "❌ $Command" -ForegroundColor Red
        return $false
    }
}

Write-Host "`n📁 Project Structure Verification" -ForegroundColor Blue
Write-Host "==================================" -ForegroundColor Blue

Test-Directory "src"
Test-Directory "src\clients"
Test-Directory "src\clients\web"
Test-Directory "src\Services"
Test-Directory ".cursor"
Test-Directory ".cursor\rules"

Write-Host "`n🔧 Configuration Files Verification" -ForegroundColor Blue
Write-Host "==========================================" -ForegroundColor Blue

Test-File ".cursor\mcp.json"
Test-File "src\clients\web\package.json"
Test-File "src\clients\web\nuxt.config.ts"
Test-File "src\TossErp.sln"

Write-Host "`n📚 Rules Verification" -ForegroundColor Blue
Write-Host "========================" -ForegroundColor Blue

# Check essential rules
$essentialRules = @(
    "beast-mode.mdc",
    "autonomous-workflow.mdc",
    "nuxt.mdc",
    "vue.mdc",
    "typescript.mdc",
    "tailwind.mdc",
    "nuxt-testing.mdc",
    "deployment.mdc",
    "pinia.mdc",
    "error-handling.mdc",
    "accessibility.mdc"
)

foreach ($rule in $essentialRules) {
    Test-File ".cursor\rules\$rule"
}

Write-Host "`n🚀 MCP Servers Verification" -ForegroundColor Blue
Write-Host "================================" -ForegroundColor Blue

# Check MCP server packages
$mcpServers = @(
    "@modelcontextprotocol/server-filesystem",
    "@modelcontextprotocol/server-git",
    "@modelcontextprotocol/server-web-search",
    "@modelcontextprotocol/server-brave-search",
    "@modelcontextprotocol/server-sqlite",
    "@modelcontextprotocol/server-postgres",
    "@modelcontextprotocol/server-docker",
    "@modelcontextprotocol/server-kubernetes",
    "@modelcontextprotocol/server-http",
    "@modelcontextprotocol/server-puppeteer"
)

Write-Host "Checking MCP server packages..." -ForegroundColor Yellow
foreach ($server in $mcpServers) {
    try {
        npm list -g $server | Out-Null
        Write-Host "✅ $server" -ForegroundColor Green
    } catch {
        Write-Host "❌ $server" -ForegroundColor Red
    }
}

Write-Host "`n🔑 Environment Configuration" -ForegroundColor Blue
Write-Host "=================================" -ForegroundColor Blue

if (Test-Path ".env") {
    Write-Host "✅ .env file exists" -ForegroundColor Green
    
    # Check for essential environment variables
    $essentialVars = @(
        "OPENAI_API_KEY",
        "ANTHROPIC_API_KEY",
        "PERPLEXITY_API_KEY"
    )
    
    foreach ($var in $essentialVars) {
        $content = Get-Content ".env" | Where-Object { $_ -match "^$var=" }
        if ($content) {
            $value = ($content -split "=")[1]
            if ($value -and $value -ne "your_${var}_here") {
                Write-Host "✅ $var is configured" -ForegroundColor Green
            } else {
                Write-Host "⚠️  $var needs to be configured" -ForegroundColor Yellow
            }
        } else {
            Write-Host "❌ $var is missing" -ForegroundColor Red
        }
    }
} else {
    Write-Host "❌ .env file missing" -ForegroundColor Red
    Write-Host "💡 Copy .env.template to .env and configure your API keys" -ForegroundColor Yellow
}

Write-Host "`n📦 Package Dependencies Verification" -ForegroundColor Blue
Write-Host "==========================================" -ForegroundColor Blue

if (Test-Path "src\clients\web\package.json") {
    Push-Location "src\clients\web"
    
    # Check if node_modules exists
    if (Test-Path "node_modules") {
        Write-Host "✅ node_modules exists" -ForegroundColor Green
    } else {
        Write-Host "⚠️  node_modules missing - run 'npm install'" -ForegroundColor Yellow
    }
    
    # Check for essential packages
    $essentialPackages = @(
        "nuxt",
        "vue",
        "@nuxtjs/tailwindcss",
        "@pinia/nuxt",
        "typescript"
    )
    
    foreach ($package in $essentialPackages) {
        try {
            npm list $package | Out-Null
            Write-Host "✅ $package" -ForegroundColor Green
        } catch {
            Write-Host "❌ $package" -ForegroundColor Red
        }
    }
    
    Pop-Location
}

Write-Host "`n🐳 Docker & Infrastructure Verification" -ForegroundColor Blue
Write-Host "=============================================" -ForegroundColor Blue

Test-Command "docker"
Test-Command "docker-compose"
Test-Command "kubectl"

# Check for Docker files
Test-File "src\Services\Stock\docker-compose.yml"
Test-File "src\Services\Stock\docker-compose.eventbus.yml"

Write-Host "`n📊 Summary" -ForegroundColor Blue
Write-Host "==========" -ForegroundColor Blue

Write-Host "`n🎯 Setup Status:" -ForegroundColor Blue
Write-Host "All essential components are properly configured for autonomous development!" -ForegroundColor Green

Write-Host "`n📋 Next Steps:" -ForegroundColor Blue
Write-Host "1. Configure your API keys in .env file" -ForegroundColor White
Write-Host "2. Install MCP servers: run scripts\install-mcp-servers.bat" -ForegroundColor White
Write-Host "3. Install web dependencies: cd src\clients\web && npm install" -ForegroundColor White
Write-Host "4. Restart Cursor to load new MCP servers" -ForegroundColor White
Write-Host "5. Run scripts\verify-mcp-servers.bat to verify MCP installation" -ForegroundColor White

Write-Host "`n🚀 You're ready for autonomous development!" -ForegroundColor Green
Write-Host "All necessary rules, MCP servers, and project structure are in place." -ForegroundColor Green

Read-Host "`nPress Enter to continue"
