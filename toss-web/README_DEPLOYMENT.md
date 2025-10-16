# TOSS MVP - Deployment Guide

## 🚀 Quick Deploy to Vercel (Recommended)

### Option 1: Vercel CLI (Fastest)

```bash
# Install Vercel CLI globally
npm install -g vercel

# Navigate to toss-web directory
cd toss-web

# Install dependencies
npm install

# Deploy to Vercel
vercel

# For production deployment
vercel --prod
```

### Option 2: Vercel Dashboard

1. **Connect Repository**
   - Go to [vercel.com](https://vercel.com)
   - Click "Add New Project"
   - Import your GitHub repository

2. **Configure Project**
   - Framework Preset: **Nuxt.js**
   - Root Directory: `toss-web`
   - Build Command: `npm run generate`
   - Output Directory: `.output/public`

3. **Environment Variables** (Optional for MVP)
   - Add any environment variables from `.env.example`
   - For MVP, no env vars are required (using mock data)

4. **Deploy**
   - Click "Deploy"
   - Wait 2-3 minutes for build to complete
   - Get your live URL!

## 📱 PWA Features

This MVP includes full Progressive Web App support:

- ✅ **Offline Mode**: Works without internet connection
- ✅ **Installable**: Add to home screen on mobile devices
- ✅ **Mobile-Optimized**: Touch-friendly, responsive design
- ✅ **Service Worker**: Automatic caching and background sync
- ✅ **Push Notifications**: Ready for future implementation

### Testing PWA Features Locally

```bash
npm run build
npm run preview
```

Then open Chrome DevTools:
- **Application** tab → **Manifest** (verify PWA config)
- **Application** tab → **Service Workers** (verify registration)
- **Lighthouse** tab → Run audit (check PWA score)

### Installing on Mobile

1. Open the deployed URL on your mobile device
2. Look for "Add to Home Screen" prompt (Chrome/Safari)
3. Tap to install
4. App appears on home screen like native app

## 🔧 Local Development

```bash
# Install dependencies
npm install

# Start development server (port 3001)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview
```

## 📊 Current MVP Status

### ✅ Completed Modules
- **Stock/Inventory**: Full inventory management with offline support
- **Logistics**: Driver registration, job assignment, delivery tracking
- **Purchasing**: Orders, suppliers, group buying, asset sharing
- **Sales**: Orders, quotations, invoices, POS system
- **Automation**: Workflows, triggers, AI recommendations
- **Dashboard**: KPIs, analytics, charts
- **Onboarding**: Multi-step user onboarding flow

### 🔄 Using Mock Data
All modules currently use mock/static data. This allows:
- Frontend development without backend dependency
- Offline functionality testing
- User experience validation
- Easy transition to real backend later

### 🚧 Coming Soon
- ❌ WhatsApp Integration (UI placeholders ready)
- ❌ Backend API Connection (architecture ready)
- ❌ Real-time AI Features
- ❌ Live Payment Processing
- ❌ Multi-tenant Support

## 🌍 Alternative Deployment Platforms

### Netlify

```bash
# Install Netlify CLI
npm install -g netlify-cli

# Deploy
netlify deploy --prod
```

**Build Settings:**
- Build command: `npm run generate`
- Publish directory: `.output/public`

### Cloudflare Pages

1. Connect repository at pages.cloudflare.com
2. Build command: `npm run generate`
3. Build output directory: `.output/public`
4. Deploy

## 📈 Performance Targets

- **Lighthouse Score**: >80 across all metrics
- **First Contentful Paint**: <2s
- **Time to Interactive**: <3s
- **Bundle Size**: <500KB initial load
- **Offline Support**: Full offline navigation

## 🔐 Security Headers

The following security headers are configured in `vercel.json`:
- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`
- Service Worker allowed for PWA functionality

## 📱 Mobile Testing Checklist

- [ ] Install PWA on iOS Safari
- [ ] Install PWA on Android Chrome
- [ ] Test offline mode (airplane mode)
- [ ] Test touch interactions (44px min targets)
- [ ] Test landscape and portrait orientations
- [ ] Verify forms don't zoom on iOS (16px inputs)
- [ ] Test on slow 3G connection
- [ ] Verify pull-to-refresh doesn't conflict
- [ ] Check safe area insets on notched devices

## 🐛 Troubleshooting

### Build Fails
```bash
# Clear cache and rebuild
rm -rf .nuxt .output node_modules
npm install
npm run build
```

### PWA Not Installing
- Ensure HTTPS (Vercel provides this automatically)
- Check manifest.json is generated (`/.output/public/manifest.json`)
- Verify service worker is registered (Chrome DevTools)
- Check all icons exist in `/public/icons/`

### Offline Mode Not Working
- Service worker must be registered first (visit site online once)
- Check cache storage in DevTools → Application → Cache Storage
- Verify workbox configuration in `nuxt.config.ts`

## 📞 Support & Next Steps

After deployment:
1. Share URL with stakeholders
2. Collect feedback via forms/WhatsApp
3. Monitor analytics for usage patterns
4. Plan backend integration based on usage
5. Prioritize features based on user feedback

## 🎯 Success Metrics

Track these metrics post-deployment:
- Number of PWA installs
- Offline usage percentage
- Most-used modules
- Mobile vs desktop traffic
- User session duration
- Bounce rate per module

---

**Deployment Date**: {{ DATE }}
**Version**: 1.0.0-mvp
**Environment**: Production (Mock Data Mode)

