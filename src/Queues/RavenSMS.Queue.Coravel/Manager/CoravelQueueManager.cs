namespace RavenSMS.Queues.Coravel;

/// <summary>
/// the queue manager implementation using Coravel
/// </summary>
public partial class CoravelQueueManager : IQueueManager
{
    /// <inheritdoc/>
    public string QueueEvent<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var queueId = _queue
            .QueueInvocableWithPayload<ProcessEventInvocable<TEvent>, ProcessEventInvocablePayload<TEvent>>(
                ProcessEventInvocablePayload<TEvent>.Create(@event));

        return queueId.ToString();
    }

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var queueId = _queue
            .QueueInvocableWithPayload<ProcessSmsMessageInvocable, ProcessSmsMessageInvocablePayload>(
                ProcessSmsMessageInvocablePayload.Create(message.Id));

        return queueId.ToString();
    }

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        var queueId = _queue
            .QueueInvocableWithPayload<ProcessSmsMessageInvocable, ProcessSmsMessageInvocablePayload>(
                ProcessSmsMessageInvocablePayload.Create(message.Id, delay));

        return queueId.ToString();
    }
}

/// <summary>
/// partial part for <see cref="CoravelQueueManager"/>
/// </summary>
public partial class CoravelQueueManager
{
    private readonly IQueue _queue;

    public CoravelQueueManager(IQueue queue)
    {
        this._queue = queue;
    }
}
