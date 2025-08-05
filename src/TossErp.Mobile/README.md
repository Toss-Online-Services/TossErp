# TOSS ERP Mobile App

## Overview
Flutter mobile application for TOSS ERP III - Inventory Management System

## Features (MVP)
- Stock item management (CRUD operations)
- Real-time stock level tracking
- Offline-first architecture with sync capabilities
- Low-stock alerts and notifications
- Barcode scanning for quick operations
- Collaborative purchasing features

## Project Structure
```
lib/
├── main.dart                 # App entry point
├── app/
│   ├── app.dart             # App configuration
│   └── routes.dart          # Navigation routes
├── core/
│   ├── constants/           # App constants
│   ├── errors/             # Error handling
│   └── utils/              # Utility functions
├── data/
│   ├── models/             # Data models
│   ├── repositories/       # Data repositories
│   └── services/           # API services
├── presentation/
│   ├── screens/            # UI screens
│   ├── widgets/            # Reusable widgets
│   └── providers/          # State management
└── domain/
    ├── entities/           # Business entities
    ├── repositories/       # Repository interfaces
    └── usecases/          # Business logic
```

## Dependencies
- Flutter SDK
- sqflite: Local database
- provider: State management
- dio: HTTP client
- flutter_barcode_scanner: Barcode scanning
- shared_preferences: Local storage

## Getting Started
1. Install Flutter SDK
2. Run `flutter pub get`
3. Run `flutter run`

## Development Phases
1. **Phase 1:** Basic UI and local storage
2. **Phase 2:** Backend integration
3. **Phase 3:** Advanced features
4. **Phase 4:** Production deployment 