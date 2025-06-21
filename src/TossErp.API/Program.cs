using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TossErp.Application.Services;
using TossErp.Domain.SeedWork;
using TossErp.Domain.AggregatesModel.TownshipEnterpriseAggregate;
using TossErp.Domain.AggregatesModel.CooperativeAggregate;
using TossErp.Domain.AggregatesModel.StokvelAggregate;
using TossErp.Infrastructure.Data;
using TossErp.Infrastructure.Repositories;
using TossErp.Copilot.Domain;
using TossErp.Copilot.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Entity Framework
builder.Services.AddDbContext<TossErpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
        };
    });

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("TownshipEnterprisePolicy", policy =>
        policy.RequireRole("TownshipEnterprise", "Admin"));
    
    options.AddPolicy("CooperativePolicy", policy =>
        policy.RequireRole("Cooperative", "Admin"));
    
    options.AddPolicy("StokvelPolicy", policy =>
        policy.RequireRole("Stokvel", "Admin"));
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "TossErp API", 
        Version = "v1",
        Description = "API for managing township and rural enterprises in South Africa"
    });

    // Configure JWT authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register repositories
builder.Services.AddScoped<ITownshipEnterpriseRepository, TownshipEnterpriseRepository>();
builder.Services.AddScoped<ICooperativeRepository, CooperativeRepository>();
builder.Services.AddScoped<IStokvelRepository, StokvelRepository>();

// Register services
builder.Services.AddScoped<ITownshipEnterpriseService, TownshipEnterpriseService>();
builder.Services.AddScoped<ICooperativeService, CooperativeService>();
builder.Services.AddScoped<IStokvelService, StokvelService>();
builder.Services.AddScoped<ICopilotService, CopilotService>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork>(provider => 
    provider.GetRequiredService<TossErpDbContext>());

// Configure logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TossErp API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TossErpDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
