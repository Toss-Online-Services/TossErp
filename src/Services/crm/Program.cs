using Crm.Application;
using Crm.Application.Commands;
using Crm.Application.Queries;
using Crm.Application.DTOs;
using Crm.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CRM API", Version = "v1" });
});

// Add CRM Application services
builder.Services.AddCrmApplication();

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors();

// Migrate database
try
{
    await app.Services.MigrateDatabaseAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while migrating the database");
}

// API Endpoints
app.MapGet("/", () => "CRM API running");

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "CRM", Timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck");

// Customer endpoints
app.MapGet("/api/customers", async (IMediator mediator) =>
{
    var customers = await mediator.Send(new GetTopCustomersQuery(100));
    return Results.Ok(customers);
})
.WithName("GetCustomers");

app.MapGet("/api/customers/{customerId:guid}", async (IMediator mediator, Guid customerId) =>
{
    var customer = await mediator.Send(new GetCustomerByIdQuery(customerId));
    if (customer == null)
        return Results.NotFound($"Customer with ID {customerId} not found");
    
    return Results.Ok(customer);
})
.WithName("GetCustomerById");

app.MapGet("/api/customers/top", async (IMediator mediator, int count = 10) =>
{
    var customers = await mediator.Send(new GetTopCustomersQuery(count));
    return Results.Ok(customers);
})
.WithName("GetTopCustomers");

app.MapGet("/api/customers/lapsed", async (IMediator mediator, int daysThreshold = 90) =>
{
    var customers = await mediator.Send(new GetLapsedCustomersQuery(daysThreshold));
    return Results.Ok(customers);
})
.WithName("GetLapsedCustomers");

app.MapPost("/api/customers", async (IMediator mediator, Crm.Application.DTOs.CreateCustomerDto createCustomerDto) =>
{
    var command = new CreateCustomerCommand
    {
        FirstName = createCustomerDto.FirstName,
        LastName = createCustomerDto.LastName,
        Email = createCustomerDto.Email,
        Phone = createCustomerDto.Phone,
        Address = createCustomerDto.Address,
        DateOfBirth = createCustomerDto.DateOfBirth
    };

    var customerId = await mediator.Send(command);
    return Results.Created($"/api/customers/{customerId}", new { Id = customerId });
})
.WithName("CreateCustomer");

app.MapPost("/api/customers/{customerId}/purchases", async (IMediator mediator, Guid customerId, RecordPurchaseRequest request) =>
{
    var command = new RecordPurchaseCommand
    {
        CustomerId = customerId,
        Amount = request.Amount,
        OrderId = request.OrderId
    };

    await mediator.Send(command);
    return Results.Ok(new { Message = "Purchase recorded successfully" });
})
.WithName("RecordPurchase");

app.Run();

// Request DTOs for API
public record RecordPurchaseRequest(decimal Amount, Guid? OrderId = null);



