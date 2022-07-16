/**
 * an enum that defines the disconnection reasons
 */
export enum DisconnectionReason {
    Disconnect = 'disconnect',
    ClientDeleted = 'client_deleted',
    ClientNotFound = 'client_not_found',
    ClientAlreadyConnected = 'client_already_connected'
}
