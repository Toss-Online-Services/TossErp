# TOSS Web Application

A modern ERP-III platform for South African SMMEs built with Nuxt 4, Vue 3, and Tailwind CSS.

## Features

- **Landing Page**: Beautiful marketing page showcasing TOSS features
- **Authentication**: Login and registration pages
- **Admin Dashboard**: Comprehensive ERP dashboard with sidebar navigation
- **ERP Modules**:
  - Dashboard with key metrics and AI insights
  - Inventory management
  - Sales & POS (Point of Sale)
  - Purchasing & supplier management
  - Customer management
  - Supplier network
  - Financial tracking
  - Reports & analytics
  - Collaborative network features
  - Settings

## Tech Stack

- **Framework**: Nuxt 4
- **UI Library**: Vue 3 (Composition API)
- **Styling**: Tailwind CSS
- **Components**: shadcn-nuxt (Material Design inspired)
- **Icons**: Lucide Vue Next

## Getting Started

### Prerequisites

- Node.js 18+ 
- npm or pnpm

### Installation

1. Install dependencies:
```bash
npm install
# or
pnpm install
```

2. Prepare Nuxt (generates types and auto-imports):
```bash
npm run postinstall
# or
pnpm postinstall
```

3. Start development server:
```bash
npm run dev
# or
pnpm dev
```

4. Open [http://localhost:3000](http://localhost:3000) in your browser

## Project Structure

```
web-app/
├── app/
│   └── app.vue              # Root component
├── assets/
│   └── css/
│       └── main.css        # Global styles and Tailwind
├── components/
│   └── ui/                 # Reusable UI components
│       ├── Button.vue
│       ├── Card.vue
│       ├── Input.vue
│       └── ...
├── layouts/
│   └── admin.vue           # Admin dashboard layout
├── lib/
│   └── utils.ts            # Utility functions
├── pages/
│   ├── index.vue           # Landing page
│   ├── login.vue           # Login page
│   ├── register.vue         # Registration page
│   └── dashboard/          # ERP modules
│       ├── index.vue       # Dashboard home
│       ├── inventory.vue
│       ├── sales.vue
│       ├── purchasing.vue
│       ├── customers.vue
│       ├── suppliers.vue
│       ├── financials.vue
│       ├── reports.vue
│       ├── network.vue
│       └── settings.vue
├── nuxt.config.ts          # Nuxt configuration
├── tailwind.config.js      # Tailwind configuration
└── package.json
```

## Key Features

### Landing Page
- Hero section with value proposition
- Three pillars explanation (ERP-III, AI Copilot, Network)
- Business types showcase
- How it works section
- Call-to-action sections

### Authentication
- Clean login page
- Comprehensive registration with business type selection
- Form validation

### Admin Dashboard
- Responsive sidebar navigation
- Mobile-friendly with hamburger menu
- AI Copilot modal interface
- Real-time notifications area
- User profile section

### ERP Modules

#### Dashboard
- Key metrics cards (sales, stock, orders, cash)
- Sales overview chart placeholder
- AI insights panel
- Recent transactions list

#### Inventory
- Product listing with stock levels
- Visual stock indicators
- Search and filter functionality
- Add/edit/delete products

#### Sales & POS
- Point of sale interface
- Product search and cart
- Transaction history
- Sales statistics

#### Purchasing
- Purchase order management
- Group buying opportunities
- Supplier integration
- Order tracking

#### Customers
- Customer database
- Credit tracking
- Purchase history
- Contact management

#### Suppliers
- Supplier network
- Ratings and reviews
- Network partner status
- Quick ordering

#### Financials
- Revenue and expense tracking
- Profit/loss calculations
- Cash flow monitoring
- Transaction history

#### Reports
- Various report types
- Quick statistics
- Export capabilities

#### Network
- Collaborative network features
- Group buying opportunities
- Nearby business connections
- Shared logistics

#### Settings
- Business information
- Account settings
- Notification preferences
- Network settings

## Development

### Adding New Pages

1. Create a new `.vue` file in `pages/` directory
2. Use `definePageMeta({ layout: 'admin' })` for admin pages
3. Use auto-imported components and composables

### Adding UI Components

1. Create component in `components/ui/`
2. Use Tailwind classes for styling
3. Follow shadcn design patterns

### Styling

- Use Tailwind utility classes
- Follow the design system in `assets/css/main.css`
- Use CSS variables for theming

## Building for Production

```bash
npm run build
# or
pnpm build
```

The output will be in `.output` directory.

## Notes

- TypeScript errors for `definePageMeta` and `useHead` are expected - these are auto-imported by Nuxt
- Run `npm run postinstall` after installing dependencies to generate types
- The project uses Nuxt 4's file-based routing
- All components are auto-imported (no need for manual imports)

## License

Proprietary - TOSS Platform
