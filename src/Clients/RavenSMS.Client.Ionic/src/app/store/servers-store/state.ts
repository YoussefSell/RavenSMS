import { IServerInfo } from 'src/app/core/models';

/**
 * this interface defines the servers module state
 */
export interface State {
    /**
     * the list of the servers
     */
    servers: ReadonlyArray<IServerInfo>;

    /**
     * the currently selected server
     */
    selectedServer: Readonly<IServerInfo> | null;

    /**
     * the error associated with the action of the server store
     */
    error: Readonly<any> | null;

    /**
     * is currently performing an action
     */
    isLoading: boolean;
}
