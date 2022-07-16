import { Actions, createEffect, ofType } from '@ngrx/effects';
import { ApplicationRoutes } from 'src/app/core/constants';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import * as ActionTypes from './actions';
import { tap } from 'rxjs';

@Injectable()
export class MainEffects {

  constructor(
    private router: Router,
    private actions$: Actions,
  ) { }

}
