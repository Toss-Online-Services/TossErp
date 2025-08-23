# SDK Generation and Publishing Guide

This document provides comprehensive guidance for generating, testing, and publishing SDKs for the TOSS ERP API.

## Overview

The TOSS ERP SDK generation pipeline supports:
- **.NET Client**: Generated using NSwag for C# applications
- **TypeScript Client**: Generated using OpenAPI Generator for web applications

## Prerequisites

- Node.js 18+ with npm
- .NET 8.0+ SDK
- Git for version control

## Quick Start

### 1. Install Dependencies

```bash
# Install root dependencies (Spectral, Redoc)
npm install

# Install SDK generation tools
npm run sdk:install
```

### 2. Generate SDKs

```bash
# Generate all SDKs
npm run sdk:generate

# Generate only .NET SDK
npm run sdk:generate:dotnet

# Generate only TypeScript SDK
npm run sdk:generate:typescript
```

### 3. Clean Generated Files

```bash
npm run sdk:clean
```

## SDK Generation Pipeline

### .NET Client Generation (NSwag)

**Configuration**: `tools/sdk/dotnet-client.nswag`

**Features**:
- Strongly-typed C# clients with async/await support
- System.Text.Json serialization
- Nullable reference types enabled
- Data annotations for validation
- HttpClient dependency injection support
- Exception handling with custom `TossApiException`

**Generated Output**: 
- `src/Client/TossErp.Client.Generated.cs` - Complete client implementation
- Auto-generated interfaces for dependency injection
- Model classes with proper nullability annotations

**Usage Example**:
```csharp
using TossErp.Client;
using Microsoft.Extensions.DependencyInjection;

// Configure services
services.AddHttpClient<FinanceClient>(client =>
{
    client.BaseAddress = new Uri("https://api.toss-erp.com");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "your-token");
});

// Use in your application
public class FinanceService
{
    private readonly FinanceClient _client;
    
    public FinanceService(FinanceClient client)
    {
        _client = client;
    }
    
    public async Task<ICollection<Invoice>> GetInvoicesAsync()
    {
        return await _client.GetInvoicesAsync();
    }
}
```

### TypeScript Client Generation (OpenAPI Generator)

**Configuration**: `tools/sdk/openapitools.json`

**Features**:
- TypeScript with full type safety
- Axios-based HTTP client
- ES6+ support with modern JavaScript features
- Single request parameter pattern for clean APIs
- Interface generation for all models
- Tree-shakable exports

**Generated Output**:
- `src/Client/TypeScript/` - Complete TypeScript client
- Type definitions for all API models
- Axios interceptors for authentication
- Environment-specific configuration support

**Usage Example**:
```typescript
import { Configuration, FinanceApi } from '@toss-erp/client';

// Configure the client
const config = new Configuration({
  basePath: 'https://api.toss-erp.com',
  accessToken: 'your-access-token'
});

const financeApi = new FinanceApi(config);

// Use with async/await
async function getInvoices() {
  try {
    const response = await financeApi.getInvoices({
      page: 1,
      limit: 50,
      status: 'pending'
    });
    
    return response.data;
  } catch (error) {
    console.error('Failed to fetch invoices:', error);
    throw error;
  }
}

// React Context Provider Example
const ApiContext = createContext<FinanceApi | null>(null);

export const ApiProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const api = useMemo(() => new FinanceApi(config), []);
  
  return (
    <ApiContext.Provider value={api}>
      {children}
    </ApiContext.Provider>
  );
};

export const useFinanceApi = () => {
  const context = useContext(ApiContext);
  if (!context) {
    throw new Error('useFinanceApi must be used within ApiProvider');
  }
  return context;
};
```

## Automated Publishing Pipeline

### GitHub Actions Workflow

The SDK generation pipeline is fully automated via GitHub Actions (`.github/workflows/sdk-generation.yml`):

**Triggers**:
- Push to `main` or `develop` branches with API changes
- Pull requests affecting OpenAPI specifications
- Manual workflow dispatch with publish option

**Pipeline Stages**:

1. **Validation**
   - OpenAPI specification linting with Spectral
   - Schema validation against TOSS ERP standards

2. **Generation**
   - .NET client generation using NSwag
   - TypeScript client generation using OpenAPI Generator
   - Artifact creation and testing

3. **Publishing** (main branch only)
   - .NET package to NuGet.org
   - TypeScript package to npm registry
   - Automatic versioning with build numbers

4. **Release Creation**
   - GitHub release with SDK artifacts
   - Installation instructions
   - Change documentation

### Package Publishing

#### .NET NuGet Package

**Package Name**: `TossErp.Client`
**Registry**: NuGet.org
**Versioning**: `1.0.0-alpha.{build-number}`

**Installation**:
```bash
dotnet add package TossErp.Client
```

**Configuration Required**:
- `NUGET_API_KEY` secret in GitHub repository

#### TypeScript npm Package

**Package Name**: `@toss-erp/client`
**Registry**: npm registry
**Versioning**: `1.0.0-alpha.{build-number}`

**Installation**:
```bash
npm install @toss-erp/client
```

**Configuration Required**:
- `NPM_TOKEN` secret in GitHub repository

## Local Development

### Manual SDK Generation

```bash
# Navigate to SDK tools directory
cd tools/sdk

# Install dependencies
npm install

# Generate .NET client
npm run generate:dotnet

# Generate TypeScript client  
npm run generate:typescript

# Clean all generated files
npm run clean
```

### Testing Generated SDKs

#### .NET Client Testing

```bash
# Create test project
dotnet new console -n SdkTest
cd SdkTest

# Add reference to generated client
dotnet add reference ../src/Client/TossErp.Client.csproj

# Add required packages
dotnet add package Microsoft.Extensions.Http
dotnet add package Microsoft.Extensions.DependencyInjection

# Build and test
dotnet build
dotnet run
```

#### TypeScript Client Testing

```bash
# Create test project
mkdir sdk-test && cd sdk-test
npm init -y

# Install generated client
npm install ../artifacts/npm

# Create test file
echo "
import { Configuration, DefaultApi } from '@toss-erp/client';

const config = new Configuration({
  basePath: 'https://localhost:7001'
});

const api = new DefaultApi(config);
console.log('SDK loaded successfully');
" > test.js

# Test
node test.js
```

## Configuration Reference

### NSwag Configuration Options

Key settings in `dotnet-client.nswag`:

| Property | Value | Description |
|----------|-------|-------------|
| `namespace` | `TossErp.Client` | Root namespace for generated classes |
| `className` | `{controller}Client` | Client class naming pattern |
| `jsonLibrary` | `SystemTextJson` | JSON serialization library |
| `generateNullableReferenceTypes` | `true` | Enable nullable reference types |
| `generateDataAnnotations` | `true` | Include validation attributes |
| `operationGenerationMode` | `MultipleClientsFromOperationId` | Generate separate clients per controller |

### OpenAPI Generator Configuration Options

Key settings in `openapitools.json`:

| Property | Value | Description |
|----------|-------|-------------|
| `generatorName` | `typescript-axios` | TypeScript generator with Axios |
| `supportsES6` | `true` | Modern JavaScript features |
| `useSingleRequestParameter` | `true` | Clean API with object parameters |
| `withInterfaces` | `true` | Generate TypeScript interfaces |
| `stringEnums` | `true` | Use string enums instead of numbers |

## Troubleshooting

### Common Issues

1. **NSwag Generation Fails**
   ```bash
   # Ensure .NET tool is installed
   cd tools/sdk
   dotnet tool install NSwag.ConsoleCore --version 14.2.0
   ```

2. **OpenAPI Generator Issues**
   ```bash
   # Update to latest version
   npm install @openapitools/openapi-generator-cli@latest
   ```

3. **TypeScript Compilation Errors**
   ```bash
   # Check TypeScript version compatibility
   npm install typescript@latest
   ```

4. **Authentication Errors During Publishing**
   - Verify `NUGET_API_KEY` and `NPM_TOKEN` secrets
   - Check package names don't conflict with existing packages
   - Ensure proper scoping for npm packages

### Debug Mode

Enable verbose logging:

```bash
# NSwag verbose mode
nswag run dotnet-client.nswag /variables:Verbose=true

# OpenAPI Generator debug
openapi-generator-cli generate --verbose
```

## Best Practices

### OpenAPI Specification Quality

1. **Complete Documentation**
   - All endpoints have descriptions
   - Request/response examples included
   - Proper schema definitions with validation

2. **Consistent Naming**
   - Use consistent operationId patterns
   - Follow REST conventions
   - Meaningful model names

3. **Version Management**
   - Semantic versioning for breaking changes
   - Deprecation notices for old endpoints
   - Migration guides for major versions

### SDK Quality Assurance

1. **Testing**
   - Integration tests with real API
   - Unit tests for generated models
   - Compatibility testing across versions

2. **Documentation**
   - Code examples for common scenarios
   - Authentication setup guides
   - Error handling patterns

3. **Maintenance**
   - Regular dependency updates
   - Security vulnerability scanning
   - Performance optimization

## Contributing

### Adding New Generators

1. Create new configuration file in `tools/sdk/`
2. Add npm script in `tools/sdk/package.json`
3. Update GitHub Actions workflow
4. Add documentation and examples

### Modifying Existing Generators

1. Update configuration files
2. Test generation locally
3. Update documentation
4. Create pull request with examples

For questions or support, please create an issue in the repository or contact the TOSS ERP development team.
