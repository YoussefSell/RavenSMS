import { MessageStatus } from 'src/app/core/constants/enums';
import { createAction, props } from '@ngrx/store';
import { IMessages } from 'src/app/core/models';

/**
 * this enums defines the action types for the companies module
 */
export enum StoreActionTypes {
    MESSAGES_LOADED = '@messages/loaded',

    SELECT_MESSAGE = '@messages/select',
    UNSELECT_MESSAGE = '@messages/unselect',

    INSERT_MESSAGE = '@messages/insert',
    UPDATE_MESSAGE_STATUS = '@messages/update/status',
    DELETE_MESSAGES_BY_ID = '@messages/delete/by/id',
    DELETE_MESSAGES_BY_SERVER_ID = '@messages/delete/by/id/server',
}

export const MessagesLoaded = createAction(StoreActionTypes.MESSAGES_LOADED, props<{ serverId: string, data: IMessages[] }>());

export const SelectMessage = createAction(StoreActionTypes.SELECT_MESSAGE, props<{ messageId: string; }>());
export const UnselectMessage = createAction(StoreActionTypes.UNSELECT_MESSAGE);

export const InsertMessage = createAction(StoreActionTypes.INSERT_MESSAGE, props<{ message: IMessages }>());
export const UpdateMessageStatus = createAction(StoreActionTypes.UPDATE_MESSAGE_STATUS, props<{ messageId: string, newStatus: MessageStatus }>());
export const DeleteMessagesByServerId = createAction(StoreActionTypes.DELETE_MESSAGES_BY_SERVER_ID, props<{ serverId?: string; }>());
export const DeleteMessagesByIds = createAction(StoreActionTypes.DELETE_MESSAGES_BY_ID, props<{ messagesIds: string[]; }>());

