namespace RavenSMS;

/// <summary>
/// the options for configuring the RavenSMS server
/// </summary>
public class RavenSmsOptions
{
    /// <summary>
    /// Get or set RavenSMS server info.
    /// </summary>
    public RavenSmsServerInfo ServerInfo { get; set; } = default!;

    /// <summary>
    /// validate if the options are all set correctly
    /// </summary>
    public void Validate()
    {
        if (ServerInfo is null)
            throw new RavenSmsOptionValueNotSpecifiedException(
                    $"{nameof(ServerInfo)}", "the given RavenSmsDeliveryChannelOptions.ServerInfo value is null or empty.");

        if (string.IsNullOrEmpty(ServerInfo.ServerId))
            throw new RavenSmsOptionValueNotSpecifiedException(
                    $"ServerInfo.ServerId", "the given RavenSmsDeliveryChannelOptions.ServerInfo.ServerId value is null or empty.");

        if (string.IsNullOrEmpty(ServerInfo.ServerName))
            throw new RavenSmsOptionValueNotSpecifiedException(
                    $"ServerInfo.ServerName", "the given RavenSmsDeliveryChannelOptions.ServerInfo.ServerName value is null or empty.");
    }
}
