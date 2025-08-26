using Aspire.Hosting;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TossErp.AppHost;

internal static class Extensions
{
    /// <summary>
    /// Adds a hook to set the ASPNETCORE_FORWARDEDHEADERS_ENABLED environment variable to true for all projects in the application.
    /// </summary>
    public static IDistributedApplicationBuilder AddForwardedHeaders(this IDistributedApplicationBuilder builder)
    {
        builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();
        return builder;
    }

    private class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var p in appModel.GetProjectResources())
            {
                p.Annotations.Add(new EnvironmentCallbackAnnotation(context =>
                {
                    context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true";
                }));
            }

            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Configures TOSS ERP projects to use OpenAI for AI capabilities.
    /// </summary>
    public static IDistributedApplicationBuilder AddOpenAI(this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> aiApi,
        IResourceBuilder<ProjectResource> crmApi)
    {
        const string openAIName = "openai";

        const string textEmbeddingName = "textEmbeddingModel";
        const string textEmbeddingModelName = "text-embedding-3-small";

        const string chatName = "chatModel";
        const string chatModelName = "gpt-4o-mini";

        // Simple OpenAI configuration via connection string
        if (builder.Configuration.GetConnectionString(openAIName) is string openAIConnectionString)
        {
            aiApi.WithReference(
                builder.AddConnectionString(textEmbeddingName, ReferenceExpression.Create($"{openAIConnectionString};Deployment={textEmbeddingModelName}")));
            crmApi.WithReference(
                builder.AddConnectionString(chatName, ReferenceExpression.Create($"{openAIConnectionString};Deployment={chatModelName}")));
        }

        return builder;
    }

    /// <summary>
    /// Configures TOSS ERP projects to use Ollama for local AI capabilities.
    /// </summary>
    public static IDistributedApplicationBuilder AddOllama(this IDistributedApplicationBuilder builder,
        IResourceBuilder<ProjectResource> aiApi,
        IResourceBuilder<ProjectResource> crmApi)
    {
        // Ollama configuration would go here when Aspire.Hosting.Ollama package is available
        // For now, just add environment variables for services to detect Ollama availability
        aiApi.WithEnvironment("AI__Provider", "Ollama")
             .WithEnvironment("OllamaEnabled", "false");
        crmApi.WithEnvironment("AI__Provider", "Ollama")
              .WithEnvironment("OllamaEnabled", "false");

        return builder;
    }

    /// <summary>
    /// Adds RabbitMQ with proper persistence and health checks.
    /// </summary>
    public static IResourceBuilder<RabbitMQServerResource> AddTossRabbitMQ(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddRabbitMQ(name)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume();
    }

    /// <summary>
    /// Adds Redis with proper persistence.
    /// </summary>
    public static IResourceBuilder<RedisResource> AddTossRedis(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddRedis(name)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume();
    }

    /// <summary>
    /// Adds PostgreSQL with pgvector support for vector operations.
    /// </summary>
    public static IResourceBuilder<PostgresServerResource> AddTossPostgreSQL(this IDistributedApplicationBuilder builder, string name)
    {
        return builder.AddPostgres(name)
            .WithImage("ankane/pgvector")
            .WithImageTag("latest")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume();
    }
}
