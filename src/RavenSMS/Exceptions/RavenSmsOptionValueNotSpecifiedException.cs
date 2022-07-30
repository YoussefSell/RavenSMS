namespace RavenSMS.Exceptions;

/// <summary>
/// exception thrown when a RavenSms option is not specified
/// </summary>
[Serializable]
public class RavenSmsOptionValueNotSpecifiedException : RavenSmsException
{
    /// <inheritdoc/>
    public RavenSmsOptionValueNotSpecifiedException(string optionsName, string message) : base(message)
    {
        OptionsName = optionsName;
    }

    /// <inheritdoc/>
    protected RavenSmsOptionValueNotSpecifiedException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
        OptionsName = string.Empty;
    }

    /// <summary>
    /// option parameter name
    /// </summary>
    public string OptionsName { get; }
}
