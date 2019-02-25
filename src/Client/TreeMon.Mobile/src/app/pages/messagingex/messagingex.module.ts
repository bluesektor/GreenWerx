import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MessagingExComponent } from './messagingex.component';
import { IonicModule } from '@ionic/angular';

@NgModule({
  imports: [
    CommonModule,
    IonicModule,
    FormsModule
  ],
  declarations: [
    MessagingExComponent,
  ], entryComponents: [
    MessagingExComponent
  ]
})
export class MessagingExModule { }
