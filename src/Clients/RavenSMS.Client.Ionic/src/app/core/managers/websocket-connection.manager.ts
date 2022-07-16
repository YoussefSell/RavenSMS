import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { DisconnectionReason, MessageStatus } from '../constants/enums';
import { IClientInfoUpdatedEventData, IMessages } from '../models';

/**
 * server for managing websocket connection using signalR
 */
export class WebsocketConnectionManager {

    private serverSuffix = "RavenSMS/Hub";
    private hubConnection: HubConnection;

    /**
     * check if the hub is connected to the server
     * @returns true if connected false if not
     */
    public isConnected(): boolean {
        return this.hubConnection ? this.hubConnection.state === HubConnectionState.Connected : false;
    }

    /**
     * init the server connection
     * @param serverUrl the ravenSMS signalR hub server url
     * @param clientId the client id
     * @returns a promise that resolve to void
     */
    public initConnection(serverUrl: string, clientId: string): Promise<void> {
        // 1- build the connection hub
        this.buildHubConnection(serverUrl, clientId);

        // 2- start the connection
        return this.connectAsync();
    }

    /**
     * build the instance of the HubConnection
     * @param serverUrl the ravenSMS server url
     */
    public buildHubConnection(serverUrl: string, clientId: string): void {
        if (serverUrl == null || serverUrl == undefined) {
            throw new Error('the server url is not specified');
        }

        // build the server url
        let url = serverUrl.endsWith('/')
            ? serverUrl + this.serverSuffix
            : serverUrl + '/' + this.serverSuffix;

        // attach the clientId asa queryString
        url += `?clientId=${clientId}`

        // build the connection hub
        this.hubConnection = new HubConnectionBuilder()
            .withAutomaticReconnect([
                100, 200, 400, 800,
                1000, 1200, 1400, 1800,
                2000, 2200, 2400, 2800,
                3000, 3200, 3400, 3800,
            ])
            .withUrl(url)
            .build();
    }

    /**
     * start the server connection
     */
    public connectAsync(): Promise<void> {
        return this.hubConnection.start();
    }

    /** 
     * Stops the connection.
     * @returns {Promise<void>} A Promise that resolves when the connection has been successfully terminated, or rejects with an error.
     */
    public stopAsync(): Promise<void> {
        return this.hubConnection.stop();
    }

    /** 
     * Registers a handler that will be invoked when the connection is closed.
     * @param {Function} callback The handler that will be invoked when the connection is closed. Optionally receives a single argument containing the error that caused the connection to close (if any).
     */
    public onclose(callback: (error?: Error) => void): void {
        this.hubConnection.onclose(callback);
    }

    /** 
     * Registers a handler that will be invoked when the connection successfully reconnects.
     * @param {Function} callback The handler that will be invoked when the connection successfully reconnects.
     */
    public onreconnected(callback: (connectionId?: string) => void): void {
        this.hubConnection.onreconnected(callback);
    }

    /** 
     * Registers a handler that will be invoked when the connection starts reconnecting.
     * @param {Function} callback The handler that will be invoked when the connection starts reconnecting. Optionally receives a single argument containing the error that caused the connection to start reconnecting (if any).
     */
    public onreconnecting(callback: (error?: Error) => void): void {
        this.hubConnection.onreconnecting(callback);
    }

    /**
     * register an event handler to handle send message requests
     * @param handler the handler to be executed when the event is triggered
     */
    public onSendMessageEvent(handler: (message: IMessages) => void): void {
        if (this.hubConnection) {
            this.hubConnection.on('sendSmsMessage', handler);
        }
    }

    /**
    * register an event handler to handle the client info updated request
    * @param handler the handler to be executed when the event is triggered
    */
    public onClientInfoUpdatedEvent(handler: (clientInfo: IClientInfoUpdatedEventData) => void): void {
        if (this.hubConnection) {
            this.hubConnection.on('updateClientInfo', handler);
        }
    }

    /**
    * register an event handler to handle disconnection event
    * @param handler the handler to be executed when the event is triggered
    */
    public onForceDisconnectionEvent(handler: (reason: DisconnectionReason) => void): void {
        if (this.hubConnection) {
            this.hubConnection.on('forceDisconnect', handler);
        }
    }

    /**
     * register an event handler to handle message deletion
     * @param handler the handler to be executed when the event is triggered
     */
    public onMessageDeletedEvent(handler: (messageId: string) => void): void {
        if (this.hubConnection) {
            this.hubConnection.on('onMessageDeleted', handler);
        }
    }

    /**
    * register an event handler to handle client connected event
    * @param handler the handler to be executed when the event is triggered
    */
    public onClientConnectedEvent(handler: () => void): void {
        if (this.hubConnection) {
            this.hubConnection.on('clientConnected', handler);
        }
    }

    /**
    * get the list of messages sent by the client
    * @param handler the handler to be executed when the event is triggered
    */
    public onReadClientSentMessagesEvent(handler: (messages: IMessages[]) => void): void {
        if (this.hubConnection) {
            this.hubConnection.on('ReadClientSentMessagesAsync', handler);
        }
    }

    /**
     * send an event to update the message status
     * @param messageId the id of the message to be updated
     * @param status the new status
     * @param error error if any
     */
    public async sendUpdateMessageStatusEventAsync(messageId: string, status: MessageStatus, error: string) {
        if (this.hubConnection.state == HubConnectionState.Connected) {
            await this.hubConnection.send('UpdateMessageStatusAsync', messageId, status, error);
        }
    }

    /**
     * send the command to persist the connection id for this client
     * @param clientId the id of the client app
     */
    public async sendPersistClientConnectionEventAsync(clientId: string): Promise<void> {
        await this.hubConnection.send('PersistClientConnectionAsync', clientId, true);
    }

    /**
     * load the messages sent by this client.
     * @param clientId the id of this client instance
     */
    public async sendLoadClientMessagesEventAsync(clientId: string) {
        if (this.hubConnection) {
            await this.hubConnection.send('LoadClientMessagesAsync', clientId);
        }
    }
}
