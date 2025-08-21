global using MediatR;
global using FluentValidation;
global using AutoMapper;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.DependencyInjection;
global using System.Diagnostics;

// Domain
global using TossErp.Projects.Domain.Entities;
global using TossErp.Projects.Domain.ValueObjects;
global using TossErp.Projects.Domain.Enums;
global using TossErp.Projects.Domain.Events;
global using TossErp.Projects.Domain.Exceptions;

// Shared Domain
global using TossErp.Shared.Domain.Common;
global using TossErp.Shared.Domain.Interfaces;
global using TossErp.Shared.Domain.ValueObjects;

// Application
global using TossErp.Projects.Application.DTOs;
global using TossErp.Projects.Application.Common.Interfaces;
global using TossErp.Projects.Application.Common;
