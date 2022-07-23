namespace RavenSMS.Queues.Coravel;

/// <summary>
/// the Invocable for processing RavenSMS messages
/// </summary>
public class ProcessSmsMessageInvocable : IInvocable, IInvocableWithPayload<ProcessSmsMessageInvocablePayload>
{
    private readonly IRavenSmsManager _manager;

    public ProcessSmsMessageInvocable(IRavenSmsManager manager) 
        => _manager = manager;

    /// <inheritdoc/>
    public ProcessSmsMessageInvocablePayload Payload { get; set; } = default!;

    /// <inheritdoc/>
    public Task Invoke()
    {
        if (Payload is null)
            throw new RavenSmsException("the payload is null");

        return ProcessAsync();
    }

    private async Task ProcessAsync()
    {
        if (Payload.Delay.HasValue)
            await Task.Delay(Payload.Delay.Value);

        await _manager.ProcessAsync(Payload.MessageId);
    }
}

/// <summary>
/// the invocable payload
/// </summary>
public class ProcessSmsMessageInvocablePayload
{
    public ProcessSmsMessageInvocablePayload(string messageId)
        => MessageId = messageId;

    public ProcessSmsMessageInvocablePayload(string messageId, TimeSpan delay)
        : this(messageId) => Delay = delay;

    /// <summary>
    /// the message id
    /// </summary>
    public string MessageId { get; }

    /// <summary>
    /// the delay to wait before sending the message
    /// </summary>
    public TimeSpan? Delay { get; }

    internal static ProcessSmsMessageInvocablePayload Create(string messageId)
        => new(messageId);
    internal static ProcessSmsMessageInvocablePayload Create(string messageId, TimeSpan delay)
        => new(messageId, delay);
}