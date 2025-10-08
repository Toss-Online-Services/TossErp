# TOSS ERP Web - Quick Start Guide

## 🚀 Getting Started

### Prerequisites
- Node.js 18+ installed
- npm or yarn package manager
- Modern web browser (Chrome, Firefox, Edge)

### Installation

```bash
# Navigate to project directory
cd toss-web

# Install dependencies (if not already done)
npm install

# Generate PWA icons (if not already done)
npm install -D sharp
node scripts/generate-icons.js

# Start development server
npm run dev
```

The application will be available at `http://localhost:3000`

## 📱 PWA Features

### Installing the App

#### Desktop
1. Open the app in Chrome/Edge
2. Look for the install icon in the address bar
3. Click "Install" or wait for the prompt
4. App will be installed and launch standalone

#### Mobile
1. Open the app in mobile browser
2. Tap the share/menu button
3. Select "Add to Home Screen" or "Install"
4. App icon will appear on home screen

### Testing Offline Mode

1. Open the app
2. Open DevTools (F12)
3. Go to Network tab
4. Enable "Offline" mode
5. Navigate the app - it should still work
6. Add items to cart in POS
7. Go back online
8. Changes will sync automatically

## 🎯 Key Features to Test

### Point of Sale (POS)
- Navigate to `/sales/pos`
- Search for products
- Add items to cart
- Adjust quantities
- Apply discounts
- Select payment method
- Complete transaction
- Print/email receipt

### Inventory Management
- Navigate to `/stock`
- View stock levels
- Check low stock alerts
- Review AI recommendations
- View recent stock movements

### CRM
- Navigate to `/crm`
- View customer list
- Add new customers
- Manage leads
- Track opportunities
- View sales pipeline

### Accounts
- Navigate to `/accounts`
- View financial dashboard
- Check assets and liabilities
- Review revenue and expenses
- Manage chart of accounts

## 🔧 Development

### Project Structure
```
toss-web/
├── components/      # Vue components
├── composables/     # Composable functions
├── layouts/         # Layout components
├── pages/           # Page routes
├── public/          # Static files
├── server/          # Server API routes
├── stores/          # Pinia stores
└── nuxt.config.ts   # Nuxt configuration
```

### Building for Production

```bash
# Build the application
npm run build

# Preview production build
npm run preview

# Generate static site
npm run generate
```

### PWA Development

The PWA is enabled in development mode. To test:

1. Run `npm run dev`
2. Open DevTools > Application
3. Check Service Workers tab
4. Test offline functionality
5. Check Cache Storage

## 🎨 Customization

### Branding
- Edit icons in `public/icon.svg`
- Regenerate icons with `node scripts/generate-icons.js`
- Update colors in `tailwind.config.js`
- Modify theme in `nuxt.config.ts` PWA manifest

### Theme Colors
Current theme:
- Primary: Blue (#1d4ed8)
- Secondary: Amber (#f59e0b)
- Accent: Emerald (#10b981)
- Background (Dark): Slate (#0f172a)

## 📊 Modules

### Available Modules
- ✅ Dashboard - Main overview
- ✅ CRM - Customer management
- ✅ Sales - Orders and invoices
- ✅ POS - Point of sale system
- ✅ Inventory - Stock management
- ✅ Purchasing - Supplier orders
- ✅ Accounts - Financial management
- ✅ HR - Employee management
- ✅ Projects - Task and time tracking
- ⏳ Manufacturing - Coming soon

## 🔐 Authentication

Current authentication flow:
1. Login page at `/login`
2. Register page at `/register`
3. Protected routes use `auth` middleware
4. Session stored in cookies/localStorage

## 🌐 API Integration

### API Endpoints
Base URL: `http://localhost:8081/api` (development)

Available services:
- `/api/crm` - Customer data
- `/api/sales` - Sales operations
- `/api/inventory` - Stock data
- `/api/financial` - Financial data
- `/api/hr` - Employee data
- `/api/projects` - Project data

### Offline Support
When offline:
- API calls are queued
- Data is cached locally
- Auto-sync when back online

## 🧪 Testing

### Manual Testing Checklist

#### PWA
- [ ] Install prompt appears
- [ ] App installs correctly
- [ ] Offline mode works
- [ ] Data syncs when online
- [ ] Icons display correctly
- [ ] Splash screen shows

#### Responsive Design
- [ ] Mobile layout works (< 640px)
- [ ] Tablet layout works (640-1024px)
- [ ] Desktop layout works (> 1024px)
- [ ] Touch targets are 44px+
- [ ] Text is readable at all sizes

#### Core Features
- [ ] Login/Register works
- [ ] Dashboard loads data
- [ ] POS completes transaction
- [ ] Inventory updates stock
- [ ] CRM adds customers
- [ ] Dark mode toggles

## 🐛 Troubleshooting

### PWA Not Installing
1. Check HTTPS is enabled
2. Verify manifest.json is accessible
3. Check service worker registration
4. Clear browser cache
5. Try incognito mode

### Offline Not Working
1. Check IndexedDB in DevTools
2. Verify service worker is active
3. Check cache storage
4. Review network requests
5. Check console for errors

### Build Errors
1. Clear `.nuxt` folder
2. Delete `node_modules`
3. Run `npm install` again
4. Rebuild with `npm run build`
5. Check Node.js version (18+)

## 📚 Resources

### Documentation
- [Nuxt 3 Docs](https://nuxt.com)
- [Vue 3 Docs](https://vuejs.org)
- [Tailwind CSS](https://tailwindcss.com)
- [PWA Guide](https://web.dev/progressive-web-apps/)

### Tools
- [Lighthouse](https://developers.google.com/web/tools/lighthouse) - PWA testing
- [PWA Builder](https://www.pwabuilder.com/) - PWA utilities
- [Can I Use](https://caniuse.com/) - Browser support

## 🎓 Best Practices

### Performance
- Use lazy loading for routes
- Optimize images
- Minimize bundle size
- Use proper caching strategies
- Monitor Core Web Vitals

### Security
- Use HTTPS in production
- Validate all inputs
- Sanitize user data
- Implement CORS properly
- Use secure headers

### Accessibility
- Use semantic HTML
- Add ARIA labels
- Ensure keyboard navigation
- Maintain color contrast
- Test with screen readers

## 🚢 Deployment

### Production Requirements
- ✅ HTTPS enabled
- ✅ Service worker configured
- ✅ PWA manifest valid
- ✅ Icons generated
- ✅ Cache headers set
- ✅ Compression enabled

### Recommended Hosting
- Vercel (recommended)
- Netlify
- AWS Amplify
- Azure Static Web Apps
- Google Firebase

### Deployment Steps
```bash
# Build for production
npm run build

# Test production build locally
npm run preview

# Deploy to hosting platform
# (Follow platform-specific instructions)
```

## ℹ️ Support

For issues or questions:
1. Check this documentation
2. Review the implementation summary
3. Check browser console for errors
4. Test in incognito mode
5. Clear cache and try again

---

**Happy coding! 🎉**

*Last updated: October 8, 2025*

