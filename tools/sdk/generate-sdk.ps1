# SDK Generation Script for TOSS ERP
# This script generates both .NET and TypeScript SDKs from OpenAPI specifications

param(
    [switch]$DotNet,
    [switch]$TypeScript,
    [switch]$All,
    [switch]$Clean,
    [string]$OutputPath = "../../src/Client"
)

$ErrorActionPreference = "Stop"

# Colors for output
function Write-Success { param($Message) Write-Host $Message -ForegroundColor Green }
function Write-Info { param($Message) Write-Host $Message -ForegroundColor Cyan }
function Write-Warning { param($Message) Write-Host $Message -ForegroundColor Yellow }
function Write-Error { param($Message) Write-Host $Message -ForegroundColor Red }

# Ensure we're in the tools/sdk directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir

Write-Info "TOSS ERP SDK Generation"
Write-Info "Working directory: $(Get-Location)"

# Clean function
function Clean-GeneratedFiles {
    Write-Info "🧹 Cleaning generated files..."
    
    $dotnetFile = "../../src/Client/TossErp.Client.Generated.cs"
    $tsDir = "../../src/Client/TypeScript"
    
    if (Test-Path $dotnetFile) {
        Remove-Item $dotnetFile -Force
        Write-Success "Removed .NET client file"
    }
    
    if (Test-Path $tsDir) {
        Remove-Item $tsDir -Recurse -Force
        Write-Success "Removed TypeScript client directory"
    }
    
    Write-Success "🧹 Cleanup completed"
}

# Generate .NET Client
function Generate-DotNetClient {
    Write-Info "🔄 Generating .NET Client with NSwag..."
    
    try {
        # Check if dotnet tool is available
        $nswagPath = dotnet tool run nswag --help 2>$null
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "NSwag tool not found. Installing..."
            dotnet tool restore
        }
        
        # Run NSwag generation
        dotnet nswag run dotnet-client.nswag
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success ".NET client generated successfully"
            
            # Check if file was created
            $generatedFile = "../../src/Client/TossErp.Client.Generated.cs"
            if (Test-Path $generatedFile) {
                $fileSize = (Get-Item $generatedFile).Length
                Write-Info "📄 Generated file: $generatedFile ($($fileSize) bytes)"
            }
        } else {
            throw "NSwag generation failed with exit code $LASTEXITCODE"
        }
    }
    catch {
        Write-Error "❌ .NET client generation failed: $($_.Exception.Message)"
        throw
    }
}

# Generate TypeScript Client
function Generate-TypeScriptClient {
    Write-Info "🔄 Generating TypeScript Client with OpenAPI Generator..."
    
    try {
        # Check Docker availability (fallback option)
        $dockerAvailable = $false
        try {
            docker --version | Out-Null
            $dockerAvailable = ($LASTEXITCODE -eq 0)
        } catch {
            $dockerAvailable = $false
        }
        
        if ($dockerAvailable) {
            Write-Info "🐳 Using Docker for TypeScript generation..."
            
            # Run OpenAPI Generator with Docker
            $currentDir = (Get-Location).Path
            $projectRoot = (Get-Item "../..").FullName
            
            docker run --rm -v "${projectRoot}:/local" `
                openapitools/openapi-generator-cli:v7.10.0 generate `
                -i /local/docs/openapi/core.yml `
                -g typescript-axios `
                -o /local/src/Client/TypeScript `
                --additional-properties=supportsES6=true,withInterfaces=true,useSingleRequestParameter=true,apiPackage=api,modelPackage=models,npmName=@toss-erp/client,npmVersion=1.0.0,removeOperationIdPrefix=true
            
            if ($LASTEXITCODE -eq 0) {
                Write-Success "TypeScript client generated successfully using Docker"
            } else {
                throw "Docker-based generation failed with exit code $LASTEXITCODE"
            }
        } else {
            Write-Warning "Docker not available. Creating simple TypeScript client stub..."
            
            # Create basic TypeScript structure
            $tsDir = "../../src/Client/TypeScript"
            New-Item -ItemType Directory -Path $tsDir -Force | Out-Null
            
            # Create basic TypeScript client file
            $basicClient = @"
// Generated TypeScript client stub for TOSS ERP API
// This is a minimal implementation - for full client generation, Docker is required

export interface ApiConfiguration {
  basePath?: string;
  accessToken?: string;
}

export class Configuration {
  constructor(private config: ApiConfiguration = {}) {}
  
  get basePath(): string {
    return this.config.basePath || 'https://api.toss-erp.com';
  }
  
  get accessToken(): string | undefined {
    return this.config.accessToken;
  }
}

export class BaseApi {
  constructor(protected configuration: Configuration) {}
  
  protected get headers(): Record<string, string> {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json'
    };
    
    if (this.configuration.accessToken) {
      headers['Authorization'] = `Bearer ${this.configuration.accessToken}`;
    }
    
    return headers;
  }
}

// Example API class - extend this pattern for actual endpoints
export class DefaultApi extends BaseApi {
  async healthCheck(): Promise<{ status: string }> {
    const response = await fetch(`${this.configuration.basePath}/health`, {
      headers: this.headers
    });
    
    if (!response.ok) {
      throw new Error(`HTTP ${response.status}: ${response.statusText}`);
    }
    
    return response.json();
  }
}

export default {
  Configuration,
  DefaultApi
};
"@
            
            Set-Content -Path "$tsDir/index.ts" -Value $basicClient
            
            # Create package.json
            $packageJson = @"
{
  "name": "@toss-erp/client",
  "version": "1.0.0",
  "description": "TypeScript client library for TOSS ERP API",
  "main": "index.js",
  "types": "index.d.ts",
  "scripts": {
    "build": "tsc",
    "prepublishOnly": "npm run build"
  },
  "keywords": ["toss-erp", "api-client", "typescript"],
  "author": "TOSS ERP Team",
  "license": "MIT",
  "dependencies": {},
  "devDependencies": {
    "typescript": "^5.3.0"
  }
}
"@
            
            Set-Content -Path "$tsDir/package.json" -Value $packageJson
            
            Write-Success "Basic TypeScript client stub created"
            Write-Warning "For full client generation, install Docker and re-run this script"
        }
    }
    catch {
        Write-Error "❌ TypeScript client generation failed: $($_.Exception.Message)"
        throw
    }
}

# Main execution
try {
    if ($Clean) {
        Clean-GeneratedFiles
        return
    }
    
    if (-not ($DotNet -or $TypeScript -or $All)) {
        Write-Info "No specific generator selected. Use -All to generate both, or -DotNet/-TypeScript for specific clients."
        Write-Info "Usage: .\generate-sdk.ps1 [-DotNet] [-TypeScript] [-All] [-Clean]"
        return
    }
    
    if ($All -or $DotNet) {
        Generate-DotNetClient
    }
    
    if ($All -or $TypeScript) {
        Generate-TypeScriptClient
    }
    
    Write-Success "🎉 SDK Generation completed successfully!"
    Write-Info ""
    Write-Info "Generated files:"
    
    if ($All -or $DotNet) {
        Write-Info ".NET Client: src/Client/TossErp.Client.Generated.cs"
    }
    
    if ($All -or $TypeScript) {
        Write-Info "TypeScript Client: src/Client/TypeScript/"
    }
    
    Write-Info ""
    Write-Info "Next steps:"
    Write-Info "1. Review generated client code"
    Write-Info "2. Test integration with your applications"
    Write-Info "3. For publishing, see docs/api/sdk-generation.md"
}
catch {
    Write-Error "SDK Generation failed: $($_.Exception.Message)"
    exit 1
}
