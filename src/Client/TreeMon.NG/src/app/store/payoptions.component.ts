// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CheckboxModule, FileUploadModule } from 'primeng/primeng';

import { SessionService } from '../services/session.service';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule, AccordionModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { FinanceAccount } from '../models/financeaccount';
import { FinanceService } from '../services/finance.service';
import { AppService } from '../services/app.service';


@Component({
    templateUrl: './payoptions.component.html',
    providers: [SessionService, FinanceService, AppService]

})
export class PayOptionsComponent implements OnInit {

    displayDialog = false;
    newItem = false;
    processingRequest = false;
    listData: FinanceAccount[];
    selectedItem: FinanceAccount = new FinanceAccount();
    listCount = 0;
    filters: Filter[];
    baseUrl: string;
    fileUploadUrl = '';
    loadDefaultData = false;
    defaultFilters: Filter[] = [];
    uploadedFiles: any[] = [];


    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService,
        private _sessionService: SessionService,
        private _financeaccountService: FinanceService) {
        this.msgBox = new MessageBoxesComponent();
    }

    ngOnInit() {
        this.baseUrl = this._appService.BaseUrl();
        this.fileUploadUrl = this._appService.BaseUrl() + 'api/File/Upload/';
        this.selectedItem.Image = '/Content/Default/Images/add.png';
    }



    loadFinanceAccounts(page?: number, pageSize?: number) {
        this.processingRequest = true;
        this.defaultFilters = [];

        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = page;
        filter.PageSize = pageSize;
        const screen = new Screen();
        filter.Screens.push(screen);

        const res = this._financeaccountService.getFinanceAccounts(filter);
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
        this.loadFinanceAccounts(event.first, event.rows);
    }


    showDialogToAdd() {
        this.newItem = true;
        this.selectedItem = null;
        this.selectedItem = new FinanceAccount();
        this.displayDialog = true;
    }

    onRowSelect(event) {
        this.newItem = false;
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
            const res = this._financeaccountService.deleteFinanceAccount(this.selectedItem.UUID);
            res.subscribe(response => {
                this.displayDialog = false;
                this.processingRequest = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                const index = this.findSelectedItemIndex(this.selectedItem.UUID); // this.locations.indexOf(this.location)
                // Here, with the splice method, we remove 1 financeaccountect
                // at the given index.
                this.listData.splice(index, 1);
                this.msgBox.ShowMessage('info', 'FinanceAccount deleted.', 10   );
                this.loadFinanceAccounts(1, 25); // not updating the list so reload for now.
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
        const res =  this._financeaccountService.updateFinanceAccount(this.selectedItem);


        res.subscribe(response => {

            this.processingRequest = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newItem) {// add
                this.msgBox.ShowMessage('info', 'FinanceAccount added', 10   );
                this.listData.push(this.selectedItem);
            } else { // update
                this.msgBox.ShowMessage('info', 'FinanceAccount updated', 10    );
                this.listData[this.findSelectedItemIndex(this.selectedItem.UUID)] = this.selectedItem;
            }
            this.selectedItem = null;
            this.newItem = false;
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

        if (this.newItem === true) {
            const idx = this.findSelectedItemIndex(itemUUID);
            this.listData[idx].Image = '/Content/Uploads/' + this._sessionService.CurrentSession.userAccountUUID + '/' + currFile.name;
        } else {
            this.selectedItem.Image = '/Content/Uploads/' + this._sessionService.CurrentSession.userAccountUUID + '/' + currFile.name;
        }
    }

    findSelectedItemIndex(uuid): number {
        for (let i = 0; i < this.listData.length; i++) {

            if (this.listData[i].UUID === uuid) {
                return i;
            }
        }
        return -1;
    }
}
