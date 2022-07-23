namespace RavenSMS.Queues.Hangfire;

/// <summary>
/// the queue manager implementation using Hangfire
/// </summary>
public partial class HangfireQueueManager : IQueueManager
{
    /// <inheritdoc/>
    public string QueueEvent<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        return BackgroundJob.Enqueue<EventsPublisher>(publisher => publisher.ProcessAsync(@event));
    }

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        return BackgroundJob.Enqueue<IRavenSmsManager>(manager => manager.ProcessAsync(message.Id, default));
    }

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        return BackgroundJob.Schedule<IRavenSmsManager>(manager => manager.ProcessAsync(message.Id, default), delay);
    }
}