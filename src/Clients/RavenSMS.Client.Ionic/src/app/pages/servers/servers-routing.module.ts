import { RouterModule, Routes } from '@angular/router';
import { NgModule } from '@angular/core';

import { DetailPage } from './pages/detail/detail.page';
import { IndexPage } from './pages/index/index.page';
import { SetupPage } from './pages/setup/setup.page';

const routes: Routes = [
  {
    path: '',
    component: IndexPage
  },
  {
    path: 'setup',
    component: SetupPage
  },
  {
    path: ':serverId',
    component: DetailPage
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ServersPageRoutingModule { }
