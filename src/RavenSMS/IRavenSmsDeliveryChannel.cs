namespace RavenSMS;

/// <summary>
/// the RavenSMS SMS delivery channel
/// </summary>
public interface IRavenSmsService
{
    /// <summary>
    /// Sends the SMS message.
    /// </summary>
    /// <param name="message">the SMS message to be send</param>
    /// <returns>a <see cref="Result"/> to indicate sending result.</returns>
    Result Send(Message message);

    /// <summary>
    /// Sends the SMS message.
    /// </summary>
    /// <param name="message">the SMS message to be send</param>
    /// <param name="delay">a delay to wait before sending the message.</param>
    /// <returns>a <see cref="Result"/> to indicate sending result.</returns>
    Result Send(Message message, TimeSpan delay);

    /// <summary>
    /// Sends the SMS message.
    /// </summary>
    /// <param name="message">the SMS message to be send</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>a <see cref="Result"/> to indicate sending result.</returns>
    /// <exception cref="OperationCanceledException">If the System.Threading.CancellationToken is canceled.</exception>
    Task<Result> SendAsync(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends the SMS message.
    /// </summary>
    /// <param name="message">the SMS message to be send</param>
    /// <param name="delay">a delay to wait before sending the message.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>a <see cref="Result"/> to indicate sending result.</returns>
    /// <exception cref="OperationCanceledException">If the System.Threading.CancellationToken is canceled.</exception>
    Task<Result> SendAsync(Message message, TimeSpan delay, CancellationToken cancellationToken = default);
}
