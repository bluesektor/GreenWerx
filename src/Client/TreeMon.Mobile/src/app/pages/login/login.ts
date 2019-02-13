import { Component, ViewEncapsulation } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';

import { SessionService } from '../../services/session.service';
import { UserService } from '../../services';
import {LoginForm} from './loginform';
import {  Session, ServiceResult } from '../../models/index';
import { Events } from '@ionic/angular';
import { Api } from '../../services';
import { ProfileService} from '../../services';
import { Storage } from '@ionic/storage';
 import {LocalSettings} from '../../services/settings/local.settings';
import {SendAccountInfoForm} from '../password/SendAccountInfoForm';

@Component({
  selector: 'page-login',
  templateUrl: 'login.html',
  styleUrls: ['./login.scss'],
  providers: [ProfileService],
  encapsulation: ViewEncapsulation.None
})
export class LoginPage {
  submitted = false;
  processing = false;
  login: LoginForm = new LoginForm();
  sessionLoaded = false;
  showMessage = false;
  message = '';

  constructor(
    private user: UserService,
    private events: Events,
    private session: SessionService,
    public router: Router,
    public messages: Events,
    public profleService: ProfileService,
    public storage: Storage ) {

    this.events.subscribe('login.load.data', (data) => {
        // this fires after login and session is loaded.
        console.log('login.ts event fired:', data);
        console.log('login.ts event fired this.sessionLoaded :', this.sessionLoaded );

        if (this.sessionLoaded === true ) {
          this.router.navigateByUrl('/tabs/home');
        }
    });
  }

  async onLogin(form: NgForm) {
    this.submitted = true;
    this.showMessage = false;
    this.message = '';

    if (!form.valid) { console.log('login.ts onLogin form invalid.'); return; }

    this.processing = true;

    this.login.ClientType = 'web';

    this.session.CurrentSession.IsPersistent = this.login.RememberMe;
    console.log('LOGIN.TS onLogin this.session.CurrentSession.IsPersistent :', this.session.CurrentSession.IsPersistent );
    if ( this.login.RememberMe ) {
      this.login.ClientType = 'mobile.app'; // this will persist the session on the server so they won't have to login all the time.
    }

    await this.session.login(this.login).subscribe((response) => {
      console.log('LOGIN.TS onLogin response:', response);

      const data = response as ServiceResult;
      this.processing = false;
       if (data.Code !== 200) {
          this.showMessage = true;
          this.message = data.Message;
          this.messages.publish('api:err', data);
          return false;
      }
      console.log('LOGIN.TS onLogin data.Result.UserRoles:', data.Result.UserRoles);
      this.session.UserRoles = data.Result.UserRoles;
      this.session.CurrentSession.IsPersistent = this.login.RememberMe;

      Api.authToken = data.Result.Authorization;
      console.log('LOGIN.TS onLogin Api.authToken:', Api.authToken);
      this.profleService.CurrentProfile = data.Result.Profile;
      console.log('LOGIN.TS  onLogin this.profleService.CurrentProfile:', this.profleService.CurrentProfile);
      this.initializeSession();
      this.messages.publish('user:login'); // events.ts is also listening so it can load favorites.
    });
  }

  onSignup() {
    this.router.navigateByUrl('/signup');
  }

  initializeSession() {
    console.log('LOGIN.TS onLogin LoadSession.  this.session :',  this.session);
      this.session.getSession(Api.authToken).subscribe(sessionResponse => {
        console.log('LOGIN.TS onLoging getSession. Api.authToken :', Api.authToken);
        const sessionData = sessionResponse as ServiceResult;
        console.log('LOGIN.TS onLogin LoadSession.sessionData :', sessionData);
        if (sessionData.Code !== 200) {
          this.messages.publish('api:err', sessionData);
            return false;
        }
        this.session.CurrentSession =  sessionData.Result as Session;
        this.session.CurrentSession.IsPersistent = this.login.RememberMe;
        this.sessionLoaded = true;
        if ( this.login.RememberMe === true) {
          this.storage.set(LocalSettings.SessionToken, Api.authToken);
          this.storage.set(LocalSettings.UserName, this.login.UserName);
          this.storage.set(LocalSettings.SessionData, this.session.CurrentSession);
          this.storage.set(LocalSettings.HasLoggedIn, true);
        }
        this.messages.publish('login.load.data', 'session');
    }, (err) => {
      this.processing = false;
      this.messages.publish('service:err', err);
    });
  }

  async onResetPassword(form: NgForm, forgotPassword: boolean) {
    this.processing = true;
    const frmChange = new SendAccountInfoForm();
    frmChange.ForgotPassword = forgotPassword; // if false send account info email, true send password reset link
    frmChange.Email = this.login.UserName;
    await this.user.sendUserInfo(frmChange).subscribe((response) => {
      this.processing = false;
          const data = response as ServiceResult;
          if (data.Code !== 200) {
            this.messages.publish('api:err', data);
            return false;
          }
          this.messages.publish('api:ok', 'Please check your email for instructions on updating your password.');
         }, (err) => {
          this.processing = false;
          this.messages.publish('service:err', err);
         });
  }
}
