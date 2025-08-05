using TossErp.Stock.Application;
using TossErp.Stock.Infrastructure;
using TossErp.Stock.Infrastructure.Data;
using TossErp.Stock.Application.Common.Interfaces;
using TossErp.Stock.API.Services;
using TossErp.Stock.API.Infrastructure;
using TossErp.Stock.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add HTTP context accessor for current user service
builder.Services.AddHttpContextAccessor();

// Register current user service
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGet("/", () => "TOSS ERP III Stock API is running");

// Map health check endpoint
app.MapHealthChecks("/health");

// Map all endpoint groups
app.MapEndpoints();

app.Run();

public partial class Program { }
