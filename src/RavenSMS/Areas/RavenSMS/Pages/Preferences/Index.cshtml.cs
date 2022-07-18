namespace RavenSMS.Pages;

/// <summary>
/// the Preferences index pages
/// </summary>
public partial class PreferencesIndexPageModel
{
}

/// <summary>
/// partial part for <see cref="PreferencesIndexPageModel"/>
/// </summary>
public partial class PreferencesIndexPageModel : BasePageModel
{
    private readonly IRavenSmsManager _manager;

    public PreferencesIndexPageModel(
        IRavenSmsManager ravenSmsManager,
        IOptions<RavenSmsOptions> options,
        ILogger<MessagesAddPageModel> logger,
        IStringLocalizer<MessagesAddPageModel> localizer)
        : base(logger, localizer, options)
    {
        _manager = ravenSmsManager;
    }
}
