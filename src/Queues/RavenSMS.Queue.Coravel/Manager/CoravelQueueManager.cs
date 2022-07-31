namespace RavenSMS.Queues.Coravel;

/// <summary>
/// the queue manager implementation using Coravel
/// </summary>
public partial class CoravelQueueManager : IQueueManager
{
    /// <inheritdoc/>
    public string QueueEvent<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var queueId = _queue
            .QueueInvocableWithPayload<ProcessEventInvocable<TEvent>, ProcessEventInvocablePayload<TEvent>>(
                ProcessEventInvocablePayload<TEvent>.Create(@event));

        return queueId.ToString();
    }

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message)
    {
        var queueId = _queue
            .QueueInvocableWithPayload<ProcessSmsMessageInvocable, ProcessSmsMessageInvocablePayload>(
                ProcessSmsMessageInvocablePayload.Create(message.Id));

        return queueId.ToString();
    }

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, TimeSpan delay)
    {
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
