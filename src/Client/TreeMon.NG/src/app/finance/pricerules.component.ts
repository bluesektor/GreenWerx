// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CheckboxModule, FileUploadModule, CalendarModule } from 'primeng/primeng';

import { SessionService } from '../services/session.service';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule, AccordionModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { PriceRule } from '../models/pricerule';
import { FinanceService } from '../services/finance.service';
import { AppService } from '../services/app.service';

@Component({
    templateUrl: './pricerules.component.html',
    providers: [SessionService, FinanceService, AppService]

})
export class PriceRulesComponent implements OnInit {

    @Input() type;

    displayDialog = false;
    newPriceRule = false;
    processingRequest = false;
    listData: PriceRule[];
    selectedItem: PriceRule = new PriceRule();
    listCount = 0;
    baseUrl: string;
    fileUploadUrl = '';
    priceRuleTypes: string[];
    customType: string;
    selectedType = '';

    operators: string[];
    price = 100;

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _appService: AppService,
        private _sessionService: SessionService,
        private _priceruleService: FinanceService) {
        this.msgBox = new MessageBoxesComponent();
    }

    ngOnInit() {
        this.baseUrl = this._appService.BaseUrl();
        this.fileUploadUrl = this._appService.BaseUrl() + 'api/File/Upload/';

        this.priceRuleTypes = ['student', 'military', 'coupon', 'promotion', 'shipping', 'delivery', 'taxes'];
        this.operators = ['=', '+', '-', '*', '/', '%'];
    }

    cboPriceRuleTypeChange(newType) {
        this.selectedType = newType;
        this.selectedItem.ReferenceType = newType;
    }

    cboPriceRuleOperatorChange(newOperator) {
        this.selectedItem.Result = this._priceruleService.calcPriceRule(this.price, this.selectedItem.Operator, this.selectedItem.Operand);
    }

    txtOperandChanged(operand) {

        if (isNaN(Number(operand))) {
            console.log('you must enter a numeric value');
            return;
        }
        this.selectedItem.Operand = operand;
        this.selectedItem.Result = this._priceruleService.calcPriceRule(this.price, this.selectedItem.Operator, this.selectedItem.Operand);
    }

    txtMaxUseChanged(operand) {

        if (isNaN(Number(operand))) {
            console.log('you must enter a numeric value');
            return;
        }
        this.selectedItem.MaxUseCount = operand;
    }

    txtMinimumChanged(operand) {

        if (isNaN(Number(operand))) {
            console.log('you must enter a numeric value');
            return;
        }
        this.selectedItem.Minimum = operand;
    }


    txtMaximumChanged(operand) {

        if (isNaN(Number(operand))) {
            console.log('you must enter a numeric value');
            return;
        }
        this.selectedItem.Maximum = operand;
    }

    loadPriceRules(page?: number, pageSize?: number) {
        this.processingRequest = true;
        const filters = [];

        const res = this._priceruleService.getPriceRules(filters);
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

    lazyLoadPriceRuleList(event) {
        this.loadPriceRules(event.first, event.rows);
    }

    loadSystemData() {
        // type

    }

    showDialogToAdd() {
        this.newPriceRule = true;
        this.selectedItem = null;
        this.selectedItem = new PriceRule();
        this.displayDialog = true;
    }

    onRowSelect(event) {
        this.newPriceRule = false;
        this.selectedItem = this.cloneItem(event);
        this.displayDialog = true;
    }

    cloneItem(c: PriceRule): PriceRule {
        const item = new PriceRule();
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
            const res = this._priceruleService.deletePriceRule(this.selectedItem.UUID);
            res.subscribe(response => {
                this.displayDialog = false;
                this.processingRequest = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                const index = this.findSelectedItemIndex(this.selectedItem.UUID);
                // Here, with the splice method, we remove 1 priceruleect
                // at the given index.
                this.listData.splice(index, 1);
                this.listCount--;
                this.msgBox.ShowMessage('info', 'PriceRule deleted.', 10);
                this.loadPriceRules(1, 25); // the array manipulation isn't working so just relaoding for now
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

  //  update(dt: any) {        dt.reset();    }
    expires: Date;

    save() {
        this.msgBox.closeMessageBox();
        this.processingRequest = true;
        let res = null;

        if (this.expires) {
            this.selectedItem.Expires = this.expires;
        }
        this.expires = new Date();

        if (this.newPriceRule) {// add
            res = this._priceruleService.addPriceRule(this.selectedItem);
        } else { // update
            res = this._priceruleService.updatePriceRule(this.selectedItem);
        }

        res.subscribe(response => {

            this.processingRequest = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newPriceRule) {// add
                this.msgBox.ShowMessage('info', 'PriceRule added', 10);
                this.selectedItem = new PriceRule();
                this.selectedItem = response.Result;
                this.listData.push(response.Result);
                this.listCount++;
            } else { // update
                this.msgBox.ShowMessage('info', 'PriceRule updated', 10);
                let idx = this.findSelectedItemIndex(this.selectedItem.UUID);
                this.listData[idx] = this.selectedItem;
            }
            this.loadPriceRules(1, 25);
            this.selectedItem = null;
            this.newPriceRule = false;

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
}
