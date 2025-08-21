using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Projects.Application.Extensions;
using Projects.Infrastructure.Extensions;
using Projects.Infrastructure.Data;
using Projects.Infrastructure.Hubs;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/projects-api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// API versioning
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = Microsoft.AspNetCore.Mvc.ApiVersioning.ApiVersionReader.Combine(
        new Microsoft.AspNetCore.Mvc.ApiVersioning.UrlSegmentApiVersionReader(),
        new Microsoft.AspNetCore.Mvc.ApiVersioning.QueryStringApiVersionReader("version"),
        new Microsoft.AspNetCore.Mvc.ApiVersioning.HeaderApiVersionReader("X-Version")
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
        Title = "Projects API",
        Version = "v1",
        Description = "Project Management and Time Tracking API for TOSS ERP",
        Contact = new OpenApiContact
        {
            Name = "TOSS Development Team",
            Email = "dev@toss.local"
        }
    });

    // JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
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

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.EnableAnnotations();
});

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings.GetValue<string>("SecretKey");

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("JWT SecretKey is not configured");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
        ValidateAudience = true,
        ValidAudience = jwtSettings.GetValue<string>("Audience"),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5),
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    // Configure SignalR JWT authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ProjectManagerOrAdmin", policy => 
        policy.RequireRole("Admin", "ProjectManager"));
    options.AddPolicy("TeamLeadOrHigher", policy => 
        policy.RequireRole("Admin", "ProjectManager", "TeamLead"));
    options.AddPolicy("ProjectAccess", policy => 
        policy.RequireRole("Admin", "ProjectManager", "TeamLead", "TeamMember"));
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProjectsApiCorsPolicy", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",    // React dev
            "http://localhost:5173",    // Vite dev
            "http://localhost:8080",    // Vue dev
            "https://localhost:7000",   // Gateway HTTPS
            "http://localhost:7000"     // Gateway HTTP
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithExposedHeaders("X-Total-Count", "X-Page", "X-Page-Size", "X-Total-Pages");
    });
});

// Rate limiting
builder.Services.AddRateLimiter(options =>
{
    // Default rate limit
    options.AddFixedWindowLimiter("DefaultPolicy", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
    });

    // Stricter limits for reporting endpoints
    options.AddFixedWindowLimiter("ReportsPolicy", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    // Stricter limits for export endpoints
    options.AddFixedWindowLimiter("ExportPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    // Time tracking endpoints - moderate limits
    options.AddFixedWindowLimiter("TimeTrackingPolicy", opt =>
    {
        opt.PermitLimit = 50;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Application and Infrastructure services
builder.Services.AddProjectsApplication();
builder.Services.AddProjectsInfrastructure(builder.Configuration);

// SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(30);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(120);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
});

// Health checks
builder.Services.AddHealthChecks()
    .AddDbContext<ProjectsDbContext>(name: "projects-database")
    .AddCheck("projects-api-health", () => HealthCheckResult.Healthy("Projects API is running"))
    .AddCheck<ProjectsHealthCheck>("projects-business-logic");

// HTTP client for external services
builder.Services.AddHttpClient();

// Background services
builder.Services.AddHostedService<ProjectStatusUpdateService>();
builder.Services.AddHostedService<TimeEntryReminderService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Projects API V1");
        c.RoutePrefix = string.Empty;
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
    });
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }
    
    await next();
});

// Request/Response logging middleware
app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    var requestId = Guid.NewGuid().ToString("N")[..8];
    
    using (Log.Logger.BeginContext())
    {
        Log.Information("Starting request {RequestId} {Method} {Path} from {RemoteIpAddress}",
            requestId, context.Request.Method, context.Request.Path, context.Connection.RemoteIpAddress);
            
        context.Items["RequestId"] = requestId;
        
        try
        {
            await next();
        }
        finally
        {
            var duration = DateTime.UtcNow - startTime;
            Log.Information("Completed request {RequestId} {Method} {Path} with {StatusCode} in {Duration}ms",
                requestId, context.Request.Method, context.Request.Path, context.Response.StatusCode, duration.TotalMilliseconds);
        }
    }
});

app.UseHttpsRedirection();

app.UseCors("ProjectsApiCorsPolicy");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers with rate limiting
app.MapControllers().RequireRateLimiting("DefaultPolicy");

// Apply specific rate limiting to certain endpoints
app.MapControllerRoute(
    name: "reports",
    pattern: "api/v{version:apiVersion}/project-reports/{action}",
    defaults: new { controller = "ProjectReports" })
    .RequireRateLimiting("ReportsPolicy");

app.MapControllerRoute(
    name: "exports",
    pattern: "api/v{version:apiVersion}/{controller}/export/{action}",
    defaults: new { })
    .RequireRateLimiting("ExportPolicy");

app.MapControllerRoute(
    name: "timeTracking",
    pattern: "api/v{version:apiVersion}/time-entries/{action}",
    defaults: new { controller = "TimeEntries" })
    .RequireRateLimiting("TimeTrackingPolicy");

// Map SignalR hubs
app.MapHub<ProjectUpdatesHub>("/hubs/project-updates");
app.MapHub<TeamCollaborationHub>("/hubs/team-collaboration");
app.MapHub<TimeTrackingHub>("/hubs/time-tracking");

// Health checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var response = new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            duration = report.TotalDuration,
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration,
                exception = x.Value.Exception?.Message,
                data = x.Value.Data
            })
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

// Global exception handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var requestId = context.Items["RequestId"]?.ToString();
            
            Log.Error(error.Error, "Unhandled exception in request {RequestId}: {ErrorMessage}", 
                requestId, error.Error.Message);

            var response = new
            {
                error = "An internal server error occurred",
                requestId = requestId,
                timestamp = DateTime.UtcNow
            };

            if (app.Environment.IsDevelopment())
            {
                response = new
                {
                    error = error.Error.Message,
                    stackTrace = error.Error.StackTrace,
                    requestId = requestId,
                    timestamp = DateTime.UtcNow
                };
            }

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    });
});

// Graceful shutdown
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStopping.Register(() =>
{
    Log.Information("Projects API is shutting down...");
});

lifetime.ApplicationStopped.Register(() =>
{
    Log.Information("Projects API has stopped");
    Log.CloseAndFlush();
});

Log.Information("Starting Projects API");
Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
Log.Information("Urls: {Urls}", string.Join(", ", builder.WebHost.GetSetting("urls")?.Split(';') ?? Array.Empty<string>()));

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Projects API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
