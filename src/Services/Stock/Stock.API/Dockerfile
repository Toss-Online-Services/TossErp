FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/Directory.Packages.props src/
COPY src/Directory.Build.props src/
COPY src/Directory.Build.targets src/
COPY src/global.json src/
COPY src/nuget.config src/
COPY src/Services/Stock/Stock.API/Stock.API.csproj src/Services/Stock/Stock.API/
COPY src/Services/Stock/Stock.Application/Stock.Application.csproj src/Services/Stock/Stock.Application/
COPY src/Services/Stock/Stock.Infrastructure/Stock.Infrastructure.csproj src/Services/Stock/Stock.Infrastructure/
COPY src/Services/Stock/Stock.Domain/Stock.Domain.csproj src/Services/Stock/Stock.Domain/
COPY src/ServiceDefaults/eShop.ServiceDefaults.csproj src/ServiceDefaults/
COPY src/EventBus/EventBus.csproj src/EventBus/
RUN dotnet restore src/Services/Stock/Stock.API/Stock.API.csproj
COPY . .
RUN dotnet publish src/Services/Stock/Stock.API/Stock.API.csproj -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "TossErp.Stock.API.dll"]



