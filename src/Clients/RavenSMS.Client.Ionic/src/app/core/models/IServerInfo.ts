import { ServerStatus } from "../constants/enums";
import { IAppIdentification } from "./IAppIdentification";

/**
 * define the server details
 */
export interface IServerInfo {
    /**
     * server id
     */
    id?: string;
    /**
     * server name
     */
    name?: string;
    /**
     * server status
     */
    status?: ServerStatus;
    /**
     * server url
     */
    url: string;
    /**
     * the server client info
     */
    clientInfo: IAppIdentification;
}
