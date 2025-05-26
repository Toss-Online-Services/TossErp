using Asp.Versioning.Builder;
using System.Reflection;
using Catalog.API.Services;
using Catalog.Domain.Interfaces;
using Catalog.Domain.Mappings;
using Catalog.Domain.Services;
using Catalog.Infrastructure.Context;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(CatalogMappingProfile));

// Add DbContext
builder.Services.AddDbContext<CatalogContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repositories
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();

// Add Services
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ICatalogAI>(sp =>
{
    var context = sp.GetRequiredService<CatalogContext>();
    var openAiApiKey = builder.Configuration["OpenAI:ApiKey"] 
        ?? throw new InvalidOperationException("OpenAI API key is not configured");
    return new CatalogAI(context, openAiApiKey);
});

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapDefaultEndpoints();

app.UseStatusCodePages();

app.MapCatalogApi();

app.UseDefaultOpenApi();
app.Run();
