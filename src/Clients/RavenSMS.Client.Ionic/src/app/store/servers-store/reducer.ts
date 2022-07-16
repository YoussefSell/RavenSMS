import { ServerStatus } from 'src/app/core/constants/enums';
import { createReducer, on } from '@ngrx/store';
import * as Actions from './actions';
import { State } from './state';

/**
 * set the initial state of the authentication module
 */
const initialState: State = {
    isLoading: false,
    error: null,
    servers: [],
    selectedServer: null,
};

/**
 * the authentication module reducer
 */
export const MainReducer = createReducer<State>(
    // set the initial state
    initialState,

    // check the actions
    on(Actions.SelectServer, (state, action): State => {
        const server = state.servers.find(m => m.id == action.serverId);

        // if we found a server set as the selected server.
        if (server !== null && server !== undefined) {
            return {
                ...state,
                selectedServer: server,
            }
        }

        return state;
    }),
    on(Actions.UnselectServer, (state, action): State => {
        return {
            ...state,
            selectedServer: null,
        }
    }),
    on(Actions.AddServer, (state, action): State => {
        // get the server index
        const serverIndex = state.servers.map(m => m.id).indexOf(action.serverInfo.serverId);

        // server already exist
        if (serverIndex >= 0) {
            return state;
        }

        let newState: State = {
            ...state,
            servers: [
                ...state.servers.slice(0, serverIndex),
                {
                    name: '',
                    status: ServerStatus.UNKNOWN,
                    id: action.serverInfo.serverId,
                    url: action.serverInfo.serverUrl,
                    clientInfo: {
                        clientName: '',
                        clientDescription: '',
                        clientPhoneNumber: '',
                        clientId: action.serverInfo.clientId,
                    }
                },
                ...state.servers.slice(serverIndex + 1),
            ]
        };

        return newState;
    }),
    on(Actions.DeleteServer, Actions.ServerDeleted, (state, action): State => {
        // get the server index
        const serverIndex = state.servers.map(m => m.id).indexOf(action.serverId);

        let newState: State = {
            ...state,
            servers: [
                ...state.servers.slice(0, serverIndex),
                ...state.servers.slice(serverIndex + 1),
            ]
        };

        if (state.selectedServer && state.selectedServer.id == action.serverId) {
            newState.selectedServer = null
        }

        return newState;
    }),
    on(Actions.UpdateServerStatus, (state, action): State => {
        // get the server index
        const serverIndex = state.servers.map(m => m.id).indexOf(action.serverId);
        if (serverIndex >= 0) {
            let newState: State = {
                ...state,
                servers: [
                    ...state.servers.slice(0, serverIndex),
                    {
                        ...state.servers[serverIndex],
                        status: action.newStatus,
                    },
                    ...state.servers.slice(serverIndex + 1),
                ]
            };

            if (state.selectedServer && state.selectedServer.id == action.serverId) {
                newState.selectedServer = {
                    ...state.selectedServer,
                    status: action.newStatus,
                }
            }

            return newState;
        }

        return state;
    }),
    on(Actions.UpdateServerInfo, (state, action): State => {
        // get the server index
        const serverIndex = state.servers.map(m => m.id).indexOf(action.serverId);
        if (serverIndex >= 0) {
            let newState: State = {
                ...state,
                servers: [
                    ...state.servers.slice(0, serverIndex),
                    {
                        ...action.data,
                    },
                    ...state.servers.slice(serverIndex + 1),
                ]
            };

            if (state.selectedServer && state.selectedServer.id == action.serverId) {
                newState.selectedServer = {
                    ...action.data,
                }
            }

            return newState;
        }

        return state;
    }),
    on(Actions.UpdateServerClientInfo, (state, action): State => {
        // get the server index
        const serverIndex = state.servers.map(m => m.id).indexOf(action.serverId);
        if (serverIndex >= 0) {
            let newState: State = {
                ...state,
                servers: [
                    ...state.servers.slice(0, serverIndex),
                    {
                        ...state.servers[serverIndex],
                        id: action.data.serverId,
                        name: action.data.serverName,
                        clientInfo: {
                            clientId: action.data.clientId,
                            clientName: action.data.clientName,
                            clientPhoneNumber: action.data.clientPhoneNumber,
                            clientDescription: action.data.clientDescription,
                        },
                    },
                    ...state.servers.slice(serverIndex + 1),
                ]
            };

            if (state.selectedServer && state.selectedServer.id == action.serverId) {
                newState.selectedServer = {
                    ...state.selectedServer,
                    id: action.data.serverId,
                    name: action.data.serverName,
                    clientInfo: {
                        clientId: action.data.clientId,
                        clientName: action.data.clientName,
                        clientPhoneNumber: action.data.clientPhoneNumber,
                        clientDescription: action.data.clientDescription,
                    },
                }
            }

            return newState;
        }

        return state;
    }),
);
