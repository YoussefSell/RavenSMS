import { MessagesStoreActions, MessagesStoreSelectors, RootStoreState } from 'src/app/store';
import { MessageStatus } from 'src/app/core/constants/enums';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { IMessages } from 'src/app/core/models';
import { Config } from '@ionic/angular';
import { Store } from '@ngrx/store';
import { SubSink } from 'subsink';
import { format } from 'date-fns';
import * as _ from 'lodash';

@Component({
  selector: 'page-messages',
  templateUrl: 'index.page.html',
  styleUrls: ['./index.page.scss'],
})
export class IndexPage implements OnInit, OnDestroy {
  _subSink = new SubSink();

  _is_ios: boolean;
  _showSearchbar: boolean;
  _searchQuery: string;
  _messageStatus = MessageStatus;

  _messages: ReadonlyArray<IMessages> = [];
  _filteredMessages: ReadonlyArray<IMessages> = [];
  _messagesGroups: { date: string; messages: IMessages[] }[] = [];

  constructor(
    private _config: Config,
    private _store: Store<RootStoreState.State>,
  ) { }

  ngOnInit() {
    this._subSink.sink = this._store.select(MessagesStoreSelectors.MessagesSelector)
      .subscribe(messages => {
        this._filteredMessages = messages;
        this._messages = messages;
        this.groupMessages();
      });

    this._is_ios = this._config.get('mode') === 'ios';
  }

  ngOnDestroy(): void {
    this._subSink.unsubscribe();
  }

  groupMessages(): void {
    // group messages by date
    const grouping = _.groupBy(this._filteredMessages, item => item.sentOn ? format(this.ToDate(item.sentOn), 'yyyy-MM-dd') : 'in_queue');

    // transform the grouping into an array
    this._messagesGroups = Object.keys(grouping)
      .map(key => ({ date: key, messages: grouping[key] }));
  }

  removeMessage(message: IMessages): void {
    this._store.dispatch(MessagesStoreActions.DeleteMessagesByIds({ messagesIds: [message.id] }));
  }

  search(): void {
    if (this._searchQuery && this._searchQuery.length > 0) {
      // perform search
      this._filteredMessages = this._messages.filter(message =>
        message.content.includes(this._searchQuery)
        || message.to.includes(this._searchQuery));

      // group messages
      this.groupMessages();
      return;
    }

    // reset the list & perform the grouping
    this._filteredMessages = this._messages;
    this.groupMessages();
  }

  /**
   * check if the given date value is a string, if so we will convert it to date
   * @param date the date value to check
   * @returns date or null
   */
  ToDate(date: any): Date | null {
    if (date instanceof Date) {
      return date;
    }

    if (typeof date === 'string') {
      return new Date(date);
    }

    return null;
  }
}
