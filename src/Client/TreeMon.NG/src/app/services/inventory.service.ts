// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { InventoryItem } from '../models/inventory';

@Injectable()
export class InventoryService     extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    addToInventory(inventoryItem) {
        return this.invokeRequest('POST', 'api/Inventory/Add', JSON.stringify(inventoryItem));
    }

    uploadImage(image) {
        return this.invokeRequest('POST', 'api/upload', image);
    }

    getInventory(locationUUID: string,   filter?: Filter) {
        return this.invokeRequest('GET', 'api/Inventory/Location/' + locationUUID + '?filter=' + JSON.stringify(filter) );
    }

    // This is different from getInvetory in that it returns published items.
    //
    getStoreInventory(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Store');
    }

    updateInventory(inventoryItems: InventoryItem[]) {
        return this.invokeRequest('PATCH', 'api/Inventory/Updates', JSON.stringify(inventoryItems));
    }

    deleteItem(inventoryItemUUID: string) {
        return this.invokeRequest('DELETE', 'api/Inventory/Delete/' + inventoryItemUUID);
     }
}
