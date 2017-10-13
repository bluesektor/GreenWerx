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
import { Order } from '../models/order';
import { OrdersService } from '../services/orders.service';
import { AppService } from '../services/app.service';

@Component({
    templateUrl: './orders.component.html',
    providers: [SessionService, AppService, OrdersService]

})
export class OrdersComponent implements OnInit {

    displayDialog = false;
    newItem = false;
    processingRequest = false;
    listData: Order[];
    selectedItem: Order = new Order();
    listCount = 0;
    baseUrl: string;
    fileUploadUrl = '';
    loadDefaultData = false;
    uploadedFiles: any[] = [];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService,
        private _sessionService: SessionService,
        private _orderService: OrdersService) {
        this.msgBox = new MessageBoxesComponent();
    }

    ngOnInit() {
        this.baseUrl = this._appService.BaseUrl();
        this.fileUploadUrl = this._appService.BaseUrl() + 'api/File/Upload/';
    }



    loadOrders(page?: number, pageSize?: number) {

        this.processingRequest = true;
        const filter = new Filter();
        filter.PageResults = true;
        filter.PageSize = pageSize;
        filter.StartIndex = page;

        if (this.loadDefaultData === true) {


            const screen = new Screen();
            screen.Field = 'NAME';
            screen.Command = 'DISTINCTBY';
            screen.Value = '';
            filter.Screens.push(screen);

            this._appService.getDefaults('Order', filter).subscribe(response => {
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                this.listData = response.Result;
                this.listCount = response.TotalRecordCount;
            });
        } else {

            const res = this._orderService.getOrders(filter);
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

    }

    lazyLoadOrderList(event) {
        this.loadOrders(event.first, event.rows);
    }

    loadSystemData() {

    }


    showDialogToAdd() {
        this.newItem = true;
        this.selectedItem = null;
        this.selectedItem = new Order();
        this.displayDialog = true;
    }

    onRowSelect(event) {
        this.newItem = false;
        this.selectedItem = this.cloneItem(event);
        this.displayDialog = true;
    }

    cloneItem(c: Order): Order {
        const item = new Order();
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
            const res = this._orderService.deleteOrder(this.selectedItem.UUID);
            res.subscribe(response => {
                this.displayDialog = false;
                this.processingRequest = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                const index = this.findSelectedItemIndex(this.selectedItem.UUID); // this.locations.indexOf(this.location)
                // Here, with the splice method, we remove 1 orderect
                // at the given index.
                this.listData.splice(index, 1);
                this.msgBox.ShowMessage('info', 'Order deleted.', 10);
                this.loadOrders(1, 25);  // not updating the list so reload for now.
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

        if (this.newItem) {// add
            res = this._orderService.addOrder(this.selectedItem);
        } else { // update
            res = this._orderService.updateOrder(this.selectedItem);
        }

        res.subscribe(response => {

            this.processingRequest = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newItem) {// add
                this.msgBox.ShowMessage('info', 'Order added', 10);
                this.selectedItem = response.Result;
                this.listData.push(this.selectedItem);
            } else { // update
                this.msgBox.ShowMessage('info', 'Order updated', 10);
                this.listData[this.findSelectedItemIndex(this.selectedItem.UUID)] = this.selectedItem;
            }
            this.selectedItem = null;
            this.newItem = false;
            this.loadOrders(1, 25);  // not updating the list so reload for now.
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

    onTabShow(e) {
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
