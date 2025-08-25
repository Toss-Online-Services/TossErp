# Use the official Node.js 18 Alpine image
FROM node:18-alpine AS base

# Install necessary packages
RUN apk update && apk add --no-cache libc6-compat

WORKDIR /app

# Copy package files
COPY package*.json ./
COPY nx.json ./

# Install dependencies
RUN npm ci --only=production && npm cache clean --force

# Copy source code
COPY . .

# Build stage
FROM base AS builder

# Install all dependencies (including dev)
RUN npm ci

# Build the application
RUN npm run build:toss-web

# Production stage
FROM node:18-alpine AS runner

WORKDIR /app

# Create non-root user
RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nuxtjs

# Copy built application
COPY --from=builder --chown=nuxtjs:nodejs /app/.output ./

# Set user
USER nuxtjs

# Expose port
EXPOSE 3000

# Set environment variables
ENV NODE_ENV=production
ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=3000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD node --version || exit 1

# Start the application
CMD ["node", "server/index.mjs"]
