namespace RavenSMS;

/// <summary>
/// the RavenSMS SMS delivery channel.
/// </summary>
public partial class RavenSmsService : IRavenSmsService
{
    /// <inheritdoc/>
    public Result Send(Message message)
        => SendAsync(message).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public Result Send(Message message, TimeSpan delay)
        => SendAsync(message, delay).GetAwaiter().GetResult();

    /// <inheritdoc/>
    public async Task<Result> SendAsync(Message message, CancellationToken cancellationToken = default)
        => await SendAsync(message, TimeSpan.Zero, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc/>
    public async Task<Result> SendAsync(Message message, TimeSpan delay, CancellationToken cancellationToken = default)
    {
        try
        {
            // check if there is any client registered with the given "FROM" phone number
            var client = await _clientsManager.FindClientByPhoneNumberAsync(message.From, cancellationToken);
            if (client is null)
            {
                return Result.Failure()
                    .WithErrors(new ResultError(
                        code: "invalid_from",
                        message: "the given sender 'FROM' phone number is not registered with any client app"));
            }

            // create the raven SMS message
            var Message = CreateMessage(message, client.Id);

            // get the delay data if any
            if (delay == TimeSpan.Zero)
            {
                // queue the message for delivery without delay
                return await _ravenSmsManager.QueueMessageAsync(Message, cancellationToken);
            }

            // queue the message for delivery with a delay
            return await _ravenSmsManager.QueueMessageAsync(Message, delay, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Failure()
                .WithErrors(ex);
        }
    }
}

/// <summary>
/// partial part for <see cref="RavenSmsService"/>
/// </summary>
public partial class RavenSmsService
{
    private readonly IRavenSmsManager _ravenSmsManager;
    private readonly IRavenSmsClientsManager _clientsManager;

    /// <summary>
    /// create an instance of <see cref="RavenSmsService"/>
    /// </summary>
    /// <param name="ravenSmsManager">the <see cref="IRavenSmsManager"/> instance</param>
    /// <param name="clientsManager">the <see cref="IRavenSmsClientsManager"/> instance</param>
    /// <param name="options">the channel options instance</param>
    /// <exception cref="ArgumentNullException">if the given provider options is null</exception>
    public RavenSmsService(
        IRavenSmsManager ravenSmsManager,
        IRavenSmsClientsManager clientsManager)
    {
        _clientsManager = clientsManager;
        _ravenSmsManager = ravenSmsManager;
    }

    /// <summary>
    /// create a <see cref="Message"/> instance from the given <see cref="Message"/>.
    /// </summary>
    /// <param name="message">the <see cref="Message"/> instance</param>
    /// <param name="clientId">the clientId this message will be sent with.</param>
    /// <returns>an instance of <see cref="Message"/> class</returns>
    public static RavenSmsMessage CreateMessage(Message message, string clientId)
        => new()
        {
            To = message.To,
            ClientId = clientId,
            Body = message.Body,
            Priority = message.Priority,
        };
}
