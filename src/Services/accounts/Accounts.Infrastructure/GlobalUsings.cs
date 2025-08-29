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

// Application Layer
global using TossErp.Accounts.Application.Common.Interfaces;

// Infrastructure Layer - only existing namespaces
global using TossErp.Accounts.Infrastructure.Data;

// Shared Kernel and SeedWork - prefer shared over domain versions
global using TossErp.Shared.SeedWork;
global using TossErp.SharedKernel.Interfaces;
