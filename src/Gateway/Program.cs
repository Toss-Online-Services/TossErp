using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Reverse proxy config from appsettings
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHttpClient();

// Basic JWT auth (POPIA-friendly defaults via env)
var authority = builder.Configuration["JWT:Authority"] ?? builder.Configuration["JWT_AUTHORITY"];
var audience = builder.Configuration["JWT:Audience"] ?? builder.Configuration["JWT_AUDIENCE"] ?? "toss-api";
if (!string.IsNullOrWhiteSpace(authority))
{
  builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
      options.Authority = authority;
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = true,
        ValidAudience = audience,
      };
    });
}

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
  options.AddPolicy("Default", p => p
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins(
      "http://localhost:3000",
      "http://localhost:5173"
    ));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors("Default");
if (!string.IsNullOrWhiteSpace(authority))
{
  app.UseAuthentication();
  app.UseAuthorization();
}

// BFF endpoints
app.MapGet("/", () => Results.Ok(new { ok = true, service = "gateway" }));
app.MapGet("/api/mobile/dashboard", (IHttpClientFactory http, IConfiguration cfg) =>
{
  var invBase = cfg["Services:Inventory:BaseUrl"] ?? cfg["Services__Inventory__BaseUrl"] ?? "http://inventory:8080";
  var salesBase = cfg["Services:Sales:BaseUrl"] ?? cfg["Services__Sales__BaseUrl"] ?? "http://sales:8080";
  var collabBase = cfg["Services:Collaboration:BaseUrl"] ?? cfg["Services__Collaboration__BaseUrl"] ?? "http://collaboration:8080";

  // Minimal fan-out placeholders (no awaits needed)
  var inventorySummary = new { lowStock = 0, items = 0 };
  var salesSummary = new { today = 0m, orders = 0 };
  var collabSummary = new { invites = 0 };
  return Results.Ok(new { inventory = inventorySummary, sales = salesSummary, collab = collabSummary });
});

app.MapGet("/api/web/dashboard", () => Results.Ok(new
{
  metrics = new { sales7d = Array.Empty<int>(), lowStock = 0 }
}));

// Sync batch stubs
app.MapPost("/api/mobile/sync/inventory", (HttpRequest req) => Results.Ok(new { accepted = true }));
app.MapPost("/api/mobile/sync/sales", (HttpRequest req) => Results.Ok(new { accepted = true }));

// Proxy downstreams
app.MapReverseProxy();

app.Run();



