// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms'; //
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { StoreService } from '../services/store.service';
import { SessionService } from '../services/session.service';
import { ShoppingCart } from '../models/shoppingcart';

@Component({

    templateUrl: './cartdetail.component.html',
    providers: [StoreService, SessionService]

})


export class CartDetailComponent implements OnInit {

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    subTotal = 0;
    totalQuantity = 0;
    shoppingCart: ShoppingCart = new ShoppingCart();
    isValidCart = false;

    constructor(
        private _sessionService: SessionService,
        private _storeService: StoreService ) {

       this.msgBox = new MessageBoxesComponent();
    }

    ngOnInit() {

        const cartUUID = this._sessionService.getCart();

        if (!cartUUID || cartUUID === '') {// if no cart create one.
            this.isValidCart = false;
            return false;
        }

        this._storeService.getShoppingCart(cartUUID).subscribe(response => {

            if (response.Code !== 200) {
                this.isValidCart = false;
                return false;
            }
            this.shoppingCart = response.Result;

            if (this.shoppingCart == null || this.shoppingCart.CartItems.length <= 0 ) {
                this.isValidCart = false;
                return false;
            }
            this.calcCartTotals();
            this.isValidCart = true;
        });
    }

    removeItemFromCart(event, cartItemUUID) {

        const cartUUID = this._sessionService.getCart();

        const idx = this.findCartItemIndex(cartItemUUID);
        const itm = this.shoppingCart.CartItems[idx];
        this._storeService.deleteCartItem(this.shoppingCart.UUID, cartItemUUID, itm.UUID ).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.shoppingCart = response.Result;

            this.calcCartTotals();
        });
    }

    calcCartTotals() {
        this.subTotal = 0;
        this.totalQuantity = 0;

        if (!this.shoppingCart || !this.shoppingCart.CartItems) {
            return;
        }

        for (let i = 0; i < this.shoppingCart.CartItems.length; i++) {
            this.subTotal += this.shoppingCart.CartItems[i].Price;
            this.totalQuantity += this.shoppingCart.CartItems[i].Quantity;
        }
    }

    findCartItemIndex(cartItemUUID: string): number {

        if (!this.shoppingCart || !this.shoppingCart.CartItems) {
            return -1;
        }
        for (let i = 0; i < this.shoppingCart.CartItems.length; i++) {

            if (this.shoppingCart.CartItems[i].CartItemUUID === cartItemUUID) {
                return i;
            }
        }
        return -1;
    }

    payForItems() {

    }
}
