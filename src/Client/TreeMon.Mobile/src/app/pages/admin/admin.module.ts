import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IonicModule } from '@ionic/angular';
import { FormsModule } from '@angular/forms';
import { AdminPage } from './admin';
import { AdminPageRoutingModule } from './admin-routing.module';
import {ChangePasswordModule} from '../password/change-password.module';
import {TreeTableModule} from 'primeng/treetable';
@NgModule({
  imports: [
    FormsModule,
    CommonModule,
    IonicModule,
    AdminPageRoutingModule,
    ChangePasswordModule,
    TreeTableModule
  ],
  declarations: [
    AdminPage,
  ]
})
export class AdminModule { }
