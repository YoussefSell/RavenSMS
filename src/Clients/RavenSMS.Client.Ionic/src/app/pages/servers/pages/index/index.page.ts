import { ServersStoreActions, ServersStoreSelectors, RootStoreState } from 'src/app/store';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ServerStatus } from 'src/app/core/constants/enums';
import { IServerInfo } from 'src/app/core/models';
import { Config } from '@ionic/angular';
import { Store } from '@ngrx/store';
import { SubSink } from 'subsink';
import * as _ from 'lodash';

@Component({
  selector: 'page-servers',
  templateUrl: 'index.page.html',
  styleUrls: ['./index.page.scss'],
})
export class IndexPage implements OnInit, OnDestroy {
  _subSink = new SubSink();
  serverStatus = ServerStatus;

  _is_ios: boolean;
  _showSearchbar: boolean;
  _searchQuery: string;

  _servers: ReadonlyArray<IServerInfo> = [];
  _filteredServers: ReadonlyArray<IServerInfo> = [];

  constructor(
    private _config: Config,
    private _store: Store<RootStoreState.State>,
  ) { }

  ngOnInit() {
    this._subSink.sink = this._store.select(ServersStoreSelectors.ServersSelector)
      .subscribe(servers => {
        this._servers = servers;
        this._filteredServers = servers;
      });

    this._is_ios = this._config.get('mode') === 'ios';
  }

  ngOnDestroy(): void {
    this._subSink.unsubscribe();
  }

  removeServer(server: IServerInfo): void {
    this._store.dispatch(ServersStoreActions.DeleteServer({ serverId: server.id }));
  }

  reconnectServer(server: IServerInfo): void {
    this._store.dispatch(ServersStoreActions.ReconnectServer({ serverId: server.id }));
  }

  search(): void {
    if (this._searchQuery && this._searchQuery.length > 0) {
      // perform search
      this._filteredServers = this._servers.filter(server =>
        server.name.includes(this._searchQuery)
        || server.id.includes(this._searchQuery)
        || server.clientInfo.clientId.includes(this._searchQuery)
        || server.clientInfo.clientName.includes(this._searchQuery)
        || server.clientInfo.clientDescription.includes(this._searchQuery));

      return;
    }

    // reset the list & perform the grouping
    this._filteredServers = this._servers;
  }

  getServerStatusBadgeColor(server: IServerInfo): string {
    switch (server.status) {
      case ServerStatus.DISCONNECTED: return 'danger'
      case ServerStatus.OFFLINE: return 'danger'
      case ServerStatus.ONLINE: return 'success'
      case ServerStatus.RECONNECTING: return 'warning'
      default: return 'light';
    }
  }
}
