import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

// stores imports
import { MessagesStoreModule } from './messages-store';
import { SettingsStoreModule } from './settings-store';
import { ServersStoreModule } from './servers-store';
import { UIStoreModule } from './ui-store';

const modules = [
    UIStoreModule,
    ServersStoreModule,
    SettingsStoreModule,
    MessagesStoreModule,
];
@NgModule({
    declarations: [],
    imports: [
        CommonModule,
        ...modules,
    ],
    exports: [
        ...modules
    ]
})
export class RootStoreModule { }
