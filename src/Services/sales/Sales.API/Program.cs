using TossErp.Sales.Application.Commands.CreateSale;
using TossErp.Sales.Application.Commands.CancelSale;
using TossErp.Sales.Application.Queries.GetDailySales;
using TossErp.Sales.Application.Common.Interfaces;
using TossErp.Sales.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TOSS ERP Sales API",
        Description = "API for managing sales transactions and POS operations",
        Version = "v1"
    });
});

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateSaleCommand).Assembly);
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(CreateSaleCommandValidator).Assembly);

// Add HttpContextAccessor for CurrentUserService
builder.Services.AddHttpContextAccessor();

// Add application services
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add mock repositories for MVP
builder.Services.AddScoped<ISaleRepository, MockSaleRepository>();
builder.Services.AddScoped<ITillRepository, MockTillRepository>();

// Add domain event and notification services
builder.Services.AddScoped<IDomainEventService, DomainEventService>();
        builder.Services.AddScoped<INotificationService, MockNotificationService>();
        builder.Services.AddScoped<IInventoryService, MockInventoryService>();
        builder.Services.AddScoped<IPaymentGatewayService, MockPaymentGatewayService>();
        builder.Services.AddScoped<IReceiptService, ReceiptService>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TOSS ERP Sales API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
