# Deployment Guide (Frontend Nuxt PWA + Future Backend)

This consolidated guide replaces older deployment fragments (`README_DEPLOYMENT.md`, `DEPLOYMENT_CHECKLIST.md`, `.github/DEPLOYMENT_GUIDE.md`). It covers environments, build & deploy steps, CI/CD, secrets, verification, and next-phase backend integration.

## 1. Environments

| Environment | Purpose | Frontend Source | Backend Source | Data Mode | Notes |
|-------------|---------|-----------------|----------------|----------|-------|
| Local Dev   | Feature build & rapid testing | `feature/*` branch (`toss-web`) | Future `.NET` service (not yet integrated) | Mock | Hot reload, Sentry disabled unless DSN set |
| Preview     | PR validation (Vercel Preview) | PR branches | N/A currently | Mock | Automatic ephemeral URL; used for UX review |
| Staging (optional) | Pre-prod integration | `develop` (planned) | Early backend container | Hybrid (mock + real) | Enable feature flags first |
| Production  | Live MVP | `main` | Backend (post-MVP) | Mock (transitioning) | Sentry + analytics enabled |

## 2. Frontend Build & Deploy (Vercel)

### Quick Deploy (CLI)
```powershell
npm install -g vercel
cd toss-web
vercel            # first link
vercel --prod     # production deployment
```

### Required Vercel Settings
| Setting | Value |
|---------|-------|
| Root Directory | `toss-web` |
| Framework Preset | Nuxt.js |
| Build Command | `npm run generate` |
| Output Directory | `.output/public` |
| Node Version | 22.x (matches `package.json` engines) |

### Monorepo Root Fix
Ensure the Vercel project "Root Directory" is set to `toss-web` or builds will fail with `ENOENT package.json`.

## 3. Backend (Future Azure Integration)

Planned: .NET 9 API deployed to Azure App Service or Azure Container Apps. Steps (draft):
1. Containerize API (`Dockerfile` multi-stage build)
2. Provision Azure resources: App Service, PostgreSQL Flexible Server, Redis Cache (optional)
3. Configure connection strings in Azure App Service Settings
4. Add CI job to build/push container to Azure Container Registry
5. Add infrastructure IaC (Bicep or Terraform) for reproducibility

## 4. Environment Variables

Maintain variables in Vercel dashboard per environment (and `.env.example` local):

| Variable | Scope | Description |
|----------|-------|-------------|
| `NUXT_PUBLIC_API_BASE_URL` | Frontend | Points to backend API (mock now) |
| `NUXT_PUBLIC_SENTRY_DSN` | Frontend | Enables Sentry client/server if set |
| `NUXT_PUBLIC_ENABLE_REAL_API` | Frontend | Feature flag for switching off mock services |
| Back-end secrets (future) | Backend | DB connection strings, JWT signing key, Redis password |

Never commit real secrets. Use Vercel Environment Variables and GitHub Actions Secrets (`VERCEL_TOKEN`, `VERCEL_ORG_ID`, `VERCEL_PROJECT_ID`).

## 5. CI/CD Overview

Pipeline (GitHub Actions):
- Trigger: Push/PR to `feature/*`, `main`, future `develop`
- Steps: Install → Build (`nuxt generate`) → (Tests upcoming stabilization) → Deploy via Vercel API
- Preview deployments for PRs; production on merges to `main`.

Future additions:
- Source map upload (Sentry)
- Contract tests vs backend
- E2E Playwright run on preview URL

## 6. Verification Checklist (Post-Deploy)

| Check | Command / Action |
|-------|------------------|
| Build success | `npm run generate` locally prior to push |
| App reachable | Open deployment URL |
| Core modules load | Navigate Dashboard, POS, Inventory, Orders |
| PWA installable | Chrome → Install App prompt |
| Offline navigation | Enable airplane mode, revisit cached routes |
| No console errors | Browser DevTools Console |
| Resource sizes acceptable | DevTools Network tab (initial bundle < ~500KB) |
| Lighthouse PWA score > 80 | DevTools Lighthouse audit |
| Sentry events (if DSN set) | Sentry dashboard issue list |

## 7. PWA & Offline
Implemented via `@vite-pwa/nuxt`: service worker precaches assets, runtime caching of API responses (mock). Future: queue write ops for sync when backend arrives. Test offline after first online visit (SW registration required).

## 8. Troubleshooting (Condensed)

| Symptom | Cause | Fix |
|---------|-------|-----|
| ENOENT package.json | Root dir not set | Set Root Directory to `toss-web` in Vercel settings |
| Build fails dependencies | Peer conflicts | Delete `node_modules`, reinstall, verify Node 22.x |
| PWA not installing | Missing manifest/SW | Validate `/manifest.json` and service worker in DevTools |
| Blank page prod | Runtime error | Check console + Sentry (if enabled) |
| Offline fails | SW not cached yet | Load site once online before testing offline |

## 9. Release & Versioning
Semantic version pattern: `1.0.0-mvp` for current release; increment patch for hotfixes, minor for feature additions pre GA. Tag releases in Git; optionally annotate with deployment URL.

## 10. Security & Observability
- Sentry integrated via `@sentry/nuxt`; DSN required.
- Add CSP & security headers via Vercel project settings / middleware (future hardening).
- Avoid storing PII client-side; plan encryption-at-rest server-side.

## 11. Future Enhancements
- Backend integration (real API base URL)
- Sentry source map upload in CI
- Husky + lint-staged pre-commit quality gate
- Playwright E2E gating before production deploy
- Infrastructure as Code for Azure resources

## 12. Minimal Local Workflow
```powershell
cd toss-web
npm install
npm run dev   # port 3001
npm run generate
npm run preview
```

## 13. Rollback Strategy
Use Vercel dashboard → select previous successful deployment → Promote to Production. Maintain previous build artifacts automatically.

## 14. Ownership
| Area | Current Owner | Future Owner |
|------|---------------|--------------|
| Frontend Deploy | Web team | Web team |
| Backend Deploy | N/A | Platform team |
| Secrets Mgmt | Web team (Vercel) | Shared DevOps |
| Monitoring | Web team (Sentry) | Shared DevOps |

---
Last updated: 2025-11-10
