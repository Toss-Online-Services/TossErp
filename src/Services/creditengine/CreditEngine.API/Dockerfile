FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CreditEngine.API.csproj", "./"]
RUN dotnet restore "CreditEngine.API.csproj"
COPY . .
RUN dotnet build "CreditEngine.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreditEngine.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TossErp.CreditEngine.API.dll"]
