namespace Ordering.API;

public class Program
{
    public static readonly string AppName = "Ordering.API";

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<OrderingContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("OrderingDb")));

        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssemblyContaining<Program>();
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        });

        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOrderQueries, OrderQueries>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddScoped<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

        builder.Services.AddSingleton<IEventBus, EventBusRabbitMQ>();
        builder.Services.AddSingleton<Func<DbConnection, IIntegrationEventLogService>>(sp => (DbConnection c) => new IntegrationEventLogService(c));
        builder.Services.AddSingleton<IIntegrationEventLogService>(sp => new IntegrationEventLogService(builder.Configuration.GetConnectionString("OrderingDb")));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
