// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {  Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from '../api/api'; // '../api/api.service';
import { Message } from '../../models/message';

@Injectable({
  providedIn: 'root'
})
export class UserService  {

    constructor(private api: Api ) {

    }

    register(user) {
        return this.api.invokeRequest('POST', 'api/Accounts/Register', JSON.stringify(user));
    }

    addUser(user) {
        return this.api.invokeRequest('POST', 'api/Users/Add', JSON.stringify(user));

    }

    changePassword(frmChangePassword) {

        if (frmChangePassword.resetPassword) {
            return this.api.invokeRequest('POST', '/api/Accounts/ChangePassword', frmChangePassword);
        }
        return this.api.invokeRequest('POST', '/api/Accounts/ChangePassword', frmChangePassword);
    }

    deleteUser(userUUID) {
        return this.api.invokeRequest('DELETE', '/api/Users/Delete/' + userUUID, '');
    }

    getUsers() {
        return this.api.invokeRequest('GET', 'api/Users/' , ''    );
    }

    getUser(userId) {
        return this.api.invokeRequest('GET', 'api/UsersBy/' + userId);
    }

    test() {
      return this.api.invokeRequest('GET', 'api/Tools/TestCode', ''    );
    }

    contactUser(message: Message) {
      return this.api.invokeRequest('POST', 'api/Users/Message' ,  JSON.stringify(message));
    }

    contactAdmin(message: Message) {
        return this.api.invokeRequest('POST', 'api/Site/SendMessage' ,  JSON.stringify(message));
      }

    sendUserInfo(userCredentials) {
        return this.api.invokeRequest('POST', 'api/Accounts/SendInfo', userCredentials);
    }

    updateUser(user) {
       return this.api.invokeRequest('PATCH', 'api/Users/Update', user);
    }

    getProfile() {
      return this.api.invokeRequest('GET', 'api/Users/Profile');
    }

    saveProfile(profile: any) {
      return this.api.invokeRequest('GET', 'api/Users/Save', profile);
    }

    validateUser( validationType: string, operation: string, validationCode: string) {
         return this.api.invokeRequest( 'POST', 'api/Users/Validate/type/' + validationType +
                                           '/operation/' + operation +
                                           '/code/' + validationCode, '');
    }
}
