namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding services to an Microsoft.Extensions.DependencyInjection.IServiceCollection.
/// </summary>
internal static class ScopeServiceCollectionServiceExtensions
{
    /// <summary>
    /// Adds a scoped service of the type specified in TService with an implementation
    /// type specified in TImplementation to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The Microsoft.Extensions.DependencyInjection.IServiceCollection to add the service to.</param>
    /// <param name="scope">the scope to register with.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    internal static IServiceCollection Add<TService, TImplementation>(this IServiceCollection services, RegistrationScope scope)
        where TService : class
        where TImplementation : class, TService => scope switch
        {
            RegistrationScope.Transient => services.AddTransient<TService, TImplementation>(),
            RegistrationScope.Scoped => services.AddScoped<TService, TImplementation>(),
            RegistrationScope.Singleton => services.AddSingleton<TService, TImplementation>(),
            _ => services,
        };
}