import { createEffect, Actions, ofType } from '@ngrx/effects';
import { exhaustMap } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import * as ActionTypes from './actions';
import { Store } from '@ngrx/store';
import { State } from './state';

@Injectable()
export class MainEffects {

  constructor(
    private store: Store<State>,
    private actions$: Actions,
  ) { }
}
