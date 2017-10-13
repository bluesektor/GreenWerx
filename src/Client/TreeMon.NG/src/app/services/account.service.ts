// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {  Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';

@Injectable()
export class AccountService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    baseUrl(): string {
        return this.BaseUrl();
    }

    addAccount(account) {

        return this.invokeRequest('POST', 'api/Accounts/Add', account);
    }

    deleteAccount(accountUUID) {

        return this.invokeRequest('GET', 'api/Accounts/' + accountUUID + '/Delete', ''    );
    }

    // NOTE: This only gets the accounts the client is a member of.
    //
    getAccounts(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Accounts/?filter=' + JSON.stringify(filter), ''    );
    }

    getAllAccounts(filter?: Filter) {
        return this.invokeRequest('GET', 'api/AllAccounts/?filter=' + JSON.stringify(filter), ''    );
    }

    getAccount(accountUUID) {
        return this.invokeRequest('GET', 'api/AccountsBy/' + accountUUID, ''    );
    }


    getNonMembers(accountUUID) {
        return this.invokeRequest('GET', 'api/Accounts/' + accountUUID + '/NonMembers', ''    );
    }

    getMembers(accountUUID) {
        return this.invokeRequest('GET', 'api/Accounts/' + accountUUID + '/Members', ''    );
    }

    addUsersToAccount(accountUUID: string, users: Node[]) {

        const newMembers = JSON.stringify(users);
        return this.invokeRequest('POST', 'api/Accounts/' + accountUUID + '/Users/Add', newMembers);
    }

    addUserToAccount(accountUUID: string, userUUID: string) {

        return this.invokeRequest('POST', 'api/Accounts/' + accountUUID + '/Users/' + userUUID + '/Add');
    }

    removeUsersFromAccount(accountUUID: string, users: Node[]) {
        const removeMembers = JSON.stringify(users);
        return this.invokeRequest('POST', 'api/Accounts/' + accountUUID + '/Users/Remove', removeMembers);
    }

    setActiveAccount(accountUUID) {
        return this.invokeRequest('GET', 'api/Accounts/SetActive/' + accountUUID, ''    );
    }

    updateAccount(account) {
       return this.invokeRequest('PATCH', 'api/Accounts/Update', account);
    }
}
