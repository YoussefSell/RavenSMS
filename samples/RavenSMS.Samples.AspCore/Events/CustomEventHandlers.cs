using RavenSMS.Managers;

namespace RavenSMS.Events;

public class ClientConnectedEventHandler : IEventHandler<ClientConnectedEvent>
{
    private readonly ILogger<ClientConnectedEventHandler> _logger;

    public ClientConnectedEventHandler(
        ILogger<ClientConnectedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ClientConnectedEvent @event)
    {
        _logger.LogInformation("client connected {@info}", new { @event.ClientId, @event.ConnectionId });
        return Task.CompletedTask;
    }
}

public class ClientDisconnectedEventHandler : IEventHandler<ClientDisconnectedEvent>
{
    private readonly ILogger<ClientDisconnectedEventHandler> _logger;

    public ClientDisconnectedEventHandler(
        ILogger<ClientDisconnectedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ClientDisconnectedEvent @event)
    {
        _logger.LogInformation("client disconnected {@info}", new { @event.ClientId, @event.ConnectionId });
        return Task.CompletedTask;
    }
}

public class MessageEventHandler :
    IEventHandler<MessageSentEvent>,
    IEventHandler<MessageUnsentEvent>
{
    private readonly IRavenSmsMessagesManager _manager;
    private readonly ILogger<MessageEventHandler> _logger;

    public MessageEventHandler(
        IRavenSmsMessagesManager manager,
        ILogger<MessageEventHandler> logger)
    {
        _manager = manager;
        _logger = logger;
    }

    public Task HandleAsync(MessageSentEvent @event)
    {
        _logger.LogInformation("message sent {@info}", new { @event.MessageId });
        return Task.CompletedTask;
    }

    public async Task HandleAsync(MessageUnsentEvent @event)
    {
        var message = await _manager.FindByIdAsync(@event.MessageId);
        if (message is not null)
        {
            _logger.LogInformation("failed to send message {@info}", new { message.Id });
        }
    }
}