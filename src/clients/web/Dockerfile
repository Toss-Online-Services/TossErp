# TOSS ERP Web Client (Nuxt 3) Dockerfile
# Multi-stage build for optimized production image

# Build stage
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Install pnpm for better performance
RUN npm install -g pnpm

# Copy package files for dependency installation (layer caching)
COPY package*.json pnpm-lock.yaml* ./

# Install dependencies
RUN pnpm install --frozen-lockfile

# Copy source code
COPY . .

# Build the application
RUN pnpm run build

# Production stage
FROM node:20-alpine AS production

# Create non-root user for security
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -u 1001 -G appgroup

# Install pnpm
RUN npm install -g pnpm

# Install curl for health checks
RUN apk add --no-cache curl

WORKDIR /app

# Copy built application from build stage
COPY --from=build /app/.output ./
COPY --from=build /app/package.json ./

# Change ownership to non-root user
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:3000/health || exit 1

# Expose port
EXPOSE 3000

# Set environment variables
ENV NODE_ENV=production
ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=3000

# Entry point
CMD ["node", "server/index.mjs"]