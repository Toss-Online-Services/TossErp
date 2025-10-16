# 🚀 TOSS MVP - Complete Deployment Guide

This comprehensive guide covers deploying your TOSS MVP frontend to Vercel, including setup, configuration, troubleshooting, and best practices.

---

## 📋 Table of Contents

1. [Quick Start (5 Minutes)](#quick-start-5-minutes)
2. [Detailed Setup Instructions](#detailed-setup-instructions)
3. [Monorepo Configuration](#monorepo-configuration)
4. [GitHub Secrets Setup](#github-secrets-setup)
5. [Troubleshooting](#troubleshooting)
6. [Deployment Verification](#deployment-verification)

---

## Quick Start (5 Minutes)

### Prerequisites
- Vercel account (free tier)
- GitHub repository with admin access
- Repository made public (or Vercel Pro plan)

### Setup Steps

**1. Install Vercel CLI**
```bash
npm install -g vercel
```

**2. Link Project**
```bash
cd toss-web
vercel link
```
- Login when prompted
- Create new project or select existing
- Framework: **Nuxt.js**
- Confirm settings

**3. Get Credentials**
```bash
cat .vercel/project.json
```
Copy the `projectId` and `orgId` values.

**4. Get Vercel Token**
- Visit: https://vercel.com/account/tokens
- Create new token: "GitHub Actions - TOSS"
- **Copy the token immediately!**

**5. Configure GitHub Secrets**
Go to: `https://github.com/Toss-Online-Services/TossErp/settings/secrets/actions`

Add these 3 secrets:
- `VERCEL_TOKEN` → Your token from step 4
- `VERCEL_ORG_ID` → From step 3
- `VERCEL_PROJECT_ID` → From step 3

**6. Configure Vercel Root Directory** ⚠️ **CRITICAL**
1. Go to Vercel project dashboard
2. Click **Settings → General**
3. Scroll to **Build & Development Settings**
4. Click **Edit** on **Root Directory**
5. Enter: `toss-web`
6. Click **Save**

**7. Deploy!**
```bash
git add .
git commit -m "feat: setup Vercel deployment"
git push origin feature/mvp
```

**8. Verify**
- Go to **GitHub → Actions** tab
- Watch "Deploy to Vercel" workflow
- Get deployment URL from workflow output
- Test your app!

---

## Detailed Setup Instructions

### Step 1: Create Vercel Account

1. Go to [vercel.com](https://vercel.com)
2. Sign up with GitHub account (recommended)
3. Verify your email

### Step 2: Link Project to Vercel

#### Option A: Using Vercel CLI (Recommended)

```bash
# Install CLI
npm install -g vercel

# Navigate to project
cd toss-web

# Link project
vercel link
```

Follow the prompts:
- **Login:** Use your Vercel account
- **Scope:** Select your account/team
- **Link to existing project?** No (create new)
- **What's your project's name?** toss-mvp (or your preference)
- **In which directory is your code located?** `./ `(already in toss-web)
- **Framework:** Nuxt.js
- **Override settings?** No

This creates `.vercel/project.json` with:
```json
{
  "projectId": "prj_xxxxxxxxxxxxxxxxxxxx",
  "orgId": "team_xxxxxxxxxxxxxxxxxxxx"
}
```

#### Option B: Using Vercel Dashboard

1. Go to [vercel.com/new](https://vercel.com/new)
2. Import Git repository
3. **IMPORTANT:** Set Root Directory to `toss-web`
4. Framework Preset: **Nuxt.js**
5. Build Command: `npm run generate`
6. Output Directory: `.output/public`
7. Click **Deploy**

### Step 3: Get Vercel Credentials

#### 3.1 Vercel Access Token

1. Go to [Vercel Account Settings → Tokens](https://vercel.com/account/tokens)
2. Click **"Create Token"**
3. Name: `GitHub Actions - TOSS ERP`
4. Scope: **Full Account** (or specific projects)
5. Expiration: Choose appropriate duration
6. Click **"Create"**
7. **Copy the token immediately** (you won't see it again!)

#### 3.2 Organization ID and Project ID

**From `.vercel/project.json`:**
```bash
cat toss-web/.vercel/project.json
```

**From Vercel Dashboard:**
1. Open your project on Vercel
2. Go to **Settings → General**
3. Scroll down to find **Project ID**
4. For Org ID:
   - Personal account: Your username (e.g., `user_abc123`)
   - Team account: Team ID (e.g., `team_xyz789`)

### Step 4: Configure GitHub Secrets

1. Go to your GitHub repository
2. Navigate to **Settings → Secrets and variables → Actions**
3. Click **"New repository secret"**
4. Add each secret:

| Secret Name | Value | Where to Get It |
|------------|-------|-----------------|
| `VERCEL_TOKEN` | `xxxxxxxxxxxxxxxxxxxx` | Vercel Account Tokens page |
| `VERCEL_ORG_ID` | `team_xxxx` or `user_xxxx` | `.vercel/project.json` or Vercel Dashboard |
| `VERCEL_PROJECT_ID` | `prj_xxxxxxxxxxxxxxxxxxxx` | `.vercel/project.json` or Vercel Dashboard |

**Important:** Don't include quotes or spaces around the values.

---

## Monorepo Configuration

### ⚠️ CRITICAL: Root Directory Setup

Since `toss-web` is in a subdirectory (monorepo structure), you **MUST** configure the Root Directory in Vercel:

**Your Repository Structure:**
```
TossErp/
├── .github/
├── backend/
├── docs/
├── toss-mobile/
└── toss-web/          ← Your Nuxt app is HERE
    ├── package.json
    ├── nuxt.config.ts
    └── ...
```

**Without this setting, you'll get:**
```
npm error code ENOENT
npm error Could not read package.json: ENOENT
```

### How to Fix:

1. **Go to Vercel Dashboard:** https://vercel.com/dashboard
2. **Select your project** (TOSS ERP / toss-mvp)
3. **Click "Settings"** in the top navigation
4. **Click "General"** in sidebar
5. **Scroll to "Build & Development Settings"**
6. **Click "Edit"** next to Root Directory
7. **Enter:** `toss-web`
8. **Click "Save"**

### Vercel Project Settings

Ensure these are configured:

- **Framework Preset:** Nuxt.js
- **Root Directory:** `toss-web` ⚠️ **REQUIRED**
- **Build Command:** `npm run generate` (default is fine)
- **Output Directory:** `.output/public` (default is fine)
- **Install Command:** `npm install` (default is fine)
- **Node.js Version:** 20.x (default is fine)

---

## GitHub Secrets Setup

### Required Secrets

Your GitHub Actions workflow requires 3 secrets:

1. **VERCEL_TOKEN**
   - Purpose: Authenticates GitHub Actions with Vercel API
   - Permissions: Full account or project-specific
   - Security: Rotate periodically, never commit to code

2. **VERCEL_ORG_ID**
   - Purpose: Identifies your Vercel organization/account
   - Format: `team_xxxxxxxxx` (team) or `user_xxxxxxx` (personal)
   - Location: `.vercel/project.json` after linking

3. **VERCEL_PROJECT_ID**
   - Purpose: Identifies specific project in Vercel
   - Format: `prj_xxxxxxxxxxxxxxx`
   - Location: `.vercel/project.json` after linking

### How to Add Secrets

```bash
# 1. Go to GitHub repository
https://github.com/Toss-Online-Services/TossErp

# 2. Navigate to
Settings → Secrets and variables → Actions

# 3. Click "New repository secret"

# 4. Add each secret:
Name: VERCEL_TOKEN
Secret: [paste your token]

Name: VERCEL_ORG_ID
Secret: [paste your org ID]

Name: VERCEL_PROJECT_ID
Secret: [paste your project ID]
```

---

## Troubleshooting

### Common Errors & Solutions

#### Error: "Could not read package.json: ENOENT"

**Cause:** Vercel Root Directory not configured  
**Solution:**
1. Go to Vercel Dashboard → Your Project → Settings → General
2. Set Root Directory to: `toss-web`
3. Save and redeploy

See [Monorepo Configuration](#monorepo-configuration) above.

#### Error: "No token found"

**Cause:** `VERCEL_TOKEN` secret not set or incorrect  
**Solution:**
1. Verify secret exists in GitHub: Settings → Secrets and variables → Actions
2. Check for typos or extra spaces
3. Regenerate token if needed

#### Error: "Project not found"

**Cause:** Wrong `VERCEL_PROJECT_ID` or project deleted  
**Solution:**
1. Verify project exists in Vercel dashboard
2. Check `VERCEL_PROJECT_ID` matches `.vercel/project.json`
3. Re-run `vercel link` if needed

#### Error: "Organization not found"

**Cause:** Wrong `VERCEL_ORG_ID`  
**Solution:**
1. For personal accounts: Use format `user_xxxxx`
2. For teams: Use format `team_xxxxx`
3. Check `.vercel/project.json` for correct value

#### Error: "Build failed" (after successful package.json read)

**Cause:** Missing dependencies or build configuration  
**Solution:**
1. Check Node.js version compatibility (using 20.x)
2. Verify all dependencies in `package.json`
3. Test build locally: `cd toss-web && npm run build`
4. Review Vercel build logs for specific errors

#### Error: "Deployment succeeded but site doesn't work"

**Cause:** Runtime errors or missing environment variables  
**Solution:**
1. Check browser console for JavaScript errors
2. Verify environment variables in Vercel dashboard
3. Test locally with production build: `npm run preview`
4. Check Vercel function logs

### Debug Mode

Enable verbose logging in GitHub Actions:

1. Go to: Settings → Secrets and variables → Actions
2. Click **Variables** tab
3. Add new variable:
   - Name: `ACTIONS_STEP_DEBUG`
   - Value: `true`
4. Re-run workflow to see detailed logs

---

## Deployment Verification

### Post-Deployment Checklist

After successful deployment, verify:

- [ ] Deployment URL is accessible
- [ ] Homepage loads without errors
- [ ] All 7 core modules are accessible:
  - [ ] Dashboard
  - [ ] Stock & Inventory
  - [ ] Logistics
  - [ ] Sales
  - [ ] Purchasing
  - [ ] Automation
  - [ ] Onboarding
- [ ] PWA is installable on mobile
- [ ] Mock data displays correctly
- [ ] Charts render properly
- [ ] No console errors
- [ ] Mobile responsive design works
- [ ] Offline mode indicator shows when offline
- [ ] Demo mode banner displays

### Testing Your Deployment

**1. Basic Functionality**
```bash
# Test homepage
curl https://your-deployment-url.vercel.app

# Check specific modules
# Navigate to /stock, /sales, /purchasing, etc.
```

**2. PWA Installation (Mobile)**
- Open site on mobile device
- Look for "Add to Home Screen" prompt
- Install and test offline functionality

**3. Performance**
- Run Lighthouse audit
- Check Core Web Vitals
- Test on slow 3G connection

**4. Cross-Browser Testing**
- Chrome/Edge
- Firefox
- Safari (iOS)

---

## Deployment Workflows

### Automatic Deployments

**Production:** Triggered by push to `main` or `feature/mvp`
- URL: Your production Vercel domain
- Example: `https://toss-mvp.vercel.app`

**Preview:** Triggered by pull requests
- URL: Unique preview URL per PR
- Example: `https://toss-mvp-git-pr-123.vercel.app`
- Auto-comment on PR with URL

**Manual:** Workflow dispatch
- Trigger: GitHub Actions → Deploy to Vercel → Run workflow
- Deploy any branch on demand

### Workflow Features

✅ Automatic deployment on git push  
✅ Preview URLs for every PR  
✅ PR comments with deployment links  
✅ E2E testing on previews (optional)  
✅ Production builds for main branches  
✅ Deployment status in GitHub Actions  
✅ Build artifact uploads  
✅ Comprehensive error handling  

---

## Environment Variables (Optional)

### For Future Backend Integration

Add these in Vercel Dashboard → Settings → Environment Variables:

```bash
# API Configuration
NUXT_PUBLIC_API_BASE_URL=https://api.toss-erp.com

# Feature Flags (if needed)
NUXT_PUBLIC_ENABLE_REAL_API=false
```

### Environment-Specific Variables

- **Production:** Set for Production environment
- **Preview:** Set for Preview environment
- **Development:** Local `.env` file (not committed)

---

## Security Best Practices

### Do's ✅

- ✅ Use GitHub Secrets for all sensitive data
- ✅ Keep `.vercel` directory in `.gitignore`
- ✅ Rotate `VERCEL_TOKEN` periodically
- ✅ Use environment-specific variables
- ✅ Enable Vercel's security headers
- ✅ Set up proper CORS policies
- ✅ Use HTTPS in production (automatic with Vercel)

### Don'ts ❌

- ❌ Never commit `.env` files
- ❌ Don't hardcode API keys in code
- ❌ Don't use production secrets in preview
- ❌ Don't commit `.vercel/` directory
- ❌ Don't share `VERCEL_TOKEN` publicly

---

## Advanced Configuration

### Custom Domain

1. Go to Vercel project → **Settings → Domains**
2. Click **"Add Domain"**
3. Enter your domain (e.g., `mvp.toss-erp.com`)
4. Follow DNS configuration instructions
5. Wait for DNS propagation (can take up to 48 hours)

### Analytics

1. Go to **Analytics** tab in Vercel
2. Enable **Web Analytics**
3. Analytics automatically tracked

### Performance Monitoring

1. Enable Vercel Speed Insights
2. Monitor Core Web Vitals
3. Set up alerts for performance degradation

---

## Next Steps After Deployment

### 1. Monitor Your App

- **Vercel Dashboard:** Check deployments, analytics, logs
- **GitHub Actions:** Monitor workflow runs
- **Error Tracking:** Consider adding Sentry

### 2. Optimize

- Review Lighthouse reports
- Optimize images (use `@nuxt/image`)
- Enable caching strategies
- Minimize bundle size

### 3. Iterate

- Deploy updates via git push
- Use preview deployments for testing
- Merge PRs after review
- Monitor user feedback

### 4. Scale

When ready for backend integration:
- Update API base URL
- Enable real API calls
- Add authentication
- Configure CORS

---

## Deployment Checklist

Use this checklist for each deployment:

### Pre-Deployment
- [ ] Code reviewed and tested locally
- [ ] All tests passing
- [ ] Build succeeds locally (`npm run build`)
- [ ] No console errors
- [ ] Dependencies up to date

### Configuration
- [ ] GitHub secrets configured (3 secrets)
- [ ] Vercel Root Directory set to `toss-web`
- [ ] Environment variables configured (if needed)
- [ ] Build settings correct in Vercel

### Post-Deployment
- [ ] Deployment succeeded
- [ ] URL accessible
- [ ] All modules functional
- [ ] PWA installable
- [ ] No console errors
- [ ] Mobile responsive
- [ ] Performance acceptable

---

## Workflow Files

### Active Workflows

**`.github/workflows/deploy-vercel.yml`**
- Handles all Vercel deployments
- Production + Preview deployments
- PR comments with URLs

**`.github/workflows/ci-cd.yml`**
- Web frontend CI (linting, testing, building)
- Runs before Vercel deployment
- Ensures code quality

### Disabled Workflows (for future use)

Workflows with `.disabled` extension are kept for reference but won't run:
- `api-validation.yml.disabled` - For when backend APIs are ready
- `sdk-generation.yml.disabled` - For generating API clients
- `security-scan.yml.disabled` - For production security audits
- `markdownlint.yml.disabled` - For documentation linting

To re-enable: Remove the `.disabled` extension.

---

## Troubleshooting Guide

### Build Issues

**Symptom:** Build fails with dependency errors  
**Solution:**
```bash
cd toss-web
rm -rf node_modules package-lock.json
npm install
npm run build
```

**Symptom:** TypeScript errors during build  
**Solution:**
- Check `nuxt.config.ts` settings
- Verify all imports are correct
- Run `npm run typecheck` locally

### Deployment Issues

**Symptom:** Deployment stuck at "Building"  
**Solution:**
- Check Vercel status page
- Review build logs for specific errors
- Verify build command is correct

**Symptom:** "Function Timeout" errors  
**Solution:**
- Optimize server-side code
- Use static generation where possible
- Upgrade Vercel plan if needed

### Runtime Issues

**Symptom:** App loads but features don't work  
**Solution:**
- Check browser console
- Verify API endpoints (currently using mocks)
- Test mock data services

**Symptom:** PWA not installing  
**Solution:**
- Verify service worker registered
- Check manifest.json
- Test on HTTPS (Vercel provides this)

---

## Support & Resources

### Documentation

- **This Guide:** `.github/DEPLOYMENT_GUIDE.md`
- **Monorepo Fix:** `.github/VERCEL_MONOREPO_FIX.md`

### External Resources

- [Vercel Documentation](https://vercel.com/docs)
- [Vercel CLI Reference](https://vercel.com/docs/cli)
- [Nuxt Deployment Guide](https://nuxt.com/deploy/vercel)
- [GitHub Actions for Vercel](https://vercel.com/guides/how-can-i-use-github-actions-with-vercel)
- [Vercel Community Forum](https://github.com/vercel/vercel/discussions)

### Getting Help

1. Check this troubleshooting guide
2. Review Vercel deployment logs
3. Check GitHub Actions logs
4. Search Vercel community forum
5. Review Nuxt.js documentation

---

## FAQ

**Q: Can I deploy from a private repository?**  
A: Yes, but requires Vercel Pro plan ($20/month). Free tier only supports public repos for organization-owned repositories.

**Q: How long does deployment take?**  
A: Typically 1-3 minutes for initial deployment, 30-60 seconds for subsequent deployments with cache.

**Q: Can I rollback a deployment?**  
A: Yes! Go to Vercel → Deployments → Select previous deployment → Click "Promote to Production"

**Q: What happens to old preview deployments?**  
A: Preview deployments are kept for 7 days by default (configurable in Vercel settings).

**Q: Can I deploy multiple branches?**  
A: Yes! Configure additional branches in `.github/workflows/deploy-vercel.yml`

**Q: How do I see deployment logs?**  
A: Vercel Dashboard → Deployments → Click on deployment → View Build Logs & Function Logs

**Q: Can I use a custom domain?**  
A: Yes! Add it in Vercel → Settings → Domains (requires DNS configuration)

---

## Success Criteria

Your deployment is successful when:

✅ Build completes without errors  
✅ Deployment URL is accessible  
✅ All pages load correctly  
✅ No console errors  
✅ PWA installable on mobile  
✅ Mock data displays  
✅ Charts render properly  
✅ Mobile responsive  
✅ Lighthouse score > 90  

---

## 🎉 You're Ready!

Follow the [Quick Start](#quick-start-5-minutes) to deploy your TOSS MVP in 5 minutes!

**Questions?** Check the [Troubleshooting](#troubleshooting) section or refer to the [FAQ](#faq).

---

*Last updated: 2025-10-16*
*Version: 1.0.0*

