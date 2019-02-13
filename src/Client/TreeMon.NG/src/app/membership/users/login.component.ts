import { Component, ViewChild, Inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup,  Validators } from '@angular/forms'; //
import { UserService } from '../../services/user.service';
import { SessionService } from '../../services/session.service';
import { CheckboxModule } from 'primeng/primeng';
import { MessageBoxesComponent } from '../../common/messageboxes.component';
import {  CookieService  } from 'angular2-cookie/services/cookies.service';

@Component({

    templateUrl: './login.component.html',
    providers: [CookieService, UserService, SessionService]
})

export class LoginComponent {
    form: FormGroup;
    userName: string;
    passWord: string;
    rememberMe = false;
    returnUrl: string;
    authorizing = false;



    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        @Inject(FormBuilder)  fb: FormBuilder,
        public _userService: UserService,
        private _router: Router,
        private _route: ActivatedRoute,
        private _cookieService: CookieService,
        public _sessionService: SessionService  ) {

        this.form = fb.group({
            userName: ['', Validators.required],
            password: ['', Validators.required],
            rememberMe: ''
        });

        this._route.params.subscribe(params => {
            this.returnUrl = params['returnUrl'];
            this.rememberMe = true;
        });
        if (!this.returnUrl) {
            this.returnUrl = '';
        }

        this.userName = this._cookieService.get('userName');

    }

    toggleRememberMe($event) {
        this.rememberMe = $event.target.checked;
    }

    LogIn() {
        this.msgBox.closeMessageBox();
        this.authorizing = true;
        if (this.rememberMe) {
            this._cookieService.put('userName', this.userName);
        } else {
            this._cookieService.remove('userName');
        }
        console.log('login..        ');
        const result = this._userService.login(this.form.value);
        result.subscribe( response => {
            this.authorizing = false;
            if (response.Code !== 200) {
                console.log('login        ', response.Message);
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.form.markAsPristine();
            this._sessionService.CurrentSession.authToken = response.Result.Authorization;
            this._sessionService.CurrentSession.isAdmin = response.Result.IsAdmin;
            this._sessionService.CurrentSession.userAccountUUID = response.Result.AccountUUID;
            this._sessionService.CurrentSession.userUUID = response.Result.UserUUID;
            this._sessionService.CurrentSession.defaultLocationUUID = response.Result.DefaultLocationUUID;
            this._sessionService.CurrentSession.validSession = true;
            this._sessionService.SaveSessionState();
            if (!this.returnUrl) {
                window.location.href  = '../';
            } else {
                window.location.href  = this.returnUrl;
            }
            // this._router.navigate([this.returnUrl ], { relativeTo: this._route });
            },
            err => {
                this.authorizing = false;
                this.msgBox.ShowResponseMessage(err.status, 10);
            }
        );
    }


    LogOut() {
        this.authorizing = true;

        const result = this._userService.logout();
        result.subscribe(
            response => {
                this.authorizing = false;
                if (response.Code !== 200) {

                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }

                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            },
            err => {
                this.authorizing = false;
                this.msgBox.ShowResponseMessage(err.status, 10);
            }
        );
        this._sessionService.ClearSessionState();
    }
}
