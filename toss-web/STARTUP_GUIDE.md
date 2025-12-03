# TOSS Web - Quick Start Guide

## Prerequisites

- Node.js 18+ or 20+ installed
- npm or pnpm package manager
- Terminal/Command Prompt access

## Initial Setup

### 1. Navigate to Project Directory

```bash
cd C:\Users\PROBOOK\source\repos\Toss-Online-Services\TossErp\toss-web
```

### 2. Install Dependencies

```bash
npm install
```

This will install all required packages including:
- Nuxt 4
- Vue 3
- Tailwind CSS
- shadcn-vue
- Pinia
- VueUse
- TypeScript

### 3. Start Development Server

```bash
npm run dev
```

The application will start on `http://localhost:3000`

## Available Scripts

```bash
# Development
npm run dev          # Start development server with hot reload

# Building
npm run build        # Build for production
npm run generate     # Generate static site
npm run preview      # Preview production build locally

# Code Quality
npm run typecheck    # Run TypeScript type checking (when enabled)
npm run lint         # Run linter (if configured)
```

## Troubleshooting

### Issue: "Cannot find module 'vue-tsc'"

**Solution:**
```bash
npm install -D vue-tsc typescript
```

### Issue: Port 3000 already in use

**Solution:**
```bash
# Use a different port
PORT=3001 npm run dev
```

Or kill the process using port 3000:
```bash
# Windows
netstat -ano | findstr :3000
taskkill /PID <PID> /F

# Linux/Mac
lsof -ti:3000 | xargs kill
```

### Issue: Module errors or missing dependencies

**Solution:**
```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
```

### Issue: Tailwind styles not loading

**Solution:**
1. Check that `assets/css/main.css` exists
2. Verify `nuxt.config.ts` includes `css: ['~/assets/css/main.css']`
3. Restart the dev server

## Project Structure Overview

```
toss-web/
├── .nuxt/              # Auto-generated (don't edit)
├── assets/
│   └── css/
│       └── main.css    # Global styles
├── components/
│   └── ui/             # Reusable UI components
├── layouts/
│   └── default.vue     # Main layout
├── pages/              # File-based routing
│   └── index.vue       # Home/Dashboard
├── public/             # Static assets
├── node_modules/       # Dependencies (don't commit)
├── nuxt.config.ts      # Nuxt configuration
├── tailwind.config.js  # Tailwind configuration
├── tsconfig.json       # TypeScript configuration
└── package.json        # Project dependencies
```

## Development Workflow

### 1. Creating New Pages

Pages are automatically routed based on file structure:

```bash
pages/
├── index.vue           # → /
├── sales.vue           # → /sales
└── stock/
    ├── index.vue       # → /stock
    └── items.vue       # → /stock/items
```

### 2. Creating New Components

```vue
<!-- components/ui/MyComponent.vue -->
<script setup lang="ts">
// Component logic
</script>

<template>
  <!-- Component template -->
</template>
```

Components in `components/` are auto-imported.

### 3. Using Tailwind CSS

```vue
<template>
  <div class="bg-blue-500 text-white p-4 rounded-lg">
    Styled with Tailwind
  </div>
</template>
```

### 4. Using Pinia Stores

```typescript
// stores/counter.ts
import { defineStore } from 'pinia'

export const useCounterStore = defineStore('counter', {
  state: () => ({
    count: 0
  }),
  actions: {
    increment() {
      this.count++
    }
  }
})
```

```vue
<!-- In component -->
<script setup>
import { useCounterStore } from '~/stores/counter'

const counter = useCounterStore()
</script>
```

## Testing the Application

### Visual Checklist

When you run `npm run dev`, you should see:

✅ Nuxt starting message
✅ Local URL: `http://localhost:3000/`
✅ No compilation errors
✅ Browser opens automatically (or open manually)

### In the Browser

You should see:

✅ **Top Navigation Bar**
  - TOSS logo
  - Hamburger menu button
  - User profile icon

✅ **Sidebar Navigation**
  - Home
  - Sales
  - Stock
  - Money
  - People
  - Jobs
  - Settings

✅ **Dashboard Content**
  - Today's Overview heading
  - 3 KPI cards (Sales, Money In, Money Out)
  - 3 Alert cards (Low Stock, Pending Orders, Overdue Invoices)
  - Sales trend chart
  - 4 Quick action buttons

✅ **Responsive Behavior**
  - Sidebar collapses on mobile
  - Cards stack vertically on small screens
  - Touch-friendly button sizes

## Common Development Tasks

### Adding a New Page

1. Create file in `pages/` directory
2. Add route to sidebar in `layouts/default.vue`
3. Implement page content

### Adding a New Component

1. Create file in `components/ui/`
2. Use in pages/layouts (auto-imported)

### Updating Styles

1. Edit `tailwind.config.js` for theme changes
2. Edit `assets/css/main.css` for global styles
3. Use Tailwind classes in components

### Connecting to Backend

1. Create `composables/useApi.ts`
2. Configure base URL
3. Use in components with `useFetch` or `$fetch`

## Next Steps

After confirming the application runs:

1. **Setup PWA** - Add offline capabilities
2. **Build Stock Module** - Inventory management
3. **Build POS Module** - Point of sale
4. **Add State Management** - Pinia stores
5. **Connect Backend** - API integration

## Support

For issues or questions:
1. Check this guide
2. Review `README.md`
3. Check Nuxt 4 documentation: https://nuxt.com
4. Check Tailwind CSS documentation: https://tailwindcss.com

## Important Notes

- **Don't commit `node_modules/`** - It's in `.gitignore`
- **Don't edit `.nuxt/`** - Auto-generated by Nuxt
- **Use TypeScript** - All new files should use `.ts` or `.vue` with `<script setup lang="ts">`
- **Follow conventions** - File names, component names, etc.
- **Test on mobile** - Use browser dev tools to test responsive design

## Quick Reference

```bash
# Start fresh
rm -rf node_modules .nuxt
npm install
npm run dev

# Check for errors
npm run build

# Update dependencies
npm update

# Add new package
npm install <package-name>
npm install -D <package-name>  # Dev dependency
```

---

**Ready to start?** Run `npm run dev` and open `http://localhost:3000` in your browser!

