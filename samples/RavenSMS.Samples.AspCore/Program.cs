var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSignalR();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddDbContext<ApplicationDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("capacitor-cors",
        builder => builder.WithOrigins(
            "http://localhost",
            "ionic://localhost",
            "capacitor://localhost",
            "http://localhost:8100",
            "http://192.168.1.99:8100",
            "http://192.168.1.108:8100",
            "http://192.168.1.102:8100"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMariaDb(builder.Configuration.GetConnectionString("hangfire")));

// Add the processing server as IHostedService
builder.Services.AddHangfireServer();
builder.Services.AddQueue();

// add RavenSMS services
builder.Services
    .AddRavenSMS(config =>
    {
        config.UseDashboard();

        config.UseInMemoryQueue();
        config.UseInMemoryStores();
        
        config.RegisterEventHandler<MessageSentEvent, MessageEventHandler>();
        config.RegisterEventHandler<MessageUnsentEvent, MessageEventHandler>();
        config.RegisterEventHandler<ClientConnectedEvent, ClientConnectedEventHandler>();
        config.RegisterEventHandler<ClientDisconnectedEvent, ClientDisconnectedEventHandler>();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors("capacitor-cors");

app.UseAuthorization();

app.MapRazorPages();
app.MapHangfireDashboard();

app.MapRavenSmsHub();

app.Run();
