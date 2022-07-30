namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// the Configurations class
/// </summary>
public static class Configurations
{
    /// <summary>
    /// configure the registration of the Dashboard
    /// </summary>
    /// <param name="builder">the <see cref="RavenSmsBuilder"/> instance</param>
    public static RavenSmsBuilder UseDashboard(this RavenSmsBuilder builder)
    {
        builder.ServiceCollection.ConfigureOptions(typeof(RavenSmsUIConfigureOptions));
        return builder;
    }
}
