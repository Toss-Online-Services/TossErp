using FluentValidation.AspNetCore;
using MediatR;
using TossErp.Procurement.API.Services;
using TossErp.Procurement.Application.Common.Interfaces;
using TossErp.Procurement.Domain.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(typeof(TossErp.Procurement.Application.Commands.CreatePurchaseOrder.CreatePurchaseOrderCommand).Assembly);

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Register application services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDomainEventService, DomainEventService>();
builder.Services.AddScoped<INotificationService, MockNotificationService>();

// Register repositories (mock implementations for MVP)
builder.Services.AddScoped<IPurchaseOrderRepository, MockPurchaseOrderRepository>();
builder.Services.AddScoped<ISupplierRepository, MockSupplierRepository>();

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
