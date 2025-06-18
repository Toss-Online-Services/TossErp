using Microsoft.EntityFrameworkCore;
using POS.Infrastructure;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.AggregatesModel.OrderAggregate;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.AggregatesModel.PaymentAggregate;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.AggregatesModel.SyncAggregate;
using POS.Domain.AggregatesModel.InventoryAggregate;
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Infrastructure.Repositories;
using MediatR;
using System.Reflection;
using Microsoft.OpenApi.Models;
using POS.API.Apis;
using POS.API.Services;
using POS.API.Application.Queries;
using FluentValidation;
using FluentValidation.AspNetCore;
using POS.Domain.SeedWork;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "TossErp POS API", 
        Version = "v1",
        Description = "Point of Sale API for TossErp system"
    });
});

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Add DbContext
builder.Services.AddDbContext<POSContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<POS.Domain.SeedWork.IRepository<POS.Domain.AggregatesModel.ProductAggregate.Product>, ProductRepository>();
builder.Services.AddScoped<POS.Domain.SeedWork.IRepository<POS.Domain.AggregatesModel.OrderAggregate.Order>, OrderRepository>();
builder.Services.AddScoped<POS.Domain.SeedWork.IRepository<POS.Domain.AggregatesModel.SaleAggregate.Sale>, SaleRepository>();
builder.Services.AddScoped<POS.Domain.SeedWork.IRepository<POS.Domain.AggregatesModel.CustomerAggregate.Customer>, CustomerRepository>();
builder.Services.AddScoped<POS.Domain.SeedWork.IRepository<POS.Domain.AggregatesModel.StoreAggregate.Store>, StoreRepository>();
builder.Services.AddScoped<POS.Domain.SeedWork.IRepository<POS.Domain.AggregatesModel.StaffAggregate.Staff>, StaffRepository>();
builder.Services.AddScoped<POS.Domain.SeedWork.IRepository<POS.Domain.AggregatesModel.ProductAggregate.ProductCategory>, ProductCategoryRepository>();

// Add Queries
builder.Services.AddScoped<IPOSQueries, POSQueries>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register MediatR pipeline behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(POS.API.Application.Behaviors.LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(POS.API.Application.Behaviors.ValidatorBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(POS.API.Application.Behaviors.TransactionBehavior<,>));

// Register Integration Event Service
builder.Services.AddScoped<POS.API.Application.IntegrationEvents.IPOSIntegrationEventService, POS.API.Application.IntegrationEvents.POSIntegrationEventService>();

// Register EventBus and IntegrationEventLogService
builder.Services.AddSingleton<eShop.EventBus.Abstractions.IEventBus, eShop.EventBusRabbitMQ.EventBusRabbitMQ>();
builder.Services.AddScoped<Func<System.Data.Common.DbConnection, eShop.IntegrationEventLogEF.Services.IIntegrationEventLogService>>(sp =>
{
    return (dbConnection) => new eShop.IntegrationEventLogEF.Services.IntegrationEventLogService<POSContext>(dbConnection);
});

// Register Event Handlers
builder.Services.AddTransient<eShop.EventBus.Abstractions.IIntegrationEventHandler<POS.API.Application.IntegrationEvents.Events.ProductCreatedIntegrationEvent>, POS.API.EventHandlers.ProductCreatedIntegrationEventHandler>();
builder.Services.AddTransient<eShop.EventBus.Abstractions.IIntegrationEventHandler<POS.API.Application.IntegrationEvents.Events.OrderCreatedIntegrationEvent>, POS.API.EventHandlers.OrderCreatedIntegrationEventHandler>();
builder.Services.AddTransient<eShop.EventBus.Abstractions.IIntegrationEventHandler<POS.API.Application.IntegrationEvents.Events.SaleCompletedIntegrationEvent>, POS.API.EventHandlers.SaleCompletedIntegrationEventHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TossErp POS API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Map minimal API endpoints
var products = app.MapGroup("api/products");
products.MapProductsApiV1();

var orders = app.MapGroup("api/orders");
orders.MapOrdersApiV1();

var sales = app.MapGroup("api/sales");
sales.MapSalesApiV1();

var customers = app.MapGroup("api/customers");
customers.MapCustomersApiV1();

var stores = app.MapGroup("api/stores");
stores.MapStoresApiV1();

app.MapControllers();
app.Run();
