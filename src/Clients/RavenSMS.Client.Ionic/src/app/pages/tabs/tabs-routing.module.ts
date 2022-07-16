import { Tabs } from './tabs-page';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: 'tabs',
    component: Tabs,
    children: [
      {
        path: '',
        redirectTo: '/app/tabs/servers',
        pathMatch: 'full'
      },
      {
        path: 'servers',
        loadChildren: () => import('../servers/servers.module').then(m => m.ServersModule)
      },
      {
        path: 'messages',
        loadChildren: () => import('../messages/messages.module').then(m => m.MessagesModule)
      },
      {
        path: 'preferences',
        loadChildren: () => import('../preferences/preferences.module').then(m => m.PreferencesModule)
      },
      {
        path: 'about',
        loadChildren: () => import('../about/about.module').then(m => m.AboutModule)
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TabsPageRoutingModule { }

