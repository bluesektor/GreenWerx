// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from '../api/api';
import { Filter } from '../../models/filter';

@Injectable()
export class SettingsService  {

    constructor(private api: Api ) {    }

    addSetting(setting) {
        return this.api.invokeRequest('POST', 'api/Apps/Settings/Add', JSON.stringify(setting));
    }

    deleteSetting(settingUUID) {
        return this.api.invokeRequest('DELETE', 'api/Apps/Settings/Delete/' + settingUUID, ''    );
    }

    getSettings(filter?: Filter) {
        return this.api.invokeRequest('POST', 'api/Apps/Settings' , JSON.stringify(filter) );
    }

    getSetting(settingId) {
        return this.api.invokeRequest('GET', 'api/Apps/Settings/' + settingId, ''    );
    }

    updateSetting(setting) {
        return this.api.invokeRequest('PATCH', 'api/Apps/Settings/Update', setting);
    }
}
