// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CheckboxModule, FileUploadModule, InputTextModule } from 'primeng/primeng';

import { SessionService } from '../services/session.service';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule, AccordionModule, AutoCompleteModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { FinanceAccount } from '../models/financeaccount';
import { FinanceService } from '../services/finance.service';
import { AppService } from '../services/app.service';
import { Currency } from '../models/currency';
import { GeoService } from '../services/geo.service';

@Component({
    templateUrl: './financeaccounts.component.html',
    providers: [SessionService, FinanceService, AppService, GeoService]

})
export class FinanceAccountsComponent implements OnInit {

    displayDialog = false;
    newFinanceAccount = false;
    processingRequest = false;
    listData: FinanceAccount[];
    selectedItem: FinanceAccount = new FinanceAccount();
    listCount = 0;
    filter: Filter;
    baseUrl = 'http:// dev.treemon.org/';
    fileUploadUrl = '';
    loadDefaultData = false;
    uploadedFiles: any[] = [];
    currencies: Currency[];
    currency = new Currency();
    selectedLocationType = '';
    locationTypes: string[];
    filteredCurrencies: string[];
    selCurrency = '';


    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService,
        private _geoService: GeoService,
        private _sessionService: SessionService,
        private _financeService: FinanceService) {
        this.msgBox = new MessageBoxesComponent();
    }

    ngOnInit() {
        this.baseUrl = this._appService.BaseUrl();
        this.fileUploadUrl = this._appService.BaseUrl() + 'api/File/Upload/';
        this.selectedItem.Image = '/Content/Default/Images/bank.png';
        this.loadLocationTypes();
        //   this.loadcurrencies();
    }

    initializeUI(page?: number, pageSize?: number) {
        const res = this._financeService.getCurrencies(this.filter);
        res.subscribe(response => {
            this.displayDialog = false;
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.currencies = response.Result;
            this.loadFinanceAccounts(page, pageSize);

        }, err => {
            this.processingRequest = false;

            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    loadFinanceAccounts(page?: number, pageSize?: number) {
        this.processingRequest = true;
        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = page;
        filter.PageSize = pageSize;
        const res = this._financeService.getFinanceAccounts(filter);
        res.subscribe(response => {
            this.displayDialog = false;
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.listData = response.Result;
            this.listCount = response.TotalRecordCount;

        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });
    }

    lazyLoadFinanceAccountList(event) {
        this.initializeUI(event.first, event.rows);
    }


    showDialogToAdd() {
        this.newFinanceAccount = true;
        this.selectedItem = null;
        this.selectedItem = new FinanceAccount();
        this.displayDialog = true;
        this.selectedItem.Image = '/Content/Default/Images/bank.png';
    }

    onRowSelect(event) {
        this.newFinanceAccount = false;
        this.selectedItem = this.cloneItem(event);
        this.displayDialog = true;
    }

    cloneItem(c: FinanceAccount): FinanceAccount {
        const item = new FinanceAccount();
        for (const prop in c) {
            item[prop] = c[prop];
        }
        return item;
    }

    cancel() {
        this.displayDialog = false;
    }

    delete() {
        this.msgBox.closeMessageBox();
        if (confirm('Are you sure you want to delete ' + this.selectedItem.Name + '?')) {
            this.processingRequest = true;
            const res = this._financeService.deleteFinanceAccount(this.selectedItem.UUID);
            res.subscribe(response => {
                this.displayDialog = false;
                this.processingRequest = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                const index = this.findSelectedItemIndex(this.selectedItem.UUID);
                // Here, with the splice method, we remove 1 financeaccountect
                // at the given index.
                this.listData.splice(index, 1); // not updating the list so reload
                this.loadFinanceAccounts(1, 25);
                this.msgBox.ShowMessage('info', 'FinanceAccount deleted.', 10);
            }, err => {
                this.processingRequest = false;
                this.msgBox.ShowResponseMessage(err.status, 10);

                if (err.status === 401) {
                    this._sessionService.ClearSessionState();
                    setTimeout(() => {
                        this._router.navigate(['/membership/login'], { relativeTo: this._route });
                    }, 3000);
                }
            });
        }
    }

    save() {
        this.msgBox.closeMessageBox();
        this.processingRequest = true;
        let res = null;

        if (this.newFinanceAccount) {// add
            res = this._financeService.addFinanceAccount(this.selectedItem);
        } else { // update
            res = this._financeService.updateFinanceAccount(this.selectedItem);
        }

        res.subscribe(response => {

            this.processingRequest = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newFinanceAccount) {// add
                this.msgBox.ShowMessage('info', 'FinanceAccount added', 10);
                this.selectedItem = response.Result;
                this.listData.push(this.selectedItem);
            } else { // update
                this.msgBox.ShowMessage('info', 'FinanceAccount updated', 10);
                this.listData[this.findSelectedItemIndex(this.selectedItem.UUID)] = this.selectedItem;
            }
            this.selectedItem = null;
            this.newFinanceAccount = false;
            this.loadFinanceAccounts(1, 25); // not updating the list so reload for now.
        }, err => {
            this.selectedItem = null;
            this.displayDialog = false;
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });

    }

    onBeforeSendFile(event) {

        event.xhr.setRequestHeader('Authorization', 'Bearer ' + this._sessionService.CurrentSession.authToken);
    }

    onImageUpload(event, itemUUID) {
        let currFile;
        for (const file of event.files) {
            this.uploadedFiles.push(file);
            currFile = file;
        }

        if (this.newFinanceAccount === true) {
            const idx = this.findSelectedItemIndex(itemUUID);
            this.listData[idx].Image = '/Content/Uploads/' + this._sessionService.CurrentSession.userAccountUUID + '/' + currFile.name;
        } else {
            this.selectedItem.Image = '/Content/Uploads/' + this._sessionService.CurrentSession.userAccountUUID + '/' + currFile.name;
        }
    }

    onTabShow(e) {
        console.log('tab index:', e.index);
    }

    findSelectedItemIndex(uuid): number {
        for (let i = 0; i < this.listData.length; i++) {

            if (this.listData[i].UUID === uuid) {
                return i;
            }
        }
        return -1;
    }

    getCurrency(uuid): Currency {

        let filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 1;

        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'UUID';
        screen.Value = uuid;
        filter.Screens.push(screen);


        if (!this.currencies) {
            let currency = new Currency();
            this._financeService.getCurrencies(filter).subscribe(response => {
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 15);
                    return false;
                }
                currency = response.Result;
                if (!currency) {
                    return new Currency();
                }
            });
            return currency;
        }

        for (let i = 0; i < this.currencies.length; i++) {

            if (this.currencies[i].UUID === uuid) {
                return this.currencies[i];
            }
        }

        filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 1;
        let currency = new Currency();
        this._financeService.getCurrencies(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            currency = response.Result;
            if (!currency) {
                return new Currency();
            }
        });
        return currency;
    }

    cboChangeCurrency(event) {
        this.selectedItem.CurrencyUUID = event;
    }

    filterCurrencies(event) {
        this.filteredCurrencies = [];
        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 25;
        const screen = new Screen();
        screen.Command = 'SearchBy';
        screen.Field = 'Name';
        screen.Value = event.query.toLowerCase();
        filter.Screens.push(screen);
        this._financeService.getCurrencies(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            for (let i = 0; i < response.Result.length; i++) {
                this.filteredCurrencies.push(response.Result[i].Name);
            }
        });
    }

    handleDropdownClick() {
        this.filteredCurrencies = [];
        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 25;

        this._financeService.getCurrencies(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }

            for (let i = 0; i < response.Result.length; i++) {
                this.filteredCurrencies.push(response.Result[i].Name);
            }
            console.log('handleDropdownClick.getCurrencies',  this.filteredCurrencies.length );
        });
    }

    onSelectCurrency(value) {

        this._financeService.getCurrency(value).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            this.selectedItem.CurrencyUUID = response.Result.UUID;
        });
    }

    cboLocationTypeChange(newType) {
        this.selectedItem.LocationType = newType;
    }

    loadLocationTypes() {
        this.processingRequest = true;
        const res = this._geoService.getLocationTypes();
        res.subscribe(response => {
            this.displayDialog = false;
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.locationTypes = response.Result;

        }, err => {
            this.processingRequest = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }
        });

    }
}
