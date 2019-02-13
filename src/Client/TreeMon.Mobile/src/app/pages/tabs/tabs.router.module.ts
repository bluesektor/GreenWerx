import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { TabsPage } from './tabs.page';

import { AboutPage } from '../about/about';
import { MapPage } from '../map/map';
import { AdminPage } from '../admin/admin';
import {EventEditPage} from '../event-edit/event-edit';
import { DetailsPage } from '../details/details.page';
import { HomePage } from '../home/home.page';
const routes: Routes = [
  {
    path: 'tabs',
    component: TabsPage,
    children: [
      {
        path: 'home',
        children: [
          {
            path: '',
            loadChildren: '../home/home.module#HomePageModule'
          }
        ]
      },
      {
        path: 'details/:uuid/:type',
        loadChildren: '../details/details.module#DetailsPageModule'
      },
      {
        path: 'edit/:uuid',
        loadChildren: '../event-edit/event-edit.module#EventEditModule',
      },
      {
        path: 'map',
        children: [
          {
            path: '',
            loadChildren: '../map/map.module#MapModule'
          }
        ]
      },
      {
        path: 'about',
        children: [
          {
            path: '',
            loadChildren: '../about/about.module#AboutModule'
          }
        ]
      },
      {
        path: '',
        redirectTo: '/tabs/home',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '',
    redirectTo: '/tabs/home',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TabsPageRoutingModule {}
