// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from '../services/account.service';
import { SessionService } from '../services/session.service';
import { GeoService } from '../services/geo.service';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule, AccordionModule, AutoCompleteModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { Account } from '../models/account';
import { Location } from '../models/location';
import { BasicValidators } from '../common/basicValidators';

@Component({
    templateUrl: './locations.component.html',
    providers: [GeoService, SessionService, AccountService]

})
export class LocationsComponent implements OnInit {

    displayDialog = false;
    newLocation = false;
    processingRequest = false;
    locations: Location[];
    location: Location = new Location();
    totalLocations = 0;
    locationTypes: string[];
    selectedLocationType = '';
    customLocation = '';

    selectedAccount: Account = new Account();
    filteredAccounts: Account[] = [];
    loadingAccounts = false;

    countries: Location[];
    states: Location[];
    cities: Location[];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _accountService: AccountService,
        private _geoService: GeoService,
        private _sessionService: SessionService) {
        this.msgBox = new MessageBoxesComponent();
    }

    ngOnInit() {
        this.loadLocationTypes();
    }

    cboLocationTypeChange(newType) {
        if (!newType || newType === '' || newType === null) {
            return;
        }
        this.selectedLocationType = newType;
        this.loadLocations(newType, 1, 25);
    }

    cboLocationAddEditLocationTypeChange(newType) {

        this.customLocation = '';
        if (newType === '') {
            return;
        }
        this.location.LocationType = newType;
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

    loadLocations(locationType: string, page: number, pageSize: number) {

        this.processingRequest = true;

        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = page;
        filter.PageSize = pageSize;
        const screen = new Screen();
        screen.Command = 'SEARCHBY';
        screen.Field = 'LocationType';
        screen.Value = locationType;
        filter.Screens.push(screen);

        const res = this._geoService.getLocations(locationType, filter);
        res.subscribe(response => {
            this.displayDialog = false;
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.locations = response.Result;
            this.totalLocations = response.TotalRecordCount;

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

    lazyLoadLocationsList(event) {

        if (!this.selectedLocationType || this.selectedLocationType === '') {
            return;
        }

        this.loadLocations(this.selectedLocationType, event.first, event.rows);
    }

    delete() {
        this.msgBox.closeMessageBox();
        if (confirm('Are you sure you want to delete ' + this.location.Name + '?')) {
            this.processingRequest = true;
            const res = this._geoService.deleteLocation(this.location.UUID);
            res.subscribe(response => {
                this.displayDialog = false;
                this.processingRequest = false;
                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }
                const index = this.findLocationIndex(this.location.UUID);
                // Here, with the splice method, we remove 1 object
                // at the given index.
                this.locations.splice(index, 1);
                this.msgBox.ShowMessage('info', 'Location deleted.', 10);
                this.loadLocations(this.selectedLocationType, 1, 25);   // not updating the list so reload for now.
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

    showDialogToAdd() {
        this.loadCountries();
        this.newLocation = true;
        this.location = new Location();
        this.location.LocationType = this.selectedLocationType;
        this.displayDialog = true;
    }

    save() {
        this.msgBox.closeMessageBox();

        if (this.location.LocationType === 'custom') {
            this.locationTypes.push(this.customLocation);
            this.location.LocationType = this.customLocation;

            this.location.AccountUUID = this._sessionService.CurrentSession.userAccountUUID;

            if (this.location.City !== '') {
                this.location.UUParentID = this.location.City;
            } else if (this.location.State !== '') {
                this.location.UUParentID = this.location.State;
            } else if (this.location.Country !== '') {
                this.location.UUParentID = this.location.Country;
            }
        }

        if (!this.locations) {
            this.locations = [];
        }

        if (this.findLocationIndex(this.location.UUID) < 0) {
            this.locations.push(this.location);
        }

        this.processingRequest = true;

        let res = null;

        if (this.newLocation) {// add
            res = this._geoService.addLocation(this.location);
        } else { // update
            res = this._geoService.updateLocation(this.location);
        }

        res.subscribe(response => {

            this.processingRequest = false;
            this.location = null;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newLocation) {// add
                this.msgBox.ShowMessage('info', 'Location added', 10);
                this.locations.push(this.location);
            } else { // update
                this.msgBox.ShowMessage('info', 'Location updated', 10);
                this.locations[this.findSelectedLocationIndex()] = this.location;
            }
            this.loadLocations(this.selectedLocationType, 1, 25);   // not updating the list so reload for now.
        }, err => {
            this.location = null;
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
        this.newLocation = false;
    }

    cancel() {
        this.displayDialog = false;
    }

    onRowSelect(event) {
        this.loadCountries();
        this.newLocation = false;
        this.location = this.cloneLocation(event.data);
        this._accountService.getAccount(this.location.AccountReference).subscribe(response => {
          //  if (response.Code !== 200) {
          //      this.msgBox.ShowMessage(response.Status, response.Message, 15);
          //      return false;
          //  }
          if(response.Code === 200){
            this.selectedAccount = response.Result;
          }
            this.displayDialog = true;
        });
    }

    onTabShow(e) {
        console.log('tab index:', e.index);
    }

    cloneLocation(c: Location): Location {
        const location = new Location();
        for (const prop in c) {
            location[prop] = c[prop];
        }
        return location;
    }

    findSelectedLocationIndex(): number {
        return this.locations.indexOf(this.location);
    }

    loadCountries() {

        this.processingRequest = true;
        const filter = new Filter();
        filter.PageResults = false;


        const res = this._geoService.getLocations('country', filter);
        res.subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.countries = response.Result;
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

    findLocationIndex(locationUUID): number {

        for (let i = 0; i < this.locations.length; i++) {

            if (this.locations[i].UUID === locationUUID) {
                return i;
            }
        }
        return -1;
    }


    loadStates(countryUUID) {

        const filter = new Filter();
        filter.PageResults = false;
        this.processingRequest = true;
        const res = this._geoService.getChildLocations(countryUUID, filter);
        res.subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.states = response.Result;
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

    loadCities(stateUUID) {
        this.processingRequest = true;
        const filter = new Filter();
        filter.PageResults = false;
        const res = this._geoService.getChildLocations(stateUUID, filter);
        res.subscribe(response => {
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.cities = response.Result;
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


    filterAccounts(event) {
        if ( this.loadingAccounts === true) {
            return;
        }

        this.loadingAccounts = true;
        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = 1;
        filter.PageSize = 25;
        if ( BasicValidators.isNullOrEmpty(event.query) === false) {
            const screen = new Screen();
            screen.Command = 'Contains';
            screen.Field = 'Name';
            screen.Value = event.query.toLowerCase();
            filter.Screens.push(screen);
        }

        this._accountService.getAllAccounts(filter).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            this.filteredAccounts = response.Result;
            this.loadingAccounts = false;
        });
    }

    onSelectAccount(value) {
        this._accountService.getAccount(value.UUID).subscribe(response => {
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 15);
                return false;
            }
            this.location.AccountReference = value.UUID;
            this.selectedAccount.Name = value.Name;
            this.selectedAccount.UUID = value.UUID;
            if ( BasicValidators.isNullOrEmpty(this.location.Name) === true) {
                this.location.Name = value.Name;
            }
        });
    }

    handleAccountDropdownClick(event) {
        if ( this.loadingAccounts === true) {
            return;
        }
        this.loadingAccounts = true;
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
            this.loadingAccounts = false;
        });
    }


}
