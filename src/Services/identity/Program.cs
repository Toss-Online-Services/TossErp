using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TossErp.Configuration;
using TossErp.Identity.Services;
using TossErp.Identity.Endpoints;
using eShop.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (includes OpenTelemetry, health checks, ProblemDetails)
builder.AddServiceDefaults();

// Add configuration
builder.Services.AddTossConfiguration(builder.Configuration);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TOSS Identity API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});

// Add JWT Authentication
var jwtOptions = builder.Configuration.GetOptions<JwtOptions>(JwtOptions.SectionName);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateIssuer = jwtOptions.ValidateIssuer,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = jwtOptions.ValidateAudience,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = jwtOptions.ValidateLifetime,
            ClockSkew = TimeSpan.FromMinutes(jwtOptions.ClockSkewMinutes)
        };
    });

builder.Services.AddAuthorization();

// Register application services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, InMemoryRefreshTokenRepository>(); // Placeholder

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultProblemDetails();
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapAuthenticationEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    service = "identity",
    version = "1.0.0",
    status = "running",
    endpoints = new[] { "/api/auth/login", "/api/auth/refresh", "/api/auth/logout", "/api/auth/validate" }
})).WithTags("Health");

app.MapGet("/health", () => Results.Ok(new { status = "healthy" })).WithTags("Health");

app.Run();



