namespace RavenSMS.Events;

/// <summary>
/// defines a class that holds the logic for handling an event
/// </summary>
/// <typeparam name="TEvent">the type of event</typeparam>
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    /// <summary>
    /// handle the event
    /// </summary>
    /// <param name="event">the event instance</param>
    Task HandleAsync(TEvent @event);
}
