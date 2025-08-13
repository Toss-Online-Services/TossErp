# Mobile Application

## Overview
Mobile application for TossErp system, designed for field operations, inventory management, and sales.

## Technology Stack
- **Framework**: React Native or Flutter (TBD)
- **State Management**: Redux Toolkit or Provider
- **Navigation**: React Navigation or Flutter Navigation
- **API Client**: Axios or Dio
- **Offline Storage**: AsyncStorage or SQLite

## Features
- Inventory scanning and management
- Sales order processing
- Real-time stock updates
- Offline capabilities
- Barcode/QR code scanning
- Location-based services
- Push notifications

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

# Run on device/simulator
npm run android
npm run ios
```

## Project Structure
```
mobile/
├── src/
│   ├── components/     # Reusable UI components
│   ├── screens/        # Screen components
│   ├── navigation/     # Navigation configuration
│   ├── services/       # API services
│   ├── store/          # State management
│   ├── utils/          # Utility functions
│   └── assets/         # Images, fonts, etc.
├── android/            # Android-specific files
├── ios/               # iOS-specific files
└── package.json       # Dependencies and scripts
```

## API Integration
- Base URL: Configured via environment variables
- Authentication: JWT tokens
- Real-time updates: WebSocket connections
- Offline sync: Background sync when online
