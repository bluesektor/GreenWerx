// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { Component, ViewChild, OnInit, Output, EventEmitter, ElementRef, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { BasicValidators } from '../common/basicValidators';
import { Filter } from '../models/filter';
import { Screen} from '../models/screen';
import { SessionService } from '../services/session.service';
import { AccordionModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule, CheckboxModule } from 'primeng/primeng';
import { CategoriesService } from '../services/categories.service';
import { ProductService } from '../services/product.service';
import { Category } from '../models/category';

@Component({
    selector: 'tm-category',
    templateUrl: './categories.component.html',

    providers: [ProductService, ConfirmationService, SessionService, CategoriesService]
})
export class CategoriesComponent implements OnInit {
    @Input() categoryType: string;

    loadingData = false;
    deletingData = false;
    processingRequest = false;
    displayDialog = false;

    categoryTypes: string[] = [];
    newCategory = false;
    categories: Category[];
    category: Category = new Category();
   //  customCategoryType: string = '';
    categoryFilter: Filter = new Filter();

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(fb: FormBuilder,
        private _productService: ProductService,
        private _confirmationService: ConfirmationService,
        private _sessionService: SessionService,
        private _categoriesService: CategoriesService,
        private _router: Router,
        private _route: ActivatedRoute) {
    }

    ngOnInit() {

        if (!this._sessionService.CurrentSession.validSession) {
            return;
        }

        this.loadingData = true;
        this.loadCategories();
    }

    loadCategories() {
        const screen = new Screen();
        screen.Field = 'CategoryType';
        screen.Command = 'SearchBy';
        screen.Value = this.categoryType;
        this.categoryFilter.Screens.push(screen);
        const res = this._categoriesService.getCategories(this.categoryFilter);
        res.subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.categories = response.Result;

            for (let i = 0; i < this.categories.length; i++) {

                if (this.findCategoryTypeIndex(this.categories[i].CategoryType) < 0) {
                    this.categoryTypes.push(this.categories[i].CategoryType);
                }
            }

        }, err => {
            this.msgBox.ShowResponseMessage(err.status, 10);

            if (err.status === 401 && err.statusText === 'Session expired.' ) {
                this._sessionService.ClearSessionState();
                  setTimeout(() => {
                    this._router.navigate(['/membership/login'], { relativeTo: this._route });
                }, 3000);
            }

        });
    }

    onRowSelect(event) {
        this.newCategory = false;
        this.category = this.cloneCategory(event.data); // to be updated in the dialog
        this.displayDialog = true;
    }

     cloneCategory(c: Category): Category {
        const category = new Category();
        for (const prop in c) {
            category[prop] = c[prop];
        }
        return category;
    }


     showDialogToAdd() {
         this.newCategory = true;
         this.category = new Category();
         this.displayDialog = true;
     }

     onClickDeleteCategory() {
       
         if (confirm('Are you sure you want to delete ' + this.category.Name + '?')) {
             this.deleteCategory(this.category.UUID);
         }
         this.displayDialog = false;
     }

    deleteCategory(categoryUUID) {
        this.msgBox.closeMessageBox();
        this.deletingData = true;
        const res = this._categoriesService.deleteCategory(categoryUUID);

        res.subscribe(response => {

            this.deletingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'Category deleted.', 10);
            const index = this.findSelectedIndex(this.category);
            // Here, with the splice method, we remove 1 object
            // at the given index.
            this.categories.splice(index, 1);
            this.loadCategories();  // not updating the list so reload for now.
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

        if (this.category.AccountUUID === '' || this.category.AccountUUID === null ) {
            this.category.AccountUUID = this._sessionService.CurrentSession.userAccountUUID;
        }

       this.processingRequest = true;
        let res = null;
        this.category.CategoryType = this.categoryType;
        //  'Product';// NOTE: make sure you change this per type your working on. For strains this would be 'Variety'

        if (this.newCategory) {// add
            res = this._categoriesService.addCategory(this.category);
        } else { // update
            res = this._categoriesService.updateCategory(this.category);
        }

        res.subscribe(response => {

            this.processingRequest = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newCategory) {// add
                this.msgBox.ShowMessage('info', 'Category added', 10);
                this.categories.push(response.Result);
            } else { // update
                this.msgBox.ShowMessage('info', 'Category updated', 10);
                this.categories[this.findSelectedIndex(this.category)] = this.category;
            }
            this.loadCategories();  // not updating the list so reload for now.
        }, err => {
            this.category = null;
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

    findSelectedIndex(category): number {

        for (let i = 0; i < this.categories.length; i++) {

            if (this.categories[i].UUID === category.UUID) {
                return i;
            }
        }
        return -1;
    }

    findCategoryTypeIndex(categoryType): number {

        for (let i = 0; i < this.categoryTypes.length; i++) {

            if (this.categoryTypes[i] === categoryType) {
                return i;
            }
        }
        return -1;
    }

    cboCategoryAddEditCategoryTypeChange(newType) {

        this.categoryType = newType;
       // this.customCategoryType = '';
        // this.categoryType = '';
        // if (newType === '')
        //     return;
        this.category.CategoryType = newType;
    }
}
