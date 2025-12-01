# TOSS ERP-III Frontend

Mobile-first, offline-first ERP-III frontend built with Nuxt 4, Vue 3, and TailwindCSS.

## Features

- **Offline-First PWA**: Works seamlessly offline with IndexedDB queuing
- **Mobile-First Design**: Optimized for township and rural SMMEs
- **Real-Time Sync**: Automatic sync when network is restored
- **Material Dashboard UI**: Modern, clean interface inspired by Material Design

## Tech Stack

- **Nuxt 4**: Vue.js framework with SSR/SSG support
- **Vue 3**: Composition API with TypeScript
- **TailwindCSS**: Utility-first CSS framework
- **PWA**: Service Worker with offline caching
- **IndexedDB**: Local storage for offline operations
- **Vitest**: Unit testing framework

## Development

### Prerequisites

- Node.js 20+
- pnpm 8+

### Setup

```bash
# Install dependencies
pnpm install

# Start development server
pnpm dev

# Type check
pnpm typecheck

# Lint
pnpm lint

# Test
pnpm test

# Build
pnpm build
```

## Environment Variables

Create a `.env` file in the project root:

```env
NUXT_PUBLIC_API_BASE=http://localhost:5000
```

## Deployment

### Using Deployment Scripts

**Linux/Mac:**
```bash
chmod +x scripts/deploy.sh
./scripts/deploy.sh
```

**Windows:**
```powershell
.\scripts\deploy.ps1
```

### Manual Deployment

1. Build the application:
   ```bash
   pnpm build
   ```

2. The build output will be in `dist/toss-web-app/`

3. Deploy to your hosting platform:
   - **Vercel**: Connect your repo, set environment variables
   - **Netlify**: Connect your repo, set environment variables
   - **Docker**: Use the provided Dockerfile (if available)

### Environment Variables for Production

Set these in your hosting platform:

- `NUXT_PUBLIC_API_BASE`: Backend API URL (e.g., `https://api.toss.africa`)

## Offline Sync

The application automatically queues operations when offline and syncs when online:

- **POS Sales**: Queued in IndexedDB, synced on reconnect
- **Stock Adjustments**: Queued for sync
- **Other Operations**: Generic offline store available

## Testing

Run tests with:
```bash
pnpm test
```

Tests are located in `src/**/__tests__/` directories.

## Project Structure

```
src/
├── components/       # Vue components
│   ├── ui/          # Reusable UI components
├── composables/     # Vue composables
│   ├── __tests__/   # Composable tests
├── layouts/         # Layout components
├── pages/           # File-based routing
└── assets/          # Static assets
```

## License

MIT
