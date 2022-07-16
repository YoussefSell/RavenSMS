/**
 * the data of the client updated event
 */
export interface IClientInfoUpdatedEventData {
    /**
     * the name of the server
     */
    serverName: string;
    /**
     * the id of the server
     */
    serverId: string;
    /**
     * the unique identifier of this application app on the server
     */
    clientId: string;
    /**
     * the name of the client application
     */
    clientName?: string;
    /**
     * the simple description of the client application
     */
    clientDescription?: string;
    /**
     * the client phone number
     */
    clientPhoneNumber?: string;
}
