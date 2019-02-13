import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';

import { MessageBoxesComponent } from '../../common/messageboxes.component';
import { BasicValidators } from '../../common/basicValidators';
import { PasswordValidators } from '../../common/passwordValidators';
import { UserService } from '../../services/user.service';
import { SessionService } from '../../services/session.service';

import { User } from '../../models/user';


@Component({

    templateUrl: './profile.component.html',
    providers: [SessionService, UserService]
})
export class UserProfileComponent implements OnInit {

    form: FormGroup;
    title: string;
    newUser = false;
    user = new User();
    testMessage: string;
    savingProfile = false;


    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        fb: FormBuilder,
        private _router: Router,
        private _route: ActivatedRoute,
        private _userService: UserService,
        private _sessionService: SessionService
        ) {
        this.user.UUID = '';
        if (this._sessionService.CurrentSession.validSession) {

            this.form = fb.group({
                name: ['', Validators.required],
                Email: ['', BasicValidators.email],
                PasswordQuestion: ['', Validators.required],
                PasswordAnswer: ['', Validators.required],
            });
        } else {
            this.form = fb.group({
                name: ['', Validators.required],
                Email: ['', BasicValidators.email],
                password: ['', Validators.compose([
                    Validators.required,
                    PasswordValidators.complexPassword
                ])],
                confirmPassword: ['', Validators.required],
                PasswordQuestion: ['', Validators.required],
                PasswordAnswer: ['', Validators.required],

            }, { validator: PasswordValidators.passwordsShouldMatch });
        }
    }

    ngOnInit() {

        this.title = this._sessionService.CurrentSession.validSession ? 'Edit User' : 'New User';
        this.newUser = this._sessionService.CurrentSession.validSession ? false : true;

        if (!this._sessionService.CurrentSession.validSession) {
            return;
        }
        this._userService.getUser(this._sessionService.CurrentSession.userUUID)
            .subscribe(response => {
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this.user = response.Result;
            }, err => {
                this.msgBox.ShowResponseMessage(err.status, 10);

                if (err.status === 401 && err.statusText === 'Session expired.') {
                    this._sessionService.ClearSessionState();
                    setTimeout(() => {
                        this._router.navigate(['/membership/login'], { relativeTo: this._route });
                    }, 3000);
                }

            });
    }

    SaveProfile() {

        this.savingProfile = true;
        this.msgBox.closeMessageBox();

        let result;

        if (this.user.UUID && this.user.UUID.length > 0) {
            result = this._userService.updateUser(this.user);
        } else {
            result = this._userService.register(this.user);
        }

        result.subscribe(response => {

            this.savingProfile = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newUser) {
                // TODO re-implement when server is fixed.
               // this.msgBox.ShowMessage('info', 'You have been sent a confirmation email.
               // Please check our inbox or spam folders and click the link to proceed.', 20);
               this.msgBox.ShowMessage('info', 'Registration successful, you will be redirected to the login page.', 20);
                this.user = new User();
               setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                },  20000);

            } else {
                this.form.markAsPristine();
               this.msgBox.ShowMessage('info', 'Profile saved.', 15);
            }
        }, err => {
            this.savingProfile = false;
            this.msgBox.ShowResponseMessage(err.status, 30);


            } );
    }
}
