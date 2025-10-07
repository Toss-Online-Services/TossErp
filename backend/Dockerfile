# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY TossErp.sln ./
COPY src/TossErp.API/TossErp.API.csproj src/TossErp.API/
COPY src/TossErp.Application/TossErp.Application.csproj src/TossErp.Application/
COPY src/TossErp.Domain/TossErp.Domain.csproj src/TossErp.Domain/
COPY src/TossErp.Infrastructure/TossErp.Infrastructure.csproj src/TossErp.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build the application
WORKDIR /src/src/TossErp.API
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
ENTRYPOINT ["dotnet", "TossErp.API.dll"]

