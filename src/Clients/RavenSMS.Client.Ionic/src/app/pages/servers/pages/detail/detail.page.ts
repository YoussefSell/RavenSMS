import { ServersStoreActions, ServersStoreSelectors, RootStoreState } from 'src/app/store';
import { ServerStatus } from 'src/app/core/constants/enums';
import { ActivatedRoute, Router } from '@angular/router';
import { IServerInfo } from 'src/app/core/models';
import { Component } from '@angular/core';
import { Store } from '@ngrx/store';
import { SubSink } from 'subsink';

@Component({
  selector: 'page-server-detail',
  styleUrls: ['./detail.page.scss'],
  templateUrl: 'detail.page.html'
})
export class DetailPage {
  _subSink = new SubSink();
  _server: IServerInfo | null = null;

  session: any;
  isFavorite = false;
  _serverStatus = ServerStatus;

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
    private _store: Store<RootStoreState.State>,
  ) { }

  ionViewWillEnter() {
    this._subSink.sink = this._route.params
      .subscribe(params => {
        const serverId = params['serverId'];
        if (serverId) {
          this._store.dispatch(ServersStoreActions.SelectServer({ serverId }));
          return;
        }

        this.navigateBack();
      });

    this._subSink.sink = this._store.select(ServersStoreSelectors.SelectedServerSelector)
      .subscribe(server => {
        this._server = server;
      });
  }

  ionViewDidLeave(): void {
    this._store.dispatch(ServersStoreActions.UnselectServer());
    this._subSink.unsubscribe();
  }

  navigateBack(): void {
    this._router.navigateByUrl(`/app/tabs/servers`);
  }

  removeServer(): void {
    this._store.dispatch(ServersStoreActions.DeleteServer({
      serverId: this._server.id
    }));
  }
}
