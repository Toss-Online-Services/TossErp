# TOSS Web - Frontend Application

## Overview

TOSS (The One-Stop Solution) is a mobile-first, offline-first ERP-III platform for South African township and rural SMMEs.

## Technology Stack

- **Framework**: Nuxt 4 (Vue 3.5+)
- **Styling**: Tailwind CSS + shadcn-vue components
- **State Management**: Pinia
- **TypeScript**: Full TypeScript support
- **PWA**: Offline-first with service workers (to be implemented)

## Project Structure

```
toss-web/
├── assets/
│   └── css/
│       └── main.css          # Global styles and Tailwind directives
├── components/
│   └── ui/
│       ├── Button.vue        # Reusable button component
│       └── Card.vue          # Reusable card component
├── layouts/
│   └── default.vue           # Main layout with sidebar and topbar
├── pages/
│   └── index.vue             # Dashboard/Home page with KPI cards
├── nuxt.config.ts            # Nuxt configuration
├── tailwind.config.js        # Tailwind CSS configuration
└── package.json              # Dependencies
```

## Features Implemented

### ✅ Completed
1. **Project Setup**
   - Nuxt 4 initialization with TypeScript
   - Tailwind CSS configuration
   - shadcn-vue components integration
   - Pinia for state management
   - VueUse utilities

2. **Base Layout**
   - Responsive sidebar navigation
   - Top navigation bar with user menu
   - Mobile-first design
   - Navigation links for all main modules:
     - Home
     - Sales
     - Stock
     - Money
     - People
     - Jobs
     - Settings

3. **Dashboard/Home Page**
   - KPI cards showing:
     - Today's Sales
     - Money In
     - Money Out
   - Alert cards for:
     - Low Stock Items
     - Pending Orders
     - Overdue Invoices
   - Sales trend chart (weekly)
   - Quick action buttons

4. **UI Components**
   - Button component with variants (primary, secondary, outline, ghost)
   - Card component with Material Dashboard styling
   - Gradient backgrounds for KPI cards
   - Icon integration

### 🚧 To Be Implemented

1. **PWA & Offline Support**
   - Service worker configuration
   - IndexedDB for offline data
   - Background sync
   - Offline indicators

2. **Module Pages**
   - Stock/Inventory module UI
   - POS module with offline support
   - Sales module (quotes, orders, invoices)
   - CRM module
   - Procurement/Buying module
   - Accounting module (Money In/Out detailed view)

3. **State Management**
   - Pinia stores for each module
   - API integration layer
   - Authentication store

4. **Testing**
   - Vitest setup for unit tests
   - Playwright setup for E2E tests
   - Component tests
   - Integration tests

5. **API Integration**
   - Connect to .NET backend
   - API client setup
   - Error handling
   - Loading states

## Running the Application

### Development

```bash
# Install dependencies
npm install

# Start development server
npm run dev
```

The application will be available at `http://localhost:3000`

### Build

```bash
# Build for production
npm run build

# Preview production build
npm run preview
```

## Design Reference

The UI is inspired by Material Dashboard Pro - Analytics, featuring:
- Clean, spacious card layouts
- Gradient backgrounds for key metrics
- Simple, friendly language
- Mobile-first responsive design
- High contrast for outdoor visibility

## Color Palette

- **Primary Blue**: #0284c7 (TOSS Blue)
- **Success Green**: #10b981 (TOSS Green)
- **Warning Orange**: #f59e0b (TOSS Orange)
- **Danger Red**: #ef4444

## Next Steps

1. Implement PWA configuration with @vite-pwa/nuxt
2. Create Stock/Inventory module pages
3. Build POS interface with offline queue
4. Setup Pinia stores for state management
5. Integrate with backend API
6. Add comprehensive testing
7. Deploy to production

## Notes

- All UI text uses plain, friendly language (e.g., "Money In/Out" instead of "Income Statement")
- Designed for low-end Android devices
- Optimized for offline-first usage
- Follows TOSS product vision for township/rural SMMEs
