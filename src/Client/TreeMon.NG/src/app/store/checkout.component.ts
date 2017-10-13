// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ViewEncapsulation, OnInit, ViewChild, Output, EventEmitter,
         Inject, ChangeDetectorRef, NgZone, ApplicationRef } from '@angular/core';
import { FormsModule, FormBuilder, FormGroup, Validators, FormControl, ReactiveFormsModule } from '@angular/forms';

import { StoreService } from '../services/store.service';
import { Router, ActivatedRoute } from '@angular/router';

import { DataTableModule, SharedModule, DialogModule, AccordionModule, InputSwitchModule } from 'primeng/primeng';
import { StepsModule, MenuItem } from 'primeng/primeng';

import { AddressComponent } from './address.component';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { PayPalComponent } from '../finance/gateways/paypal.component';

import { Address } from '../models/address';
import { FinanceAccount } from '../models/financeaccount';
import { Filter } from '../models/filter';
import { PriceRule } from '../models/pricerule';
import { LoginForm } from '../models/loginform';
import { Screen } from '../models/screen';
import { ShoppingCart } from '../models/shoppingcart';
import { User } from '../models/user';

import { AppService } from '../services/app.service';
import { CookieService } from 'angular2-cookie/services/cookies.service';
import { FinanceService } from '../services/finance.service';
import { SessionService } from '../services/session.service';
import { UserService } from '../services/user.service';

@Component({

    templateUrl: './checkout.component.html',
    styleUrls: [
        './checkout.component.css'
    ],
    encapsulation: ViewEncapsulation.None,
    providers: [SessionService, FinanceService, AppService, StoreService, UserService]
})

    //   See install component for example
export class CheckOutComponent implements OnInit {

    private items: MenuItem[];
    activeIndex = 0;

    step1Valid = false;
    step2Valid = false;
    step3Valid = false;

    enableNext = false;

    maxSteps = 3;
    authorizing = false;
    currencySymbol = '$';
    newUser = true;
    rememberMe = true;
    shoppingCart: ShoppingCart = new ShoppingCart();
    isValidCart = false;
    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    requiresShipping = false; //   if any of the products in the cart are not virtual we'll need a shipping address.

    loggedIn = false;

    formCheckout: FormGroup;
    baseUrl: string;

    customerName = '';
    customerPassword = '';
    shippingFormGroup: FormGroup = new FormGroup({});
    paymentOptions: FinanceAccount[];

    constructor(
        private appr: ApplicationRef,
        private zone: NgZone,
        private cdRef: ChangeDetectorRef,
        @Inject(FormBuilder) fb: FormBuilder,
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService,
        private _cookieService: CookieService,
        private _storeService: StoreService,
        private _userService: UserService,
        private _sessionService: SessionService,
        private _financeService: FinanceService) {

        this.msgBox = new MessageBoxesComponent();

        this.formCheckout = fb.group({
            UserName: '',
            RememberMe: '',
            UserPassword: '',
            ConfirmPassword: '',
            UserEmail: '',
            SecurityQuestion: '',
            UserSecurityAnswer: '',
            CouponCode: '',
        });
    }

    ngOnInit() {
        this.baseUrl = this._appService.BaseUrl();
        this.items = [{ label: 'Account', }, { label: 'Shipping' }, { label: 'Payment' }
        ];

        const filter = new Filter();
        filter.StartIndex = 0;
        filter.PageSize = 1;
        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'Name';
        screen.Value = 'default.currency.symbol';
        filter.Screens.push(screen);

        this._appService.getPublicSettings(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (response.Result.length > 0) {
                this.currencySymbol = response.Result[0].Value;
            }
        });

        const cartUUID = this._sessionService.getCart();

        if (!cartUUID || cartUUID === '') {
            this.isValidCart = false;
            this.msgBox.ShowMessage('error', 'You do not have a any items to checkout!', 15);
            return;
        }

        this._storeService.getShoppingCart(cartUUID).subscribe(response => {

            if (response.Code !== 200) {
                this.isValidCart = false;
                return false;
            }
            this.shoppingCart = response.Result;

            if (this.shoppingCart.CartItems.length <= 0) {
                this.isValidCart = false;
                return false;
            }

            if (this.shoppingCart.ShippingAddress === null) { this.shoppingCart.ShippingAddress = new Address(); }

            if (this.shoppingCart.BillingAddress === null) { this.shoppingCart.BillingAddress = new Address(); }

            if (!this.shoppingCart.customer) { this.shoppingCart.customer = new User(); }

            if (!this.shoppingCart.coupon) {
                this.shoppingCart.coupon = new PriceRule();
            }
            this.shoppingCart.customer.Name = this._cookieService.get('userName');
            this.customerName = this.shoppingCart.customer.Name;

            if (this.shoppingCart.customer.Name && this.shoppingCart.customer.Name.length > 0) {
                this.newUser = false;
            }
            this.requiresShipping = false;
            //    Check if we even need shipping info..
            for (let i = 0; i < this.shoppingCart.CartItems.length; i++) {
                if (this.shoppingCart.CartItems[i].Virtual === false) {
                    this.requiresShipping = true;
                    break;
                }
            }

            if (!this.shoppingCart.discount) { this.shoppingCart.discount = 0; }

            if (!this.shoppingCart.shippingCost) { this.shoppingCart.shippingCost = 0; }

            if (!this.shoppingCart.taxes) { this.shoppingCart.taxes = 0; }

            this.isValidCart = true;
            this.loadPaymentOptions();
            //    If client is logged in set the flags and then check if they have anything that needs
            //    to be shipped.
            if (this._sessionService.CurrentSession.validSession) {
                this.loggedIn = true;
                this.activeIndex = 1; //  don't need to login go to next tab.
                this.step1Valid = true;
                this.shoppingCart.UserUUID = this._sessionService.CurrentSession.userUUID;
                this.shoppingCart.customer.UUID = this._sessionService.CurrentSession.userUUID;
                this.shoppingCart.customer.AccountUUID = this._sessionService.CurrentSession.userAccountUUID;
            }

            if (this.requiresShipping === false || this.shoppingCart.ShippingAddress.isValid === true ) {
                this.activeIndex = 2;
                this.step2Valid = true;
            }
            //  this should put them on the correct tab according to the
            this.setNextStatus();
        });
    }


    onClickNextStep() {
        if (this.activeIndex === this.maxSteps) {
            return;
        }

        this.activeIndex++;
        switch (this.activeIndex) {
            case 1:
                if (this.requiresShipping === false) { //  all virtual
                    this.activeIndex++; //  skip over shipping.
                }
        }
        this.setNextStatus();
    }

    onClickBackStep() {
        if (this.activeIndex === 0) {
            return;
        }
        this.activeIndex--;
        switch (this.activeIndex) {

            case 1:
                if (this.requiresShipping === false) { //  all virtual
                    this.activeIndex--; //  skip back to login page, we don't need to see shipping.
                }
        }
        this.setNextStatus();
    }

    shippingUpdate(event: object) {
        this.setNextStatus();
    }

    setNextStatus() {
        switch (this.activeIndex) {

            case 0:
                this.enableNext = this.step1Valid;
                break;
            case 1:
                if (this.requiresShipping === false) { //  all virtual
                    this.step2Valid = true;
                    this.enableNext = true;
                }else if (this.shoppingCart.ShippingAddress != null) {
                    if ( this.shoppingCart.ShippingAddress.isValid === false) {
                            this.enableNext = false;
                            this.step2Valid = false;
                    } else { //   shipping address is valid..
                        this.step2Valid = true;
                        this.enableNext = true;
                    }
                }
                break;
            case 2:
                this.enableNext = this.step3Valid;
                this.shoppingCart = this._storeService.calcCartTotals(this.shoppingCart);

                break;
        }
    }

    onClickRedeemCoupon(event) {
       this.msgBox.closeMessageBox();

        this._financeService.getPriceRule(this.shoppingCart.coupon.Code).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }

            this.shoppingCart.coupon = response.Result;
            this.shoppingCart = this._storeService.calcCartTotals(this.shoppingCart);

        }, err => {
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    login(event) {
        this.enableNext = false;
        this.step1Valid = false;
        this.msgBox.closeMessageBox();

        this.authorizing = true;
        if (this.rememberMe) {
            this._cookieService.put('userName', this.formCheckout.get('UserName').value);
        } else {
            this._cookieService.remove('userName');
        }
        const frmLogin = new LoginForm();
        frmLogin.ClientType = 'web';
        frmLogin.Password = this.formCheckout.get('UserPassword').value;
        frmLogin.UserName = this.formCheckout.get('UserName').value;

        const result = this._userService.login(frmLogin);
        result.subscribe(
            response => {
                this.authorizing = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }

                this.step1Valid = true;
                this.loggedIn = true;
                this.formCheckout.markAsPristine();
                this._sessionService.CurrentSession.authToken = response.Result.Authorization;
                this._sessionService.CurrentSession.isAdmin = response.Result.IsAdmin;  //  .set('userUUID', response.UserUUID)
                this._sessionService.CurrentSession.userAccountUUID = response.Result.AccountUUID;
                this._sessionService.CurrentSession.userUUID = response.Result.UserUUID;
                this._sessionService.CurrentSession.defaultLocationUUID = response.Result.DefaultLocationUUID;
                this.shoppingCart.AccountUUID = response.Result.AccountUUID;
                this.shoppingCart.UserUUID = response.Result.UserUUID;
                this.shoppingCart.CreatedBy = response.Result.UserUUID;
                this.shoppingCart.customer.UUID = response.Result.UserUUID;
                this.shoppingCart.customer.AccountUUID = response.Result.AccountUUID;

                //    this._sessionService.CurrentSession.sessionExpires.setMinutes(response.Result.SessionLength);
                this._sessionService.CurrentSession.validSession = true;
                this._sessionService.SaveSessionState();
                this.onClickNextStep(); //  move to next step so user doesn't have to click
            },
            err => {
                this.authorizing = false;
                this.msgBox.ShowResponseMessage(err.status, 10);

                if (err.status === 401) {
                    this._sessionService.ClearSessionState();
                    setTimeout(() => {
                        this._router.navigate(['/membership/login'], { relativeTo: this._route });
                    }, 3000);
                }

            }
        );
    }

    logOut() {
        this.authorizing = true;

        const result = this._userService.logout();
        result.subscribe(
            response => {
                this.authorizing = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
            },
            err => {
                this.authorizing = false;
                this.msgBox.ShowResponseMessage(err.status, 10);

                if (err.status === 401) {
                    this._sessionService.ClearSessionState();
                    setTimeout(() => {
                        this._router.navigate(['/membership/login'], { relativeTo: this._route });
                    }, 3000);
                }

            }
        );
        this._sessionService.ClearSessionState();
    }

    registerUser(event) {
     //     this.enableNext = false;
        this.step1Valid = false;
        this.msgBox.closeMessageBox();
        this.authorizing = true;
        this.shoppingCart.customer.Name = this.formCheckout.get('UserName').value;
        this.shoppingCart.customer.Email = this.formCheckout.get('UserEmail').value;
        this.shoppingCart.customer.Password = this.formCheckout.get('UserPassword').value;
        this.shoppingCart.customer.ConfirmPassword = this.formCheckout.get('ConfirmPassword').value;
        this.shoppingCart.customer.PasswordQuestion = this.formCheckout.get('SecurityQuestion').value;
        this.shoppingCart.customer.PasswordAnswer = this.formCheckout.get('UserSecurityAnswer').value;

        const result = this._userService.register(this.shoppingCart.customer);
        result.subscribe(response => {
            this.authorizing = false;
            if (response.Code !== 200 || response.Message !== '' ) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                if (response.Message !== '') {
                   //   this.enableNext = true;
                    this.step1Valid = true;
                    this.onClickNextStep();
                }
                return false;
            }
            this.formCheckout.markAsPristine();
           //   this.enableNext = true;
            this.step1Valid = true;
            this.onClickNextStep(); //  move to next step so user doesn't have to click
        }, err => {
            this.authorizing = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    toggleShippingSameAsBilling(event) {
        if (this.shoppingCart.shippingSameAsBiling) {
            this.shoppingCart.ShippingAddress = this.cloneAddress( this.shoppingCart.BillingAddress);
        } else {
            this.shoppingCart.ShippingAddress.ContactName = '';
            this.shoppingCart.ShippingAddress.StreetAddress1 = '';
            this.shoppingCart.ShippingAddress.StreetAddress2 = '';
            this.shoppingCart.ShippingAddress.City = '';
            this.shoppingCart.ShippingAddress.State = '';
            this.shoppingCart.ShippingAddress.PhoneNumber = '';
            this.shoppingCart.ShippingAddress.PostalCode = '';


        }
    }

    cloneAddress(a: Address): Address {
        const address = new Address();
        for (const prop in a) {
            address[prop] = a[prop];
        }
        return address;
    }

 

    loadPaymentOptions(page?: number, pageSize?: number) {

        //  var filter = new Filter();
        //  var screen = new Screen();
        //  screen.Command = "SEARCHBY";
        //  screen.Field = "LocationType";
        //  screen.Value = 'ONLINE STORE';
        //  filter.Screens.push(screen);

        const res = this._financeService.getPaymenOptions();
        res.subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.paymentOptions = response.Result;

        });
    }

    saveCartToServer(event: any) {

        //  note: when loading the payment options in the html set an input for the uuid, then just pass that uuid back through this event.
        this.shoppingCart.FinanceAccountUUID = event.FinanceAccountUUID;
        this.shoppingCart.PaidTo = event.Payto;
        this.shoppingCart.PaidType = event.PaidType;
        this.shoppingCart.IsPaymentSandbox = event.Sandbox;
        this.shoppingCart.PaymentGateway = event.Gateway;

        this._storeService.checkOut(this.shoppingCart).subscribe(response => {
            //  post info to server and save
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }


            this.shoppingCart = new ShoppingCart();
            this._sessionService.clearCart();

            const filter = new Filter();
            filter.PageResults = true;
            filter.StartIndex = 1;
            filter.PageSize = 1;

            const screen = new Screen();
            screen.Command = 'SearchBy';
            screen.Field = 'Name';
            screen.Value = 'SiteDomain';
            filter.Screens.push(screen);

            this._appService.getPublicSettings(filter).subscribe( response => {
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                const domain = response.Result;
                //  todo get app setting if this a  POS system don't show this message
                this.msgBox.ShowMessage('info',
                    'Thank you for shopping at ' + domain +
                    '. A confirmation email will be sent. Check your spam/junk folder for the confirmation email if you have not received it.', 35);
            });

        });
    }

}
