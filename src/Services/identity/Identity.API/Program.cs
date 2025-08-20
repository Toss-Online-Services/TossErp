using Identity.Application.Commands;
using Identity.Application.Queries;
using Identity.Application.DTOs;
using Identity.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(typeof(CreateUserCommand));

// Add Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

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

// User endpoints
app.MapPost("/api/users", async (CreateUserDto dto, IMediator mediator) =>
{
    var command = new CreateUserCommand(dto);
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapGet("/api/users/{id}", async (Guid id, IMediator mediator) =>
{
    var query = new GetUserByIdQuery(id);
    var result = await mediator.Send(query);
    return result != null ? Results.Ok(result) : Results.NotFound();
});

app.MapGet("/api/users", async (IMediator mediator) =>
{
    var query = new GetUsersQuery();
    var result = await mediator.Send(query);
    return Results.Ok(result);
});

// Consent endpoints
app.MapPost("/api/consents", async (CreateUserConsentDto dto, IMediator mediator) =>
{
    var command = new CreateUserConsentCommand(dto);
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapPost("/api/consents/{id}/revoke", async (Guid id, RevokeUserConsentDto dto, IMediator mediator) =>
{
    var command = new RevokeUserConsentCommand(dto with { ConsentId = id });
    var result = await mediator.Send(command);
    return result ? Results.Ok() : Results.NotFound();
});

app.MapGet("/api/users/{userId}/consents", async (Guid userId, [FromQuery] string tenantId, IMediator mediator) =>
{
    var query = new GetUserConsentsQuery(userId, tenantId);
    var result = await mediator.Send(query);
    return Results.Ok(result);
});

// Audit trail endpoints
app.MapPost("/api/audit-trail", async (CreateAuditTrailDto dto, IMediator mediator) =>
{
    var command = new CreateAuditTrailCommand(dto);
    var result = await mediator.Send(command);
    return Results.Ok(result);
});

app.MapGet("/api/audit-trail", async ([FromQuery] AuditTrailFilterDto filter, IMediator mediator) =>
{
    var query = new GetAuditTrailQuery(filter);
    var result = await mediator.Send(query);
    return Results.Ok(result);
});

app.MapGet("/api/users/{userId}/audit-trail", async (Guid userId, [FromQuery] string tenantId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, IMediator mediator) =>
{
    var filter = new AuditTrailFilterDto(UserId: userId, TenantId: tenantId, FromDate: fromDate, ToDate: toDate);
    var query = new GetAuditTrailQuery(filter);
    var result = await mediator.Send(query);
    return Results.Ok(result);
});

app.Run();
