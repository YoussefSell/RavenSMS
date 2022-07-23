namespace RavenSMS.Internal.Queues.InMemory;

/// <summary>
/// interface to identify the InMemory queue in DI
/// </summary>
public interface IInMemoryQueue
{
    /// <summary>
    /// put the message in the queue to be processed
    /// </summary>
    /// <param name="messageId">the messageId</param>
    /// <param name="delay">the delay if any</param>
    /// <returns>the queue job id</returns>
    string EnqueueMessage(string messageId, TimeSpan? delay = null);

    /// <summary>
    /// put the event in the queue to be processed
    /// </summary>
    /// <typeparam name="TEvent">the event type</typeparam>
    /// <param name="event">the event instance</param>
    /// <returns>the queue job id</returns>
    string EnqueueEvent<TEvent>(TEvent @event) where TEvent : IEvent;
}