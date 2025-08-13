FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY services/accounting/Accounting.API.csproj services/accounting/
RUN dotnet restore services/accounting/Accounting.API.csproj
COPY . .
RUN dotnet publish services/accounting/Accounting.API.csproj -c Release -o /out

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Accounting.API.dll"]



