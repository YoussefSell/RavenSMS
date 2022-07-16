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

