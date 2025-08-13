# Web Application

## Overview
Web-based interface for TossErp system, providing general business operations and management capabilities.

## Technology Stack
- **Framework**: Vue.js 3 with Nuxt.js 3
- **UI Framework**: Tailwind CSS or Vuetify
- **State Management**: Pinia
- **API Client**: Nuxt's built-in $fetch or Axios
- **Authentication**: Nuxt Auth module

## Features
- Stock management and inventory control
- Purchase order management
- Sales order processing
- Basic reporting and analytics
- User management
- Real-time notifications

## Development Setup
```bash
# Install dependencies
npm install
# or
yarn install

# Run development server
npm run dev
# or
yarn dev

# Build for production
npm run build
# or
yarn build
```

## Project Structure
```
web/
├── components/        # Reusable Vue components
├── pages/            # Page components (auto-routed)
├── layouts/          # Layout components
├── composables/      # Composable functions
├── stores/           # Pinia stores
├── server/           # Server-side API routes
├── public/           # Static assets
├── assets/           # Source assets (CSS, images)
├── utils/            # Utility functions
└── nuxt.config.ts    # Nuxt configuration
```

## API Integration
- Base URL: Configured via runtime config
- Authentication: JWT tokens with refresh
- Real-time updates: WebSocket or Server-Sent Events
- File uploads: Multipart form data

## Environment Variables
```env
NUXT_PUBLIC_API_BASE_URL=http://localhost:8080
NUXT_PUBLIC_WS_URL=ws://localhost:8080/ws
```
