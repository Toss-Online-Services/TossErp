# ERP III Admin Panel

## Overview
Comprehensive administrative interface for TossErp system, providing advanced analytics, reporting, and system management capabilities.

## Technology Stack
- **Framework**: React 18 with TypeScript
- **UI Framework**: Material-UI (MUI) or Ant Design
- **State Management**: Redux Toolkit with RTK Query
- **Routing**: React Router v6
- **Charts**: Recharts or Chart.js
- **Forms**: React Hook Form with Yup validation

## Features
- Advanced analytics and reporting dashboards
- User and role management
- System configuration and settings
- Master data management
- Financial management and accounting
- Advanced inventory control
- Audit trails and compliance
- Workflow management
- Integration management

## Development Setup
```bash
# Install dependencies
npm install
# or
yarn install

# Run development server
npm start
# or
yarn start

# Build for production
npm run build
# or
yarn build

# Run tests
npm test
# or
yarn test
```

## Project Structure
```
admin/
├── src/
│   ├── components/     # Reusable UI components
│   ├── pages/          # Page components
│   ├── features/       # Feature-based modules
│   ├── hooks/          # Custom React hooks
│   ├── services/       # API services
│   ├── store/          # Redux store configuration
│   ├── utils/          # Utility functions
│   ├── types/          # TypeScript type definitions
│   ├── assets/         # Images, icons, etc.
│   └── styles/         # Global styles
├── public/             # Static assets
├── package.json        # Dependencies and scripts
└── tsconfig.json       # TypeScript configuration
```

## Feature Modules
- **Dashboard**: Analytics and KPIs
- **Users**: User and role management
- **Inventory**: Advanced stock management
- **Finance**: Accounting and financial reports
- **Sales**: Sales analytics and management
- **Purchasing**: Procurement management
- **Reports**: Custom report generation
- **Settings**: System configuration

## API Integration
- Base URL: Environment-based configuration
- Authentication: JWT with role-based access
- Real-time updates: WebSocket connections
- File handling: Upload/download capabilities
- Export functionality: PDF, Excel, CSV

## Environment Variables
```env
REACT_APP_API_BASE_URL=http://localhost:8080
REACT_APP_WS_URL=ws://localhost:8080/ws
REACT_APP_ENVIRONMENT=development
```
