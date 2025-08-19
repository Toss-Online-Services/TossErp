using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TossErp.Procurement.API.Services;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;
using TossErp.Procurement.Infrastructure.Persistence;
using TossErp.Procurement.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TossErp.Procurement.Application.Commands.CreatePurchaseOrder.CreatePurchaseOrderCommand).Assembly));

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Add Entity Framework
builder.Services.AddDbContext<ProcurementDbContext>(options =>
{
    // For development, use InMemory database
    // For production, use SQL Server
    if (builder.Environment.IsDevelopment())
    {
        options.UseInMemoryDatabase("ProcurementDb");
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    }
});

// Register application services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUnitOfWork, TossErp.Procurement.Infrastructure.Persistence.UnitOfWork>();
builder.Services.AddScoped<IDomainEventService, DomainEventService>();
builder.Services.AddScoped<INotificationService, MockNotificationService>();

// Register repositories (using real EF implementations)
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
