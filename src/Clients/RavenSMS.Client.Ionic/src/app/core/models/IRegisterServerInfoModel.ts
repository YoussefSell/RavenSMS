/**
 * the interface that defines the server registration data
 */
export interface IRegisterServerInfoModel {
    /**
     * the client id
     */
    clientId: string;
    /**
     * the id of the server
     */
    serverId: string;
    /**
     * the server url
     */
    serverUrl: string;
}
