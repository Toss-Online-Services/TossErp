using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Accounts.Application;
using Accounts.Infrastructure;
using Accounts.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ApiVersionReader = Microsoft.AspNetCore.Mvc.ApiVersionReader.Combine(
        new Microsoft.AspNetCore.Mvc.QueryStringApiVersionReader("version"),
        new Microsoft.AspNetCore.Mvc.HeaderApiVersionReader("X-Version"),
        new Microsoft.AspNetCore.Mvc.UrlSegmentApiVersionReader()
    );
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Accounts API", 
        Version = "v1",
        Description = "TOSS ERP Accounts Management API - Financial management, accounting, and reporting services",
        Contact = new OpenApiContact
        {
            Name = "TOSS ERP Support",
            Email = "support@tosserp.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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

    // Add response examples
    c.EnableAnnotations();
});

// Add Application layer services
builder.Services.AddAccountsApplication();

// Add Infrastructure layer services
builder.Services.AddAccountsInfrastructure(builder.Configuration);

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true
    };

    // Add logging for JWT events
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Log.Debug("JWT Token validated for user: {User}", context.Principal?.Identity?.Name);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Log.Warning("JWT Challenge triggered: {Error}", context.Error);
            return Task.CompletedTask;
        }
    };
});

// Add Authorization with policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireAccountManagerRole", policy => policy.RequireRole("Admin", "AccountManager"));
    options.AddPolicy("RequireAccountantRole", policy => policy.RequireRole("Admin", "AccountManager", "Accountant"));
    options.AddPolicy("RequireViewerRole", policy => policy.RequireRole("Admin", "AccountManager", "Accountant", "Viewer"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Rate Limiting
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("DefaultPolicy", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 10;
    });

    rateLimiterOptions.AddFixedWindowLimiter("ReportsPolicy", options =>
    {
        options.PermitLimit = 10;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    });

    rateLimiterOptions.RejectionStatusCode = 429;
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AccountsDbContext>("database")
    .AddCheck<AccountsServiceHealthCheck>("accounts-service");

// Add custom health check
builder.Services.AddSingleton<AccountsServiceHealthCheck>();

// Configure response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounts API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");
    
    await next();
});

// Global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var contextFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            Log.Error(contextFeature.Error, "Unhandled exception occurred");

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
            {
                error = "An internal server error occurred",
                requestId = context.TraceIdentifier
            }));
        }
    });
});

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseCors("DefaultPolicy");

app.UseRateLimiter();

// Apply rate limiting to specific endpoints
app.MapControllers().RequireRateLimiting("DefaultPolicy");

// Apply special rate limiting to reports endpoints
app.Map("/api/v{version:apiVersion}/reports", reportsApp =>
{
    reportsApp.UseRateLimiter();
    reportsApp.UseRouting();
    reportsApp.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers().RequireRateLimiting("ReportsPolicy");
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        
        if (app.Environment.IsDevelopment())
        {
            await context.Database.EnsureCreatedAsync();
            Log.Information("Database ensured for Accounts service");
        }
        else
        {
            await context.Database.MigrateAsync();
            Log.Information("Database migrated for Accounts service");
        }
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while migrating the Accounts database");
        throw;
    }
}

// Graceful shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("Accounts API is shutting down...");
});

lifetime.ApplicationStopped.Register(() =>
{
    Log.Information("Accounts API has stopped");
    Log.CloseAndFlush();
});

Log.Information("Accounts API started successfully");

app.Run();

// Custom health check class
public class AccountsServiceHealthCheck : Microsoft.Extensions.Diagnostics.HealthChecks.IHealthCheck
{
    private readonly AccountsDbContext _context;

    public AccountsServiceHealthCheck(AccountsDbContext context)
    {
        _context = context;
    }

    public async Task<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult> CheckHealthAsync(
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check database connectivity
            await _context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);

            // Check if we can query accounts
            var accountCount = await _context.Accounts.CountAsync(cancellationToken);

            var data = new Dictionary<string, object>
            {
                { "account_count", accountCount },
                { "timestamp", DateTime.UtcNow }
            };

            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(
                "Accounts service is healthy", data);
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(
                "Accounts service is unhealthy", ex);
        }
    }
}
