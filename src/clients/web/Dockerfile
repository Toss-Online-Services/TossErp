# TOSS ERP Web Client (Nuxt 3) Dockerfile
# Multi-stage build for optimized production image

# Build stage
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy package files for dependency installation (layer caching)
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM nginx:alpine AS production

# Copy built static files to nginx
COPY --from=build /app/.output/public /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD wget --quiet --tries=1 --spider http://localhost:80/ || exit 1

# Start nginx
CMD ["nginx", "-g", "daemon off;"]