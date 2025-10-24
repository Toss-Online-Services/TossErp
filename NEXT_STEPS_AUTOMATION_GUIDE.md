# TOSS Automated Execution Guide 🚀

**Current Status:** ✅ Migrations Generated  
**Date:** October 24, 2025

---

## Automated Order Progress

```
✅ 1. Generate migrations     - COMPLETE
🔄 2. Start testing          - READY TO BEGIN
⏳ 3. Create the database    - READY (needs PostgreSQL)
⏳ 4. Deploy to Azure        - PENDING
⏳ 5. Add external services  - PENDING
```

---

## Step 2: Start Testing 🧪

### What to Test

#### A. Application Layer Handlers (37 handlers)
**Location:** `backend/Toss/src/Application/*/Commands/` and `*/Queries/`

**Critical Handlers to Test:**
1. **Sales:** CreateSale, VoidSale, GetSales, GetDailySummary
2. **Inventory:** CreateProduct, AdjustStock, GetLowStockAlerts
3. **Group Buying:** CreatePool, JoinPool, ConfirmPool, GetActivePools
4. **Logistics:** CreateSharedDeliveryRun, CaptureProofOfDelivery
5. **Payments:** RecordPayment, GeneratePayLink
6. **Dashboard:** GetDashboardSummary, GetSalesTrends

**Test Framework:** Use xUnit or NUnit with Moq for mocking

**Sample Test Structure:**
```csharp
public class CreateSaleCommandTests
{
    [Fact]
    public async Task Handle_ValidRequest_CreatesSale()
    {
        // Arrange
        var dbContext = CreateInMemoryDbContext();
        var handler = new CreateSaleCommandHandler(dbContext);
        var command = new CreateSaleCommand { /* ... */ };
        
        // Act
        var result = await handler.Handle(command, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(SaleStatus.Pending, result.Status);
    }
}
```

#### B. Web API Endpoints (9 endpoint groups)
**Location:** `backend/Toss/src/Web/Endpoints/`

**Endpoint Groups:**
1. Auth (login, refresh, verify)
2. Sales (create, void, list, receipt)
3. Inventory (products, stock, alerts)
4. GroupBuying (pools, join, confirm)
5. Buying (purchase orders, approve)
6. Suppliers (create, list, pricing)
7. Logistics (delivery runs, POD)
8. CRM (customers, interactions)
9. Payments (record, generate link)

**Test Framework:** Use WebApplicationFactory for integration tests

**Sample Integration Test:**
```csharp
public class SalesEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public SalesEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateSale_ReturnsCreated()
    {
        // Arrange
        var sale = new { ShopId = 1, CustomerId = 1, Items = new[] { /* ... */ } };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", sale);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

#### C. Entity Configurations
**Test:** Verify EF Core model is correctly built without errors

```csharp
[Fact]
public void DbContext_ModelBuilds_WithoutErrors()
{
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase("TestDb")
        .Options;
        
    using var context = new ApplicationDbContext(options);
    var model = context.Model;
    
    Assert.NotNull(model);
    // Verify specific entities exist
    Assert.Contains(model.GetEntityTypes(), e => e.Name.Contains("Sale"));
}
```

### Testing Commands

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Application.IntegrationTests/

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## Step 3: Create the Database 💾

### Prerequisites
1. ✅ Migration files generated (DONE)
2. ⚠️ PostgreSQL must be running
3. ⚠️ Connection string configured

### PostgreSQL Setup

#### Option A: Local Docker Container
```bash
docker run --name toss-postgres \
  -e POSTGRES_USER=toss \
  -e POSTGRES_PASSWORD=your_password_here \
  -e POSTGRES_DB=TossErp \
  -p 5432:5432 \
  -d postgres:16
```

#### Option B: Local PostgreSQL Installation
1. Install PostgreSQL 16 or higher
2. Create database: `CREATE DATABASE TossErp;`
3. Create user with permissions

### Connection String

**Update in:** `backend/Toss/src/Web/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=TossErp;Username=toss;Password=your_password_here"
  }
}
```

**Or use environment variable:**
```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=TossErp;Username=toss;Password=your_password_here"
```

### Apply Migration

```bash
cd backend/Toss

# Apply migration to create database schema
dotnet ef database update \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web/Web.csproj

# Verify migration applied
dotnet ef migrations list \
  --project src/Infrastructure/Infrastructure.csproj \
  --startup-project src/Web/Web.csproj
```

### Verification

```bash
# Start the API
dotnet run --project src/Web/Web.csproj

# Test health endpoint
curl http://localhost:5001/health

# Test API endpoint
curl http://localhost:5001/api/sales
```

---

## Step 4: Deploy to Azure ☁️

### Prerequisites
1. ✅ Database created and tested (from Step 3)
2. ⚠️ Azure subscription active
3. ⚠️ Azure CLI installed (`az --version`)
4. ⚠️ Authenticated (`az login`)

### Azure Resources Needed

**Infrastructure (from Bicep templates):**
- App Service Plan (B1 or higher)
- App Service for Web API
- Azure Database for PostgreSQL Flexible Server
- Azure Key Vault (for secrets)
- Application Insights (monitoring)

### Deploy Using Azure Developer CLI (azd)

```bash
cd backend/Toss

# Initialize azd (if not done)
azd init

# Provision infrastructure
azd provision

# Deploy application
azd deploy

# Get deployment URL
azd show
```

### Manual Azure Setup

```bash
# Create resource group
az group create --name rg-toss-erp --location eastus

# Deploy Bicep template
az deployment group create \
  --resource-group rg-toss-erp \
  --template-file infra/main.bicep \
  --parameters environmentName=prod

# Deploy app
az webapp deployment source config-zip \
  --resource-group rg-toss-erp \
  --name app-toss-web-prod \
  --src ./deploy.zip
```

### Post-Deployment Verification

```bash
# Test deployed API
curl https://app-toss-web-prod.azurewebsites.net/health

# Check logs
az webapp log tail \
  --resource-group rg-toss-erp \
  --name app-toss-web-prod
```

---

## Step 5: Add External Services 🔌

### A. WhatsApp Business API Integration

**Purpose:** Send alerts and notifications to shop owners

**Setup:**
1. Register with WhatsApp Business Platform
2. Create app and get API credentials
3. Configure webhook for incoming messages

**Configuration:**
```json
{
  "WhatsApp": {
    "ApiKey": "YOUR_WHATSAPP_API_KEY",
    "PhoneNumberId": "YOUR_PHONE_NUMBER_ID",
    "WebhookSecret": "YOUR_WEBHOOK_SECRET"
  }
}
```

**Alert Types to Implement:**
- Low stock alerts
- Daily sales summary
- Group buying pool confirmations
- Delivery notifications
- Payment confirmations

### B. Payment Gateway Integration

**Recommended:** Stripe or PayStack (popular in SA/Africa)

**Setup for Stripe:**
```json
{
  "Stripe": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  }
}
```

**Features to Implement:**
- Payment link generation
- Webhook for payment confirmations
- Refund processing
- Payment history tracking

### C. AI Copilot Service

**Option 1: OpenAI API**
```json
{
  "OpenAI": {
    "ApiKey": "sk-...",
    "OrganizationId": "org-...",
    "Model": "gpt-4"
  }
}
```

**Option 2: Azure OpenAI**
```json
{
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "YOUR_API_KEY",
    "DeploymentName": "gpt-4"
  }
}
```

**AI Copilot Features:**
- Business insights from sales data
- Stock optimization suggestions
- Demand forecasting
- Customer behavior analysis
- Natural language queries

---

## Quick Reference Commands

### Build & Test
```bash
# Build entire solution
dotnet build backend/Toss/Toss.sln

# Run all tests
dotnet test backend/Toss/Toss.sln

# Run specific test
dotnet test --filter "FullyQualifiedName~CreateSaleCommandTests"
```

### Database
```bash
# Update database
dotnet ef database update --project src/Infrastructure/Infrastructure.csproj

# Generate SQL script
dotnet ef migrations script --project src/Infrastructure/Infrastructure.csproj

# Remove last migration
dotnet ef migrations remove --project src/Infrastructure/Infrastructure.csproj
```

### Run Application
```bash
# Development
dotnet run --project backend/Toss/src/Web/Web.csproj

# Production
dotnet backend/Toss/src/Web/bin/Release/net9.0/Web.dll
```

### Azure
```bash
# Login
az login

# Deploy
azd deploy

# View logs
az webapp log tail --name app-toss-web-prod --resource-group rg-toss-erp
```

---

## Troubleshooting

### Issue: PostgreSQL Connection Failed
**Solution:** Verify PostgreSQL is running and connection string is correct
```bash
docker ps | grep postgres
psql -h localhost -U toss -d TossErp
```

### Issue: Migration Already Applied
**Solution:** Check migration history
```bash
dotnet ef migrations list --project src/Infrastructure/Infrastructure.csproj
```

### Issue: Azure Deployment Failed
**Solution:** Check deployment logs
```bash
azd show --output json
az webapp log download --name app-toss-web-prod --resource-group rg-toss-erp
```

---

## Success Checklist

### Step 2 Complete ✅
- [ ] All 37 Application handlers tested
- [ ] All 9 API endpoint groups tested
- [ ] Entity configurations verified
- [ ] Integration tests passing
- [ ] >80% code coverage

### Step 3 Complete ✅
- [ ] PostgreSQL running
- [ ] Migration applied successfully
- [ ] Database schema verified
- [ ] Sample data seeded (optional)
- [ ] API connects to database

### Step 4 Complete ✅
- [ ] Azure resources provisioned
- [ ] Application deployed
- [ ] Database connection string configured
- [ ] Health check passing
- [ ] All endpoints accessible

### Step 5 Complete ✅
- [ ] WhatsApp Business API configured
- [ ] Payment gateway integrated
- [ ] AI Copilot service connected
- [ ] All webhooks tested
- [ ] External service alerts working

---

**Next Command to Run:**
```bash
# Start with testing
dotnet test backend/Toss/Toss.sln
```

**Generated:** October 24, 2025  
**For:** TOSS ERP Development Team  
**Status:** Ready for Step 2 - Testing Phase

