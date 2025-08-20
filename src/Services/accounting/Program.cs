using TossErp.Accounting.Application.Common.Interfaces;
using TossErp.Accounting.Infrastructure.Persistence;
using TossErp.Accounting.Infrastructure.Persistence.Repositories;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TossErp.Accounting.Application.Commands.CreateAccount.CreateAccountCommand).Assembly));

// Add repositories and services
builder.Services.AddScoped<IAccountRepository, MockAccountRepository>();
builder.Services.AddScoped<ICashbookRepository, MockCashbookRepository>();
builder.Services.AddScoped<ICashbookEntryRepository, MockCashbookEntryRepository>();
builder.Services.AddScoped<IStockValuationSnapshotRepository, MockStockValuationSnapshotRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add application services
builder.Services.AddScoped<TossErp.Accounting.Application.Services.PostingRulesService>();
builder.Services.AddScoped<TossErp.Accounting.Application.Services.StockValuationService>();
builder.Services.AddScoped<IReportingService, TossErp.Accounting.Application.Services.ReportingService>();
builder.Services.AddScoped<IReconciliationService, TossErp.Accounting.Application.Services.ReconciliationService>();

// Add CORS for web UI
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Accounting API running");

app.Run();



