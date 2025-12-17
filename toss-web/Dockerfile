# Multi-stage build for Nuxt 4 application
FROM node:20-alpine AS builder

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy application code
COPY . .

# Build the application
RUN npm run build

# Production stage
FROM node:20-alpine

WORKDIR /app

# Copy built application from builder
COPY --from=builder /app/.output /app/.output
COPY --from=builder /app/package*.json ./

# Set environment to production
ENV NODE_ENV=production

# Expose port
EXPOSE 3000

# Start the application
CMD ["node", ".output/server/index.mjs"]
