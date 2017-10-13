// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {  Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';


@Injectable()
export class UserService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    register(user) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Accounts/Register', JSON.stringify(user), options);
    }

    addUser(user) {
        return this.invokeRequest('POST', 'api/Users/Add', JSON.stringify(user));

    }

    changePassword(frmChangePassword) {

        if (frmChangePassword.resetPassword) {
            const headers = new Headers({ 'Content-Type': 'application/json' });
            const options = new RequestOptions({ headers: headers });
            return this.invokeRequest('POST', '/api/Accounts/ChangePassword', frmChangePassword,  options);
        }
        return this.invokeRequest('POST', '/api/Accounts/ChangePassword', frmChangePassword);
    }

    deleteUser(userUUID) {
        return this.invokeRequest('DELETE', '/api/Users/Delete/' + userUUID, '');
    }

    getUsers() {
        return this.invokeRequest('GET', 'api/Users/' , ''    );
    }

    getUser(userId) {
        return this.invokeRequest('GET', 'api/UsersBy/' + userId);
    }

    login(userCredentials) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Accounts/Login', userCredentials, options);
    }

    logout() {
        return this.invokeRequest('GET', 'api/Accounts/LogOut', ''    );
    }

    sendUserInfo(userCredentials) {

        const headers = new Headers({ 'Content-Type': 'application/json'  });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Accounts/SendInfo', userCredentials, options);
    }

    updateUser(user) {
       return this.invokeRequest('PATCH', 'api/Users/Update', user);
    }


    private getUserUrl(userId) {
        return this._url + '/' + userId;
    }

    validateUser( validationType: string, operation: string, validationCode: string) {
        const headers = new Headers({ 'Content-Type': 'application/json'  });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest( 'POST', 'api/Users/Validate/type/' + validationType +
                                           '/operation/' + operation +
                                           '/code/' + validationCode, '', options);
    }
}
