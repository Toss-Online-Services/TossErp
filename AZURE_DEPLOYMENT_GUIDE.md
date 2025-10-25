# 🚀 TOSS ERP - Azure Deployment Guide

**Status:** ⚠️ **PREREQUISITES REQUIRED**  
**Current Issue:** Azure CLI tools not installed

---

## 📋 Prerequisites Checklist

Before deploying to Azure, you need:

### 1. Azure Developer CLI (azd) ✅ RECOMMENDED
**What it is:** Modern deployment tool for Azure apps

**Install (Windows PowerShell - Run as Administrator):**
```powershell
winget install microsoft.azd
```

**Alternative (using PowerShell):**
```powershell
powershell -ex AllSigned -c "Invoke-RestMethod 'https://aka.ms/install-azd.ps1' | Invoke-Expression"
```

**Verify installation:**
```powershell
azd version
```

### 2. Azure CLI (az) ✅ REQUIRED
**What it is:** Azure command-line tool for resource management

**Install (Windows):**
```powershell
winget install -e --id Microsoft.AzureCLI
```

**Alternative:** Download installer from https://aka.ms/installazurecliwindows

**Verify installation:**
```powershell
az --version
```

### 3. Azure Subscription ✅ REQUIRED
**What you need:**
- Active Azure account
- Subscription with appropriate permissions
- Owner or Contributor role

**Sign up:** https://azure.microsoft.com/free/

---

## 🎯 Quick Deployment Steps

### Step 1: Install Required Tools

1. **Install Azure Developer CLI:**
```powershell
# Run PowerShell as Administrator
winget install microsoft.azd

# Restart terminal after installation
```

2. **Install Azure CLI:**
```powershell
winget install -e --id Microsoft.AzureCLI

# Restart terminal after installation
```

### Step 2: Login to Azure

```bash
# Login to Azure
az login

# Verify logged in
az account show

# List subscriptions
az account list --output table

# Set default subscription (if needed)
az account set --subscription "YOUR_SUBSCRIPTION_ID"
```

### Step 3: Initialize Azure Developer CLI

```bash
# Navigate to project
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss

# Initialize azd (first time only)
azd init

# When prompted:
# - Environment name: toss-mvp (or your choice)
# - Azure subscription: Select your subscription
# - Azure location: southafricanorth (or your choice)
```

### Step 4: Deploy to Azure

```bash
# Provision infrastructure and deploy in one command
azd up

# This will:
# ✅ Create resource group
# ✅ Provision PostgreSQL database
# ✅ Create App Service
# ✅ Configure networking
# ✅ Deploy application
# ✅ Apply database migrations
```

---

## 📊 What Gets Deployed

### Azure Resources Created

| Resource | Type | Purpose | Monthly Cost |
|----------|------|---------|--------------|
| **App Service** | B1 Basic | Web API hosting | ~$13 |
| **PostgreSQL** | B1ms Burstable | Database | ~$30-40 |
| **App Insights** | PAYG | Monitoring | ~$2-5 |
| **Key Vault** | Standard | Secrets | ~$0.03 |
| **Resource Group** | - | Container | Free |

**Estimated Total:** ~$45-58/month

### Infrastructure Configuration

**File:** `backend/Toss/infra/main.bicep`

Resources deployed:
- ✅ App Service Plan (B1)
- ✅ App Service (Web API)
- ✅ PostgreSQL Flexible Server (B1ms, 32 GB storage)
- ✅ Application Insights
- ✅ Key Vault
- ✅ Log Analytics Workspace
- ✅ Networking & firewall rules

---

## 🔧 Configuration Files

### 1. Azure Configuration
**File:** `backend/Toss/azure.yaml`

```yaml
name: toss
services:
  web:
    project: ./src/Web
    language: dotnet
    host: appservice
```

### 2. Infrastructure as Code
**Directory:** `backend/Toss/infra/`

Files:
- `main.bicep` - Main infrastructure definition
- `main.parameters.json` - Parameter values
- `core/` - Reusable Bicep modules
- `services/web.bicep` - App Service configuration
- `core/database/postgresql/flexibleserver.bicep` - PostgreSQL setup

### 3. Connection Strings
Automatically configured during deployment:
- `TossDb` - PostgreSQL connection string
- Stored securely in App Service configuration
- Database migrations run automatically

---

## 🎬 Deployment Process Flow

```
1. az login
   └─> Authenticates with Azure

2. azd init
   ├─> Creates .azure/ directory
   ├─> Stores environment configuration
   └─> Links to Azure subscription

3. azd up
   ├─> Provisions infrastructure (Bicep templates)
   │   ├─> Creates resource group
   │   ├─> Deploys PostgreSQL
   │   ├─> Deploys App Service
   │   └─> Configures networking
   │
   ├─> Builds application
   │   ├─> dotnet publish
   │   └─> Creates deployment package
   │
   ├─> Deploys to App Service
   │   ├─> Uploads application
   │   ├─> Applies migrations
   │   └─> Starts application
   │
   └─> Shows deployment URLs
       ├─> API: https://toss-web-xxx.azurewebsites.net
       └─> Swagger: https://toss-web-xxx.azurewebsites.net/swagger
```

---

## 📝 Post-Deployment Steps

### 1. Verify Deployment

```bash
# Check deployment status
azd show

# View application logs
az webapp log tail --name toss-web-xxx --resource-group rg-toss-mvp

# Open application in browser
azd show --output json | jq -r '.services.web.endpoint'
```

### 2. Test API Endpoints

**Swagger UI:**
```
https://[your-app-name].azurewebsites.net/swagger
```

**Health Check:**
```bash
curl https://[your-app-name].azurewebsites.net/health
```

### 3. Configure Custom Domain (Optional)

```bash
# Add custom domain
az webapp config hostname add \
  --webapp-name toss-web-xxx \
  --resource-group rg-toss-mvp \
  --hostname yourdomain.com

# Enable HTTPS
az webapp config ssl bind \
  --certificate-thumbprint xxx \
  --ssl-type SNI \
  --name toss-web-xxx \
  --resource-group rg-toss-mvp
```

### 4. Update Frontend Configuration

**File:** `toss-web/nuxt.config.ts`

```typescript
runtimeConfig: {
  public: {
    apiBase: process.env.NUXT_PUBLIC_API_BASE || 
             'https://[your-app-name].azurewebsites.net'
  }
}
```

---

## 🔒 Security Configuration

### Environment Variables

Set in Azure App Service Configuration:

```bash
# Database connection
TossDb__ConnectionString="[Auto-configured]"

# Authentication
JwtSettings__SecretKey="[Generated during deployment]"
JwtSettings__Issuer="https://[your-app-name].azurewebsites.net"
JwtSettings__Audience="https://[your-app-name].azurewebsites.net"

# External services (add later)
WhatsApp__ApiKey="[Your API key]"
PaymentGateway__ApiKey="[Your API key]"
OpenAI__ApiKey="[Your API key]"
```

### Firewall Rules

PostgreSQL firewall configured to:
- ✅ Allow Azure services
- ✅ Allow App Service only
- ❌ Block public internet access

---

## 🐛 Troubleshooting

### Issue: Deployment Fails

**Solution:**
```bash
# View detailed logs
azd deploy --debug

# Check Azure portal
az portal
```

### Issue: Database Connection Fails

**Solution:**
```bash
# Verify connection string
az webapp config connection-string list \
  --name toss-web-xxx \
  --resource-group rg-toss-mvp

# Test database connectivity
az postgres flexible-server show \
  --name toss-db-xxx \
  --resource-group rg-toss-mvp
```

### Issue: Application Won't Start

**Solution:**
```bash
# View application logs
az webapp log tail --name toss-web-xxx --resource-group rg-toss-mvp

# Restart application
az webapp restart --name toss-web-xxx --resource-group rg-toss-mvp
```

---

## 💰 Cost Management

### Monitor Costs

```bash
# View cost analysis
az consumption usage list --output table

# Set budget alerts (optional)
az consumption budget create \
  --budget-name toss-mvp-budget \
  --amount 100 \
  --time-period Monthly
```

### Cost Optimization

**Development:**
- Use B1 tier (~$13/month)
- Scale down during off-hours
- Use burstable database (B1ms)

**Production:**
- Use P1v2 tier (~$75/month)
- Enable auto-scaling
- Use General Purpose database

---

## 📈 Scaling Options

### Manual Scaling

```bash
# Scale App Service
az appservice plan update \
  --name plan-toss-mvp \
  --resource-group rg-toss-mvp \
  --sku P1V2

# Scale database
az postgres flexible-server update \
  --name toss-db-xxx \
  --resource-group rg-toss-mvp \
  --tier GeneralPurpose \
  --sku-name Standard_D2s_v3
```

### Auto-Scaling

```bash
# Enable auto-scale for App Service
az monitor autoscale create \
  --resource-group rg-toss-mvp \
  --resource toss-web-xxx \
  --resource-type Microsoft.Web/sites \
  --min-count 1 \
  --max-count 3 \
  --count 1
```

---

## 🔄 Update & Redeploy

### Deploy Code Changes

```bash
# Build and deploy updated code
azd deploy

# This only deploys code (faster than full provision)
```

### Update Infrastructure

```bash
# Modify infra/main.bicep as needed

# Re-provision infrastructure
azd provision

# Deploy updated code
azd deploy

# Or do both at once
azd up
```

---

## 🗑️ Cleanup

### Delete All Resources

```bash
# Delete entire environment
azd down

# This removes:
# - All Azure resources
# - Resource group
# - Local .azure/ configuration

# Costs stop immediately after deletion
```

### Keep Data, Delete Compute

```bash
# Stop App Service (saves compute cost)
az webapp stop --name toss-web-xxx --resource-group rg-toss-mvp

# Delete only App Service (keep database)
az webapp delete --name toss-web-xxx --resource-group rg-toss-mvp
```

---

## 📚 Additional Resources

### Documentation
- [Azure Developer CLI Docs](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [App Service Docs](https://learn.microsoft.com/azure/app-service/)
- [PostgreSQL Flexible Server Docs](https://learn.microsoft.com/azure/postgresql/flexible-server/)

### Support
- [Azure Support Portal](https://portal.azure.com/#blade/Microsoft_Azure_Support/HelpAndSupportBlade)
- [Azure Pricing Calculator](https://azure.microsoft.com/pricing/calculator/)
- [Azure Free Tier](https://azure.microsoft.com/free/)

---

## ✅ Next Steps After Installation

Once tools are installed:

1. **Restart your terminal** (to load new commands)
2. **Login to Azure:** `az login`
3. **Initialize project:** `azd init`
4. **Deploy:** `azd up`
5. **Test API:** Visit Swagger UI at deployed URL
6. **Update frontend:** Configure API base URL
7. **Add external services:** Configure API keys

---

**Generated by:** AI Development Assistant  
**Date:** October 24, 2025  
**Status:** ⏸️ Awaiting Azure CLI installation

---

*"The cloud is just someone else's computer... that you pay for monthly." – Anonymous*

Let's get your app in the cloud! 🚀☁️

