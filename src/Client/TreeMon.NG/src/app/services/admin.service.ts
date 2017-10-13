// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
@Injectable()
export class AdminService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    addSetting(setting) {
        return this.invokeRequest('POST', 'api/Apps/Settings/Add', JSON.stringify(setting));
    }

    deleteSetting(settingUUID) {
        return this.invokeRequest('DELETE', 'api/Apps/Settings/Delete/' + settingUUID, ''    );
    }

    getSettings(filter?: Filter) {

        return this.invokeRequest('GET', 'api/Apps/Settings?filter=' + JSON.stringify(filter) );
    }

    getSetting(settingId) {
        return this.invokeRequest('GET', 'api/Apps/Settings/' + settingId, ''    );
    }

    updateSetting(setting) {
        return this.invokeRequest('PATCH', 'api/Apps/Settings/Update', setting);
    }

    getToolsDashboard() {
        return this.invokeRequest('GET', 'api/Tools/Dashboard');
    }

    backupDatabase() {
        return this.invokeRequest('GET', 'api/Tools/Database/Backup');
    }

    restoreDatabase(backupFiles: string) {
        return this.invokeRequest('GET', 'api/Tools/Database/Restore', backupFiles);
    }


    cipherText(text: string, encrypt: boolean) {
        if ( encrypt === true) {
            return this.invokeRequest('GET', encodeURIComponent('api/Tools/Cipher/' + text + '/Encrypt/' + encrypt) );
        }
        return this.invokeRequest('GET', 'api/Tools/Cipher/' + text + '/Encrypt/' + encrypt );
    }

    import(type: string) {
        return this.invokeRequest('GET', '/api/Tools/Import/' + type);
    }

    testCode() {
        return this.invokeRequest('GET', '/api/Tools/TestCode' );
    }
}
