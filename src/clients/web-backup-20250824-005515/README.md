# TOSS ERP - Stock Management Web Application

A modern, responsive web application built with Nuxt 3 for enterprise stock management and inventory control.

## 🚀 Features

### Core Functionality
- **Dashboard Overview**: Real-time stock statistics and insights
- **Stock Management**: Comprehensive inventory tracking and control
- **Item Management**: Add, edit, and manage inventory items
- **Warehouse Management**: Multi-warehouse support with location tracking
- **Stock Movements**: Track incoming and outgoing inventory
- **Reports & Analytics**: Generate detailed stock reports and analytics
- **User Management**: Role-based access control and user administration
- **Settings**: System configuration and preferences

### Technical Features
- **Modern UI/UX**: Built with Tailwind CSS and Heroicons
- **Dark Mode Support**: Automatic theme switching with system preference
- **Responsive Design**: Optimized for desktop, tablet, and mobile devices
- **Real-time Updates**: Live data synchronization
- **Offline Support**: Progressive Web App capabilities
- **Internationalization**: Multi-language support (English, Spanish)
- **Accessibility**: WCAG 2.1 compliant design
- **Performance**: Optimized loading and rendering

## 🛠️ Technology Stack

- **Framework**: Nuxt 3 (Vue 3)
- **Styling**: Tailwind CSS
- **Icons**: Heroicons
- **State Management**: Pinia
- **Charts**: Chart.js with Vue-Chartjs
- **Internationalization**: @nuxtjs/i18n
- **Color Mode**: @nuxtjs/color-mode
- **Fonts**: Google Fonts (Inter, Poppins)
- **TypeScript**: Full type safety
- **Build Tool**: Vite

## 📋 Prerequisites

- Node.js 18+ 
- npm or yarn package manager
- Modern web browser

## 🚀 Getting Started

### 1. Install Dependencies

```bash
npm install
```

### 2. Environment Setup

Create a `.env` file in the project root:

```env
# API Configuration
API_BASE_URL=http://localhost:5000/api

# App Configuration
APP_NAME="TOSS ERP Stock Management"
APP_VERSION=1.0.0
```

### 3. Development Server

```bash
npm run dev
```

The application will be available at `http://localhost:3000`

### 4. Build for Production

```bash
npm run build
```

### 5. Preview Production Build

```bash
npm run preview
```

## 📁 Project Structure

```
src/clients/web/
├── assets/                 # Static assets
│   └── css/
│       └── main.css       # Main stylesheet
├── components/            # Reusable Vue components
├── layouts/              # Layout components
│   └── default.vue       # Default layout with sidebar
├── pages/                # Application pages
│   ├── index.vue         # Dashboard
│   ├── stock.vue         # Stock management
│   ├── items.vue         # Item management
│   ├── warehouses.vue    # Warehouse management
│   ├── reports.vue       # Reports & analytics
│   ├── users.vue         # User management
│   └── settings.vue      # Settings
├── plugins/              # Nuxt plugins
├── stores/               # Pinia stores
├── types/                # TypeScript type definitions
├── utils/                # Utility functions
├── app.vue              # Main app component
├── nuxt.config.ts       # Nuxt configuration
├── tailwind.config.ts   # Tailwind CSS configuration
└── package.json         # Dependencies and scripts
```

## 🎨 Design System

### Color Palette
- **Primary**: Blue (#2563eb) - Main brand color
- **Secondary**: Green (#22c55e) - Success states
- **Accent**: Yellow (#f59e0b) - Warning states
- **Gray**: Neutral grays for text and backgrounds

### Typography
- **Primary Font**: Inter - Clean and modern
- **Display Font**: Poppins - For headings and emphasis

### Components
- **Buttons**: Primary, secondary, outline, and danger variants
- **Cards**: Consistent card layouts with headers, bodies, and footers
- **Forms**: Styled form inputs with validation states
- **Tables**: Responsive data tables with sorting and pagination
- **Badges**: Status indicators and labels
- **Modals**: Overlay dialogs for forms and confirmations

## 🔧 Configuration

### Nuxt Configuration
The application is configured in `nuxt.config.ts` with:
- Module registration
- App metadata
- CSS and styling setup
- Internationalization
- Google Fonts
- Build optimizations

### Tailwind Configuration
Custom Tailwind configuration in `tailwind.config.ts` includes:
- Custom color palette
- Typography settings
- Component utilities
- Animation keyframes
- Plugin integrations

## 📱 Responsive Design

The application is fully responsive with breakpoints:
- **Mobile**: < 768px
- **Tablet**: 768px - 1024px
- **Desktop**: > 1024px

## 🌙 Dark Mode

Automatic dark mode support with:
- System preference detection
- Manual toggle in header
- Persistent user preference
- Consistent theming across all components

## 🔐 Security Features

- **Input Validation**: Client-side and server-side validation
- **XSS Protection**: Built-in Vue.js security features
- **CSRF Protection**: Token-based protection
- **Content Security Policy**: Secure resource loading

## 📊 Performance Optimization

- **Code Splitting**: Automatic route-based code splitting
- **Lazy Loading**: Components and images loaded on demand
- **Caching**: Browser and service worker caching
- **Compression**: Gzip compression for assets
- **Image Optimization**: Automatic image optimization

## 🧪 Testing

### Unit Testing
```bash
npm run test
```

### E2E Testing
```bash
npm run test:e2e
```

### Coverage
```bash
npm run test:coverage
```

## 📈 Analytics

The application includes analytics tracking for:
- Page views and navigation
- User interactions
- Performance metrics
- Error tracking

## 🔄 Deployment

### Vercel (Recommended)
```bash
npm run build
vercel --prod
```

### Netlify
```bash
npm run build
netlify deploy --prod
```

### Docker
```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
RUN npm run build
EXPOSE 3000
CMD ["npm", "start"]
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

## 🔄 Version History

- **v1.0.0**: Initial release with core stock management features
- **v1.1.0**: Added reports and analytics
- **v1.2.0**: Enhanced UI/UX and performance improvements

## 🎯 Roadmap

- [ ] Real-time notifications
- [ ] Advanced reporting
- [ ] Mobile app integration
- [ ] AI-powered insights
- [ ] Multi-tenant support
- [ ] Advanced search and filtering
- [ ] Bulk operations
- [ ] API documentation
- [ ] Performance monitoring
- [ ] Advanced security features
