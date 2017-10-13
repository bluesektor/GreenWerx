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
export class GeoService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    addLocation(location) {
        return this.invokeRequest('POST', 'api/Locations/Add', JSON.stringify(location));
    }
    getLocationTypes() {
        return this.invokeRequest('GET', 'api/Locations/LocationTypes', ''    );
    }

    getCustomLocations() {
        return this.invokeRequest('GET', 'api/Locations/Custom');
    }


    getLocations(locationType: string, filter?: Filter) {
        return this.invokeRequest('GET', 'api/Locations/LocationType/' + locationType + '?filter=' + JSON.stringify(filter) );
    }

    deleteLocation(settingUUID) {
        return this.invokeRequest('DELETE', 'api/Locations/Delete/' + settingUUID, ''    );
    }

    updateLocation(location) {
        return this.invokeRequest('PATCH', 'api/Locations/Update', JSON.stringify(location));
    }


    getChildLocations(parentUUID: string ,  filter?: Filter) {
        return this.invokeRequest('GET', 'api/ChildLocations/' + parentUUID + '?filter=' + JSON.stringify(filter) );
    }
}
