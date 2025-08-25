using Crm.Application;
using Crm.Infrastructure;
using Crm.Infrastructure.Data.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Crm.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "TOSS ERP - CRM API", 
        Version = "v1",
        Description = "Customer Relationship Management API for TOSS ERP"
    });
});

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CRM API v1"));
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CrmDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<CrmDataSeeder>>();
    
    try
    {
        await context.Database.EnsureCreatedAsync();
        
        var seeder = new CrmDataSeeder(context, logger);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var appLogger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        appLogger.LogError(ex, "An error occurred while seeding the database");
    }
}

app.Run();