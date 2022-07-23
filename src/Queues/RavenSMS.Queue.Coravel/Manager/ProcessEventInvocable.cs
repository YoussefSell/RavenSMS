namespace RavenSMS.Queues.Coravel;

/// <summary>
/// the Invocable for processing RavenSMS messages
/// </summary>
public class ProcessEventInvocable<TEvent> : IInvocable, IInvocableWithPayload<ProcessEventInvocablePayload<TEvent>>
    where TEvent : IEvent
{
    private readonly EventsPublisher _publisher;

    public ProcessEventInvocable(EventsPublisher publisher)
    {
        _publisher = publisher;
    }

    /// <inheritdoc/>
    public ProcessEventInvocablePayload<TEvent> Payload { get; set; } = default!;

    /// <inheritdoc/>
    public Task Invoke()
    {
        if (Payload is null)
            throw new RavenSmsException("the payload is null");

        return ProcessAsync();
    }

    private async Task ProcessAsync()
    {
        if (ProcessEventInvocableSingleton.ProcessAsyncMethodInfo is null)
            return;

        var genericMethod = ProcessEventInvocableSingleton.ProcessAsyncMethodInfo.MakeGenericMethod(typeof(TEvent));
        if (genericMethod is null)
            return;

        var invocation = genericMethod.Invoke(_publisher, new[] { Payload.Event });
        if (invocation is Task task)
        {
            await task.ConfigureAwait(false);
        }
    }
}

/// <summary>
/// the invocable payload
/// </summary>
public class ProcessEventInvocablePayload<TEvent>
    where TEvent : IEvent
{
    public ProcessEventInvocablePayload(TEvent @event)
        => Event = @event;

    /// <summary>
    /// the event
    /// </summary>
    public IEvent Event { get; }

    internal static ProcessEventInvocablePayload<TEvent> Create(TEvent @event)
        => new(@event);
}

internal static class ProcessEventInvocableSingleton
{
    internal static readonly System.Reflection.MethodInfo? ProcessAsyncMethodInfo = typeof(EventsPublisher).GetMethod(nameof(EventsPublisher.ProcessAsync));
}