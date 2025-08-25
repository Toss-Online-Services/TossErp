# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy Directory.Packages.props for central package management
COPY ["Directory.Packages.props", "./"]

# Copy csproj files and restore dependencies
COPY ["src/Services/crm/Crm.API/Crm.API.csproj", "Services/crm/Crm.API/"]
COPY ["src/Services/crm/Crm.Application/Crm.Application.csproj", "Services/crm/Crm.Application/"]
COPY ["src/Services/crm/Crm.Domain/Crm.Domain.csproj", "Services/crm/Crm.Domain/"]
COPY ["src/Services/crm/Crm.Infrastructure/Crm.Infrastructure.csproj", "Services/crm/Crm.Infrastructure/"]

RUN dotnet restore "Services/crm/Crm.API/Crm.API.csproj"

# Copy the rest of the source code
COPY src/ .

# Build the application
WORKDIR "/src/Services/crm/Crm.API"
RUN dotnet build "Crm.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Crm.API.csproj" -c Release -o /app/publish

# Use the runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=publish /app/publish .

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "Crm.API.dll"]
