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

app.MapGet("/", () => "TOSS ERP III AssetSharing API is running");
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "assetsharing" }));
app.MapGet("/_status", () => Results.Ok(new { 
    service = "TossErp.AssetSharing.API", 
    version = "1.0.0",
    timestamp = DateTime.UtcNow 
}));

app.MapHealthChecks("/health");

app.Run();
