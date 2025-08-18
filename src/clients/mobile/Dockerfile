# TOSS ERP Mobile App (Flutter Web) Dockerfile
# Multi-stage build for optimized production image

# Build stage
FROM ghcr.io/cirruslabs/flutter:3.16.0 AS build

# Set working directory
WORKDIR /app

# Enable web support
RUN flutter config --enable-web

# Copy pubspec files for dependency installation (layer caching)
COPY pubspec.yaml pubspec.lock ./

# Get dependencies
RUN flutter pub get

# Copy source code
COPY . .

# Build for web
RUN flutter build web --release --web-renderer html

# Production stage (nginx for serving static files)
FROM nginx:1.25-alpine AS production

# Create non-root user for security
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -u 1001 -G appgroup

# Install curl for health checks
RUN apk add --no-cache curl

# Copy built web app from build stage
COPY --from=build /app/build/web /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Change ownership of web files
RUN chown -R appuser:appgroup /usr/share/nginx/html
RUN chown -R appuser:appgroup /var/cache/nginx
RUN chown -R appuser:appgroup /var/log/nginx
RUN chown -R appuser:appgroup /etc/nginx/conf.d
RUN chown -R appuser:appgroup /var/run

# Create nginx.pid file with proper permissions
RUN touch /var/run/nginx.pid && chown appuser:appgroup /var/run/nginx.pid

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Start nginx
CMD ["nginx", "-g", "daemon off;"]