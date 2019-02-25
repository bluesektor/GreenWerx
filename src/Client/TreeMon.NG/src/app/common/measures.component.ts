// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
import { Component, ViewChild, OnInit, Output, EventEmitter, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { BasicValidators } from '../common/basicValidators';
import { SessionService } from '../services/session.service';
import { AccountService } from '../services/account.service';
import { CategoriesService } from '../services/categories.service';

import { AccordionModule } from 'primeng/primeng';
import { CheckboxModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule, AutoCompleteModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';

import { UnitsOfMeasureService } from '../services/unitsofmeasure.service';
import { UnitOfMeasure } from '../models/unitofmeasure';
import { Category } from '../models/category';


@Component({
    templateUrl: './measures.component.html',
    selector: 'tm-measures',
    providers: [UnitsOfMeasureService, ConfirmationService, SessionService,   AccountService, CategoriesService]
})
export class MeasuresComponent implements OnInit {

    loadingData = false;
    deletingData = false;
    displayDialog = false;
    selectedMeasure: UnitOfMeasure = new UnitOfMeasure();
    selectedCategoryUUID: string;
    categories: Category[];
    newUnitOfMeasure = false;
    measures: UnitOfMeasure[] = [];
    totalRecords: number;
    formUnitOfMeasureDetail: FormGroup;

    // ===--- Top Menu Bar ---===
    strainActiveUnitOfMeasure = false;
    msgs: any[] = [];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(fb: FormBuilder,
        private _confirmationService: ConfirmationService,
        private _sessionService: SessionService,
        private _accountService: AccountService,
        private _measuresService: UnitsOfMeasureService,
        private _categoriesService: CategoriesService,
        private _router: Router,
        private _route: ActivatedRoute) {

        this.formUnitOfMeasureDetail = fb.group({
            Name: ['', Validators.required],
            Email: ['', BasicValidators.email],
            Private: '',
            Active: '',
            SortOrder: 0
        });
    }

    ngOnInit() {
        if (!this._sessionService.CurrentSession.validSession) {
            return;
        }
        this.loadCategoriesDropDown();
    }

    onDeleteMeasure() {

        if (confirm('Are you sure you want to delete ' + this.selectedMeasure.Name + '?')) {
            this.deleteUnitOfMeasure(this.selectedMeasure.UUID);
        }
    }

    deleteUnitOfMeasure(uuid) {
        this.msgBox.closeMessageBox();
        this.deletingData = true;
        const res = this._measuresService.delete(uuid);

        res.subscribe(response => {

            this.deletingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'UnitOfMeasure deleted.', 10);
            const index = this.findSelectedIndex(this.selectedMeasure);
            // Here, with the splice method, we remove 1 object
            // at the given index.
            this.measures.splice(index, 1);
            this.loadUnitOfMeasures(this.selectedCategoryUUID, 1, 25);  // not updating the list so reload for now.

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

    loadUnitOfMeasures(categoryUUID: string, page?: number, pageSize?: number) {
        let filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = page;
        filter.PageSize = pageSize;

        const res = this._measuresService.get(filter);
        res.subscribe(response => {
            this.loadingData = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.measures = response.Result;
            this.totalRecords = response.TotalRecordCount;
        }, err => {
            this.msgBox.ShowResponseMessage(err.status, 10);
            this.loadingData = false;
            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    lazyLoadUnitOfMeasuresList(event) {
        this.loadUnitOfMeasures(this.selectedCategoryUUID, event.first, event.rows);

    }

    showDialogToAdd() {
        this.newUnitOfMeasure = true;
        this.selectedMeasure = null;
        this.selectedMeasure = new UnitOfMeasure();
        this.displayDialog = true;
    }

    cancel() {
        this.displayDialog = false;
    }

    onRowSelect(event) {
        this.newUnitOfMeasure = false;
        this.selectedMeasure = this.cloneUnitOfMeasure(event.data);

       /*
            this._measuresService.getAccount(this.selectedMeasure.BreederUUID).subscribe(response => {

                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 15);
                    return false;
                }
                this.selectedMeasure = response.Result.Name;
            });
       */
        this.displayDialog = true;
    }

    cloneUnitOfMeasure(c: UnitOfMeasure): UnitOfMeasure {
        const measure = new UnitOfMeasure();
        for (const prop in c) {
            if (prop != null) {
                measure[prop] = c[prop];
            }
        }
        return measure;
    }

    findSelectedIndex(strain): number {
        for (let i = 0; i < this.measures.length; i++) {

            if (this.measures[i].UUID === strain.UUID) {
                return i;
            }
        }
        return -1;
    }

    saveMeasure() {
        this.loadingData = true;
        this.msgBox.closeMessageBox();

        let res;

        if (this.newUnitOfMeasure === true) {
            this.selectedMeasure.UUID = '';
            res = this._measuresService.add(this.selectedMeasure);

        } else {
            res = this._measuresService.update(this.selectedMeasure);
        }

        res.subscribe(response => {

            this.loadingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            if (this.newUnitOfMeasure) {
                this.msgBox.ShowMessage('info', 'UnitOfMeasure added.', 10);
                this.selectedMeasure.UUID = response.Result.UUID;
                this.newUnitOfMeasure = false;
                this.measures.push(this.selectedMeasure);

            } else {
                this.msgBox.ShowMessage('info', 'UnitOfMeasure updated.', 10);
                this.measures[this.findSelectedIndex(this.selectedMeasure)] = this.selectedMeasure;
            }
            this.loadUnitOfMeasures(this.selectedCategoryUUID, 1, 25);  // not updating the list so reload for now.
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

    cboCategoryChange(categoryUUID) {
        this.selectedCategoryUUID = categoryUUID;
    }

    loadCategoriesDropDown() {
       this.loadingData = true;
        const filter = new Filter();
        const screen = new Screen();
        screen.Field = 'CategoryType';
        screen.Command = 'SearchBy';
        screen.Value = 'Product';
        filter.Screens.push(screen);

        const res = this._categoriesService.getCategories(filter);
        res.subscribe(response => {
            this.loadingData = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.categories = response.Result;

            if (this.categories.length > 0) {

                const c = new Category();
                this.selectedCategoryUUID = '';
            }
        }, err => {
            this.msgBox.ShowResponseMessage(err.status, 10);
            this.loadingData = false;
            if (err.status === 401) {
                this._sessionService.ClearSessionState();
                setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }
}
