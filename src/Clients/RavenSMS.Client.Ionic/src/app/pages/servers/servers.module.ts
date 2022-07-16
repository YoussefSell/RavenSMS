import { TranslocoModule, TRANSLOCO_SCOPE } from '@ngneat/transloco';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { NgModule } from '@angular/core';

import { ServersPageRoutingModule } from './servers-routing.module';

import { IndexPage } from './pages/index/index.page';
import { DetailPage } from './pages/detail/detail.page';
import { SetupPage } from './pages/setup/setup.page';

@NgModule({
  imports: [
    IonicModule,
    FormsModule,
    CommonModule,
    TranslocoModule,
    ReactiveFormsModule,
    ServersPageRoutingModule,
  ],
  declarations: [
    IndexPage,
    DetailPage,
    SetupPage
  ],
  providers: [
    { provide: TRANSLOCO_SCOPE, useValue: 'common', multi: true },
    { provide: TRANSLOCO_SCOPE, useValue: 'servers', multi: true },
  ]
})
export class ServersModule { }
