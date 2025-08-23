FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Create a non-root user
RUN adduser --disabled-password --gecos '' --no-create-home --uid 10001 appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for dependencies
COPY ["src/Services/hr/HR.API/HR.API.csproj", "src/Services/hr/HR.API/"]
COPY ["src/Services/hr/HR.Application/HR.Application.csproj", "src/Services/hr/HR.Application/"]
COPY ["src/Services/hr/HR.Infrastructure/HR.Infrastructure.csproj", "src/Services/hr/HR.Infrastructure/"]
COPY ["src/Services/hr/HR.Domain/HR.Domain.csproj", "src/Services/hr/HR.Domain/"]
COPY ["src/ServiceDefaults/eShop.ServiceDefaults.csproj", "src/ServiceDefaults/"]
COPY ["src/EventBus/EventBus.csproj", "src/EventBus/"]

# Restore dependencies
RUN dotnet restore "src/Services/hr/HR.API/HR.API.csproj"

# Copy all source code
COPY . .

# Build and publish
WORKDIR "/src/src/Services/hr/HR.API"
RUN dotnet build "HR.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HR.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Switch to non-root user
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "TossErp.HR.API.dll"]
