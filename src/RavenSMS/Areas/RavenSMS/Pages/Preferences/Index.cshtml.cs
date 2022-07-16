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
        RavenSmsDeliveryChannelOptions options,
        IStringLocalizer<PreferencesIndexPageModel> localizer,
        ILogger<PreferencesIndexPageModel> logger)
        : base(options, localizer, logger)
    {
        _manager = ravenSmsManager;
    }
}
