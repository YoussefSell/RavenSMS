﻿namespace RavenSMS.Internal.Managers;

/// <summary>
/// the websocket hub for managing the connection between the server and the client
/// </summary>
public class RavenSmsWebSocketManager : Hub
{
    private readonly RavenSmsOptions _options;
    private readonly IRavenSmsClientsManager _clientsManager;
    private readonly IRavenSmsMessagesManager _messagesManager;

    public RavenSmsWebSocketManager(
        IRavenSmsClientsManager manager,
        IOptions<RavenSmsOptions> options,
        IRavenSmsMessagesManager messagesManager)
    {
        _options = options.Value;
        _clientsManager = manager;
        _messagesManager = messagesManager;
    }

    public override async Task OnConnectedAsync()
    {
        // send an event to the client app to indicate that the connection has been established
        // because we don't have a way to get this info from the client app
        await Clients.Caller.SendAsync("ClientConnected");
    }

    public override async Task OnDisconnectedAsync(Exception? exception) 
        => await _clientsManager.ClientDisconnectedAsync(Context.ConnectionId);

    public async Task PersistClientConnectionAsync(string clientId, bool forceConnection)
    {
        // get the client associated with the given id
        var client = await _clientsManager.FindClientByIdAsync(clientId);
        if (client is null)
        {
            await Clients.Caller.SendAsync("forceDisconnect", DisconnectionReason.ClientNotFound);
            return;
        }

        // check if the client already connected
        if (client.Status == RavenSmsClientStatus.Connected)
        {
            // check if the client is already connected
            if (client.ConnectionId == Context.ConnectionId)
                return;

            // new connection, check if we need to force the connection, or not
            if (!forceConnection)
            {
                await Clients.Caller.SendAsync("forceDisconnect", DisconnectionReason.ClientAlreadyConnected);
                return;
            }
        }

        // attach the client to the current connection
        await _clientsManager.ClientConnectedAsync(client, Context.ConnectionId);

        // send the command to update the client info
        await Clients.Caller.SendAsync("updateClientInfo", new
        {
            clientId = client.Id,
            clientName = client.Name,
            clientDescription = client.Description,
            clientPhoneNumber = client.PhoneNumber,
            serverId = _options.ServerId,
            serverName = _options.ServerName,
        });
    }

    public async Task LoadClientMessagesAsync(string clientId)
    {
        // get the client associated with the given id
        var client = await _clientsManager.FindClientByIdAsync(clientId);
        if (client is null)
        {
            await Clients.Caller.SendAsync("forceDisconnect", DisconnectionReason.ClientNotFound);
            return;
        }

        // get the list of messages
        var messages = await _messagesManager.GetAllMessagesAsync(clientId);

        // send the event to read the client messages
        await Clients.Caller.SendAsync("ReadClientSentMessagesAsync", messages
            .Select(message => new
            {
                id = message.Id,
                content = message.Body,
                sentOn = message.SentOn,
                status = message.Status,
                from = client.PhoneNumber,
                to = message.To.ToString(),
                createdOn = message.CreateOn,
                deliverAt = message.DeliverAt,
                serverId = _options.ServerId,
            }));
    }

    public Task UpdateMessageStatusAsync(string messageId, RavenSmsMessageStatus status, string error)
        => _messagesManager.UpdateMessageDeliveryStatusAsync(messageId, status, error);
}

public static class RavenSmsHubExtensions
{
    public static async Task<Result> UpdateClientInfosync(this IHubContext<RavenSmsWebSocketManager> hub, RavenSmsClient client, CancellationToken cancellationToken = default)
    {
        try
        {
            if (client.Status != RavenSmsClientStatus.Connected)
                return Result.Failure()
                    .WithMessage("the client is not connected")
                    .WithCode("client_disconnected");

            if (client.ConnectionId is null)
                return Result.Failure()
                    .WithMessage("the client connection id is null or empty")
                    .WithCode("invalid_client_connection_id");

            await hub.Clients.Client(client.ConnectionId).SendAsync("updateClientInfo", new
            {
                clientId = client.Id,
                clientName = client.Name,
                clientDescription = client.Description,
                clientPhoneNumber = client.PhoneNumber,
            }, cancellationToken: cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure()
                .WithError(ex);
        }
    }

    public static async Task<Result> SendSmsMessageAsync(this IHubContext<RavenSmsWebSocketManager> hub, RavenSmsClient client, RavenSmsMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            if (client.Status != RavenSmsClientStatus.Connected)
                return Result.Failure()
                    .WithMessage("the client is not connected")
                    .WithCode("client_disconnected");

            if (client.ConnectionId is null)
                return Result.Failure()
                    .WithMessage("the client connection id is null or empty")
                    .WithCode("invalid_client_connection_id");

            await hub.Clients.Client(client.ConnectionId).SendAsync("sendSmsMessage", new
            {
                from = client.PhoneNumber,
                createdOn = message.CreateOn,
                deliverAt = message.DeliverAt,
                to = message.To.ToString(),
                sentOn = message.SentOn,
                status = message.Status,
                content = message.Body,
                id = message.Id,

            }, cancellationToken: cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure()
                .WithError(ex);
        }
    }

    public static async Task<Result> ForceDisconnectAsync(this IHubContext<RavenSmsWebSocketManager> hub, RavenSmsClient client, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            if (client.Status != RavenSmsClientStatus.Connected)
                return Result.Failure()
                    .WithMessage("the client is not connected")
                    .WithCode("client_disconnected");

            if (client.ConnectionId is null)
                return Result.Failure()
                    .WithMessage("the client connection id is null or empty")
                    .WithCode("invalid_client_connection_id");

            await hub.Clients.Client(client.ConnectionId)
                .SendAsync("forceDisconnect", reason, cancellationToken: cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure()
                .WithError(ex);
        }
    }
}