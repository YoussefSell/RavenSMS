import { DisconnectionReason, MessageStatus, ServerStatus } from "src/app/core/constants/enums";
import { MessagesStoreActions, RootStoreState, ServersStoreActions } from "src/app/store";
import { WebsocketConnectionManager } from "src/app/core/managers";
import { AlertController, ToastController } from "@ionic/angular";
import { IMessages, IServerInfo } from "src/app/core/models";
import { TranslocoService } from "@ngneat/transloco";
import { Injectable } from "@angular/core";
import { SmsService } from "./sms.service";
import { Store } from "@ngrx/store";

/**
 * service for managing servers connectivity
 */
@Injectable({ providedIn: 'root' })
export class ServersConnectivityService {
    private _serversConnectivity: {
        [key: string]: {
            serverInfo: IServerInfo,
            connection: WebsocketConnectionManager
        }
    } = {};

    constructor(
        private _alert: AlertController,
        private _toast: ToastController,
        private _smsService: SmsService,
        private _translation: TranslocoService,
        private _store: Store<RootStoreState.State>,
    ) { }

    /**
     * attach the given service for connectivity monitoring
     * @param servers the servers to attach
     */
    attachServers(servers: readonly IServerInfo[]): void {
        for (const server of servers) {
            if (server) {
                if (this._serversConnectivity[server.id]) {
                    continue;
                }

                this._serversConnectivity[server.id] = {
                    serverInfo: server,
                    connection: new WebsocketConnectionManager(),
                };

                this.setupConnectivityEvents(
                    this._serversConnectivity[server.id].serverInfo,
                    this._serversConnectivity[server.id].connection);
            }
        }

        // handel removed servers
        for (const key in this._serversConnectivity) {
            if (Object.prototype.hasOwnProperty.call(this._serversConnectivity, key)) {
                const server = this._serversConnectivity[key];
                const exist = servers.find(s => s.id == server.serverInfo.id);
                if (exist == null || exist == undefined) {
                    server.connection.stopAsync();
                    delete this._serversConnectivity[key];
                }
            }
        }
    }

    /**
     * try to reconnect the server with the given id
     * @param serveId the id of the serve
     */
    reconnect(serveId: string) {
        var serversConnectivity = this._serversConnectivity[serveId];
        if (serversConnectivity) {
            serversConnectivity.connection.connectAsync();
        }
    }

    private setupConnectivityEvents(serverInfo: IServerInfo, connectivity: WebsocketConnectionManager): void {
        // init the connection
        connectivity
            .initConnection(serverInfo.url, serverInfo.clientInfo.clientId)
            .catch(async () => this._store.dispatch(ServersStoreActions.UpdateServerStatus({
                serverId: serverInfo.id,
                newStatus: ServerStatus.OFFLINE
            })));

        // register the event for on close
        connectivity
            .onclose(async () => this._store.dispatch(ServersStoreActions.UpdateServerStatus({
                serverId: serverInfo.id,
                newStatus: ServerStatus.OFFLINE
            })));

        // register the event for on reconnecting
        connectivity
            .onreconnecting(async () => this._store.dispatch(ServersStoreActions.UpdateServerStatus({
                serverId: serverInfo.id,
                newStatus: ServerStatus.RECONNECTING
            })));

        // register the handler for the force disconnect event
        connectivity
            .onForceDisconnectionEvent(async (reason) => {
                console.log("client forced to disconnect", { reason }, serverInfo);
                switch (reason) {
                    case DisconnectionReason.ClientAlreadyConnected:
                        await this.presentToastAsync(this._translation.translate('messages.client_already_connected'), 5000);
                        this._store.dispatch(ServersStoreActions.DeleteServer({ serverId: serverInfo.id }));
                        break;
                    case DisconnectionReason.ClientNotFound:
                        await this.presentToastAsync(this._translation.translate('messages.Client_Not_Found'), 5000);
                        this._store.dispatch(ServersStoreActions.DeleteServer({ serverId: serverInfo.id }));
                        break;
                    case DisconnectionReason.ClientDeleted:
                        await this.presentToastAsync(this._translation.translate('messages.client_deleted'), 5000);
                        this._store.dispatch(ServersStoreActions.DeleteServer({ serverId: serverInfo.id }));
                        break;
                    case DisconnectionReason.Disconnect:
                        await this.presentToastAsync(this._translation.translate('messages.disconnect_client'), 5000);
                        connectivity.stopAsync();
                        break;
                }
            });

        // register the handler for the force disconnect event
        connectivity
            .onClientConnectedEvent(async () => {
                console.log('client connected', serverInfo);
                await connectivity.sendPersistClientConnectionEventAsync(serverInfo.clientInfo.clientId);
                await connectivity.sendLoadClientMessagesEventAsync(serverInfo.clientInfo.clientId);
                this._store.dispatch(ServersStoreActions.UpdateServerStatus({ serverId: serverInfo.id, newStatus: ServerStatus.ONLINE }));
            });

        // register the handler for the client info updated event
        connectivity
            .onClientInfoUpdatedEvent((eventData) => this._store.dispatch(ServersStoreActions.UpdateServerClientInfo({
                serverId: serverInfo.id,
                data: eventData
            })));

        // register the handler for the client messages retrieval event
        connectivity
            .onReadClientSentMessagesEvent((messages) => this._store.dispatch(MessagesStoreActions.MessagesLoaded({
                serverId: serverInfo.id,
                data: messages
            })));

        // register the handler for the message deletion event
        connectivity
            .onMessageDeletedEvent(async (messageId) => {
                if (serverInfo.clientInfo.clientId) {
                    this._store.dispatch(MessagesStoreActions.DeleteMessagesByIds({ messagesIds: [messageId] }));
                }
            });

        // register the handler for the send message event
        connectivity
            .onSendMessageEvent(async (message: IMessages) => {
                // check for sms permission, if not show an alert to ask the user to allow the permission
                if (!await this._smsService.HasPermissionAsync()) {
                    const alertResult = await this._alert.create({
                        backdropDismiss: false,
                        header: this._translation.translate('messages.sms_permission.header'),
                        message: this._translation.translate('messages.sms_permission.message'),
                        buttons: [
                            {
                                text: this._translation.translate('messages.sms_permission.action_accept'),
                                role: 'accept',
                            },
                            {
                                text: this._translation.translate('messages.sms_permission.action_denied'),
                                role: 'denied',
                            }
                        ]
                    });

                    // show the alert
                    await alertResult.present();

                    // get the result on dismiss
                    const dismissResult = await alertResult.onDidDismiss();
                    if (dismissResult.role == "denied") {
                        // update message status based on the sending result
                        this.persistSmsMessage(connectivity, serverInfo.id, message, false, 'sms_permission_denied');
                        return;
                    }
                }

                // send the message
                var result = await this._smsService.sendSmsAsync(message.to, message.content);

                // update message status based on the sending result
                this.persistSmsMessage(connectivity, serverInfo.id, message, result.isSuccess, result.error);
            });
    }

    /**
   * save the message to the store and update the status of the message on the server
   * @param message the message to persist
   * @param isSuccess if the sending operation has succeed or not
   * @param errorCode an error code if exist
   */
    private persistSmsMessage(connectivity: WebsocketConnectionManager, serverId: string, message: IMessages, isSuccess: boolean, errorCode?: string): void {
        message = {
            ...message,
            serverId,
            sentOn: new Date(),
            deliverAt: isSuccess
                ? new Date()
                : null,
            status: isSuccess
                ? MessageStatus.Sent
                : MessageStatus.Failed,
        };

        // insert the message
        this._store.dispatch(MessagesStoreActions.InsertMessage({ message: message }));

        // update the status of the message on the server
        connectivity.sendUpdateMessageStatusEventAsync(message.id, message.status, errorCode);
    }

    private async presentToastAsync(message: string, duration: number | undefined = undefined) {
        const toast = await this._toast.create({
            duration: duration, message: message,
        });
        toast.present();
    }
}


