# TOSS Web Application

TOSS ERP III Web Client - A comprehensive business management platform designed for South African SMMEs (Small, Medium & Micro Enterprises) with AI-powered insights and collaborative features.

## Features

### 🤖 AI Co-Pilot
- Intelligent business assistant with natural language interface
- Real-time insights and recommendations
- Context-aware responses based on your business data
- Voice and text interaction support

### 🏪 Core Business Modules
- **Accounts & Finance**: Financial management, invoicing, payments
- **CRM**: Customer relationship management and sales tracking  
- **Inventory**: Stock management with automatic reorder points
- **Sales & POS**: Point of sale system and sales analytics
- **Group Buying**: Collaborative purchasing for better prices
- **Asset Sharing**: Community tool and asset sharing network
- **Logistics**: Delivery management and supply chain coordination

### 🚀 Modern Technology Stack
- **Frontend**: Nuxt 3, Vue 3, TypeScript, Tailwind CSS
- **State Management**: Pinia stores
- **Authentication**: JWT-based with role-based permissions
- **API**: RESTful APIs with Nitro server
- **Database**: PostgreSQL (production), Redis (sessions)
- **Testing**: Vitest, Playwright E2E
- **Development**: Nx monorepo, Hot reload, Docker support

## Quick Start

### Prerequisites
- Node.js 18+ 
- npm or yarn
- Docker (optional)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd TossErp/toss-web
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start development server**
   ```bash
   npm run dev
   ```

4. **Open your browser**
   ```
   http://localhost:3000
   ```

### Demo Login
Use these credentials to explore the application:

| Role | Email | Password |
|------|--------|----------|
| Business Owner | owner@demo.toss.co.za | password123 |
| Manager | manager@demo.toss.co.za | password123 |
| Employee | employee@demo.toss.co.za | password123 |

## Development

### Project Structure
```
toss-web/
├── components/          # Reusable Vue components
│   ├── AICopilotChat.vue
│   ├── AppNavigation.vue
│   ├── ModuleCard.vue
│   └── NotificationContainer.vue
├── layouts/             # Application layouts
│   └── default.vue
├── middleware/          # Route middleware
│   └── auth.ts
├── pages/              # File-based routing pages
│   ├── dashboard/
│   ├── accounts/
│   ├── crm/
│   ├── inventory/
│   └── ...
├── server/             # Nitro server API routes
│   └── api/
│       └── auth/
├── stores/             # Pinia state management
│   ├── user.ts
│   ├── settings.ts
│   └── notifications.ts
├── tests/              # Test files
├── assets/             # Static assets
└── public/             # Public files
```

### Available Scripts

```bash
# Development
npm run dev              # Start development server
npm run build           # Build for production  
npm run preview         # Preview production build

# Testing
npm run test            # Run unit tests
npm run test:watch      # Run tests in watch mode
npm run test:e2e        # Run E2E tests

# Linting & Formatting
npm run lint            # Run ESLint
npm run lint:fix        # Fix ESLint errors
npm run typecheck       # TypeScript type checking

# Docker
docker-compose up       # Start with Docker
docker-compose up -d    # Start in background
```

### Environment Variables

Create a `.env` file in the project root:

```env
# Database
DATABASE_URL=postgresql://toss_user:toss_password@localhost:5432/toss_erp

# Redis
REDIS_URL=redis://localhost:6379

# Authentication
JWT_SECRET=your-super-secret-jwt-key
SESSION_SECRET=your-session-secret

# External APIs (optional)
OPENAI_API_KEY=your-openai-key
TWILIO_SID=your-twilio-sid
TWILIO_TOKEN=your-twilio-token
```

## Architecture

### Authentication & Authorization
- JWT-based authentication
- Role-based access control (Owner, Manager, Employee)
- Route protection with middleware
- Session management with Redis

### State Management  
- **Pinia stores** for centralized state
- **Reactive data** with Vue 3 Composition API
- **Persistent storage** for user preferences
- **Real-time updates** via WebSocket connections

### API Design
- **RESTful endpoints** following OpenAPI standards
- **Nitro server** for serverless-ready deployment
- **Type-safe** requests with TypeScript
- **Error handling** with standardized responses

### Testing Strategy
- **Unit tests** with Vitest and Vue Test Utils
- **E2E tests** with Playwright
- **Component testing** for Vue components  
- **API testing** for server endpoints
- **Continuous Integration** with automated testing

## Deployment

### Docker Deployment

1. **Build the image**
   ```bash
   docker build -t toss-web .
   ```

2. **Run with Docker Compose**
   ```bash
   docker-compose up -d
   ```

### Production Build

1. **Build the application**
   ```bash
   npm run build
   ```

2. **Preview locally**
   ```bash
   npm run preview
   ```

### Environment Setup
- Configure environment variables
- Set up database connections
- Configure Redis for sessions
- Set up SSL certificates for HTTPS

## Features in Detail

### AI Co-Pilot
The AI assistant provides intelligent business insights:
- **Natural Language Queries**: Ask questions about your business
- **Contextual Responses**: Answers based on your actual data
- **Quick Actions**: Common tasks with single clicks
- **Business Intelligence**: Sales trends, inventory alerts, financial insights

### Group Buying Module
Collaborative purchasing platform:
- **Group Orders**: Join buying groups for bulk discounts
- **Vendor Management**: Negotiate better prices together
- **Split Shipments**: Share delivery costs
- **Savings Tracking**: Monitor collective savings

### Asset Sharing Network
Community resource sharing:
- **Tool Sharing**: Share equipment with other businesses
- **Booking System**: Reserve assets when needed
- **Cost Splitting**: Fair usage-based billing
- **Trust Network**: Rating and review system

## Contributing

1. **Fork the repository**
2. **Create feature branch** (`git checkout -b feature/amazing-feature`)
3. **Commit changes** (`git commit -m 'Add amazing feature'`)
4. **Push to branch** (`git push origin feature/amazing-feature`)
5. **Open Pull Request**

### Development Guidelines
- Follow TypeScript best practices
- Write tests for new features
- Use conventional commit messages
- Follow Vue 3 composition API patterns
- Maintain responsive design principles

## Support

### Documentation
- [API Documentation](./docs/api/)
- [Component Library](./docs/components/)
- [Deployment Guide](./docs/deployment/)
- [Troubleshooting](./docs/troubleshooting/)

### Community
- **Issues**: Report bugs and feature requests
- **Discussions**: Ask questions and share ideas
- **Wiki**: Comprehensive documentation and guides

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**TOSS ERP III** - Empowering South African SMMEs with collaborative business technology.
