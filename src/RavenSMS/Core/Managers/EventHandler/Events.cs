namespace RavenSMS.Events;

/// <summary>
/// event raised when a client is connected
/// </summary>
public struct ClientConnectedEvent : IEvent
{
    /// <summary>
    /// create an instance of <see cref="ClientConnectedEvent"/>
    /// </summary>
    /// <param name="clientId">the id of the client</param>
    /// <exception cref="ArgumentNullException">if the client id is null</exception>
    public ClientConnectedEvent(string clientId, string connectionId)
    {
        ConnectionId = connectionId;
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
    }

    /// <summary>
    /// the id if the client
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// the connection id
    /// </summary>
    public string ConnectionId { get; }
}

/// <summary>
/// event raised when a client is disconnected
/// </summary>
public struct ClientDisconnectedEvent : IEvent
{
    /// <summary>
    /// create an instance of <see cref="ClientConnectedEvent"/>
    /// </summary>
    /// <param name="clientId">the id of the client</param>
    /// <param name="connectionId">the id of the connection</param>
    /// <exception cref="ArgumentNullException">if the client id is null</exception>
    public ClientDisconnectedEvent(string clientId, string connectionId)
    {
        ConnectionId = connectionId;
        ClientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
    }

    /// <summary>
    /// the id of the client
    /// </summary>
    public string ClientId { get; }

    /// <summary>
    /// the connection id
    /// </summary>
    public string ConnectionId { get; }
}

/// <summary>
/// event raised when the message sending has succeeded
/// </summary>
public struct MessageSentEvent : IEvent
{
    /// <summary>
    /// create an instance of <see cref="MessageSentEvent"/>
    /// </summary>
    /// <param name="messageId">the id of message</param>
    /// <exception cref="ArgumentNullException">if the message id is null</exception>
    public MessageSentEvent(string messageId)
    {
        MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
    }

    /// <summary>
    /// the id of the message
    /// </summary>
    public string MessageId { get; set; }
}

/// <summary>
/// event raised when the message sending has failed
/// </summary>
public struct MessageUnsentEvent : IEvent
{
    /// <summary>
    /// create an instance of <see cref="MessageUnsentEvent"/>
    /// </summary>
    /// <param name="messageId">the id of message</param>
    /// <exception cref="ArgumentNullException">if the message id is null</exception>
    public MessageUnsentEvent(string messageId)
    {
        MessageId = messageId ?? throw new ArgumentNullException(nameof(messageId));
    }

    /// <summary>
    /// the id of the message
    /// </summary>
    public string MessageId { get; set; }
}
