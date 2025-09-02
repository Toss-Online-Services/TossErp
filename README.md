# TOSS ERP POS & Store Management Mobile Application

A comprehensive Flutter mobile application for Point of Sale (POS) and Store Management, integrated with the TOSS ERP system. This application provides offline-first functionality with real-time synchronization capabilities.

## Functional Specification & Platform Scope
- See docs/TOSS_Functional_Spec.md for a concise, source-backed overview of TOSS ERP III vision, MVP modules, AI Business Co-Pilots, and collaborative economy features, with acceptance criteria and security/compliance notes.

## Features

### 🛒 POS (Point of Sale)
- **Quick Sales Interface**: Fast and intuitive sales screen
- **Barcode/QR Code Scanning**: Scan products for quick lookup
- **Customer Management**: Track customer information and history
- **Payment Processing**: Multiple payment methods support
- **Receipt Printing**: Bluetooth thermal printer support
- **Offline Sales**: Complete functionality without internet connection

### 📦 Stock Management
- **Real-time Inventory**: Live stock level tracking
- **Stock Movements**: Track all inventory transactions
- **Low Stock Alerts**: Automatic notifications for reorder points
- **Stock Adjustments**: Manual stock corrections
- **Batch/Serial Tracking**: Support for batch and serial number tracking
- **Multi-warehouse**: Support for multiple warehouse locations

### 📊 Reports & Analytics
- **Sales Reports**: Daily, weekly, monthly sales analysis
- **Stock Reports**: Inventory valuation and movement reports
- **Customer Reports**: Customer purchase history and analytics
- **Performance Metrics**: Store and staff performance tracking
- **Export Capabilities**: PDF and Excel export functionality

### ⚙️ Settings & Configuration
- **User Management**: Multi-user support with role-based access
- **Store Configuration**: Store details and settings
- **Printer Setup**: Thermal printer configuration
- **Sync Settings**: Data synchronization preferences
- **Backup & Restore**: Data backup and restoration

## Architecture

### Offline-First Design
- **SQLite Database**: Local data storage for offline operation
- **Sync Queue**: Automatic synchronization when online
- **Conflict Resolution**: Smart conflict handling for data integrity
- **Background Sync**: Automatic background synchronization

### State Management
- **Riverpod**: Modern state management with dependency injection
- **Repository Pattern**: Clean separation of data access logic
- **Service Layer**: Business logic encapsulation

### Navigation
- **GoRouter**: Declarative routing with deep linking support
- **Bottom Navigation**: Intuitive tab-based navigation
- **Nested Routes**: Complex navigation patterns support

## Technology Stack

- **Framework**: Flutter 3.x
- **Language**: Dart
- **State Management**: Riverpod
- **Navigation**: GoRouter
- **Database**: SQLite (sqflite)
- **HTTP Client**: Dio + Retrofit
- **UI Components**: Material Design 3
- **Charts**: FL Chart
- **Barcode Scanning**: Mobile Scanner
- **Printing**: ESC/POS Utils
- **Authentication**: Local Auth + Secure Storage

## Getting Started

### Prerequisites

1. **Flutter SDK**: Install Flutter 3.x or higher
   ```bash
   # Check Flutter installation
   flutter doctor
   ```

2. **Android Studio / VS Code**: Install your preferred IDE
3. **Android SDK**: For Android development
4. **Xcode**: For iOS development (macOS only)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd pos_store
   ```

2. **Install dependencies**
   ```bash
   flutter pub get
   ```

3. **Generate code**
   ```bash
   flutter packages pub run build_runner build
   ```

4. **Run the application**
   ```bash
   flutter run
   ```

### Configuration

1. **API Configuration**
   - Update `lib/core/network/api_service.dart` with your API endpoints
   - Configure authentication tokens and headers

2. **Database Configuration**
   - The SQLite database is automatically created on first run
   - Database schema is defined in `lib/core/database/database_service.dart`

3. **Printer Configuration**
   - Configure thermal printer settings in the app
   - Test printer connectivity

## Project Structure

```
lib/
├── core/                    # Core functionality
│   ├── database/           # Database services
│   ├── network/            # API services
│   ├── providers/          # App-wide providers
│   └── router/             # Navigation configuration
├── features/               # Feature modules
│   ├── auth/              # Authentication
│   ├── pos/               # Point of Sale
│   ├── stock/             # Stock Management
│   ├── reports/           # Reports & Analytics
│   └── settings/          # Settings & Configuration
├── shared/                 # Shared components
│   ├── models/            # Data models
│   ├── services/          # Shared services
│   ├── widgets/           # Reusable widgets
│   ├── utils/             # Utility functions
│   └── theme/             # App theming
└── main.dart              # Application entry point
```

## Development Guidelines

### Code Style
- Follow Dart/Flutter best practices
- Use meaningful variable and function names
- Add comprehensive comments for complex logic
- Follow the existing code structure and patterns

### State Management
- Use Riverpod for all state management
- Keep providers focused and single-purpose
- Use `@riverpod` annotation for code generation
- Implement proper error handling in providers

### Database Operations
- Always use the DatabaseService for data access
- Implement proper error handling for database operations
- Use transactions for complex operations
- Add items to sync queue for offline operations

### UI/UX Guidelines
- Follow Material Design 3 principles
- Ensure responsive design for different screen sizes
- Implement proper loading and error states
- Use consistent theming throughout the app

## Testing

### Unit Tests
```bash
flutter test
```

### Widget Tests
```bash
flutter test test/widget_test.dart
```

### Integration Tests
```bash
flutter test integration_test/
```

## Building for Production

### Android
```bash
flutter build apk --release
```

### iOS
```bash
flutter build ios --release
```

## Deployment

### Android
1. Generate signed APK/AAB
2. Upload to Google Play Console
3. Configure app signing

### iOS
1. Archive the app in Xcode
2. Upload to App Store Connect
3. Configure app signing and provisioning

## Troubleshooting

### Common Issues

1. **Build Errors**
   - Run `flutter clean` and `flutter pub get`
   - Regenerate code with `flutter packages pub run build_runner build`

2. **Database Issues**
   - Check database initialization in `DatabaseService.initialize()`
   - Verify database schema matches the current version

3. **Sync Issues**
   - Check network connectivity
   - Verify API endpoints and authentication
   - Check sync queue for pending items

4. **Printer Issues**
   - Verify Bluetooth permissions
   - Check printer compatibility
   - Test printer connectivity

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

## Roadmap

### Version 1.1
- [ ] Advanced reporting features
- [ ] Multi-language support
- [ ] Enhanced offline capabilities
- [ ] Performance optimizations

### Version 1.2
- [ ] Advanced inventory features
- [ ] Customer loyalty program
- [ ] Advanced analytics
- [ ] API integrations

### Version 2.0
- [ ] Web dashboard
- [ ] Multi-store support
- [ ] Advanced security features
- [ ] Cloud backup and restore 

## OpenAPI Specification Export

The backend AI-related services expose Swagger/OpenAPI definitions. To export and commit the specs:

```powershell
dotnet tool install --global Swashbuckle.AspNetCore.Cli   # once
pwsh ./scripts/export-openapi.ps1 -Configuration Release
```

Outputs are written to `docs/openapi/agentmanager-v1.json` and `docs/openapi/orchestrator-v1.json` for client generation and documentation.