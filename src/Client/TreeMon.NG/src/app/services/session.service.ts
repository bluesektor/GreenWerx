// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable} from '@angular/core';
import 'rxjs/add/operator/map';
import { CookieService } from 'angular2-cookie/services/cookies.service';
import { Session } from '../models/session';
import { ShoppingCart } from '../models/shoppingcart';
import { InventoryItem } from '../models/inventory';
import { User } from '../models/user';
import { Address } from '../models/address';

@Injectable()
export class SessionService  {
    CurrentSession: Session;

    constructor( private _cookieService: CookieService) {

        this.CurrentSession = new Session();
        this.CurrentSession.sessionExpires = new Date();
        this.CurrentSession.validSession = false;
        this.CurrentSession.isAdmin = false;
        this.CurrentSession.defaultLocationUUID = '';
        this.CurrentSession.authToken = '';
        this.CurrentSession.userAccountUUID = '';
        this.CurrentSession.userUUID = '';
        this.LoadSessionState();
    }

    // Saves properties to cookie
    //
    SaveSessionState() {
        const session = JSON.stringify(this.CurrentSession);
        this._cookieService.put('session', session);
    }

    ClearSessionState() {
        this.CurrentSession.authToken = '';
        this.CurrentSession.isAdmin = false;
        this.CurrentSession.sessionExpires = new Date();
        this.CurrentSession.userAccountUUID = '';
        this.CurrentSession.userUUID = '';
        this.CurrentSession.validSession = false;
        this.CurrentSession.defaultLocationUUID = '';
        this.SaveSessionState();
    }

    LoadSessionState() {
        const cookie = this._cookieService.get('session');
        if (cookie === null || cookie === undefined) {
            return;
        }
        const temp = JSON.parse(cookie);

        if (temp != null && temp !== undefined) {
            this.CurrentSession = temp;
        }
    }


    // Use this when the user is a guest or there is no connection.
    saveCart(shoppingCartUUID: string) {
        this._cookieService.put('shoppingcart', shoppingCartUUID);
    }

    getCart(): string {
        const cookie = this._cookieService.get('shoppingcart');
        if (cookie === null || cookie === undefined) {
            return '';
        }
        return cookie;
    }

    clearCart() {
        this._cookieService.remove('shoppingcart');
    }
}
