namespace RavenSMS;

/// <summary>
/// the sms message
/// </summary>
public struct Message
{
    public Message(string message, string from, string to, Priority priority = Priority.Normal)
    {
        if (string.IsNullOrEmpty(from))
            throw new ArgumentException($"'{nameof(from)}' cannot be null or empty.", nameof(from));

        if (string.IsNullOrEmpty(to))
            throw new ArgumentException($"'{nameof(to)}' cannot be null or empty.", nameof(to));
        
        To = to;
        From = from;
        Body = message;
        Priority = priority;
    }

    /// <summary>
    /// Gets or sets the phone number of the sender.
    /// </summary>
    public string From { get; }

    /// <summary>
    /// Get or set the phone numbers of recipients to send the SMS message to.
    /// </summary>
    public string To { get; set; } = default!;

    /// <summary>
    /// Gets or sets the message body.
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// Gets or sets the priority of this e-mail message.
    /// </summary>
    public Priority Priority { get; }
}