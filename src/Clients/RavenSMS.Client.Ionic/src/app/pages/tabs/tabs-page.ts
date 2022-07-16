import { Component } from '@angular/core';

@Component({
  template: `
  <ng-container *transloco="let translate">
    <ion-tabs>
      <ion-tab-bar slot="bottom">
        <ion-tab-button tab="servers">
          <ion-icon name="cloud-outline"></ion-icon>
          <ion-label>{{translate('tabs.servers')}}</ion-label>
        </ion-tab-button>
        <ion-tab-button tab="messages">
          <ion-icon name="mail"></ion-icon>
          <ion-label>{{translate('tabs.messages')}}</ion-label>
        </ion-tab-button>
        <ion-tab-button></ion-tab-button>
        <ion-tab-button tab="preferences">
          <ion-icon name="settings"></ion-icon>
          <ion-label>{{translate('tabs.preferences')}}</ion-label>
        </ion-tab-button>
        <ion-tab-button tab="about">
          <ion-icon name="information-circle"></ion-icon>
          <ion-label>{{translate('tabs.about')}}</ion-label>
        </ion-tab-button>
      </ion-tab-bar>
    </ion-tabs>
    <ion-fab vertical="bottom" horizontal="center" slot="fixed">
      <ion-fab-button routerLink="/app/tabs/servers/setup">
          <ion-icon name="add"></ion-icon>
      </ion-fab-button>
    </ion-fab>
  </ng-container>
  `,
  styles: [
    '.tabbar {justify-content: center;}',
    '.tab-button {max-width: 200px;}'
  ]
})
export class Tabs { }
