# 🔓 Unblock TOSS Deployment - Quick Action Guide

**Current Status:** ⚠️ Deployment blocked by disabled Azure subscription  
**Solution Time:** 5-30 minutes (depending on method)

---

## 🚨 The Problem

Your Azure subscription is **disabled** and cannot create new resources.

**Error:** `ReadOnlyDisabledSubscription`  
**Subscription:** Microsoft Azure Sponsorship (21311827-24f8-4c36-ab72-40d58a54dd45)

---

## ✅ Solution Options (Pick One)

### Option A: Re-enable Your Current Subscription ⭐ RECOMMENDED

**Best if:** You want to keep using your Microsoft Azure Sponsorship

**Steps:**

1. **Open Azure Portal**
   - Visit: https://portal.azure.com
   - Sign in with: moses.gontse@tossonline.co.za

2. **Navigate to Subscriptions**
   - Search bar: Type "Subscriptions"
   - Click: "Microsoft Azure Sponsorship"

3. **Check Status**
   Look for status indicators:
   - ❌ "Disabled" or "Expired"
   - ⚠️ "Spending limit reached"
   - ⚠️ "Payment required"

4. **Re-enable**
   Common solutions:
   - Click **"Reactivate"** button (if visible)
   - Click **"Remove spending limit"** (if sponsorship allows)
   - Click **"Update payment method"** (if required)
   - Click **"Renew sponsorship"** (if expired)

5. **Contact Support** (if stuck)
   - Azure Portal → Help + Support
   - Create support ticket
   - Issue: "Subscription disabled - need reactivation"
   - Or call: https://aka.ms/azuresupport

6. **Verify Activation**
   ```bash
   az account show
   # Should show "state": "Enabled"
   ```

7. **Deploy!**
   ```bash
   cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss
   azd up
   ```

**Time:** 5-30 minutes (instant if just a button, up to 1 day if support ticket needed)

---

### Option B: Use a Different Subscription

**Best if:** You have another active Azure subscription

**Steps:**

1. **List Available Subscriptions**
   ```bash
   az account list --output table
   ```

2. **Look for Active Subscriptions**
   Find subscriptions with:
   - State: **"Enabled"**
   - NOT your current one (21311827-24f8-4c36-ab72-40d58a54dd45)

3. **Switch Subscription**
   ```bash
   # Replace with your active subscription ID
   az account set --subscription "YOUR_SUBSCRIPTION_ID"
   ```

4. **Verify Switch**
   ```bash
   az account show
   # Should show your new subscription
   ```

5. **Update azd Configuration**
   ```bash
   cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss
   
   # Remove old environment
   rm -r .azure
   
   # Reinitialize with new subscription
   azd init --environment toss-mvp
   ```

6. **Deploy**
   ```bash
   azd up
   ```

**Time:** 5 minutes

---

### Option C: Sign Up for Azure Free Trial 🆓

**Best if:** You want to try Azure with free credits

**Benefits:**
- ✅ $200 free credit (valid 30 days)
- ✅ 12 months of free services
- ✅ No automatic charges after trial
- ✅ Perfect for development/testing

**Steps:**

1. **Sign Up**
   - Visit: https://azure.microsoft.com/free/
   - Click: **"Start free"**
   - Create new account or use existing Microsoft account
   - **Note:** Use different email than moses.gontse@tossonline.co.za

2. **Complete Verification**
   - Phone number verification
   - Credit card (for identity, won't be charged)
   - Complete registration

3. **Login with New Account**
   ```bash
   az login
   # Use your NEW Azure account
   
   azd auth login
   # Use your NEW Azure account
   ```

4. **Initialize Project**
   ```bash
   cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss
   
   # Remove old config
   rm -r .azure
   
   # Initialize fresh
   azd init --environment toss-mvp
   ```

5. **Deploy**
   ```bash
   azd up
   ```

**Time:** 15-30 minutes (including signup)  
**Cost:** FREE for 30 days ($200 credit)

---

### Option D: Deploy Locally (Temporary Workaround)

**Best if:** You want to test while waiting for subscription fix

**What this does:**
- Runs application on your local machine
- Uses local PostgreSQL database
- Good for testing and development
- NOT accessible from internet

**Steps:**

1. **Ensure Database is Running**
   ```bash
   docker ps | grep postgres
   # Should show running PostgreSQL container
   ```

2. **Run Application**
   ```bash
   cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss
   dotnet run --project src/Web/Web.csproj
   ```

3. **Access Application**
   - API: http://localhost:5001
   - Swagger: http://localhost:5001/swagger
   - Health: http://localhost:5001/health

4. **Run Tests**
   ```bash
   dotnet test tests/Application.FunctionalTests/Application.FunctionalTests.csproj
   ```

5. **Test with Frontend**
   ```bash
   cd ../../../toss-web
   npm run dev
   # Frontend will connect to local API
   ```

**Time:** 2 minutes  
**Cost:** FREE  
**Limitation:** Only accessible on your computer

---

## 🎯 Recommended Path

**For Production Deployment:**
1. Try **Option A** first (re-enable current subscription)
2. If that fails, try **Option C** (free trial)
3. Deploy using `azd up`

**For Immediate Testing:**
1. Use **Option D** (local deployment)
2. Test all features locally
3. Deploy to Azure when subscription is fixed

---

## 📋 Checklist - After Subscription is Fixed

Once you have an active subscription:

- [ ] Verify subscription is active
  ```bash
  az account show
  # Check: "state": "Enabled"
  ```

- [ ] Navigate to project
  ```bash
  cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\backend\Toss
  ```

- [ ] Deploy to Azure
  ```bash
  azd up
  ```

- [ ] Note the deployment URL
  ```
  Example: https://toss-web-xyz123.azurewebsites.net
  ```

- [ ] Test API with Swagger
  ```
  Open: https://your-url.azurewebsites.net/swagger
  ```

- [ ] Update frontend configuration
  ```typescript
  // toss-web/nuxt.config.ts
  runtimeConfig: {
    public: {
      apiBase: 'https://your-url.azurewebsites.net'
    }
  }
  ```

- [ ] Deploy frontend (choose one):
  - Netlify: `netlify deploy --prod`
  - Vercel: `vercel --prod`
  - Azure Static Web Apps: `swa deploy`

- [ ] Configure external services (Phase 5):
  - WhatsApp Business API
  - Payment gateway
  - OpenAI API

- [ ] Set up monitoring:
  - Application Insights dashboards
  - Alert rules
  - Log queries

- [ ] Enable security features:
  - HTTPS only
  - CORS policies
  - API authentication
  - Rate limiting

---

## 💰 Cost Reminder

After deployment, you'll be charged:

**Monthly Costs:**
- App Service (B1): ~$13
- PostgreSQL (B1ms): ~$30-40
- Monitoring: ~$2-5
- **Total: ~$45-60/month**

**Free Trial Coverage:**
- $200 credit covers ~3-4 months
- No charges until credit is exhausted
- Can set spending alerts

**To Minimize Costs:**
- Stop resources when not in use
- Scale down during development
- Set up budget alerts
- Delete test deployments

---

## 🆘 Need Help?

### Azure Support
- **Subscription issues:** https://aka.ms/azuresupport
- **Portal support:** Help + Support blade in Azure Portal
- **Documentation:** https://learn.microsoft.com/azure/

### Community Support
- **Stack Overflow:** [azure] tag
- **Microsoft Q&A:** https://learn.microsoft.com/answers/
- **Reddit:** r/AZURE

### Email Support
**For this specific issue:**
- Contact Azure Support with subscription ID: 21311827-24f8-4c36-ab72-40d58a54dd45
- Mention: "ReadOnlyDisabledSubscription" error
- Request: Subscription reactivation

---

## ⏱️ Time Estimates

| Action | Time | Difficulty |
|--------|------|------------|
| Re-enable subscription (portal) | 5 mins | Easy |
| Re-enable subscription (support) | 1-3 days | Easy |
| Switch to different subscription | 5 mins | Easy |
| Sign up for free trial | 15-30 mins | Easy |
| Deploy after fix | 10-15 mins | Easy |
| Local deployment (testing) | 2 mins | Easy |

---

## ✨ You're So Close!

**What's Already Done:**
- ✅ Backend: 100% complete
- ✅ Database: Running and migrated
- ✅ Tests: All created and building
- ✅ Azure tooling: Installed and authenticated
- ✅ Application: Packaged and ready

**What's Blocking:**
- ⚠️ Subscription disabled (not your code!)

**What's Next:**
- 🔓 Unblock subscription (5-30 mins)
- 🚀 Run `azd up` (10-15 mins)
- 🎉 Your app is live!

---

**Pick an option above and let's get your TOSS ERP deployed! 🚀**

---

**Last Updated:** October 24, 2025  
**Status:** Awaiting subscription activation  
**Next Step:** Choose Option A, B, C, or D above

