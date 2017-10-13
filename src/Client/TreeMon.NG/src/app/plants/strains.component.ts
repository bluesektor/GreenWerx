import { Component, ViewChild, OnInit, Output, EventEmitter, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { BasicValidators } from '../common/basicValidators';
import { SessionService } from '../services/session.service';
import { AccountService } from '../services/account.service';

import { AccordionModule } from 'primeng/primeng';
import { CheckboxModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule, AutoCompleteModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { PlantsService } from '../services/plants.service';
import { CategoriesService } from '../services/categories.service';
import { Strain } from '../models/strain';
import { Category } from '../models/category';
import { Location } from '../models/location';
import { Account } from '../models/account';
import { GeoService } from '../services/geo.service';


@Component({
    templateUrl: './strains.component.html',

    providers: [PlantsService, ConfirmationService, SessionService, GeoService, CategoriesService, AccountService]
})
export class StrainsComponent implements OnInit {

    loadingData = false;
    deletingData = false;
    displayDialog = false;
    selectedStrain: Strain = new Strain();
    selectedCategoryUUID: string;
    selectedAccount: string; // breeder
    newStrain = false;
    strains: Strain[] = [];
    categories: Category[];
    departments: Category[];
    selectedDepartmentUUID: string;
    selectedLocationtUUID = '';
    locations: Location[];
    totalRecords: number;
    formStrainDetail: FormGroup;
    filteredAccounts: Account[] = [];
    searching = false;
    searchStrains: Filter = new Filter();

    // ===--- Top Menu Bar ---===
    strainActiveStrain = false;
    msgs: any[] = [];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(fb: FormBuilder,
        private _strainService: PlantsService,
        private _geoService: GeoService,
        private _confirmationService: ConfirmationService,
        private _sessionService: SessionService,
        private _accountService: AccountService,
        private _categoriesService: CategoriesService,
        private _router: Router,
        private _route: ActivatedRoute) {

        this.formStrainDetail = fb.group({
            Name: ['', Validators.required],
            Email: ['', BasicValidators.email],
            Private: '',
            Active: '',
            SortOrder: 0
        });

    }

    // ===--- General Events ---===

    ngOnInit() {
        this.loadingData = true;

        if (!this._sessionService.CurrentSession.validSession) {
            return;
        }

        this.loadCategoriesDropDown();
        this.loadLocations();
    }

    onTabShow(e) {
        console.log('tab index:', e.index);
    }

    // ===--- Top Menu Bar ---===

    loadCategoriesDropDown() {
        this.loadingData = true;
        this.searching = false; // switch when using the combo. this way we know what filter to use.
        const filter = new Filter();
        const screen = new Screen();
        screen.Field = 'CategoryType';
        screen.Command = 'SearchBy';
        screen.Value = 'Variety';
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

    cboCategoryChange(categoryUUID) {
        this.selectedDepartmentUUID = '';
        this.selectedCategoryUUID = categoryUUID;
        this.loadStrains(categoryUUID);
    }

    loadLocations() {
        this.loadingData = true;
        const res = this._geoService.getCustomLocations();
        res.subscribe(response => {
            this.loadingData = false;
            this.displayDialog = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.locations = response.Result;

            const l = new Location();
            l.UUID = '';
            l.Name = 'Select a location';
            this.locations.push(l);

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

    onClickDeleteStrain() {

        if (confirm('Are you sure you want to delete ' + this.selectedStrain.Name + '?')) {
            this.deleteStrain(this.selectedStrain.UUID);
        }
    }

    deleteStrain(strainUUID) {
        this.msgBox.closeMessageBox();
        this.deletingData = true;
        const res = this._strainService.deleteStrain(strainUUID);

        res.subscribe(response => {

            this.deletingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            this.msgBox.ShowMessage('info', 'Strain deleted.', 10);
            const index = this.findSelectedIndex(this.selectedStrain);
            // Here, with the splice method, we remove 1 object
            // at the given index.
            this.strains.splice(index, 1);
            this.loadStrains(this.selectedCategoryUUID, 1, 25);  // not updating the list so reload for now.

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

    loadStrains(categoryUUID: string, page?: number, pageSize?: number) {

        if (this.searching === false && (categoryUUID == null || categoryUUID === '')) {
            return;
        }
        this.loadingData = true;
        let filter = new Filter();
        if (this.searching === true) {
            filter = this.searchStrains;
        } else {
            const screen = new Screen();
            screen.Command = 'SearchBy';
            screen.Field = 'VarietyUUID';
            screen.Value = categoryUUID;
            filter.Screens.push(screen);
        }
        filter.PageResults = true;
        filter.StartIndex = page;
        filter.PageSize = pageSize;

        const res = this._strainService.getStrains(filter);
        res.subscribe(response => {
            this.loadingData = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.strains = response.Result;
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

    lazyLoadStrainsList(event) {
        this.loadStrains(this.selectedCategoryUUID, event.first, event.rows);

    }

    showDialogToAdd() {
        this.newStrain = true;
        this.selectedStrain = null;
        this.selectedStrain = new Strain();
        this.displayDialog = true;
    }

    cancel() {
        this.displayDialog = false;
    }

    onRowSelect(event) {
        this.newStrain = false;
        this.selectedStrain = this.cloneStrain(event.data);

        if ( !BasicValidators.isNullOrEmpty(  this.selectedStrain.BreederUUID)) {
            // get the acount name. todo add this to the service when retrieving the record.
            this._accountService.getAccount(this.selectedStrain.BreederUUID).subscribe(response => {

                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 15);
                    return false;
                }
                this.selectedAccount = response.Result.Name;
            });
        }
        this.displayDialog = true;
    }

    cloneStrain(c: Strain): Strain {
        const strain = new Strain();
        for (const prop in c) {
            strain[prop] = c[prop];
        }
        return strain;
    }

    findSelectedIndex(strain): number {
        for (let i = 0; i < this.categories.length; i++) {

            if (this.strains[i].UUID === strain.UUID) {
                return i;
            }
        }
        return -1;
    }

    saveStrain() {
        this.loadingData = true;
        this.msgBox.closeMessageBox();

        let res;

        if (this.newStrain === true) {
            this.selectedStrain.UUID = '';
            res = this._strainService.addStrain(this.selectedStrain);

        } else {
            res = this._strainService.updateStrain(this.selectedStrain);
        }

        res.subscribe(response => {

            this.loadingData = false;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            if (this.newStrain) {
                this.msgBox.ShowMessage('info', 'Strain added.', 10);
                this.selectedStrain.UUID = response.Result.UUID;
                this.newStrain = false;
                this.strains.push(this.selectedStrain);

            } else {
                this.msgBox.ShowMessage('info', 'Strain updated.', 10);
                this.strains[this.findSelectedIndex(this.selectedStrain)] = this.selectedStrain;
            }
            this.loadStrains(this.selectedCategoryUUID, 1, 25);  // not updating the list so reload for now.
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

    filterAccounts(event) {

        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 25;
        const screen = new Screen();
        screen.Command = 'Contains';
        screen.Field = 'Name';
        screen.Value = event.query.toLowerCase();
        filter.Screens.push(screen);
        this._accountService.getAllAccounts(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            this.filteredAccounts = response.Result;
        });
    }

    onSelectAccount(value) {
        this._accountService.getAccount(value.UUID).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            this.selectedStrain.BreederUUID = value.UUID;
            this.selectedAccount = value.Name;
        });
    }

    handleAccountDropdownClick(event) {
        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 25;

        this._accountService.getAllAccounts(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            this.filteredAccounts = response.Result;
        });
    }



    onSearchStrains(searchText) {

        if (!searchText || searchText.length < 2) {
            return;
        }
        this.loadingData = true;
        this.searching = true; // switch when text is being searched for in table..
        this.searchStrains.Screens = [];
        const screen = new Screen();
        screen.Command = 'Contains';
        screen.Field = 'Name';
        screen.Value = searchText;
        this.searchStrains.Screens.push(screen);


        setTimeout(() => {

            this._strainService.getStrains(this.searchStrains).subscribe(response => {
                this.loadingData = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 15);
                    return false;
                }
                this.strains = response.Result;
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

        }, 1000);
    }

}
