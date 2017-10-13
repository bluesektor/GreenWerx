// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { UnitOfMeasure } from '../models/unitofmeasure';


@Injectable()
export class UnitsOfMeasureService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    add(uom: UnitOfMeasure) {
        return this.invokeRequest('POST', 'api/UnitsOfMeasure/Add', JSON.stringify(uom));
    }


    delete(uuid) {
        return this.invokeRequest('DELETE', 'api/UnitsOfMeasure/Delete/' + uuid, ''    );
    }

    get(filter?: Filter) {
        return this.invokeRequest('GET', 'api/UnitsOfMeasure/?filter=' + JSON.stringify(filter) );
    }

    update(uom: UnitOfMeasure) {
        return this.invokeRequest('PATCH', 'api/UnitsOfMeasure/Update', JSON.stringify( uom ));
    }
}
