# TOSS MVP - Vercel Deployment Quick Start

## 🚀 Quick Setup (5 Minutes)

### 1. Install Vercel CLI
```bash
npm install -g vercel
```

### 2. Link Project to Vercel
```bash
cd toss-web
vercel link
```
- Login when prompted
- Create new project or select existing
- Framework: **Nuxt.js**
- Root Directory: `./toss-web`

### 3. Get Your IDs
```bash
cat .vercel/project.json
```
Copy the `projectId` and `orgId` values.

### 4. Get Vercel Token
1. Visit: https://vercel.com/account/tokens
2. Create new token: "GitHub Actions - TOSS"
3. **Copy the token immediately!**

### 5. Add GitHub Secrets
Go to: `https://github.com/[YOUR-ORG]/TossErp/settings/secrets/actions`

Add these 3 secrets:
- `VERCEL_TOKEN` → Your token from step 4
- `VERCEL_ORG_ID` → From step 3
- `VERCEL_PROJECT_ID` → From step 3

### 6. Deploy!
```bash
git add .
git commit -m "feat: setup Vercel deployment"
git push origin feature/mvp
```

## ✅ Verification

1. Go to **GitHub → Actions** tab
2. Watch "Deploy to Vercel" workflow
3. Get deployment URL from workflow output
4. Test your app!

## 📋 Checklist

- [ ] Vercel CLI installed
- [ ] Project linked to Vercel
- [ ] Got `projectId` and `orgId`
- [ ] Created Vercel token
- [ ] Added 3 GitHub secrets
- [ ] Pushed to `feature/mvp` branch
- [ ] Workflow ran successfully
- [ ] App deployed and accessible

## 🆘 Quick Troubleshooting

**Workflow fails with "No token found"**
→ Check `VERCEL_TOKEN` secret is set correctly

**"Project not found" error**
→ Verify `VERCEL_PROJECT_ID` matches your project

**Build succeeds but site doesn't work**
→ Check browser console for errors
→ Verify `toss-web` is the root directory in Vercel settings

## 📚 Full Documentation

See `.github/VERCEL_DEPLOYMENT_SETUP.md` for detailed instructions.

## 🔗 Useful Links

- [Vercel Dashboard](https://vercel.com/dashboard)
- [GitHub Actions](https://github.com/Toss-Online-Services/TossErp/actions)
- [Vercel Docs](https://vercel.com/docs)

---

**Need help?** Check the full setup guide or review the workflow logs in GitHub Actions.

