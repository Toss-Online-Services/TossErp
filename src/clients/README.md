# Client Applications

This directory contains all front-end client applications for the TossErp system.

## Structure

```
clients/
├── mobile/          # Mobile application (React Native/Flutter)
├── web/             # Web application (Vue.js/Nuxt.js)
├── admin/           # ERP III Admin Panel (React/Angular)
└── README.md        # This file
```

## Applications

### Mobile App (`mobile/`)
- **Technology**: React Native or Flutter
- **Purpose**: Mobile interface for field operations, inventory management, and sales
- **Features**: 
  - Inventory scanning and management
  - Sales order processing
  - Real-time stock updates
  - Offline capabilities

### Web App (`web/`)
- **Technology**: Vue.js with Nuxt.js
- **Purpose**: Web-based interface for general business operations
- **Features**:
  - Stock management
  - Purchase orders
  - Sales management
  - Basic reporting

### Admin Panel (`admin/`)
- **Technology**: React or Angular
- **Purpose**: ERP III comprehensive administrative interface
- **Features**:
  - Advanced analytics and reporting
  - User and role management
  - System configuration
  - Master data management
  - Financial management
  - Advanced inventory control

## Development

Each client application should be developed independently with its own:
- Package management (package.json, requirements.txt, etc.)
- Build configuration
- Testing framework
- Documentation

## Integration

All clients integrate with the backend services through:
- REST APIs via the Gateway
- WebSocket connections for real-time updates
- Shared authentication and authorization
