// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ElementRef, ViewChild, OnInit, HostListener, Input, Output, EventEmitter  } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { CheckboxModule, FileUploadModule } from 'primeng/primeng';

import { SessionService } from '../../services/session.service';
import { MessageBoxesComponent } from '../../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule, AccordionModule } from 'primeng/primeng';
import { Filter } from '../../models/filter';
import { Screen } from '../../models/screen';
import { FinanceService } from '../../services/finance.service';
import { AppService } from '../../services/app.service';
import { ShoppingCart } from '../../models/shoppingcart';
import { Setting } from '../../models/setting';
import { StoreService } from '../../services/store.service';
import { Address } from '../../models/address';

@Component({
    templateUrl: './paypal.component.html',
    selector: 'tm-finance-gateways-paypal',
    providers: [SessionService, FinanceService, AppService, StoreService]

})
export class PayPalComponent implements OnInit {

    baseUrl: string;
    shoppingCart: ShoppingCart = new ShoppingCart();
    paypalImg = '';
    paypalUrl = ''; // https:// www.paypal.com/cgi-bin/webscr // paypal.use.sandbox   paypal.production.url
    useSandbox = true; // paypal.use.sandbox
    notifyUrl = ''; // paypal.notify.url
    returnUrl = ''; // paypal.return.url
    cancelUrl = ''; // paypal.cancel.url
    paytoEmail = ''; // paypal.payto.email
    customValue = ''; // paypal.custom.value
    imageUrl = ''; // paypal.image.url
    currencyCode = ''; // paypal.currency.code
    itemName = '';
    itemNumber = '';
    itemCount = 0;
    settings: Setting[];
    @Input() financeAccountUUID;
    @Output() checkout = new EventEmitter<any>();

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;
    @ViewChild('btnPayPalSubmit') btnPayPalSubmit: ElementRef;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService,
        private _storeService: StoreService,
        private _sessionService: SessionService,
        private _objService: FinanceService) {
    }

    ngOnInit() {
        this.baseUrl = this._appService.BaseUrl();
        this.paypalImg = this.baseUrl + 'Content/Default/Images/checkout-logo-medium.png';
        const cartUUID = this._sessionService.getCart();


        if (!cartUUID || cartUUID === '') {//  if no cart bail
            return;
        }
        this.customValue = cartUUID;
        this._storeService.getShoppingCart(cartUUID).subscribe(response => {

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return;
            }
            this.shoppingCart = response.Result;
            this.initializeCart();
        });
        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 100;
        this._appService.getPublicSettings(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.settings = response.Result;
            this.initializeSettings();
        });
    }

    initializeCart() {
        this.itemCount = 0;
        this.shoppingCart = this._storeService.calcCartTotals(this.shoppingCart);

        if (this.shoppingCart.CartItems.length === 1) {

            this.itemName = this.shoppingCart.CartItems[0].Name;
            this.itemNumber = this.shoppingCart.CartItems[0].SKU;
            this.itemCount = this.shoppingCart.CartItems[0].Quantity;

        } else {
              for (let i = 0; i < this.shoppingCart.CartItems.length; i++) {
                  this.itemCount += this.shoppingCart.CartItems[i].Quantity;
                if ( i === this.shoppingCart.CartItems.length + 1) {
                    this.itemName += this.shoppingCart.CartItems[i].Name;
                    this.itemNumber += this.shoppingCart.CartItems[i].SKU;
                } else {
                    this.itemName += this.shoppingCart.CartItems[i].Name + ',';
                    this.itemNumber += this.shoppingCart.CartItems[i].SKU + ',';
                }

            }
        }
    }

    initializeSettings() {
        let productionUrl = '';
        let sandboxUrl = '';

        for (let i = 0; i < this.settings.length; i++) {
            switch (this.settings[i].Name.toLowerCase()) {
                case 'paypal.use.sandbox':
                    if (this.settings[i].Value.toLowerCase() === 'false') {
                        this.useSandbox = false;
                    } else {
                        this.useSandbox = true;
                    }
                    break;
                case 'paypal.production.url':
                    productionUrl   = this.settings[i].Value;
                    break;
                case 'paypal.sandbox.url':
                    sandboxUrl   = this.settings[i].Value;
                    break;
                case 'paypal.notify.url':
                    this.notifyUrl = this.settings[i].Value;

                    break;
                case 'paypal.return.url':
                    this.returnUrl = this.settings[i].Value;

                    break;
                case 'paypal.cancel.url':
                    this.cancelUrl = this.settings[i].Value;

                    break;
                case 'paypal.payto.email':
                    this.paytoEmail = this.settings[i].Value;

                    break;
                    // case "paypal.custom.value":// this is set to the cartUUID
                    //     this.customValue = this.settings[i].Value;
                    //    break;
                case 'paypal.image.url':

                    this.imageUrl = this.settings[i].Value;
                    break;
                case 'paypal.currency.code':
                    this.currencyCode = this.settings[i].Value;
                    break;
            }
        }
        if (this.useSandbox === true) {
            this.paypalUrl = sandboxUrl;
        } else {
            this.paypalUrl = productionUrl;
        }
    }

    checkOutPaypal(event) {

        this.checkout.emit({
            FinanceAccountUUID: this.financeAccountUUID,
            Gateway: 'paypal',
            Sandbox: this.useSandbox,
            Payto: this.paytoEmail,
            PaidType: 'email'
        });
       this.btnPayPalSubmit.nativeElement.click();
    }
}
