<img width="100" height="100" alt="RavenSMS" src="https://github.com/YoussefSell/RavenSMS/blob/master/assets/logo.png">

# RavenSMS - SMS delivery channel

RavenSMS is a custom developed channel that utilizes our phones to send sms messages without the need for buying a subscription from services like Twilio and MessageBird etc.

the idea behind RavenSMS has raised when i found that most SMS delivery channels don't offer a great testing experience. the testing budgets is very low on most of the services, Twilio for example gives you only 15$ to be used for testing which i don't find enough. also the sending limitation, most services restrict you to send SMS messages to only a specific test phone number.

so i thought, i have a phone subscription with unlimited SMS messages to send, why not use my phone to send the messages. and RavenSMS has born.

## How it works?

there are two main component in the RavenSMS architecture:

- server: responsible for managing and sending messages, and broadcasting events to the clients.
- client: is the receiver of the events, which is your phone with RavenSMS app installed on it.

when you send a message from RavenSMS server the message will be queued for immediate delivery or after a delay. than when it time to send the message a websocket command is sent to the client (your phone) using SignalR with the message details and the message will be sent.

![ravensms demo](https://github.com/YoussefSell/RavenSMS/blob/master/assets/screenshots/ravensms-demo.gif)

## Getting started

first you need to download the latest version of the APP into your Android phone, you can find the APK file in the [releases page](https://github.com/YoussefSell/RavenSMS/releases)

then we need to configure the server, in your ASP core project install RavenSMS `Install-Package RavenSMS`.  
NOTE: RavenSMS is built on .NET 6, so your project needs to be at least targeting .NET 6 or above.

after installing the package add RavenSMS in the services configuration
```csharp
builder.Services
    .AddRavenSMS(config =>
    {
        config.UseInMemoryQueue();
        config.UseInMemoryStores();
    });
```
and in your app pipline endpoints configuration add RavenSMS Hub
```csharp
app.MapRavenSmsHub();

```
a complet example of the configuration can be found on the [sample app](https://github.com/YoussefSell/RavenSMS/blob/master/samples/RavenSMS.Samples.AspCore/Program.cs).

and that it, now you can run the app and navigate to `/RavenSMS`.  

now lets connect your phone with the server, (you can follow the demo above)
1. first navigate to `/RavenSMS/Clients`, here you will see a client already added for you by default.
2. click on *Setup* action.
3. you will be redirected to the client setup page and a QR code will be presented to you.
4. open the app on you phone and on the Servers page click on the + button
5. now click on *START SCANNING* (the camera permission is required), and scan the QR code.
6. done, your phone should now be connected to your server.

Note: when you run your ASP core project it will be served on localhost, which is not accessible to the internet it is only on your machine. in order to be able to connect your phone to a localhost server, you need to use a reverse proxy like [Ngrok](https://ngrok.com/).

## RavenSMS Dashboard

![ravensms dashboard screenshot](https://github.com/YoussefSell/RavenSMS/blob/master/assets/screenshots/ravensms-dashboard.png)

the dashboard allows you to control and monitor your connected clients and have statistics on the sent messages. and also you can monitor the sending status of messages so that if one fails you can resend it.

## RavenSMS In Code

RavenSMS provide you with a service called `IRavenSmsService` that you can inject into your code and use to send SMS messages

```csharp
IRavenSmsService service;

var message = new Message("this is a test message :)",
    from: "00**********",
    to: "00**********");
    
await service.SendAsync(message);
```

## RavenSMS Events

in some cases, we need to perform actions based on events that happen in our system, for example when a client is disconnected we need to get notified so that we can reconnect the client again.

RavenSms allows you to perform this types of actions using EventHandlers, it exposes a number of internal events that you can subscribe to them and perform the actions accordingly.

```csharp
// 1. create your event handler
public class ClientDisconnectedEventHandler : IEventHandler<ClientDisconnectedEvent>
{
    private readonly ILogger _logger;

    public ClientDisconnectedEventHandler(ILogger<ClientDisconnectedEventHandler> logger)
        => _logger = logger;

    public Task HandleAsync(ClientDisconnectedEvent @event)
    {
        _logger.LogInformation("client disconnected {@info}", new { @event.ClientId, @event.ConnectionId });
        return Task.CompletedTask;
    }
}

// 2. register the event hanlder 
builder.Services
    .AddRavenSMS(config =>
    {
        config.RegisterEventHandler<ClientDisconnectedEvent, ClientDisconnectedEventHandler>();
        
        // code ...
    });

```

a complete documentation can be found on the [Wiki page](https://github.com/YoussefSell/RavenSMS/wiki).
