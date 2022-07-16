import { Component, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessagesStoreActions, MessagesStoreSelectors, RootStoreState, ServersStoreActions, ServersStoreSelectors } from 'src/app/store';
import { Store } from '@ngrx/store';
import { SubSink } from 'subsink';
import { IMessages, IServerInfo } from 'src/app/core/models';
import { MessageStatus } from 'src/app/core/constants/enums';

@Component({
  selector: 'page-message-detail',
  styleUrls: ['./detail.page.scss'],
  templateUrl: 'detail.page.html'
})
export class DetailPage {
  _subSink = new SubSink();
  _message: IMessages | null = null;
  _serverInfo: IServerInfo | null = null;

  session: any;
  isFavorite = false;
  _messageStatus = MessageStatus;

  constructor(
    private _router: Router,
    private _route: ActivatedRoute,
    private _store: Store<RootStoreState.State>,
  ) { }

  ionViewWillEnter() {
    this._subSink.sink = this._route.params
      .subscribe(params => {
        const messageId = params['messageId'];
        if (messageId) {
          this._store.dispatch(MessagesStoreActions.SelectMessage({ messageId }));
          return;
        }

        this.navigateBack();
      });

    this._subSink.sink = this._store.select(MessagesStoreSelectors.SelectedMessageSelector)
      .subscribe(message => {
        if (message) {
          this._message = message;
          this._store.dispatch(ServersStoreActions.SelectServer({ serverId: message.serverId }));
          return;
        }
      });

    this._subSink.sink = this._store.select(ServersStoreSelectors.SelectedServerSelector)
      .subscribe(server => {
        if (server) {
          this._serverInfo = server;
          return;
        }
      });
  }

  ionViewDidLeave(): void {
    this._store.dispatch(MessagesStoreActions.UnselectMessage());
    this._subSink.unsubscribe();
  }

  navigateBack(): void {
    this._router.navigateByUrl(`/app/tabs/messages`);
  }

  removeMessage(): void {
    this._store.dispatch(MessagesStoreActions.DeleteMessagesByIds({
      messagesIds: [this._message.id]
    }));
  }
}
