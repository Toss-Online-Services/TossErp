global using AutoMapper;
global using FluentValidation;
global using MediatR;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System.Diagnostics;

// Domain references
global using TossErp.Setup.Domain.Entities;
global using TossErp.Setup.Domain.Enums;
global using TossErp.Setup.Domain.ValueObjects;
global using TossErp.Setup.Domain.Events;

// Application references
global using TossErp.Setup.Application.Common.Interfaces;
global using TossErp.Setup.Application.DTOs;
global using TossErp.Setup.Application.Commands;
global using TossErp.Setup.Application.Queries;
