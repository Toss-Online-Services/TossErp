FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["HR.API.csproj", "./"]
RUN dotnet restore "HR.API.csproj"
COPY . .
RUN dotnet build "HR.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HR.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TossErp.HR.API.dll"]
