﻿namespace RavenSMS.Pages;

/// <summary>
/// the Messages index pages
/// </summary>
public partial class ClientsPreviewPage
{
    /// <summary>
    /// Get or set the message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// the client qr code test
    /// </summary>
    public string? QrCodeText { get; set; } = default!;

    /// <summary>
    /// Get or set the client to setup.
    /// </summary>
    [BindProperty]
    public ClientsUpdatePageModelInput Input { get; set; }

    /// <summary>
    /// the page model input
    /// </summary>
    public class ClientsUpdatePageModelInput
    {
        /// <summary>
        /// the id of the clients
        /// </summary>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Get or set for the client.
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Get or set a description for the client.
        /// </summary>
        [MaxLength(300)]
        public string? Description { get; set; } = default!;

        /// <summary>
        /// the phone numbers associated with this client
        /// </summary>
        [
            Required,
            RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,8}$"),
        ]
        public string PhoneNumber { get; set; } = default!;
    }
}

/// <summary>
/// partial part for <see cref="ClientsPreviewPage"/>
/// </summary>
public partial class ClientsPreviewPage
{
    public async Task<IActionResult> OnGetAsync(string? id)
    {
        if (string.IsNullOrEmpty(id))
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });

        var client = await _manager.FindClientByIdAsync(id);
        if (client is null)
        {
            Message = $"Couldn't find a client with the Id: {id}";
            return Page();
        }

        BuildInputModel(client);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateClientAsync([FromRoute] string? id)
    {
        // if the id is null redirect to the clients list to select a client
        if (string.IsNullOrEmpty(id))
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });

        // get the client by the id, if not exist, redirect to the clients list to select an existed client
        var clientInDatabase = await _manager.FindClientByIdAsync(id);
        if (clientInDatabase is null)
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });

        if (!ModelState.IsValid)
        {
            BuildInputModel(clientInDatabase);
            return Page();
        }

        if (!clientInDatabase.PhoneNumber.Equals(Input.PhoneNumber) && await _manager.AnyClientByPhoneNumberAsync(Input.PhoneNumber))
        {
            ModelState.AddModelError("", "the given phone number already used by another client app.");
            BuildInputModel(clientInDatabase);
            return Page();
        }

        clientInDatabase.Name = Input.Name;
        clientInDatabase.PhoneNumber = Input.PhoneNumber;
        clientInDatabase.Description = Input.Description ?? string.Empty;

        var updateResult = await _manager.SaveClientAsync(clientInDatabase);
        if (updateResult.IsSuccess())
        {
            // instruct the client app to update the client info
            await _hubContext.UpdateClientInfosync(clientInDatabase);
        }

        BuildInputModel(clientInDatabase);
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteClient([FromRoute] string? id)
    {
        // if the id is null redirect to the clients list to select a client
        if (string.IsNullOrEmpty(id))
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });

        // get the client by the id, if not exist, redirect to the clients list to select an existed client
        var clientInDatabase = await _manager.FindClientByIdAsync(id);
        if (clientInDatabase is null)
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });

        // delete the client
        var deleteResult = await _manager.DeleteClientAsync(clientInDatabase.Id);
        if (deleteResult.IsSuccess())
        {
            // force the client to disconnect
            await _hubContext.ForceDisconnectAsync(clientInDatabase, DisconnectionReason.ClientDeleted);

            // redirect to the clients list page
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });
        }

        BuildInputModel(clientInDatabase);
        return Page();
    }

    public async Task<IActionResult> OnPostDisconnectClient([FromRoute] string? id)
    {
        // if the id is null redirect to the clients list to select a client
        if (string.IsNullOrEmpty(id))
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });

        // get the client by the id, if not exist, redirect to the clients list to select an existed client
        var clientInDatabase = await _manager.FindClientByIdAsync(id);
        if (clientInDatabase is null)
            return RedirectToPage("/Clients/index", new { area = "RavenSMS" });

        // disconnect the client
        var disconnectionResult = await _hubContext.ForceDisconnectAsync(clientInDatabase, DisconnectionReason.Disconnect);
        if (disconnectionResult.IsSuccess())
        {
            clientInDatabase.Status = RavenSmsClientStatus.Disconnected;
            await _manager.SaveClientAsync(clientInDatabase);
        }

        BuildInputModel(clientInDatabase);
        return Page();
    }

    public async Task<JsonResult> OnGetPhoneNumberExistAsync([FromQuery(Name = "Input.string")] string phoneNumber)
    {
        // return json result instance
        return new JsonResult(!await _manager.AnyClientByPhoneNumberAsync(phoneNumber));
    }
}

/// <summary>
/// partial part for <see cref="ClientsPreviewPage"/>
/// </summary>
public partial class ClientsPreviewPage : BasePageModel
{
    private readonly IRavenSmsClientsManager _manager;
    private readonly IHubContext<RavenSmsHub> _hubContext;

    public ClientsPreviewPage(
        IHubContext<RavenSmsHub> hubContext,
        IRavenSmsClientsManager ravenSmsManager,
        RavenSmsOptions options,
        IStringLocalizer<ClientsPreviewPage> localizer,
        ILogger<ClientsPreviewPage> logger)
        : base(options, localizer, logger)
    {
        _hubContext = hubContext;
        _manager = ravenSmsManager;
        Input = new ClientsUpdatePageModelInput();
    }

    private void BuildInputModel(RavenSmsClient client)
    {
        Input = new ClientsUpdatePageModelInput
        {
            Name = client.Name,
            ClientId = client.Id,
            Description = client.Description,
            PhoneNumber = client.PhoneNumber,
        };

        QrCodeText = BuildClientQrCodeContent(client);
    }
}
