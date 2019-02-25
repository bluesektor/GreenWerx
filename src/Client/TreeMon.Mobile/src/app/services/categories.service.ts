// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from './api/api';
import { Filter } from '../models/filter';

@Injectable({
    providedIn: 'root'
  })
export class CategoriesService  {

    constructor( private api: Api) {
    }

    addCategory(category) {
        return this.api.invokeRequest('POST', 'api/Categories/Add', JSON.stringify(category));
    }

    getCategories(filter: Filter) {
        return this.api.invokeRequest('POST', 'api/Categories' , JSON.stringify(filter), );
    }

    deleteCategory(categoryUUID: string) {
        return this.api.invokeRequest('DELETE', 'api/Categories/Delete/' + categoryUUID , ''    );
    }

    updateCategory(category) {
        return this.api.invokeRequest('PATCH', 'api/Categories/Update', category);
    }
}
