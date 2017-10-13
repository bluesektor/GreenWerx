import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms'; //
import { UserService } from '../../services/user.service';
import { MessageBoxesComponent } from '../../common/messageboxes.component';

@Component({

    templateUrl: './login-help.component.html',
    providers: [UserService]
})

export class LoginHelpComponent {
    form: FormGroup;
    Email: string;
    sending = false;

    public forgotPassword = false;
    public resendValidation = false;

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        fb: FormBuilder,
        private _userService: UserService,
        private _router: Router,
        private _route: ActivatedRoute
    ) {

        this.form = fb.group({
            Email : ['', Validators.required],
            forgotPassword: '',
            resendValidation: ''
        });
    }


    toggleForgotPassword($event) {
        this.forgotPassword = !this.forgotPassword;
        $event.stopPropagation();
    }

    changeHelpOption($event) {
        $event.stopPropagation();
    }


    SendAccountInfo() {
        if (this.forgotPassword) {
            console.log('forgot password');
        }


        this.msgBox.closeMessageBox();
        this.sending = true;
        const result = this._userService.sendUserInfo(this.form.value);

        result.subscribe(
            response => {
                this.sending = false;
                if (response.Code !== 200) {

                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this.form.markAsPristine();

                if (this.forgotPassword) {
                    this.msgBox.ShowMessage(response.Status, 'Please check your email for instructions on updating your password.', 10);
                } else {
                    this.msgBox.ShowMessage(response.Status, 'Please check your email for your account information.', 10);
                }
            },
            err => {
                this.sending = false;
                this.msgBox.ShowResponseMessage(err.status, 10);
            }
        );
    }
}
