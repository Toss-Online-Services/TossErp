#!/bin/bash

# Service creation script for TOSS ERP domain services
# This script creates the standard 4-layer architecture for each service

create_service() {
    local service_name=$1
    local service_title=$2
    local service_port=$3
    
    echo "Creating $service_title service..."
    
    # Create directory structure
    mkdir -p "src/Services/$service_name/$service_title.Domain"
    mkdir -p "src/Services/$service_name/$service_title.Application" 
    mkdir -p "src/Services/$service_name/$service_title.Infrastructure"
    mkdir -p "src/Services/$service_name/$service_title.API"
    
    # Domain project
    cat > "src/Services/$service_name/$service_title.Domain/$service_title.Domain.csproj" << EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <RootNamespace>TossErp.$service_title.Domain</RootNamespace>
    <AssemblyName>TossErp.$service_title.Domain</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MediatR" />
    <PackageReference Include="FluentValidation" />
    <PackageReference Include="System.ComponentModel.Annotations" />
  </ItemGroup>
</Project>
EOF

    # Application project  
    cat > "src/Services/$service_name/$service_title.Application/$service_title.Application.csproj" << EOF
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <Nullable>enable</Nullable>
    <RootNamespace>TossErp.$service_title.Application</RootNamespace>
    <AssemblyName>TossErp.$service_title.Application</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Ardalis.GuardClauses" />
    <PackageReference Include="AutoMapper" />
    <PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
    <PackageReference Include="FluentValidation.DependencyInjectionExtensions" />
    <PackageReference Include="MediatR" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" />
    <PackageReference Include="Microsoft.Extensions.Hosting" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" />
    <PackageReference Include="System.IdentityModel.Tokens.Jwt" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="../$service_title.Domain/$service_title.Domain.csproj" />
    <ProjectReference Include="../../../EventBus/EventBus.csproj" />
  </ItemGroup>
</Project>
EOF

    echo "Service $service_title created successfully!"
}

# Create all services
create_service "logistics" "Logistics" "5004"
create_service "pooledcredit" "PooledCredit" "5005" 
create_service "creditengine" "CreditEngine" "5006"
create_service "assetsharing" "AssetSharing" "5007"
