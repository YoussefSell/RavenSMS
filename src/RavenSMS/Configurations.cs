namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// the Configurations class
/// </summary>
public static class Configurations
{
    /// <summary>
    /// add the RavenSMS channel to be used with your SMS service.
    /// </summary>
    /// <param name="services">the SMS.Net builder instance.</param>
    /// <param name="config">the configuration builder instance.</param>
    /// <returns>instance of <see cref="RavenSmsBuilder"/> to enable methods chaining.</returns>
    public static RavenSmsBuilder AddRavenSMS(this IServiceCollection services, Action<RavenSmsBuilder> config) 
        => RavenSmsBuilder.InitBuilder(services, config)
            .RegisterOptions()
            .RegisterManagers()
            .RegisterServices();

    /// <summary>
    /// Maps incoming requests with the ravenSMS hub path to the <see cref="RavenSmsWebSocketManager"/>
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    public static void MapRavenSmsHub(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<RavenSmsWebSocketManager>("RavenSMS/Hub");
    }

    /// <summary>
    /// Set RavenSMS to use in memory stores to persist the data.
    /// </summary>
    /// <param name="builder">the RavenSMS builder instance.</param>
    /// <returns>instance of <see cref="RavenSmsBuilder"/> to enable methods chaining.</returns>
    public static RavenSmsBuilder UseInMemoryStores(this RavenSmsBuilder builder)
    {
        builder.ServiceCollection.AddSingleton<IRavenSmsClientsStore, RavenSmsClientsInMemoryStore>();
        builder.ServiceCollection.AddSingleton<IRavenSmsMessagesStore, RavenSmsMessagesInMemoryStore>();

        return builder;
    }

    /// <summary>
    /// Set RavenSMS to use in memory Queue to queue and process sending events.
    /// </summary>
    /// <param name="builder">the RavenSMS builder instance.</param>
    /// <returns>instance of <see cref="RavenSmsBuilder"/> to enable methods chaining.</returns>
    public static RavenSmsBuilder UseInMemoryQueue(this RavenSmsBuilder builder)
    {
        builder.ServiceCollection.AddSingleton<IQueueManager, InMemoryQueueManager>();
        builder.ServiceCollection.AddSingleton<IInMemoryQueue, InMemoryQueue>();
        builder.ServiceCollection.AddHostedService<InMemoryQueueHost>();

        return builder;
    }

    /// <summary>
    /// register options
    /// </summary>
    /// <param name="builder">the <see cref="RavenSmsBuilder"/> instance</param>
    internal static RavenSmsBuilder RegisterOptions(this RavenSmsBuilder builder)
    {
        builder.ServiceCollection.ConfigureOptions(typeof(RavenSmsUIConfigureOptions));
        builder.ServiceCollection.Configure<RavenSmsOptions>(o => Options.Options.Create(builder.InitOptions()));
        return builder;
    }

    /// <summary>
    /// register managers
    /// </summary>
    /// <param name="builder">the <see cref="RavenSmsBuilder"/> instance</param>
    internal static RavenSmsBuilder RegisterManagers(this RavenSmsBuilder builder)
    {
        builder.ServiceCollection.AddScoped<IRavenSmsManager, RavenSmsManager>();
        builder.ServiceCollection.AddScoped<IRavenSmsClientsManager, RavenSmsClientsManager>();
        builder.ServiceCollection.AddScoped<IRavenSmsMessagesManager, RavenSmsMessagesManager>();
        return builder;
    }

    /// <summary>
    /// register services
    /// </summary>
    /// <param name="builder">the <see cref="RavenSmsBuilder"/> instance</param>
    internal static RavenSmsBuilder RegisterServices(this RavenSmsBuilder builder)
    {
        builder.ServiceCollection.AddScoped<IRavenSmsService, RavenSmsService>();
        return builder;
    }
}
