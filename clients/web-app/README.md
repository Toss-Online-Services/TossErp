# TossErp Web Application

## Overview
The TossErp Web Application is a modern Vue.js/Nuxt.js frontend client that provides a comprehensive interface for managing enterprise resource planning operations.

## Architecture
- **Vue.js 3**: Progressive JavaScript framework with Composition API
- **Nuxt.js 3**: Full-stack framework for Vue.js applications
- **TypeScript**: Type-safe development
- **Tailwind CSS**: Utility-first CSS framework
- **Pinia**: State management for Vue.js
- **Vue Router**: Client-side routing

## Features
- **Stock Management**: Complete inventory management interface
- **AI Assistant**: LangChain-powered natural language interface
- **Real-time Updates**: WebSocket integration for live data
- **Responsive Design**: Mobile-first responsive layout
- **Dark Mode**: Theme switching capability
- **Internationalization**: Multi-language support
- **Progressive Web App**: PWA capabilities

## Structure
```
web-app/
├── components/           # Reusable Vue components
│   ├── common/          # Shared components
│   ├── stock/           # Stock-specific components
│   └── ui/              # UI components
├── pages/               # Page components (Nuxt.js routing)
│   ├── stock/           # Stock management pages
│   └── ai-assistant/    # AI assistant pages
├── composables/         # Vue 3 composables
├── stores/              # Pinia state stores
├── utils/               # Utility functions
├── assets/              # Static assets
├── public/              # Public static files
├── nuxt.config.ts       # Nuxt.js configuration
├── package.json         # Dependencies
└── README.md           # This file
```

## Development
```bash
# Install dependencies
npm install

# Run development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run tests
npm run test
```

## Environment Configuration
```env
# API Configuration
NUXT_PUBLIC_API_BASE_URL=http://localhost:8080
NUXT_PUBLIC_WS_URL=ws://localhost:8080/ws

# Feature Flags
NUXT_PUBLIC_ENABLE_AI_ASSISTANT=true
NUXT_PUBLIC_ENABLE_REAL_TIME=true

# Analytics
NUXT_PUBLIC_ANALYTICS_ID=your-analytics-id
```

## API Integration
The web app communicates with microservices through the API Gateway:
- **Stock Service**: `/api/stock/*` - Inventory management
- **User Service**: `/api/users/*` - User management
- **AI Service**: `/api/ai/*` - AI-powered features

## State Management
- **Pinia Stores**: Centralized state management
- **Stock Store**: Inventory state and operations
- **User Store**: Authentication and user data
- **UI Store**: Application UI state

## Styling
- **Tailwind CSS**: Utility-first styling
- **Custom Components**: Reusable UI components
- **Dark Mode**: Theme switching
- **Responsive**: Mobile-first design

## Testing
- **Vitest**: Unit testing framework
- **Vue Test Utils**: Component testing utilities
- **Playwright**: End-to-end testing
- **Coverage**: Test coverage reporting

## Deployment
- **Static Generation**: Pre-rendered pages for SEO
- **CDN Ready**: Optimized for content delivery networks
- **Docker Support**: Containerized deployment
- **CI/CD**: Automated deployment pipeline

## Dependencies
- Vue.js 3.x
- Nuxt.js 3.x
- TypeScript
- Tailwind CSS
- Pinia
- Axios
- Vue Router
- Vite
