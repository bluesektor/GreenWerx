// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { Strain } from '../models/strain';

@Injectable()
export class PlantsService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }


    addPlant(product) {
        return this.invokeRequest('POST', 'api/Plants/Add', JSON.stringify(product));
    }

    deletePlant(plantUUID) {
        return this.invokeRequest('DELETE', 'api/Plants/Delete/' + plantUUID, ''    );
    }

    getPlants(filter: Filter) {
        return this.invokeRequest('GET', 'api/Plants?filter=' + JSON.stringify(filter) );
    }

    getPlant(plantUUID) {
        return this.invokeRequest('GET', 'api/Plants/' + plantUUID, ''    );
    }

    getPlantDetails(plantUUID, productType) {
        return this.invokeRequest('GET', 'api/Plant/' + plantUUID + '/' + productType + '/Details' , ''    );
    }

    updatePlant(plant) {
        return this.invokeRequest('PATCH', 'api/Plants/Update', plant);
    }


    // ===--- Strains ---===

    addStrain(strain: Strain) {
        return this.invokeRequest('POST', 'api/Strains/Add', JSON.stringify(strain));
    }

    getStrains(filter: Filter) {
        return this.invokeRequest('GET', 'api/Strains?filter=' + JSON.stringify(filter), );
    }

    getStrain(strainUUID) {
        return this.invokeRequest('GET', 'api/StrainsBy/' + strainUUID, ''    );
    }

    deleteStrain(strainUUID: string) {
        return this.invokeRequest('DELETE', 'api/Strains/Delete/' + strainUUID, ''    );
    }

    updateStrain(strain: Strain) {
        return this.invokeRequest('PATCH', 'api/Strains/Update', JSON.stringify(strain));
    }
}
