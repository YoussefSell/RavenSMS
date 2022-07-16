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
    public void SetServerId(string serverId)
        => _serverId = serverId;

    /// <summary>
    /// set the name of the server
    /// </summary>
    /// <param name="serverName">the server name</param>
    public void SetServerName(string serverName)
        => _serverName = serverName;

    /// <summary>
    /// set the options values
    /// </summary>
    internal RavenSmsOptions InitOptions()
    {
        return new RavenSmsOptions
        {
            ServerInfo = new RavenSmsServerInfo
            {
                ServerId = _serverId,
                ServerName = _serverName,
            }
        };
    }
}

