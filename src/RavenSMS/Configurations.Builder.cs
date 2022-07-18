namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// the options for building the RavenSMS integration.
/// </summary>
public class RavenSmsBuilder
{
    private string _serverId = "srv_defaultserver";
    private string _serverName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "Default Server";

    /// <summary>
    /// create an instance of <see cref="RavenSmsBuilder"/>
    /// </summary>
    /// <param name="services"></param>
    internal RavenSmsBuilder(IServiceCollection services) => ServiceCollection = services;

    /// <summary>
    /// Get the service collection.
    /// </summary>
    public IServiceCollection ServiceCollection { get; }

    /// <summary>
    /// set the server id
    /// </summary>
    /// <param name="serverId">the id of the server</param>
    /// <exception cref="RavenSmsOptionValueNotSpecifiedException">if the given server id is null or empty</exception>
    public void SetServerId(string serverId)
    {
        if (string.IsNullOrEmpty(serverId))
            throw new RavenSmsOptionValueNotSpecifiedException(
                    $"ServerId", "the given RavenSmsOptions.ServerId value is null or empty.");

        _serverId = serverId;
    }

    /// <summary>
    /// set the name of the server
    /// </summary>
    /// <param name="serverName">the server name</param>
    /// <exception cref="RavenSmsOptionValueNotSpecifiedException">if the given server name is null or empty</exception>
    public void SetServerName(string serverName)
    {
        if (string.IsNullOrEmpty(serverName))
            throw new RavenSmsOptionValueNotSpecifiedException(
                    $"ServerName", "the given RavenSmsOptions.ServerName value is null or empty.");

        _serverName = serverName;
    }

    /// <summary>
    /// set the options values
    /// </summary>
    internal RavenSmsOptions InitOptions()
    {
        return new RavenSmsOptions
        {
            ServerId = _serverId,
            ServerName = _serverName,
        };
    }

    /// <summary>
    /// init an instance of the <see cref="RavenSmsBuilder"/>
    /// </summary>
    /// <param name="services">the service collection</param>
    /// <param name="config">the configuration action</param>
    /// <returns>instance of <see cref="RavenSmsBuilder"/></returns>
    internal static RavenSmsBuilder InitBuilder(IServiceCollection services, Action<RavenSmsBuilder> config)
    {
        var builderOptions = new RavenSmsBuilder(services);
        config(builderOptions);
        return builderOptions;
    }
}