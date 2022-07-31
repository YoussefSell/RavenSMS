namespace RavenSMS.Managers;

/// <summary>
/// the SMS Queue manager
/// </summary>
public interface IQueueManager
{
    /// <summary>
    /// queue the message for processing.
    /// </summary>
    /// <param name="message">the message to queue</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the if the background job Id generated the by the underlying queue manager.
    ///  </returns>
    /// <exception cref="OperationCanceledException">If the System.Threading.CancellationToken is canceled.</exception>
    string QueueMessage(RavenSmsMessage message);

    /// <summary>
    /// queue the message for processing with a delay
    /// </summary>
    /// <param name="message">the message to queue.</param>
    /// <param name="delay">the delay to use before sending the message.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the if the background job Id generated the by the underlying queue manager.
    ///  </returns>
    /// <exception cref="OperationCanceledException">If the System.Threading.CancellationToken is canceled.</exception>
    string QueueMessage(RavenSmsMessage message, TimeSpan delay);

    /// <summary>
    /// queue the event for processing
    /// </summary>
    /// <typeparam name="TEvent">the event type</typeparam>
    /// <param name="event">the event instance</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the if the background job Id generated the by the underlying queue manager.
    ///  </returns>
    /// <exception cref="OperationCanceledException">If the System.Threading.CancellationToken is canceled.</exception>
    string QueueEvent<TEvent>(TEvent @event) where TEvent : IEvent;
}