import { Component, ViewEncapsulation } from '@angular/core';
import { NgForm } from '@angular/forms';

import { AlertController, ToastController } from '@ionic/angular';
import { UserService } from '../../services';
import { ServiceResult } from '../../models/serviceresult';
import { Message } from '../../models/index';
import { Events } from '@ionic/angular';
@Component({
  selector: 'page-support',
  templateUrl: 'support.html',
  styleUrls: ['./support.scss'],
  encapsulation: ViewEncapsulation.None
})
export class SupportPage {
  sendingMessage = false;
  message: Message;
  submitted = false;
  result = '';

  constructor(
    public alertCtrl: AlertController,
    public toastCtrl: ToastController,
    private userService: UserService,
    private messages: Events
      ) {
        this.message = new Message();
      }

  async ionViewDidEnter() {
    this.submitted = false;
    this.message.Comment = '';
    this.message.SentFrom = '';
  }

  async submit(form: NgForm) {
    this.submitted = true;

    if (form.valid === false) {
      return;
    }
    this.sendingMessage = true;
    console.log('sendMessage:', this.message);

    this.message.Type  = 'SUPPORT'; // todo add dropdown for different message types so user can select.

    await this.userService.contactAdmin( this.message).subscribe(response => {
      const data = response as ServiceResult;
      this.sendingMessage = false;

      if (data.Code !== 200) {
          this.result = data.Message;
          return false;
        }
       this.result = 'Your support request has been sent.';

    }, err => {
      this.sendingMessage = false;
      this.messages.publish('service:err', err);
         if (err.status === 401) {
           console.log('session expired');
         }
      });

  }

  hideEmailError(): boolean {

    if ( this.submitted === false) {
      return true;
    }

    if (this.message.SentFrom === '') {
      console.log('support.ts hideEmailError this.message.SentFrom === ""');
      return false;
    }
    console.log('support.ts hideEmailError true');
    return true;
  }

  hideCommentError(): boolean {
    if ( this.submitted === false) {
      return true;
    }
    if (this.message.Comment === '') {
      return false;
    }

    return true;
  }



  // If the user enters text in the support question and then navigates
  // without submitting first, ask if they meant to leave the page
  // async ionViewCanLeave(): Promise<boolean> {
  //   // If the support message is empty we should just navigate
  //   if (!this.supportMessage || this.supportMessage.trim().length === 0) {
  //     return true;
  //   }

  //   return new Promise((resolve: any, reject: any) => {
  //     const alert = await this.alertCtrl.create({
  //       title: 'Leave this page?',
  //       message: 'Are you sure you want to leave this page? Your support message will not be submitted.',
  //       buttons: [
  //         { text: 'Stay', handler: reject },
  //         { text: 'Leave', role: 'cancel', handler: resolve }
  //       ]
  //     });

  //     await alert.present();
  //   });
  // }
}
