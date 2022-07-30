namespace RavenSMS.Events;

/// <summary>
/// the events broadcaster
/// </summary>
public class EventsPublisher
{
    private readonly IQueueManager _queueManager;
    private readonly ILogger<EventsPublisher> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public EventsPublisher(
        IQueueManager queueManager,
        ILogger<EventsPublisher> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _queueManager = queueManager;
        _scopeFactory = scopeFactory;
    }

    /// <summary>
    /// publish this event for processing
    /// </summary>
    /// <typeparam name="TEvent">the event type</typeparam>
    /// <param name="event">the event instance</param>
    public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        => _queueManager.QueueEvent(@event);

    /// <summary>
    /// process the given event
    /// </summary>
    /// <typeparam name="TEvent">the event type</typeparam>
    /// <param name="event">the event instance</param>
    public async Task ProcessAsync<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        using var scope = _scopeFactory.CreateScope();

        var handlers = scope.ServiceProvider.GetServices<IEventHandler<TEvent>>();
        if (handlers.Any())
        {
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(@event);
            }
        }
    }
}
