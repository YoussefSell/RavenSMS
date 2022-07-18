namespace RavenSMS;

/// <summary>
/// the options for configuring the RavenSMS server
/// </summary>
public class RavenSmsOptions
{
    /// <summary>
    /// the id of the server
    /// </summary>
    public string ServerId { get; internal set; } = default!;

    /// <summary>
    /// the name of the server
    /// </summary>
    public string ServerName { get; internal set; } = default!;
}
