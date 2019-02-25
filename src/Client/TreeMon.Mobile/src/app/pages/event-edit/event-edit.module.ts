import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventEditPage } from './event-edit';
import { IonicModule } from '@ionic/angular';
import {GeoLocationComponentModule } from '../../components/geo/geolocation.module';
import {DropdownModule} from 'primeng/dropdown';


const routes: Routes = [
  {
    path: '',
    component: EventEditPage
  }
];

@NgModule({
  imports: [
    CommonModule,
    IonicModule,
    FormsModule,
    GeoLocationComponentModule,
    DropdownModule,
    RouterModule.forChild(routes)
  ],
  declarations: [
    EventEditPage,
  ], exports: [EventEditPage],
})
export class EventEditModule { }
