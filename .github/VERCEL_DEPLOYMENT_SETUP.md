# Vercel Deployment Setup Guide

This guide will help you configure GitHub Actions to automatically deploy your TOSS MVP frontend to Vercel.

## Prerequisites

- A Vercel account (free tier works fine)
- GitHub repository with admin access
- `toss-web` project ready for deployment

## Step 1: Link Your Project to Vercel

### Option A: Using Vercel CLI (Recommended)

1. **Install Vercel CLI globally:**
   ```bash
   npm install -g vercel
   ```

2. **Navigate to your project:**
   ```bash
   cd toss-web
   ```

3. **Link the project to Vercel:**
   ```bash
   vercel link
   ```
   
   Follow the prompts:
   - Login to your Vercel account
   - Select or create a new project
   - Choose the framework preset: **Nuxt.js**
   - Confirm the project settings

4. **This creates a `.vercel` directory** with a `project.json` file containing:
   - `projectId`
   - `orgId`

### Option B: Using Vercel Dashboard

1. Go to [vercel.com/new](https://vercel.com/new)
2. Import your Git repository
3. Select `toss-web` as the root directory
4. Framework Preset: **Nuxt.js**
5. Build Command: `npm run generate`
6. Output Directory: `.output/public`
7. Click "Deploy"

## Step 2: Get Your Vercel Credentials

### 1. Get Vercel Access Token

1. Go to [Vercel Account Settings → Tokens](https://vercel.com/account/tokens)
2. Click **"Create Token"**
3. Name it: `GitHub Actions - TOSS ERP`
4. Scope: **Full Account** (or limit to specific projects)
5. **Copy the token** (you won't see it again!)

### 2. Get Organization ID and Project ID

**Method 1: From `.vercel/project.json`**

After running `vercel link`, check the file:
```bash
cat toss-web/.vercel/project.json
```

You'll see:
```json
{
  "projectId": "prj_xxxxxxxxxxxxxxxxxxxx",
  "orgId": "team_xxxxxxxxxxxxxxxxxxxx"
}
```

**Method 2: From Vercel Dashboard**

1. Go to your project settings on Vercel
2. Scroll to **"Project ID"** - copy it
3. For `orgId`:
   - If personal account: Your username slug
   - If team: Navigate to team settings and find the team ID

## Step 3: Configure GitHub Secrets

1. Go to your GitHub repository
2. Navigate to **Settings → Secrets and variables → Actions**
3. Click **"New repository secret"** and add:

### Required Secrets:

| Secret Name | Value | Description |
|------------|-------|-------------|
| `VERCEL_TOKEN` | `xxxxxxxxxxxxxxxxxxxx` | Your Vercel Access Token from Step 2.1 |
| `VERCEL_ORG_ID` | `team_xxxxxxxxxxxx` or `user_xxxx` | Your Organization/Team ID |
| `VERCEL_PROJECT_ID` | `prj_xxxxxxxxxxxxxxxxxxxx` | Your Project ID |

**Example:**
- Name: `VERCEL_TOKEN`
- Secret: `cPZAKf2hgj9lQ5pNm7Xr8wY3vT6sU4zL` (example, use your actual token)

## Step 4: Verify Workflow File

Ensure `.github/workflows/deploy-vercel.yml` exists in your repository. The workflow should:

✅ Trigger on push to `main` and `feature/mvp` branches  
✅ Trigger on pull requests  
✅ Install Vercel CLI  
✅ Build the project  
✅ Deploy to Vercel (preview for PRs, production for main/mvp)  
✅ Comment on PRs with deployment URL  

## Step 5: Configure Vercel Project Settings (Optional)

### Environment Variables

If your app needs environment variables in production:

1. Go to your Vercel project dashboard
2. Navigate to **Settings → Environment Variables**
3. Add any required variables:
   - `NUXT_PUBLIC_API_BASE_URL` (for future backend integration)
   - Any other configuration

### Build & Development Settings

Ensure these are set correctly:

- **Framework Preset:** Nuxt.js
- **Build Command:** `npm run generate`
- **Output Directory:** `.output/public`
- **Install Command:** `npm install`
- **Root Directory:** `toss-web` (if in monorepo)

## Step 6: Test the Deployment

### Automatic Deployment

1. **Push to trigger deployment:**
   ```bash
   git add .
   git commit -m "feat: setup Vercel deployment"
   git push origin feature/mvp
   ```

2. **Monitor the deployment:**
   - Go to **Actions** tab in GitHub
   - Watch the "Deploy to Vercel" workflow run
   - Check for any errors

3. **Verify deployment:**
   - Once successful, you'll get a deployment URL
   - Test the deployed app
   - Check PWA functionality
   - Verify all modules load correctly

### Manual Deployment (Optional)

You can also trigger deployments manually:

1. Go to **Actions** tab in GitHub
2. Select "Deploy to Vercel" workflow
3. Click **"Run workflow"**
4. Choose the branch
5. Click **"Run workflow"**

## Step 7: Branch-Specific Deployments

The workflow is configured to:

- **Production Deployments:** `main` and `feature/mvp` branches
  - URL: Your production Vercel domain
  - Example: `https://toss-mvp.vercel.app`

- **Preview Deployments:** All pull requests
  - URL: Unique preview URL for each PR
  - Example: `https://toss-mvp-git-feature-xyz-username.vercel.app`

## Troubleshooting

### Common Issues

**1. "Error: No token found"**
- Ensure `VERCEL_TOKEN` secret is set correctly in GitHub
- Token should not have spaces or quotes

**2. "Error: Project not found"**
- Verify `VERCEL_PROJECT_ID` matches your project
- Check if the project exists in your Vercel dashboard

**3. "Error: Organization not found"**
- Verify `VERCEL_ORG_ID` is correct
- For personal accounts, use your username
- For teams, use the team ID

**4. "Build failed"**
- Check Node.js version compatibility
- Ensure `package.json` dependencies are correct
- Review build logs in GitHub Actions

**5. "Deployment succeeded but site doesn't work"**
- Check environment variables in Vercel dashboard
- Verify build output directory is correct
- Check browser console for errors

### Debug Mode

To enable verbose logging in the workflow:

1. Go to GitHub repository settings
2. Navigate to **Settings → Secrets and variables → Actions**
3. Under **Variables**, add:
   - Name: `ACTIONS_STEP_DEBUG`
   - Value: `true`

## Next Steps

After successful deployment:

1. **Set up custom domain** (optional):
   - Go to Vercel project settings
   - Navigate to **Domains**
   - Add your custom domain

2. **Configure production environment variables**:
   - Add any API keys or secrets needed
   - Set `NUXT_PUBLIC_API_BASE_URL` for backend integration

3. **Enable Vercel Analytics** (optional):
   - Go to **Analytics** tab
   - Enable Web Analytics
   - Add the script to your Nuxt config

4. **Set up monitoring**:
   - Configure Lighthouse CI for performance monitoring
   - Set up Sentry for error tracking
   - Enable Vercel's built-in monitoring

## Security Best Practices

- ✅ Never commit `.vercel` directory to Git
- ✅ Keep `VERCEL_TOKEN` secret and rotate it periodically
- ✅ Use environment-specific variables in Vercel dashboard
- ✅ Enable Vercel's security headers
- ✅ Set up proper CORS policies for production

## Additional Resources

- [Vercel Documentation](https://vercel.com/docs)
- [GitHub Actions for Vercel](https://vercel.com/guides/how-can-i-use-github-actions-with-vercel)
- [Nuxt Deployment Guide](https://nuxt.com/deploy/vercel)
- [Vercel CLI Reference](https://vercel.com/docs/cli)

## Support

If you encounter issues:

1. Check the GitHub Actions logs
2. Review Vercel deployment logs
3. Consult the [Vercel Community](https://github.com/vercel/vercel/discussions)
4. Check the troubleshooting section above

---

**Ready to Deploy?** 🚀

Once you've completed all steps above, your TOSS MVP will automatically deploy to Vercel on every push!

