import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';

import { ValidatePage } from './validate';
import { MembershipRoutingModule } from './membership-routing.module';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    MembershipRoutingModule
  ],
  declarations: [
    ValidatePage,
  ]
})
export class MembershipModule { }
