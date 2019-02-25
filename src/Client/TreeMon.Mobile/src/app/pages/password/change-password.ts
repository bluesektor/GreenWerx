
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services';
import { ServiceResult } from '../../models/serviceresult';
import { NgForm } from '@angular/forms';
import {ChangePasswordForm} from './changePassword';
import { Events, ModalController } from '@ionic/angular';

@Component({
  selector: 'modal-changepassword',
  templateUrl: 'change-password.html'

})

export class ChangePasswordPage implements OnInit {
    submitted = false;
    errorString: string;
    loading = false;
    passwordForm: ChangePasswordForm = new ChangePasswordForm();
    error = '';

    constructor(
        public modalCtrl: ModalController,
        private _userService: UserService,
        public messages: Events        ) { }

    ngOnInit() {
        this.passwordForm.ResetPassword = false;
    }

    onSave(form: NgForm ) {
        this.error = '';
        console.log('change-password.TS onSignup form', form);
        this.submitted = true;
        this.loading = true;

        if (!form.valid || this.validateForm() === false) { return; }

        this._userService.changePassword(this.passwordForm).subscribe((response) => {
          const data = response as ServiceResult;
          if (data.Code !== 200) {
            this.messages.publish('api:err', data);
            this.error = data.Message;
            this.loading =  false;
            return false;
          }

          this.messages.publish('api:ok', 'Password change successful.');
          this.passwordForm = new ChangePasswordForm();
          this.dismiss();
         }, (err) => {
          this.messages.publish('service:err', err);
          this.error = err.statusText;
         });

    }

    validateForm(): boolean {
        console.log('validateForm  NOT IMPLEMENTED');
        this.errorString = '';
        if (this.passwordForm.NewPassword !== this.passwordForm.ConfirmPassword) {
          this.errorString = 'Passwords must match.';
          return false;
        }
        return true;
      }

      dismiss() {
        this.error = '';
        this.modalCtrl.dismiss();
      }
}
