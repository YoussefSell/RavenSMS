import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ApplicationFeatures } from 'src/app/core/constants';
import { State } from './state';

const featureStateSelector = createFeatureSelector<State>(ApplicationFeatures.servers);

export const StateSelector = createSelector(
  featureStateSelector,
  (state: State) => state
);

export const IsLoadingSelector = createSelector(
  featureStateSelector,
  (state: State) => state.isLoading
);

export const ErrorSelector = createSelector(
  featureStateSelector,
  (state: State) => state.error
);

/**
 * select the servers list
 */
export const ServersSelector = createSelector(
  featureStateSelector,
  (state: State) => state.servers
);

/**
 * select the currently selected server
 */
export const SelectedServerSelector = createSelector(
  featureStateSelector,
  (state: State) => state.selectedServer
);
