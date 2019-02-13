import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Routes, RouterModule } from '@angular/router';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { IonicModule } from '@ionic/angular';
import { HomePage } from './home.page';
import { InViewportModule } from 'ng-in-viewport';
 import { HomeFilterPage } from '../home-filter/home-filter';
// import {MenuPageModule} from '../menu/menu.module';
 const routes: Routes = [  {    path: '',    component: HomePage  }];

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    ScrollingModule,
    DragDropModule,
    InViewportModule,
  //  EventsFilterPage,
   // MenuPageModule,
    RouterModule.forChild(routes)
  ],
  declarations: [HomePage, HomeFilterPage],
  entryComponents: [    HomeFilterPage  ]
})
export class HomePageModule {}
