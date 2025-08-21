global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

global using MediatR;
global using AutoMapper;
global using FluentValidation;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.DependencyInjection;

global using TossErp.Accounts.Domain.Entities;
global using TossErp.Accounts.Domain.ValueObjects;
global using TossErp.Accounts.Domain.Enums;
global using TossErp.Accounts.Domain.Events;
global using TossErp.Accounts.Application.DTOs;
global using TossErp.Accounts.Application.Common.Interfaces;
