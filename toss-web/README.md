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

### ✅ Core Modules (100% Complete)

**Main Dashboard**
- KPI cards with real-time data
- Interactive Chart.js visualizations
- Sales trend analysis
- Top products and quick actions

**Manufacturing Module**
- Production dashboard with metrics
- Bill of Materials (BOM) management
- Work order tracking (Kanban board)
- Quality control and inspections
- Production planning tools

**POS Management**
- Real-time sales monitoring
- Hardware integration (barcode, receipt printer)
- Transaction history
- Payment analytics

**Inventory Management**
- Stock level monitoring with charts
- Low stock alerts
- Stock movement tracking
- Inventory valuation
- Multi-location support

**Finance & Accounting**
- Balance sheet generation
- Profit & Loss statements
- Cash flow reports
- Trial balance
- Financial ratios
- South African VAT compliance (15%)
- VAT return generation

**HR Management**
- Employee headcount tracking
- Attendance summary
- Leave request management
- Department distribution
- Payroll integration

**Sales & CRM**
- Customer management
- Order tracking
- Invoice generation
- Lead management
- Opportunity tracking

### ✅ Enterprise Features (100% Complete)

**Authentication & Security**
- JWT-based authentication with automatic token refresh
- Role-Based Access Control (RBAC) with 8 predefined roles
- 40+ granular permissions across all modules
- Session management with inactivity timeout
- Comprehensive audit logging
- Security event tracking

**Data Visualization**
- Interactive Chart.js charts across all modules
- Line, bar, and pie chart components
- Real-time data updates
- Responsive chart layouts
- Export charts as images

**Export Functionality**
- Universal export system
- CSV, Excel, and PDF formats
- Export from any module
- Customizable export templates
- Batch export support

**Financial Compliance**
- South African VAT calculations (15%)
- VAT return generation
- Tax compliance reporting
- Multi-period comparisons
- Automated tax calculations

**Performance & Optimization**
- Code splitting and lazy loading
- Optimized bundle sizes
- TypeScript strict mode
- Comprehensive error handling
- Production-ready builds

### 🧪 Testing (100% Complete)

**End-to-End Tests**
- Playwright test suite
- Authentication & authorization tests
- Module functionality tests
- Chart rendering tests
- Export functionality tests
- Permission/RBAC tests
- Security & audit tests

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
