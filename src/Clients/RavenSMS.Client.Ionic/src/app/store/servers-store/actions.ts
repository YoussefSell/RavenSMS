import { ServerStatus } from 'src/app/core/constants/enums';
import { createAction, props } from '@ngrx/store';
import { IAppIdentification, IClientInfoUpdatedEventData, IRegisterServerInfoModel, IServerInfo } from 'src/app/core/models';

/**
 * this enums defines the action types for the companies module
 */
export enum StoreActionTypes {
    SELECT_SERVER = '@servers/select',
    UNSELECT_SERVER = '@servers/unselect',

    RECONNECT = '@servers/reconnect',

    INSERT_SERVER = '@servers/insert',

    UPDATE_SERVER_INFO = '@servers/update/info',
    UPDATE_SERVER_STATUS = '@servers/update/status',
    UPDATE_SERVER_CLIENT_INFO = '@servers/update/client/info',

    DELETE_SERVER = '@servers/delete',
    SERVER_DELETED = '@servers/deleted',
}

export const SelectServer = createAction(StoreActionTypes.SELECT_SERVER, props<{ serverId: string; }>());
export const UnselectServer = createAction(StoreActionTypes.UNSELECT_SERVER);

export const ReconnectServer = createAction(StoreActionTypes.RECONNECT, props<{ serverId: string }>());

export const AddServer = createAction(StoreActionTypes.INSERT_SERVER, props<{ serverInfo: IRegisterServerInfoModel }>());

export const DeleteServer = createAction(StoreActionTypes.DELETE_SERVER, props<{ serverId: string; }>());
export const ServerDeleted = createAction(StoreActionTypes.DELETE_SERVER, props<{ serverId: string; }>());

export const UpdateServerInfo = createAction(StoreActionTypes.UPDATE_SERVER_INFO, props<{ serverId: string, data: IServerInfo | null }>());
export const UpdateServerStatus = createAction(StoreActionTypes.UPDATE_SERVER_STATUS, props<{ serverId: string, newStatus: ServerStatus }>());

export const UpdateServerClientInfo = createAction(StoreActionTypes.UPDATE_SERVER_CLIENT_INFO, props<{ serverId: string, data: IClientInfoUpdatedEventData }>());