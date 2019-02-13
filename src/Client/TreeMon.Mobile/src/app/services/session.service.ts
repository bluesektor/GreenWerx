// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable} from '@angular/core';
import 'rxjs/add/operator/map';
import { Session, Role, ServiceResult } from '../models/index';
import { Events } from '@ionic/angular';
import { Api } from './api/api';
import * as moment from 'moment';
import { Storage } from '@ionic/storage';
// import {StorageKeys} from '../services/settings/storagekeys';
import {LocalSettings} from '../services/settings/local.settings';
import {ObjectFunctions} from '../common/object.functions';
import * as jwt_decode from 'jwt-decode';

@Injectable({
    providedIn: 'root'
  })
export class SessionService {

    CurrentSession: Session;
    UserRoles: Role[] = [];

   constructor(
    private api: Api,
    public messages: Events,
    public storage: Storage) {
       this.CurrentSession = new Session();
    }

    isUserInRole(roleName: string): boolean {
      //  console.log('session.service.ts isUserInRole roleName:', roleName);
      //  console.log('session.service.ts isUserInRole Api.authToken:', Api.authToken);
        if ( ObjectFunctions.isNullOrWhitespace(Api.authToken) === true ) {
          //  console.log('session.service.ts isUserInRole invalid authtoken');
            return false;
        }

        const tokens = jwt_decode(Api.authToken);
        if ( ObjectFunctions.isValid(tokens) === false ) {
        //    console.log('session.service.ts isUserInRole tokens:', tokens);
            return false;
        }
        roleName = roleName.toUpperCase();
     //   console.log('session.service.ts isUserInRole jwt_decode tokens:', tokens);
        const roleNames = tokens.roleNames.split(',');
        for (let i = 0; i < roleNames.length; i++) {
            if (roleName === roleNames[i]) {
        //        console.log('session.service.ts isUserInRole TRUE');
                return true;
            }
        }
        console.log('session.service.ts isUserInRole FALSE');
        return false;
    }

    validSession() {

        if (ObjectFunctions.isValid( this.CurrentSession) === false ) {
            console.log('SESSION.SERVICE.TS validSession ObjectFunctions.isValid( this.CurrentSession) = false');
            this.messages.publish('user:logout');
            return false; // no session object so piss off
        }
          // check if session has expired if not persistent.
        console.log('SESSION.SERVICE.TS validSession.this.CurrentSession.IsPersistent:', this.CurrentSession.IsPersistent );

        if (this.CurrentSession.IsPersistent === true) {
            return true; // has a session and it's persistant so no need to check expiration.
        }
        const end = moment(new Date());
        const now =  moment(this.CurrentSession.Expires).local();
        const duration = moment.duration(now.diff(end));
        const ms = duration.asMilliseconds();
        console.log('SESSION.SERVICE.TS validSession this.CurrentSession.expires:', this.CurrentSession.Expires);
        console.log('SESSION.SERVICE.TS validSession ms left in session:', ms);

        if (ms > 0) {
            console.log('SESSION.SERVICE.TS validSession RETURNING TRUE:');
            return true;
        }
        return false;
    }

    logOut(): Promise<any> {
        console.log('session.service.ts logOut');
        this.clearSession();
       if ( this.api.invokeRequest('GET', 'api/Accounts/LogOut', ''    )) {  // .then(() => {
            this.clearSession();
            return this.storage.remove(LocalSettings.HasLoggedIn).then(() => {
                return this.storage.remove( LocalSettings.UserName);
                 }).then(() => {
                     this.messages.publish('user:logout');
              });
        }
    }

    login(userCredentials) {
        return this.api.invokeRequest('POST', 'api/Accounts/Login', userCredentials);
    }

    clearSession() {
        this.UserRoles = [];
         this.CurrentSession = new Session();
        this.CurrentSession.IsAdmin = false;
        this.CurrentSession.Expires = new Date();
        this.CurrentSession.AccountUUID = '';
        this.CurrentSession.UserUUID = '';
        this.CurrentSession.LastSettingUUID = '';
        this.CurrentSession.IsPersistent = false;
        Api.authToken = '';
        this.CurrentSession.ProfileUUID = '';
        this.storage.remove(LocalSettings.SessionToken );
        this.storage.remove(LocalSettings.UserName );
        this.storage.remove(LocalSettings.SessionData );
        this.storage.remove(LocalSettings.HasLoggedIn );
    }

    // When app is loading is calls checkLoginStatus() (basically checks the session)
    // if it returns false then look for local storage to see if there's a key and/or
    // session data to load.
    //
    loadSession() {
        console.log('session.service.ts loadSession');
        this.storage.get(LocalSettings.SessionToken).then(res => {
            console.log('session.service.ts loadSession res:', res);
            if (ObjectFunctions.isValid(res) === false) {
                console.log('SESSION.SERVICE.TS loadSession ObjectFunctions.isValid = false');
                return false; // no token so we can't load any data.
            }
            Api.authToken = res; // set the authorization token
            console.log('SESSION.SERVICE.TS loadSession Api.authToken:', Api.authToken);
            // Load the session data..
            this.storage.get( LocalSettings.SessionData ).then(sData => {
                if (ObjectFunctions.isValid(sData) === false) {
                    this.getSession(Api.authToken).subscribe((sessionResponse) => {
                        const data = sessionResponse as ServiceResult;
                        if (data.Code !== 200) {
                            // this.clearSession();
                            this.messages.publish('api:err', data);
                            // this.messages.publish('user:logout');
                            return false;
                        }
                        this.CurrentSession = data.Result;
                        console.log('SESSION.SERVICE.TS loadSession api.getSession() this.CurrentSession:', this.CurrentSession);
                        this.messages.publish('user:session.loaded');
                    });
                } else {
                    this.CurrentSession = sData;
                    console.log('SESSION.SERVICE.TS loadSession storage.get() this.CurrentSession:', this.CurrentSession);
                    this.messages.publish('user:session.loaded');
                }
            });

            if (ObjectFunctions.isValid( this.CurrentSession) === false ) {
                console.log('SESSION.SERVICE.TS loadSession ObjectFunctions.isValid( this.CurrentSession) = false');
                this.messages.publish('user:logout');
                return false;
            }
        });
    }

    getSession(sessionToken: string) {
       return this.api.invokeRequest('POST', 'api/Sessions' , sessionToken );
    }
}
