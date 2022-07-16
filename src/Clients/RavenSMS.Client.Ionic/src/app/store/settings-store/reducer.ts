import { createReducer, on } from '@ngrx/store';
import { ServerStatus } from 'src/app/core/constants/enums';
import * as Actions from './actions';
import { State } from './state';

/**
 * set the initial state of the settings module
 */
const initialState: State = {
    appIdentification: null,
    IServerInfo: null,
};

/**
 * the main ui module reducer
 */
export const MainReducer = createReducer<State>(
    // set the initial state
    initialState,

    // check the actions
);
