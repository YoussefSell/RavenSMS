namespace RavenSMS.Domain.Models;

/// <summary>
/// a class that defines the server info
/// </summary>
public class RavenSmsServerInfo
{
    /// <summary>
    /// the id of the server
    /// </summary>
    public string ServerId { get; set; } = default!;

    /// <summary>
    /// the name of the server
    /// </summary>
    public string ServerName { get; set; } = default!;
}