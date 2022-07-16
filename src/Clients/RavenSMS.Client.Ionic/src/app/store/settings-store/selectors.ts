import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ApplicationFeatures } from 'src/app/core/constants';
import { State } from './state';

const featureStateSelector = createFeatureSelector<State>(ApplicationFeatures.settings);

export const StateSelector = createSelector(
  featureStateSelector,
  (state: State) => state
);
