// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, OnInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { SessionService } from '../services/session.service';
import { Setting } from '../models/setting';
import { SettingsService } from '../services/settings.service';
import { MessageBoxesComponent } from '../common/messageboxes.component';
import { DataTableModule, SharedModule, DialogModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
@Component({
    templateUrl: './settings.component.html',
    providers: [SettingsService, SessionService]

})
export class SettingsComponent implements OnInit {

    displayDialog = false;
    newSetting = false;
    processingRequest = false;
    settings: Setting[];
    setting: Setting = new Setting();
    settingFilters: Filter[] = [];
    totalSettings = 0;

    types: any[] = [
        { 'name': 'Select one...', 'value': '' },
        { 'name': 'String', 'value': 'STRING' },
        { 'name': 'Numeric', 'value': 'INT' },
        { 'name': 'Decimal', 'value': 'DECIMAL' },
        { 'name': 'Date Time', 'value': 'DATETIME' },
        { 'name': 'True/False', 'value': 'BOOL' },
        { 'name': 'Encrypted String', 'value': 'STRING.ENCRYPTED' }
    ];

    @ViewChild(MessageBoxesComponent) msgBox: MessageBoxesComponent;

    constructor(
        private _router: Router,
        private _route: ActivatedRoute,
        private _settingService: SettingsService,
        private _sessionService: SessionService) {

        this.msgBox = new MessageBoxesComponent();

    }

    ngOnInit() {
        this.loadSettings(1, 25);
    }

    cboTypeChange(newType) {
        this.setting.Type = newType;
    }

    loadSettings(  page?: number, pageSize?: number) {
        this.processingRequest = true;
        const filter = new Filter();
        filter.PageResults = true;
        filter.StartIndex = page;
        filter.PageSize = pageSize;
        const res = this._settingService.getSettings(filter);

        res.subscribe(response => {
            this.displayDialog = false;
            this.processingRequest = false;
            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }
            this.settings = response.Result;
            this.totalSettings = response.TotalRecordCount;
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

    lazyLoadSettingsList(event) {

        this.loadSettings( event.first, event.rows);
    }

    keyPressRoleWeight(event: any) {
        const keychar = String.fromCharCode(event.charCode);

        //  numbers
         if ((('0123456789').indexOf(keychar) > -1)) {
             return;
        }
        event.preventDefault();
    }

    delete() {
        this.msgBox.closeMessageBox();

        if (confirm('Are you sure you want to delete ' + this.setting.Name + '?')) {

            this.processingRequest = true;

            const res = this._settingService.deleteSetting(this.setting.UUID);

            res.subscribe(response => {
                this.displayDialog = false;
                this.processingRequest = false;

                if (response.Code !== 200) {
                    this.msgBox.ShowMessage(response.Status, response.Message, 10);
                    return false;
                }

                const index = this.findSelectedSettingIndex(); //  this.settings.indexOf(this.setting)
                // Here, with the splice method, we remove 1 object
                // at the given index.
                this.settings.splice(index, 1);
                this.msgBox.ShowMessage('info', 'Setting deleted.', 10);
                this.loadSettings(1, 25);
            }, err => {
                this.processingRequest = false;
                this.msgBox.ShowResponseMessage(err.status, 10);
                if (err.status === 429) {
                    this._sessionService.ClearSessionState();
                    this.msgBox.ShowMessage('error', 'Too many requests being sent.', 10);
                    return;
                }
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
        this.newSetting = true;
        this.setting = new Setting();
        this.displayDialog = true;
    }

    save() {
        this.msgBox.closeMessageBox();

        if ( this.setting.AccountUUID ===  '' || this.setting.AccountUUID === null ) {
            this.setting.AccountUUID = this._sessionService.CurrentSession.userAccountUUID;
        }

        this.settings.push(this.setting);
        this.processingRequest = true;

        let res = null;

        if (this.newSetting) {// add
            res = this._settingService.addSetting(this.setting);
        } else { // update
            res = this._settingService.updateSetting(this.setting);
        }

        res.subscribe(response => {

            this.processingRequest = false;
            this.setting = null;
            this.displayDialog = false;

            if (response.Code !== 200) {
                this.msgBox.ShowMessage(response.Status, response.Message, 10);
                return false;
            }

            if (this.newSetting) {// add
                this.msgBox.ShowMessage('info', 'Setting added', 10);
                this.settings.push(this.setting);
            } else { // update
                this.msgBox.ShowMessage('info', 'Setting updated', 10);
                this.settings[this.findSelectedSettingIndex()] = this.setting;
            }
            this.loadSettings(1, 25);
        }, err => {
            this.setting = null;
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

    cancel() {
        this.displayDialog = false;
    }

    onRowSelect(event) {
        this.newSetting = false;
        this.setting = this.cloneSetting(event.data);
        this.displayDialog = true;
    }

    cloneSetting(c: Setting): Setting {
        const setting = new Setting();
        for ( var prop in c ) {
            setting[prop] = c[prop];
        }
        return setting;
    }

    findSelectedSettingIndex(): number {
        return this.settings.indexOf(this.setting);
    }
}
