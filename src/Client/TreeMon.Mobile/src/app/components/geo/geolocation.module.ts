import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';
import { GeoLocationComponent} from './geolocation.component';
import {AutoCompleteModule} from 'primeng/autocomplete';
import {DropdownModule} from 'primeng/dropdown';

@NgModule({
  declarations: [GeoLocationComponent],
  imports: [
    CommonModule,
    IonicModule,
    FormsModule,
    AutoCompleteModule,
    DropdownModule
  ],
  exports: [GeoLocationComponent]
})
export class GeoLocationComponentModule { }
