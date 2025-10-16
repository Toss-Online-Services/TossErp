# 🔧 Vercel Monorepo Setup - Quick Fix

## ⚠️ Common Error

If you see this error during Vercel deployment:

```
npm error code ENOENT
npm error syscall open
npm error path /vercel/path0/package.json
npm error errno -2
npm error enoent Could not read package.json
```

## ✅ Solution

Your project is in a **subdirectory** (`toss-web`), and Vercel needs to know this!

### Fix in Vercel Dashboard (Required)

1. **Go to Vercel Dashboard:** https://vercel.com/dashboard
2. **Select your project** (TOSS ERP)
3. **Click "Settings"** in the top navigation
4. **Scroll to "Build & Development Settings"**
5. **Set Root Directory:**
   ```
   toss-web
   ```
6. **Click "Save"**
7. **Redeploy** (trigger a new deployment)

### Verify in vercel.json (Optional)

You can also add this to `toss-web/vercel.json`:

```json
{
  "buildCommand": "npm run generate",
  "outputDirectory": ".output/public",
  "installCommand": "npm install",
  "framework": "nuxtjs"
}
```

## 📋 Checklist

After fixing:

- [ ] Root Directory set to `toss-web` in Vercel dashboard
- [ ] Saved the settings in Vercel
- [ ] Triggered a new deployment
- [ ] Deployment builds successfully
- [ ] Site is accessible

## 🎯 Why This Happens

Your repository structure is:
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

Vercel defaults to the repository root, but your app is in `toss-web/`, so it can't find `package.json` without the Root Directory setting.

## 🚀 After Fixing

Once the Root Directory is set, your deployments will work automatically via:

- ✅ GitHub Actions workflow
- ✅ Git push to `main` or `feature/mvp`
- ✅ Pull request previews

## 📞 Still Having Issues?

1. Check the full troubleshooting guide: `.github/VERCEL_DEPLOYMENT_COMPLETE.md`
2. Verify all 3 GitHub secrets are set correctly
3. Check Vercel deployment logs for specific errors
4. Ensure `toss-web/package.json` exists in your repository

---

**Quick Link:** [Vercel Root Directory Documentation](https://vercel.com/docs/projects/project-configuration#root-directory)

