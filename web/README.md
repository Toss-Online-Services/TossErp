# TOSS ERP - Web Application

Township Operations Support System - A modern ERP platform built with Nuxt 4, Vue 3, and Material Dashboard design.

## Overview

This is the web frontend for TOSS ERP, built using:
- **Nuxt 4** - Full-stack Vue framework
- **Vue 3** - Progressive JavaScript framework
- **Tailwind CSS** - Utility-first CSS framework
- **Material Dashboard** - Design system and styling
- **shadcn-vue** - UI component library
- **Radix Vue** - Accessible component primitives
- **Lucide Vue Next** - Icon library

## Project Structure

```
web/
├── app/              # Nuxt 4 app directory
│   └── app.vue       # Root component
├── assets/           # Static assets
│   └── css/
│       └── main.css  # Main stylesheet with Material Dashboard theme
├── components/       # Vue components
│   ├── layout/       # Layout components (Navbar, Footer)
│   └── ui/           # shadcn-vue UI components
├── layouts/          # Nuxt layouts
│   └── default.vue   # Default layout with sidebar
├── lib/              # Utility functions
│   └── utils.ts      # Helper functions (cn, etc.)
├── pages/            # File-based routing
│   ├── index.vue     # Dashboard
│   ├── pos.vue       # Point of Sale
│   ├── sales.vue     # Sales module
│   ├── stock.vue     # Stock Management
│   ├── procurement.vue # Procurement & Group Buying
│   ├── accounting.vue # Accounting
│   ├── crm.vue       # Customer Relationship Management
│   ├── manufacturing.vue # Manufacturing
│   ├── projects.vue  # Projects & Jobs
│   ├── assets.vue     # Assets Management
│   ├── hr.vue        # HR & Payroll
│   ├── reports.vue   # Reports & Analytics
│   └── settings.vue  # Settings
├── public/           # Static files
├── nuxt.config.ts    # Nuxt configuration
├── tailwind.config.ts # Tailwind configuration
└── components.json   # shadcn-vue configuration
```

## Features

### ERP Modules (Based on TOSS Functional Specification)

1. **Dashboard** - Overview with key metrics and recent activity
2. **Point of Sale (POS)** - Quick sales and transactions
3. **Sales** - Quotes, Orders, and Invoices
4. **Stock Management** - Inventory tracking and management
5. **Procurement** - Purchase orders and group buying
6. **Accounting** - Money in and money out
7. **CRM** - Customers and credit management
8. **Manufacturing** - Production and recipes
9. **Projects** - Jobs and work tracking
10. **Assets** - Equipment and tools management
11. **HR & Payroll** - People and pay management
12. **Reports** - Analytics and performance metrics
13. **Settings** - System configuration

## Design System

The project uses Material Dashboard design system with:
- Material Design color palette
- Custom CSS variables for theming
- Dark mode support
- Responsive layout
- Custom scrollbar styles
- Grain texture overlay (optional)

### Color Scheme

- **Primary**: Black (hsl(0, 0%, 0%))
- **Background**: Light gray (hsl(0, 0%, 97%))
- **Card**: White (hsl(0, 0%, 100%))
- **Border**: Light blue-gray (hsl(214, 32%, 91%))

## Getting Started

### Prerequisites

- Node.js 18+ 
- pnpm (package manager)

### Installation

```bash
# Install dependencies
pnpm install

# Start development server
pnpm run dev

# Build for production
pnpm run build

# Preview production build
pnpm run preview
```

The development server will start at `http://localhost:3000`

## Development

### Adding New Components

Components are auto-imported in Nuxt. Place them in:
- `components/` for reusable components
- `components/ui/` for shadcn-vue UI components
- `components/layout/` for layout-specific components

### Adding New Pages

Create `.vue` files in the `pages/` directory. Nuxt will automatically create routes based on the file structure.

### Styling

- Use Tailwind CSS utility classes
- Custom styles go in `assets/css/main.css`
- CSS variables are defined in `:root` and `.dark` selectors
- Use the `cn()` utility function for conditional classes

### Icons

Icons are from `lucide-vue-next`. Import and use:

```vue
<script setup>
import { ShoppingCart } from 'lucide-vue-next'
</script>

<template>
  <ShoppingCart :size="20" />
</template>
```

## Configuration

### Nuxt Config

Edit `nuxt.config.ts` to configure:
- Modules
- Build options
- Runtime config
- Dev tools

### Tailwind Config

Edit `tailwind.config.ts` to customize:
- Theme colors
- Spacing
- Typography
- Plugins

### shadcn-vue Config

Edit `components.json` to configure:
- Component paths
- Style preferences
- Aliases

## Project Status

This is the initial setup of the TOSS ERP web application. All modules are currently placeholder pages that will be implemented according to the TOSS Functional Specification v1.0.

## Next Steps

1. Implement authentication and authorization
2. Set up API integration with backend
3. Implement core ERP modules
4. Add data visualization and charts
5. Implement offline support
6. Add multi-language support (i18n)
7. Set up testing framework

## License

Proprietary - TOSS ERP Project
