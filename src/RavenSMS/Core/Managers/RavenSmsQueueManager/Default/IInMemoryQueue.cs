﻿namespace RavenSMS.Queues.InMemory;

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
    /// <returns>the job id</returns>
    string Enqueue(string messageId, TimeSpan? delay = null);
}