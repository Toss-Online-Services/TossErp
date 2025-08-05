# TOSS ERP III - Inventory Management System

## Overview
TOSS ERP III is a **third-generation ERP platform** designed for township and rural micro-enterprises in South Africa. This MVP focuses exclusively on the **Stock/Inventory Management module** with a **front-end first approach**.

## 🎯 Problem Statement
Informal micro-enterprises operate with thin margins and inefficient inventory processes due to lack of digital tools and collective bargaining power. TOSS provides an intuitive, mobile-first inventory management system that enables real-time stock tracking, automated reordering, and collaborative purchasing.

## 🏗️ Architecture

### Frontend (Priority 1)
- **Flutter Mobile App:** Offline-first architecture with local SQLite database
- **Nuxt.js Web Portal:** Dashboard and detailed reports (Phase 2)

### Backend (.NET Core)
- **Clean Architecture:** Domain, Application, Infrastructure, API layers
- **Microservices:** Following eShopOnContainers patterns
- **Database:** PostgreSQL with Entity Framework Core
- **Authentication:** JWT tokens
- **Event Bus:** RabbitMQ for integration events

## 📁 Project Structure
```
TossErp/
├── .taskmaster/           # Task management
├── src/
│   ├── TossErp.Mobile/   # Flutter mobile app
│   └── TossErp.Backend/  # .NET Core backend
│       ├── TossErp.Api/           # Web API
│       ├── TossErp.Application/   # Application layer
│       ├── TossErp.Domain/        # Domain layer
│       └── TossErp.Infrastructure/ # Infrastructure layer
└── docs/                 # Documentation
```

## 🚀 Features (MVP)

### Stock Management
- ✅ Stock item CRUD operations
- ✅ Real-time stock level tracking
- ✅ Stock movement history
- ✅ Category management
- ✅ Barcode scanning support

### Smart Alerts
- ✅ Low-stock notifications
- ✅ Reorder point alerts
- ✅ Margin tracking
- ✅ Stock level indicators

### Offline Capabilities
- ✅ Local SQLite database
- ✅ Offline-first design
- ✅ Sync when online
- ✅ Conflict resolution

### Collaborative Features
- ✅ Group purchasing
- ✅ Supplier matching
- ✅ Cost sharing
- ✅ Progress tracking

## 🛠️ Technology Stack

### Frontend
- **Flutter:** Mobile app framework
- **Dart:** Programming language
- **Provider:** State management
- **sqflite:** Local database
- **Dio:** HTTP client

### Backend
- **.NET 8:** Framework
- **Entity Framework Core:** ORM
- **PostgreSQL:** Database
- **JWT:** Authentication
- **Swagger:** API documentation

### Infrastructure
- **Docker:** Containerization
- **Kubernetes:** Orchestration
- **RabbitMQ:** Message broker
- **Redis:** Caching

## 📋 Development Roadmap

### Phase 1: Frontend MVP (Weeks 1-4)
- [ ] Flutter project setup
- [ ] Basic UI framework
- [ ] Stock item management
- [ ] Local database integration

### Phase 2: Backend Integration (Weeks 5-8)
- [ ] .NET Core API
- [ ] Database integration
- [ ] Authentication
- [ ] Real-time sync

### Phase 3: Advanced Features (Weeks 9-12)
- [ ] Barcode scanning
- [ ] Collaborative purchasing
- [ ] Reports and analytics
- [ ] Pilot testing

### Phase 4: Production Ready (Weeks 13-16)
- [ ] Performance optimization
- [ ] Security hardening
- [ ] Documentation
- [ ] Production deployment

## 🎯 Target Users
- **Sindi, Spaza Owner:** Quick stock updates and group buys
- **Thabo, Township Tailor:** Material tracking and forecasting
- **Mary, Home Baker:** Ingredient management and bulk purchasing

## 📊 Success Metrics
- User engagement (daily active users)
- Inventory accuracy (reduction in stockouts)
- Cost savings (average reduction in inventory costs)
- User satisfaction (app ratings and feedback)

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Flutter SDK (for mobile development)
- PostgreSQL
- Docker (optional)

### Backend Setup
```bash
cd src/TossErp.Backend
dotnet restore
dotnet build
dotnet run --project src/TossErp.Api
```

### Mobile App Setup
```bash
cd src/TossErp.Mobile
flutter pub get
flutter run
```

## 📚 Documentation
- [PRD](./.taskmaster/docs/prd.text) - Product Requirements Document
- [API Documentation](./docs/api.md) - API endpoints and usage
- [Architecture Guide](./docs/architecture.md) - System architecture details

## 🤝 Contributing
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## 📄 License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments
- Inspired by [ERPNext](https://frappe.io/erpnext) modules
- Architecture patterns from [eShopOnContainers](https://github.com/dotnet/eShop)
- Best practices from [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) 