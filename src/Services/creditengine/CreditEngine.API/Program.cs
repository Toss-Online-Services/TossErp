var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "TOSS ERP III CreditEngine API is running");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "creditengine" }));
app.MapGet("/_status", () => Results.Ok(new { 
    service = "TossErp.CreditEngine.API", 
    version = "1.0.0",
    timestamp = DateTime.UtcNow 
}));

app.MapHealthChecks("/health");

app.Run();
