FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["PooledCredit.API.csproj", "./"]
RUN dotnet restore "PooledCredit.API.csproj"
COPY . .
RUN dotnet build "PooledCredit.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PooledCredit.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TossErp.PooledCredit.API.dll"]
