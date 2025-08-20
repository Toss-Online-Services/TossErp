# TOSS ERP - Enterprise Resource Planning System

A comprehensive, modern enterprise resource planning system built with .NET 9.0 microservices, Nuxt 4 web application, and Flutter mobile application. This system provides a complete solution for business management with a focus on stock management, CRM, collaboration, and financial operations.

## 🚀 Quick Start

### Prerequisites
- .NET 9.0 SDK
- Docker & Docker Compose
- Node.js 18+ (for frontend development)
- PowerShell (Windows) or Make (Unix/Linux/macOS)

### Development Environment Setup

**Windows (PowerShell):**
```powershell
# Build backend
.\scripts\build.ps1 -Restore -Test

# Start development environment
.\scripts\docker.ps1 dev

# Or start infrastructure only
.\scripts\docker.ps1 infra
```

**Unix/Linux/macOS (Make):**
```bash
# Build backend
make build

# Start development environment
make docker-dev

# Or start infrastructure only
make docker-infra
```

**Manual Setup:**
```bash
# 1. Build backend
dotnet restore TossErp.sln
dotnet build TossErp.sln

# 2. Start infrastructure
docker-compose -f docker/docker-compose.yml up postgres redis rabbitmq

# 3. Start development environment
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.dev.yml up

# 4. Frontend development
cd src/clients/web
npm install
npm run dev
```

## 🏗️ System Architecture

### Backend Services
- **Identity Service** (Port 5001): User management, authentication, authorization
- **Stock Service** (Port 5002): Inventory management, stock tracking
- **CRM Service** (Port 5003): Customer relationship management
- **Collaboration Service**: Group-buy and business collaboration
- **API Gateway** (Port 8080): Centralized API routing and aggregation

### Frontend Applications
- **Web Client** (Port 3000): Nuxt 4 web application with modern UI
- **Mobile Client**: Flutter mobile application for POS and field operations

### Infrastructure
- **PostgreSQL**: Primary database
- **Redis**: Caching and session management
- **RabbitMQ**: Message queuing and event bus
- **Nginx**: Reverse proxy and load balancing

## 📱 Features

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

## 🏗️ Architecture

### Monorepo Structure
This project follows a systematic monorepo structure with clear separation of concerns:

```
TossErp/
├── 📁 src/                        # Source code
│   ├── 📁 AppHost/                # Single Aspire AppHost project
│   ├── 📁 Services/               # Backend microservices
│   ├── 📁 Gateway/                # API Gateway
│   ├── 📁 Shared/                 # Shared libraries
│   └── 📁 Clients/                # Frontend applications
├── 📁 docker/                     # Docker orchestration
├── 📁 scripts/                    # Build and deployment scripts
├── 📁 tools/                      # Development tools
└── 📁 configs/                    # Configuration files
```

### Backend Architecture
- **Clean Architecture**: Domain, Application, Infrastructure layers
- **Microservices**: Independent services with well-defined boundaries
- **MediatR**: Command/Query separation and event handling
- **Entity Framework Core**: Data access with in-memory repositories for development

### Frontend Architecture
- **Nuxt 4**: Modern Vue.js framework with app router
- **Tailwind CSS**: Utility-first CSS framework
- **Composition API**: Modern Vue 3 patterns
- **Auto-imports**: Automatic component and composable imports

### Mobile Architecture
- **Flutter**: Cross-platform mobile development
- **Offline-First Design**: Local SQLite database with sync capabilities
- **Riverpod**: Modern state management
- **GoRouter**: Declarative routing

### Infrastructure
- **Docker Compose**: Local development environment
- **PostgreSQL**: Primary database
- **Redis**: Caching and session management
- **RabbitMQ**: Message queuing and event bus
- **Nginx**: Reverse proxy and load balancing

## 🛠️ Development Tools

### Scripts
- **`scripts/build.ps1`**: PowerShell build script for Windows
- **`scripts/docker.ps1`**: PowerShell Docker management for Windows
- **`Makefile`**: Cross-platform commands for Unix/Linux/macOS

### Docker Management
```bash
# Development environment
docker-compose -f docker/docker-compose.yml -f docker/docker-compose.dev.yml up

# Production environment
docker-compose -f docker/docker-compose.yml up

# Infrastructure only
docker-compose -f docker/docker-compose.yml up postgres redis rabbitmq
```

## 📚 Documentation

- **[README-STRUCTURE.md](README-STRUCTURE.md)**: Detailed monorepo structure documentation
- **[README-DOCKER-ASPIRE.md](README-DOCKER-ASPIRE.md)**: Docker and Aspire setup guide

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