namespace RavenSMS.Internal.Constants;

/// <summary>
///  class that defines the disconnection reasons
/// </summary>
internal static class DisconnectionReason
{
    public const string Disconnect = "disconnect";
    public const string ClientDeleted = "client_deleted";
    public const string ClientNotFound = "client_not_found";
    public const string ClientAlreadyConnected = "client_already_connected";
}
