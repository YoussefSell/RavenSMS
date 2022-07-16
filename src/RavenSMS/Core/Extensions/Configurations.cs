using RavenSMS.Core.Services;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// the Configurations class
/// </summary>
public static class Configurations
{
    /// <summary>
    /// add the RavenSMS channel to be used with your SMS service, 
    /// this will register RavenSMS with the InMemory implementation of the Queue and the stores.
    /// </summary>
    /// <param name="services">the SMS.Net builder instance.</param>
    /// <returns>instance of <see cref="SmsNetBuilder"/> to enable methods chaining.</returns>
    public static IServiceCollection UseRavenSMS(this IServiceCollection services)
        => services.UseRavenSMS(options =>
        {
            options.UseInMemoryQueue();
            options.UseInMemoryStores();
        });

    /// <summary>
    /// add the RavenSMS channel to be used with your SMS service.
    /// </summary>
    /// <param name="services">the SMS.Net builder instance.</param>
    /// <param name="config">the configuration builder instance.</param>
    /// <returns>instance of <see cref="SmsNetBuilder"/> to enable methods chaining.</returns>
    public static IServiceCollection UseRavenSMS(this IServiceCollection services, Action<RavenSmsBuilder> config)
    {
        // load the configuration
        var builderOptions = new RavenSmsBuilder(services);
        config(builderOptions);

        // build and validate the options
        var options = builderOptions.InitOptions();
        options.Validate();

        services.AddScoped((s) => options);
        services.ConfigureOptions(typeof(RavenSmsUIConfigureOptions));
                
        services.AddScoped<IRavenSmsManager, RavenSmsManager>();
        services.AddScoped<IRavenSmsClientsManager, RavenSmsClientsManager>();
        services.AddScoped<IRavenSmsMessagesManager, RavenSmsMessagesManager>();
                
        services.AddScoped<IRavenSmsService, RavenSmsService>();

        return services;
    }

    /// <summary>
    /// Maps incoming requests with the ravenSMS hub path to the <see cref="RavenSmsHub"/>
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the route to.</param>
    public static void MapRavenSmsHub(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<RavenSmsHub>("RavenSMS/Hub");
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
}
