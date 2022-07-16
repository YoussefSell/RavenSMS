import { RootActions, RootStoreSelectors, RootStoreState, ServersStoreSelectors, StorePersistenceActions, UIStoreSelectors } from './store';
import { ServersConnectivityService } from './core/services';
import { DeviceNetworkStatus } from './core/constants/enums';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { AlertController } from '@ionic/angular';
import { Network } from '@capacitor/network';
import { App } from '@capacitor/app';
import { Store } from '@ngrx/store';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-root',
  template: `
  <ion-app [class.dark-theme]="_dark">
    <ion-router-outlet id="main-content"></ion-router-outlet>
  </ion-app>
  `,
  styleUrls: ['app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {

  _subSink = new SubSink();

  _dark: boolean = false;
  _networkAlert: HTMLIonAlertElement | null = null;

  constructor(
    private _alert: AlertController,
    private _store: Store<RootStoreState.State>,
    private _translationService: TranslocoService,
    private _serversConnectivity: ServersConnectivityService,
  ) {
    // add a listener on the app state
    App.addListener('appStateChange', ({ isActive }) => {
      if (!isActive) {
        console.log("persisting the state");
        this._store.dispatch(StorePersistenceActions.PersistStore());
      }
    });

    // add a listener on the network state
    Network.addListener('networkStatusChange', status => {
      this._store.dispatch(RootActions.UpdateNetworkConnectionStatus({
        newStatus: status.connected ? DeviceNetworkStatus.ONLINE : DeviceNetworkStatus.OFFLINE
      }));
    });
  }

  async ngOnInit(): Promise<void> {
    this._subSink.sink = this._store.select(ServersStoreSelectors.ServersSelector)
      .subscribe(servers => this._serversConnectivity.attachServers(servers));

    this._subSink.sink = this._store.select(UIStoreSelectors.StateSelector)
      .subscribe(state => {
        this._dark = state.darkMode;
        this._translationService.setActiveLang(state.language);
      });

    this._subSink.sink = this._store.select(RootStoreSelectors.NetworkConnectionSelector)
      .subscribe(async status => {
        // if the status is unknown we can't do anything
        if (status === DeviceNetworkStatus.UNKNOWN) {
          return;
        }

        if (status == DeviceNetworkStatus.ONLINE) {
          await this._networkAlert?.dismiss();
          this._networkAlert = null;
          return;
        }

        // check if the network alert is already presented.
        if (this._networkAlert) {
          return;
        }

        this._networkAlert = await this._alert.create({
          backdropDismiss: false,
          message: "you have been disconnected, please check your internet connection."
        });
        await this._networkAlert.present();
      });

    // check current network status
    this._store.dispatch(RootActions.UpdateNetworkConnectionStatus({
      newStatus: (await Network.getStatus()).connected
        ? DeviceNetworkStatus.ONLINE
        : DeviceNetworkStatus.OFFLINE
    }));
  }

  ngOnDestroy(): void {
    this._subSink.unsubscribe();
    Network.removeAllListeners();
  }
}
