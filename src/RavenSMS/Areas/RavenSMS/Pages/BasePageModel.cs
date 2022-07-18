namespace RavenSMS.Pages;

/// <summary>
/// the base page model
/// </summary>
public class BasePageModel : PageModel
{
    protected readonly RavenSmsOptions _options;
    protected readonly IStringLocalizer _localizer;
    protected readonly ILogger _logger;

    public BasePageModel(
        ILogger logger,
        IStringLocalizer localizer,
        IOptions<RavenSmsOptions> options)
    {
        _logger = logger;
        _localizer = localizer;
        _options = options.Value;
    }

    /// <summary>
    /// the status message a TempData used to share alert messages between pages
    /// </summary>
    [TempData]
    public string? StatusMessage { get; set; }

    /// <summary>
    /// Get the RavenSMS server info.
    /// </summary>
    public RavenSmsOptions RavenSmsServerInfo => _options;

    /// <summary>
    /// the server url
    /// </summary>
    public string ServerUrl => $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

    /// <summary>
    /// translate the given name string, using the provided translation implantation
    /// </summary>
    /// <param name="name">the name to localize</param>
    /// <param name="arguments">the args if any</param>
    /// <returns>the translated text</returns>
    public virtual string Localize(string name, params object[] arguments)
    {
        var translation = _localizer[name, arguments];
        if (translation is not null && !translation.ResourceNotFound)
            return translation.Value;

        return name;
    }

    protected string BuildClientQrCodeContent(RavenSmsClient client)
    {
        // build the json model
        var jsonModel = System.Text.Json.JsonSerializer.Serialize(new
        {
            clientId = client.Id,
            serverUrl = ServerUrl,
            serverId = _options.ServerId,
        });

        // convert the json model to a base64 string
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(jsonModel));
    }
}
