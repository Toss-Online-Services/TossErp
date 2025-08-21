// Global using statements for Accounts Infrastructure
global using System;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Threading;
global using System.Threading.Tasks;
global using System.Text.Json;

// Microsoft Extensions
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Options;
global using Microsoft.Extensions.Diagnostics.HealthChecks;

// Entity Framework Core 9
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Storage;

// Azure Storage
global using Azure.Storage.Blobs;
global using Azure.Storage.Blobs.Models;
global using Azure.Storage.Sas;

// MediatR
global using MediatR;

// Domain Layer
global using TossErp.Accounts.Domain.Entities;
global using TossErp.Accounts.Domain.Enums;
global using TossErp.Accounts.Domain.ValueObjects;
global using TossErp.Accounts.Domain.Interfaces;
global using TossErp.Accounts.Domain.Specifications;

// Application Layer
global using TossErp.Accounts.Application.Common.Interfaces;

// Infrastructure Layer
global using TossErp.Accounts.Infrastructure.Data;
global using TossErp.Accounts.Infrastructure.Repositories;
global using TossErp.Accounts.Infrastructure.Services;
global using TossErp.Accounts.Infrastructure.BackgroundServices;
global using TossErp.Accounts.Infrastructure.Migrations;

// Shared Kernel
global using TossErp.SharedKernel.Interfaces;
global using TossErp.SharedKernel.Common;
