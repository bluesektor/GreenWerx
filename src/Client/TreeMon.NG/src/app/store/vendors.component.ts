// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, ViewChild, OnInit, Output, EventEmitter, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { BasicValidators } from '../common/basicValidators';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { SessionService } from '../services/session.service';
import { AccordionModule } from 'primeng/primeng';
import { CheckboxModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule } from 'primeng/primeng';

import { ProductService } from '../services/product.service';
import { Vendor } from '../models/vendor';

@Component({
    templateUrl: './vendors.component.html',

    providers: [ProductService, ConfirmationService, SessionService]
})
export class VendorsComponent implements OnInit {

    loadingData = false;
    deletingData = false;
    processingRequest = false;
    displayDialog = false;
    vendor: Vendor = new Vendor();
    newVendor = false;
    vendors: Vendor[];
    vendorFilter: Filter = new Filter();

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(fb: FormBuilder,
        private _productService: ProductService,
        private _confirmationService: ConfirmationService,
        private _sessionService: SessionService,
        private _router: Router,
        private _route: ActivatedRoute) {
    }

    ngOnInit() {

        if (!this._sessionService.CurrentSession.validSession) {
            return;
        }

        this.loadingData = true;
        this.loadVendors();
    }

    loadVendors() {

        const res = this._productService.getVendors(this.vendorFilter);
        res.subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.vendors = response.Result;

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

    onRowSelect(event) {
        this.newVendor = false;
        this.vendor = this.cloneVendor(event.data); // to be updated in the dialog
        this.displayDialog = true;
    }

    cloneVendor(c: Vendor): Vendor {
        const vendor = new Vendor();
        for (const prop in c) {
            vendor[prop] = c[prop];
        }
        return vendor;
    }


    showDialogToAdd() {
        this.newVendor = true;
        this.vendor = new Vendor();
        this.displayDialog = true;
    }
/*
    onClickDeleteVendor() {

        if (confirm('Are you sure you want to delete ' + this.vendor.Name + '?')) {
            this.deleteVendor(this.vendor.UUID);
        }
        this.displayDialog = false;

        // this._confirmationService.confirm({
        //    message: 'Do you want to delete this vendor?',
        //    header: 'Delete Confirmation',
        //    icon: 'fa fa-trash',
        //    accept: () => {
        //        this.msgs = [];
        //        this.msgs.push({ severity: 'info', summary: 'Confirmed', detail: 'Vendor deleted' });
        //        this.deleteVendor(this.vendor.UUID);
        //        this.vendor = new Vendor();
        //    }
        // });
    }

    deleteVendor(vendorUUID) {
        this.msgBox.closeMessageBox();
        this.deletingData = true;
        const res = this._productService.deleteVendor(vendorUUID);

        res.subscribe(response => {

            this.deletingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'Vendor deleted.', 10);
            const index = this.findSelectedIndex(this.vendor);
            // Here, with the splice method, we remove 1 object
            // at the given index.
            this.vendors.splice(index, 1);


        }, err => {
            this.deletingData = false;
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });

    }

    cancel() {
        this.displayDialog = false;
    }

    save() {
        this.msgBox.closeMessageBox();

        if ( this.vendor.AccountUUID === '' || this.vendor.AccountUUID == null ) {
            this.vendor.AccountUUID = this._sessionService.CurrentSession.userAccountUUID;
        }
        this.processingRequest = true;

        let res = null;

        if (this.newVendor) {// add
            res = this._productService.addVendor(this.vendor);
        } else { // update
            res = this._productService.updateVendor(this.vendor);
        }

        res.subscribe(response => {

            this.processingRequest = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newVendor) {// add
                this.msgBox.ShowMessage('info', 'Vendor added', 10   );
                this.vendors.push(this.vendor);
            } else { // update
                this.msgBox.ShowMessage('info', 'Vendor updated', 10   );
                this.vendor.UUID = response.Result.UUID;
                this.vendors[this.findSelectedIndex(this.vendor)] = this.vendor;
            }
        }, err => {
            this.vendor = null;
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
*/
    findSelectedIndex(vendor): number {

        for (let i = 0; i < this.vendors.length; i++) {

            if (this.vendors[i].UUID === vendor.UUID) {
                return i;
            }
        }
        return -1;
    }
}
