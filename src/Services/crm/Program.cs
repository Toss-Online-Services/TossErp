using Crm.Application.Commands;
using Crm.Application.Queries;
using Crm.Application.DTOs;
using Crm.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(typeof(CreateCustomerCommand).Assembly);

// Add Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// API Endpoints
app.MapGet("/", () => "CRM API running");

// Customer endpoints
app.MapGet("/api/customers", async (IMediator mediator) =>
{
    var customers = await mediator.Send(new GetTopCustomersQuery(100));
    return Results.Ok(customers);
})
.WithName("GetCustomers")
.WithOpenApi();

app.MapGet("/api/customers/top", async (IMediator mediator, int count = 10) =>
{
    var customers = await mediator.Send(new GetTopCustomersQuery(count));
    return Results.Ok(customers);
})
.WithName("GetTopCustomers")
.WithOpenApi();

app.MapGet("/api/customers/lapsed", async (IMediator mediator, int daysThreshold = 90) =>
{
    var customers = await mediator.Send(new GetLapsedCustomersQuery(daysThreshold));
    return Results.Ok(customers);
})
.WithName("GetLapsedCustomers")
.WithOpenApi();

app.MapPost("/api/customers", async (IMediator mediator, CreateCustomerDto createCustomerDto) =>
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
.WithName("CreateCustomer")
.WithOpenApi();

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
.WithName("RecordPurchase")
.WithOpenApi();

app.Run();

// Request DTOs for API
public record RecordPurchaseRequest(decimal Amount, Guid? OrderId = null);



