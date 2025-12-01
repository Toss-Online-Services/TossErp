# TOSS ERP-III Frontend (Nuxt 4)

Mobile-first, offline-first ERP frontend built with Nuxt 4, Vue 3, TailwindCSS, and shadcn-style components.

## Tech Stack

- **Framework**: Nuxt 4 (Vue 3, Composition API, TypeScript)
- **Styling**: TailwindCSS with Material Dashboard design tokens
- **Components**: shadcn-style components (Headless UI + Tailwind)
- **Icons**: lucide-vue-next
- **PWA**: @vite-pwa/nuxt for offline-first capabilities
- **State**: Pinia (when needed), composables for local state
- **Offline Storage**: IndexedDB via `idb` library

## Design System

### Color Palette

Based on Material Dashboard Pro:
- **Primary**: Rose/Pink (#e91e63) - Main brand color
- **Success**: Green (#4CAF50)
- **Info**: Blue (#1A73E8)
- **Warning**: Orange (#fb8c00)
- **Danger**: Red (#F44335)

### Typography

- **Font Family**: System UI stack (Roboto on Material, system fallbacks)
- **Scale**: xs (0.75rem) → 4xl (2.25rem)
- **Line Heights**: Tight (1.25), Normal (1.5), Relaxed (1.75)

### Spacing

Material Dashboard spacing scale:
- xs: 0.25rem (4px)
- sm: 0.5rem (8px)
- md: 1rem (16px)
- lg: 1.5rem (24px)
- xl: 2rem (32px)
- 2xl: 3rem (48px)

## Project Structure

```
toss-web-app/
├── src/
│   ├── assets/
│   │   └── css/
│   │       └── styles.css          # Global styles + Tailwind directives
│   ├── components/
│   │   ├── ui/                      # shadcn-style components
│   │   │   ├── Button.vue
│   │   │   ├── Card.vue
│   │   │   └── ...
│   │   ├── Navbar.vue
│   │   ├── Footer.vue
│   │   └── BottomNav.vue           # Mobile bottom navigation
│   ├── composables/
│   │   ├── useIndexedDB.ts         # IndexedDB wrapper
│   │   ├── useNetworkStatus.ts     # Network connectivity
│   │   └── usePosSync.ts           # POS offline sync
│   ├── layouts/
│   │   └── MainLayout.vue          # Main app shell
│   ├── pages/                      # File-based routing
│   │   ├── dashboard.vue
│   │   ├── sales/
│   │   │   └── pos.vue
│   │   └── ...
│   └── lib/
│       └── utils.ts                # cn() utility for class merging
├── nuxt.config.ts                  # Nuxt configuration
├── tailwind.config.js              # Tailwind + Material Dashboard tokens
└── postcss.config.js               # PostCSS config
```

## Development

### Prerequisites

- Node.js 18+
- pnpm (recommended) or npm

### Setup

```bash
# Install dependencies
pnpm install

# Start dev server
pnpm dev
# or
nx serve toss-web-app
```

The app will be available at `http://localhost:4200`

### Available Scripts

```bash
# Development
pnpm dev                    # Start dev server
nx serve toss-web-app       # Same as above

# Building
pnpm build                  # Build for production
nx build toss-web-app       # Same as above

# Type checking
pnpm typecheck              # Run TypeScript type check
nx typecheck toss-web-app   # Same as above

# Linting
pnpm lint                   # Run ESLint
nx lint toss-web-app        # Same as above

# Testing
pnpm test                   # Run Vitest tests
nx test toss-web-app        # Same as above
```

## Component Development

### Creating New Components

Components are auto-imported in Nuxt. Place them in `src/components/`:

```vue
<!-- src/components/ui/MyComponent.vue -->
<script setup lang="ts">
import { cn } from '@/lib/utils'

defineProps<{
  class?: string
}>()
</script>

<template>
  <div :class="cn('base-classes', $props.class)">
    <slot />
  </div>
</template>
```

### Using Components

No imports needed thanks to Nuxt auto-imports:

```vue
<template>
  <MyComponent class="custom-class">
    Content
  </MyComponent>
</template>
```

## Styling Guidelines

### Use Tailwind Utilities

Prefer Tailwind utility classes over custom CSS:

```vue
<div class="flex items-center gap-4 p-6 bg-card rounded-lg shadow-material-md">
```

### Use Design Tokens

Reference CSS variables for colors:

```vue
<div class="bg-primary text-primary-foreground">
```

### Material Dashboard Shadows

Use Material-specific shadows for cards:

```vue
<div class="shadow-material-sm">  <!-- Small elevation -->
<div class="shadow-material-md">  <!-- Medium elevation -->
<div class="shadow-material-lg">  <!-- Large elevation -->
```

## PWA Configuration

The app is configured as a PWA with:

- **Offline caching**: Shell assets cached for offline use
- **API caching**: NetworkFirst strategy for API calls (5min TTL)
- **Service Worker**: Auto-update on new deployments
- **Manifest**: Standalone app with shortcuts to POS and Stock

### Offline Sync

Use composables for offline-first features:

```typescript
import { usePosSync } from '@/composables/usePosSync'

const { queueSale, syncQueue } = usePosSync()

// Queue sale when offline
await queueSale(saleData)

// Sync when online
await syncQueue()
```

## Testing

### Unit Tests (Vitest)

Place tests next to components:

```typescript
// src/components/ui/Button.test.ts
import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import Button from './Button.vue'

describe('Button', () => {
  it('renders correctly', () => {
    const wrapper = mount(Button, {
      slots: { default: 'Click me' }
    })
    expect(wrapper.text()).toBe('Click me')
  })
})
```

### E2E Tests (Playwright)

E2E tests are in `toss-web-app-e2e/`:

```bash
nx e2e toss-web-app-e2e
```

## Deployment

### Static Export

```bash
nx build toss-web-app
# Output: dist/toss-web-app/
```

### Environment Variables

Create `.env` file:

```env
NUXT_PUBLIC_API_BASE_URL=https://api.tosserp.com
NUXT_PUBLIC_APP_NAME=TOSS ERP-III
```

Access in code:

```typescript
const apiUrl = useRuntimeConfig().public.apiBaseUrl
```

## Resources

- [Nuxt 4 Docs](https://nuxt.com/docs)
- [Vue 3 Docs](https://vuejs.org/)
- [TailwindCSS Docs](https://tailwindcss.com/docs)
- [Material Dashboard Pro](https://demos.creative-tim.com/material-dashboard-pro/pages/dashboards/analytics.html) (Design reference)
- [shadcn/ui](https://ui.shadcn.com/) (Component patterns)

## Contributing

1. Follow the existing component patterns
2. Use TypeScript for all new code
3. Write tests for new components/composables
4. Ensure mobile-first responsive design
5. Test offline functionality
6. Follow the Material Dashboard design language

