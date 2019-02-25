import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';

import { ChangePasswordPage } from './change-password';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule
  ],
  declarations: [
    ChangePasswordPage,
  ],
  exports: [
    ChangePasswordPage
  ], entryComponents: [
    ChangePasswordPage
  ]
})
export class ChangePasswordModule { }
