// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import {  Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from '../api/api'; // '../api/api.service';
import { Account, EventLocation , Favorite, Filter,  Screen} from '../../models/index';
import { Observable, of as observableOf} from 'rxjs';

@Injectable({
    providedIn: 'root'
  })
export class AccountService  {

    public AvailableScreens:  Screen[] = []; // cache th

    public Favorites: Favorite[] = [];

    public Categories: string[] = [];

       // These are selected screens by user in the event-filter.ts
   // NOTE: in the filter dialog this only supports boolean fields i.e. private, active..
   // public EventScreens: Screen[] = [];
   public AccountFilter: Filter = new Filter();

    constructor(private api: Api ) {    }

    addAccount(account) {
        return this.api.invokeRequest('POST', 'api/Accounts/Add', account);
    }

    getAccountCategories() {
        return this.api.invokeRequest('GET', 'api/Accounts/Categories' );
    }

    deleteAccount(accountUUID) {
        return this.api.invokeRequest('GET', 'api/Accounts/' + accountUUID + '/Delete', ''    );
    }

    // NOTE: This only gets the accounts the user is a member of.
    //
    getAccounts(filter?: Filter) {
        return this.api.invokeRequest('POST', 'api/Accounts' , JSON.stringify(filter)     );
    }

    getAllAccounts(filter?: Filter) {
        console.log('account.service.ts getAllAccounts filter:', filter);
        return this.api.invokeRequest('POST', 'api/AllAccounts' , JSON.stringify(filter)   );
    }

    getAccount(accountUUID) {
        return this.api.invokeRequest('GET', 'api/AccountsBy/' + accountUUID, ''    );
    }


    getNonMembers(accountUUID) {
        return this.api.invokeRequest('GET', 'api/Accounts/' + accountUUID + '/NonMembers', ''    );
    }

    getMembers(accountUUID) {
        return this.api.invokeRequest('GET', 'api/Accounts/' + accountUUID + '/Members', ''    );
    }

    addUsersToAccount(accountUUID: string, users: Node[]) {

        const newMembers = JSON.stringify(users);
        return this.api.invokeRequest('POST', 'api/Accounts/' + accountUUID + '/Users/Add', newMembers);
    }

    addUserToAccount(accountUUID: string, userUUID: string) {
        return this.api.invokeRequest('POST', 'api/Accounts/' + accountUUID + '/Users/' + userUUID + '/Add');
    }

    removeUsersFromAccount(accountUUID: string, users: Node[]) {
        const removeMembers = JSON.stringify(users);
        return this.api.invokeRequest('POST', 'api/Accounts/' + accountUUID + '/Users/Remove', removeMembers);
    }

    setActiveAccount(accountUUID) {
        return this.api.invokeRequest('GET', 'api/Accounts/SetActive/' + accountUUID, ''    );
    }

    updateAccount(account) {
       return this.api.invokeRequest('PATCH', 'api/Accounts/Update', account);
    }

    addFavorite(accountUUID: string):  Observable<Object> {
        return this.api.invokeRequest('POST', 'api/Accounts/' + accountUUID + '/Favorite'  );
    }

    removeFavorite(accountUUID: string):  Observable<Object> {
        return this.api.invokeRequest('DELETE', 'api/Accounts/' + accountUUID + '/Favorite'  );
    }

     // Returns reminders flagged as favorite
     getFavorites(filter: Filter) {
        return this.api.invokeRequest('POST', 'api/Accounts/Favorites', JSON.stringify(filter) );
    }
}
