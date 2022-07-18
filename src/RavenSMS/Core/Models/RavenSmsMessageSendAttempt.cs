﻿namespace RavenSMS.Models;

/// <summary>
/// this class used to hold info about a send attempt of a sms message
/// </summary>
public class RavenSmsMessageSendAttempt
{
    public RavenSmsMessageSendAttempt()
    {
        Date = DateTimeOffset.UtcNow;
        Id = Generator.GenerateUniqueId("msa");
        Errors = new List<RavenSmsMessageSendAttemptError>();
    }
    
    /// <summary>
    /// Get or set the id of the message attempt.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// the date of the attempt
    /// </summary>
    public DateTimeOffset Date { get; set; }

    /// <summary>
    /// Get or set the status of the message
    /// </summary>
    public SendAttemptStatus Status { get; set; }

    /// <summary>
    /// the list of errors associated with this attempt if any
    /// </summary>
    public ICollection<RavenSmsMessageSendAttemptError> Errors { get; set; }

    /// <summary>
    /// the id of the message associated with this attempt
    /// </summary>
    public string MessageId { get; set; } = default!;

    /// <summary>
    /// add a new error to the list of errors
    /// </summary>
    /// <param name="code">the error code</param>
    /// <param name="message">the error message</param>
    public void AddError(string code, string message)
    {
        Errors.Add(new RavenSmsMessageSendAttemptError
        {
            Code = code,
            Message = message
        });
    }
}
