namespace RavenSMS.Queues.Hangfire;

/// <summary>
/// the queue manager implementation using Hangfire
/// </summary>
public partial class HangfireQueueManager : IQueueManager
{
    /// <inheritdoc/>
    public string QueueEvent<TEvent>(TEvent @event) where TEvent : IEvent 
        => BackgroundJob.Enqueue<EventsPublisher>(publisher => publisher.ProcessAsync(@event));

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message) 
        => BackgroundJob.Enqueue<IRavenSmsManager>(manager => manager.ProcessAsync(message.Id, default));

    /// <inheritdoc/>
    public string QueueMessage(RavenSmsMessage message, TimeSpan delay) 
        => BackgroundJob.Schedule<IRavenSmsManager>(manager => manager.ProcessAsync(message.Id, default), delay);
}