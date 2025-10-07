# TOSS ERP Web Admin

## Overview

Modern web-based admin dashboard for TOSS ERP III built with Nuxt 4, Vue 3, and Tailwind CSS.

## Tech Stack

- **Nuxt 4** - Full-stack Vue framework
- **Vue 3** - Progressive JavaScript framework
- **TypeScript** - Type-safe development
- **Tailwind CSS** - Utility-first CSS
- **Pinia** - State management
- **VueUse** - Composition utilities

## Features

### Implemented Dashboards

✅ **Main Dashboard**
- KPI cards (Revenue, Orders, Customers, Low Stock)
- Sales trend charts
- Top products table
- Quick actions

✅ **POS Management Dashboard**
- Real-time sales monitoring
- Transaction history
- Cashier performance
- Payment analytics

✅ **Inventory Dashboard**
- Stock level monitoring
- Low stock alerts
- Stock movement tracking
- Inventory valuation

✅ **Finance Dashboard**
- Balance sheet overview
- P&L summary
- Accounts receivable/payable aging
- Tax liability tracking

✅ **HR Dashboard**
- Employee headcount
- Attendance summary
- Leave requests
- Department distribution

## Getting Started

### Prerequisites

- Node.js 18+ or Bun
- TOSS ERP Backend API running

### Installation

```bash
# Install dependencies
npm install
# or
bun install
```

### Environment Configuration

Create a `.env` file:

```env
API_BASE_URL=http://localhost:5000
```

### Development

```bash
# Start development server
npm run dev
# or
bun dev
```

Visit `http://localhost:3000`

### Build for Production

```bash
# Build the application
npm run build

# Preview production build
npm run preview
```

## Project Structure

```
├── assets/           # Styles and static assets
├── components/       # Reusable Vue components
├── composables/      # Composition API composables
│   ├── useApi.ts    # API request helper
│   ├── useAuth.ts   # Authentication logic
│   └── useDashboard.ts # Dashboard data
├── layouts/          # Layout components
│   └── dashboard.vue # Main dashboard layout
├── middleware/       # Route middleware
│   └── auth.ts      # Authentication guard
├── pages/            # File-based routing
│   ├── dashboard/   # Main dashboard
│   ├── sales/       # Sales & POS pages
│   ├── inventory/   # Inventory pages
│   ├── finance/     # Finance pages
│   ├── hr/          # HR pages
│   └── ...
├── stores/           # Pinia stores
└── types/            # TypeScript type definitions
```

## Key Features

### Authentication
- JWT-based authentication
- Role-based access control
- Persistent login (localStorage)
- Automatic token refresh (planned)

### Dashboard Features
- Real-time metrics
- Interactive charts (planned: Chart.js integration)
- Responsive design
- Quick actions
- Multi-module navigation

### API Integration
- Centralized API composable
- Type-safe requests
- Error handling
- Loading states
- Request caching (planned)

## Module Dashboards

### Sales & POS
- `/sales/pos/dashboard` - POS monitoring
- `/sales/analytics` - Sales analytics

### Inventory
- `/inventory/dashboard` - Stock overview
- `/inventory` - Stock management
- `/inventory/items` - Product catalog

### Finance
- `/finance/dashboard` - Financial overview
- `/accounts` - Chart of accounts
- `/accounts/journal` - Journal entries

### HR
- `/hr/dashboard` - HR overview
- `/hr/employees` - Employee management
- `/hr/leave` - Leave management
- `/hr/payroll` - Payroll processing

### Procurement
- `/purchasing/suppliers` - Supplier management
- `/purchasing/orders` - Purchase orders

## Planned Features

### Extended Modules
- [ ] Manufacturing dashboard
- [ ] Supply chain tracking
- [ ] Project management
- [ ] Warehouse management
- [ ] Marketing automation
- [ ] E-commerce integration

### Collaboration Features (ERP III)
- [ ] Group buying network
- [ ] Shared logistics
- [ ] Asset sharing
- [ ] Pooled credit
- [ ] Community forum

### AI Features
- [ ] Natural language queries
- [ ] Predictive analytics
- [ ] Smart recommendations
- [ ] Automated insights

### Advanced Features
- [ ] Real-time notifications
- [ ] Offline mode with sync
- [ ] Advanced reporting
- [ ] Custom dashboards
- [ ] Mobile responsive optimization
- [ ] Multi-language support
- [ ] Dark mode

## Development Guidelines

### Component Creation
- Use `<script setup lang="ts">` syntax
- Follow Vue 3 Composition API patterns
- Use Tailwind for styling
- Implement proper TypeScript types

### API Integration
- Use `useApi()` composable for all API calls
- Handle loading and error states
- Implement proper error messages
- Use TypeScript interfaces for responses

### Authentication
- Use `useAuth()` composable
- Protect routes with `auth` middleware
- Check roles/permissions where needed

### State Management
- Use composables for local state
- Use Pinia stores for global state
- Minimize store usage when possible

## Testing

```bash
# Run unit tests
npm run test

# Run e2e tests
npm run test:e2e
```

## Deployment

### Docker

```bash
# Build image
docker build -t tosserp-web:latest .

# Run container
docker run -p 3000:3000 -e API_BASE_URL=https://api.tosserp.com tosserp-web:latest
```

### Static Hosting

```bash
# Generate static site
npm run generate

# Deploy dist/ to your hosting provider
```

## Contributing

Follow the existing code patterns and architecture. Ensure all new features include:
- TypeScript types
- Error handling
- Loading states
- Responsive design
- Accessibility considerations

## License

Proprietary - TOSS ERP System
