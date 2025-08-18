using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eShop.ServiceDefaults;

public static class ProblemDetailsExtensions
{
    public static IHostApplicationBuilder AddDefaultProblemDetails(this IHostApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        return builder;
    }

    public static WebApplication UseDefaultProblemDetails(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        return app;
    }
}


