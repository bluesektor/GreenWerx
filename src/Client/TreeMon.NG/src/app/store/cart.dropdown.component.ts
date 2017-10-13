// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit } from '@angular/core';
import { SessionService } from '../services/session.service';
import { StoreService } from '../services/store.service';
import { ShoppingCart } from '../models/shoppingcart';

@Component({
    selector: 'tm-cart-dropdown',
    template: `
    <li *ngIf="showInMenubar" class="dropdown" (click)="toggleCartDropDown()" [class.open]="cartDropDownExpanded" >
            <a aria-expanded="false" class="dropdown-toggle" data-toggle="dropdown" >
                <i class="fa fa-shopping-cart fa-fw"></i>  <i class="fa fa-caret-down"></i>
            </a>
            <ul  class="dropdown-menu dropdown-user">
                <li  id="mnuAdminItem"><a routerLink="/store/shoppingcart"><i class="fa fa-gear fa-fw"></i> View Cart</a></li>
                <li class="divider"></li>
                <li><a routerLink="/store/checkout" routerLinkActive="active"><i class="fa fa-sign-out fa-fw"></i> Check out</a></li>

            </ul>
    </li>`,
    providers: [SessionService, StoreService]

})

export class CartDropdownComponent implements OnInit {

    cartDropDownExpanded = false;
    showInMenubar = false;

    constructor(private _sessionService: SessionService,
        private _storeService: StoreService) {
    }

    ngOnInit() {

        const cartUUID = this._sessionService.getCart();

        if (!cartUUID || cartUUID === '') {// if no cart create one.
            this.showInMenubar = false;
            return false;
        }

        this._storeService.getShoppingCart(cartUUID).subscribe(response => {

            if (response.Code !== 200) {
                this.showInMenubar = false;
                return false;
            }
            const shoppingCart = response.Result;

            if (shoppingCart == null || shoppingCart.CartItems.length <= 0) {
                this.showInMenubar = false;
                return false;
            }
        });
        this.showInMenubar = true;
    }

    toggleCartDropDown() {
        this.cartDropDownExpanded = !this.cartDropDownExpanded;

    }
}
