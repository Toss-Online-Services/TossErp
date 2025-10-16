# .github Directory Structure

This directory contains GitHub-specific configuration files for the TOSS MVP project.

## 📁 Directory Contents

### Active Workflows

**`workflows/deploy-vercel.yml`**
- Automatically deploys `toss-web` frontend to Vercel
- Triggers: Push to `main`/`feature/mvp`, Pull Requests
- Features: Production + Preview deployments, PR comments

**`workflows/ci-cd.yml`**
- CI pipeline for web frontend
- Runs: Linting, testing, building
- Ensures code quality before deployment

### Documentation

**`DEPLOYMENT_GUIDE.md`** - Comprehensive deployment guide
- Quick start (5 minutes)
- Detailed setup instructions
- Monorepo configuration
- Troubleshooting
- Best practices

**`VERCEL_MONOREPO_FIX.md`** - Quick fix for monorepo setup
- Specific solution for "Could not read package.json" error
- Step-by-step Root Directory configuration

### AI Instructions

**`instructions/`** - AI coding assistant rules
- Development workflow guidelines
- Taskmaster task management rules
- Code quality standards
- Self-improvement patterns

### Disabled Workflows

Workflows with `.disabled` extension are kept for future use:
- `api-validation.yml.disabled` - For backend API validation
- `sdk-generation.yml.disabled` - For API client generation
- `security-scan.yml.disabled` - For production security audits
- `markdownlint.yml.disabled` - For documentation linting

**To re-enable:** Remove the `.disabled` extension and update paths/configuration.

## 🚀 Quick Links

- **Deploy to Vercel:** See `DEPLOYMENT_GUIDE.md`
- **Fix Monorepo Error:** See `VERCEL_MONOREPO_FIX.md`
- **View Workflows:** Check `workflows/` directory
- **GitHub Actions:** [View in GitHub](../../actions)

## 📋 Deployment Checklist

Before first deployment:

- [ ] Repository is public (or Vercel Pro plan)
- [ ] Vercel account created
- [ ] Project linked via `vercel link`
- [ ] GitHub secrets configured (3 secrets)
- [ ] **Vercel Root Directory set to `toss-web`** ⚠️
- [ ] Push to `feature/mvp` branch

## 🔐 Required GitHub Secrets

Configure these in: Settings → Secrets and variables → Actions

1. `VERCEL_TOKEN` - From Vercel account tokens
2. `VERCEL_ORG_ID` - From `.vercel/project.json`
3. `VERCEL_PROJECT_ID` - From `.vercel/project.json`

## 🆘 Need Help?

1. Check `DEPLOYMENT_GUIDE.md` for comprehensive instructions
2. See `VERCEL_MONOREPO_FIX.md` for common error fixes
3. Review GitHub Actions logs for specific errors
4. Check Vercel deployment logs in dashboard

---

**Ready to deploy?** Start with the [Deployment Guide](DEPLOYMENT_GUIDE.md)! 🚀

