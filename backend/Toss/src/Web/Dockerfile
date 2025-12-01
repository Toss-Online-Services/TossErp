# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["backend/Toss/src/Web/Web.csproj", "backend/Toss/src/Web/"]
COPY ["backend/Toss/src/Application/Application.csproj", "backend/Toss/src/Application/"]
COPY ["backend/Toss/src/Domain/Domain.csproj", "backend/Toss/src/Domain/"]
COPY ["backend/Toss/src/Infrastructure/Infrastructure.csproj", "backend/Toss/src/Infrastructure/"]
COPY ["backend/Toss/src/ServiceDefaults/ServiceDefaults.csproj", "backend/Toss/src/ServiceDefaults/"]
COPY ["backend/Toss/Directory.Build.props", "backend/Toss/"]
COPY ["backend/Toss/Directory.Packages.props", "backend/Toss/"]

# Restore dependencies
RUN dotnet restore "backend/Toss/src/Web/Web.csproj"

# Copy all source files
COPY backend/Toss/src/ backend/Toss/src/

# Build the application
WORKDIR "/src/backend/Toss/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build --no-restore

# Publish stage
FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create a non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application
COPY --from=publish /app/publish .

# Set ownership
RUN chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check (using wget as curl may not be available)
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health/live || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Toss.Web.dll"]

