import { createEffect, Actions, ofType } from '@ngrx/effects';
import { ApplicationRoutes } from 'src/app/core/constants';
import { MessagesStoreActions } from '../messages-store';
import { exhaustMap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import * as ActionTypes from './actions';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { ServersConnectivityService } from 'src/app/core/services';

@Injectable()
export class MainEffects {

  constructor(
    private router: Router,
    private actions$: Actions,
    private service: ServersConnectivityService,
  ) { }

  DeleteServer$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ActionTypes.DeleteServer),
      exhaustMap(async (props) => {
        return MessagesStoreActions.DeleteMessagesByServerId({ serverId: props.serverId });
      })
    );
  });

  onAddServer$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ActionTypes.AddServer),
      tap((_) => {
        this.router.navigateByUrl(ApplicationRoutes.servers, { replaceUrl: true });
      })
    );
  }, { dispatch: false });

  onReconnectServer$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(ActionTypes.ReconnectServer),
      tap((props) => {
        this.service.reconnect(props.serverId);
      })
    );
  }, { dispatch: false });
}
