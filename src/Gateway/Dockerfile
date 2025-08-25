# TOSS ERP API Gateway Dockerfile
# Multi-stage build for optimized production image

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy project files and restore dependencies (layer caching optimization)
COPY ["src/Gateway/Gateway.csproj", "Gateway/"]
COPY ["src/Directory.Build.props", "src/Directory.Build.targets", "Directory.Packages.props", "./"]

RUN dotnet restore "Gateway/Gateway.csproj"

# Copy source code and build
COPY src/Gateway/ Gateway/

WORKDIR /src/Gateway
RUN dotnet build "Gateway.csproj" -c Release --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish "Gateway.csproj" -c Release -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final

# Create non-root user for security
RUN addgroup -g 1001 -S appgroup && \
    adduser -S appuser -u 1001 -G appgroup

# Install curl for health checks
RUN apk add --no-cache curl

WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Change ownership to non-root user
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "Gateway.dll"]