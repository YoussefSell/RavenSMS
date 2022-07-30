namespace RavenSMS.Internal.Queues.InMemory;

/// <summary>
/// the in memory queue implementation of the <see cref="IQueueManager"/>
/// </summary>
public partial class InMemoryQueueManager : IQueueManager
{
    private readonly IInMemoryQueue _queue;

    public InMemoryQueueManager(IInMemoryQueue queue)
        => _queue = queue;

    /// <inheritdoc/>
    public string QueueEvent<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
        => _queue.EnqueueEvent(@event);

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, CancellationToken cancellationToken = default) 
        => _queue.EnqueueMessage(message.Id);

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, TimeSpan delay, CancellationToken cancellationToken = default)
        => _queue.EnqueueMessage(message.Id, delay);
}
