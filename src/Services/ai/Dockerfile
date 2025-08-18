# TOSS ERP AI Service Dockerfile
# Multi-stage build for optimized production image

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copy project files and restore dependencies (layer caching optimization)
COPY ["src/Services/ai/Ai.API.csproj", "ai/"]
COPY ["src/ServiceDefaults/eShop.ServiceDefaults.csproj", "ServiceDefaults/"]
COPY ["src/Services/_shared/Configuration/Configuration.csproj", "Services/_shared/Configuration/"]
COPY ["src/EventBus/EventBus.csproj", "EventBus/"]
COPY ["Directory.Build.props", "Directory.Build.targets", "Directory.Packages.props", "./"]

RUN dotnet restore "ai/Ai.API.csproj" --runtime alpine-x64

# Copy source code and build
COPY src/Services/ai/ ai/
COPY src/ServiceDefaults/ ServiceDefaults/
COPY src/Services/_shared/Configuration/ Services/_shared/Configuration/
COPY src/EventBus/ EventBus/

WORKDIR /src/ai
RUN dotnet build "Ai.API.csproj" -c Release --runtime alpine-x64 --self-contained false --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish "Ai.API.csproj" -c Release -o /app/publish \
    --runtime alpine-x64 \
    --self-contained false \
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
ENTRYPOINT ["dotnet", "Ai.API.dll"]