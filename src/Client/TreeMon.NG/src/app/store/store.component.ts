
// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
import { Component, OnInit, ViewChild, Input} from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { AppService } from '../services/app.service';

import { DataGridModule, PanelModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { SessionService } from '../services/session.service';
import { ProductService } from '../services/product.service';
import { InventoryService } from '../services/inventory.service';
import { StoreService } from '../services/store.service';
import { GeoService } from '../services/geo.service';
import { AccountService } from '../services/account.service';
import { Account } from '../models/account';
import { InventoryItem } from '../models/inventory';
import { Setting } from '../models/setting';
import { Guid } from '../common/guid';
import { BasicValidators } from '../common/basicValidators';
import { ShoppingCart } from '../models/shoppingcart';

@Component({
    templateUrl: './store.component.html',

    providers: [InventoryService, SessionService, GeoService, AppService, AccountService, ProductService, StoreService]
})
export class StoreComponent implements OnInit {

    @Input() defaultOnly = 'true';

    currencySymbol = '$';
    loadingData = false;
    inventoryItems: any[] = [];
    totalInventoryItems = 0;
    locationFilters: Filter[] = [];
    selectedProduct: InventoryItem;
    selectedProductDetails: any[] = [];
    displayDialog = false;
    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    shoppingCart: ShoppingCart = new ShoppingCart();

    constructor(
        private _geoService: GeoService,
        private _inventoryService: InventoryService,
        private _sessionService: SessionService,
        private _accountService: AccountService,
        private _productService: ProductService,
        private _storeService: StoreService,
        private _appService: AppService,
        private _router: Router,
        private _route: ActivatedRoute) {
    }

    // ===--- General Events ---===

    ngOnInit() {
        const filter = new Filter();
        filter.PageResults = true;
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

        const fltInventory = new Filter();
        fltInventory.PageResults = true;
        fltInventory.StartIndex = 1;
        fltInventory.PageSize = 20;
        this.loadInventory(this._sessionService.CurrentSession.defaultLocationUUID, fltInventory);
        this.createShoppingCart();
    }


    loadInventory(locationUUID: string, filter: Filter) {

        this.loadingData = true;
        const res = this._storeService.getStoreInventory(filter);
        res.subscribe(response => {
            this.loadingData = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.inventoryItems = response.Result;
            this.totalInventoryItems = response.TotalRecordCount;
        }, err => {
            this.loadingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    createShoppingCart(): string {
        let cartUUID = this._sessionService.getCart();

        if (!cartUUID || cartUUID === '') {// if no cart create one.
            this._storeService.getNewShoppingCart().subscribe(response => {

                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this.shoppingCart = new ShoppingCart();
                this.shoppingCart = response.Result;
                cartUUID = this.shoppingCart.UUID;
                this._sessionService.saveCart(cartUUID); // save id  locally for guest purposes
            });
        } else {// get existing cart
            this._storeService.getShoppingCart(cartUUID).subscribe(response => {
                if (response.Code !== 200) {
                    // cart wasn't found in the database. so clear the local cart and create a new one.
                    this._sessionService.clearCart();
                    this.createShoppingCart();
                    return false;
                }
                this.shoppingCart = new ShoppingCart();
                this.shoppingCart = response.Result;
                cartUUID = this.shoppingCart.UUID;
                this._sessionService.saveCart(cartUUID); // save id  locally for guest purposes
            });
        }
        return cartUUID;
    }

    showProductDetail(product) {

        this.displayDialog = true;
        this.selectedProduct = product;

        // call to load keyvalue list of product details/attributes.

        this._productService.getProductDetails(product.ReferenceUUID, product.ReferenceType).subscribe(
            response => {
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this.selectedProductDetails = response.Result;
            });
    }

    onDialogHide() {
        this.displayDialog = false;
    }

    addToCart(event, productUUID) {
        let cartUUID = this._sessionService.getCart();

        if (!cartUUID || cartUUID === '') {// if no cart create one.
            cartUUID = this.createShoppingCart();
            this.shoppingCart = new ShoppingCart();
        }

        this._storeService.getShoppingCart(cartUUID).subscribe(response => {

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.shoppingCart = response.Result;
        });

        const idx = this.getInventoryItem(productUUID);
        if (idx < 0) {
            return false;
        }

        const res = this._storeService.addToCart(this.shoppingCart.UUID, this.inventoryItems[idx], 1.0)
            .subscribe(response => {
                this.loadingData = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this.shoppingCart = response.Result;
        });
    }

    getInventoryItem(productUUID: string): number {
        for (let i = 0; i < this.inventoryItems.length; i++) {

            if (this.inventoryItems[i].UUID === productUUID) {
                return i;
            }
        }
        return -1;
    }

    isNullOrEmpty(link): boolean {
        return BasicValidators.isNullOrEmpty(link);
    }
}
