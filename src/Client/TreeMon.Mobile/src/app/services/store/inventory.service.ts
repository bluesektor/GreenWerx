// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import 'rxjs/add/operator/map';
import { Api } from '../api/api';
import { Filter } from '../../models/filter';
import { InventoryItem } from '../../models/inventory';

@Injectable({
    providedIn: 'root'
  })
export class InventoryService      {

    constructor(private api: Api) {    }
    addToInventory(inventoryItem) {
        return this.api.invokeRequest('POST', 'api/Inventory/Add', inventoryItem);
    }

    uploadImage(image) {
        return this.api.invokeRequest('POST', 'api/upload', image);
    }

    getInventory(locationUUID: string,   filter?: Filter) {
        return this.api.invokeRequest('POST', 'api/Public/Inventory/Location/' + locationUUID , JSON.stringify(filter) );
    }

    // This is different from getInvetory in that it returns published items.
    //
    getStoreInventory(filter?: Filter) {
        return this.api.invokeRequest('POST', 'api/Store', JSON.stringify(filter) );
    }

    getPublishedInventoryByLocation(locationName: string, distance: number, filter?: Filter) {
        if (locationName === '' || locationName === null || locationName === undefined) {
            return this.getPublishedInventory(filter);
        }
        return this.api.invokeRequest('POST', 'api/Public/Inventory/' + locationName + '/distance/' + distance,  JSON.stringify(filter));
    }

    getPublishedInventory(filter?: Filter) {
       return this.api.invokeRequest('POST', 'api/Public/Inventory/Published',  JSON.stringify(filter));
   }

    searchPublishedInventory(locationName: string, distance: number, filter?: Filter) {
         if (!locationName || locationName === '') {
             locationName = ' ';
         }
        return this.api.invokeRequest('POST', 'api/Public/Inventory/' + locationName + '/distance/' + distance + '/search',
        JSON.stringify(filter));
    }

    publishItem(itemUUID: string) {
        return this.api.invokeRequest('PATCH', 'api/Inventory/Publish/' + itemUUID );
    }
    getUserInventory(userUUID: string,  filter?: Filter) {
        return this.api.invokeRequest('POST', 'api/Inventory/User/' + userUUID , JSON.stringify(filter) );
    }

    getDetails(itemUUID: string) {
        return this.api.invokeRequest('GET', 'api/Public/Item/' + itemUUID + '/Details' );
    }

    updateItem(inventoryItem: InventoryItem) {
        return this.api.invokeRequest('PATCH', 'api/Inventory/Update', JSON.stringify(inventoryItem));
    }

    updateItems(inventoryItems: InventoryItem[]) {
        return this.api.invokeRequest('PATCH', 'api/Inventory/Updates', JSON.stringify(inventoryItems));
    }

    deleteItem(inventoryItemUUID: string) {
        return this.api.invokeRequest('DELETE', 'api/Inventory/Delete/' + inventoryItemUUID);
     }

    deleteImage(inventoryItemUUID: string, fileName: string) {
          return this.api.invokeRequest('DELETE', 'api/Inventory/Delete/' + inventoryItemUUID + '/File/' + fileName);
    }

    uploadFormEx( form: FormData, UUID: string, type: string) {
         return this.api.uploadForm( '/api/file/upload/' + UUID + '/' + type, form); }

    uploadFileEx( files: File[], accountUUID: string, type: string) {
        return this.api.uploadFile( '/api/file/upload/' + accountUUID + '/' + type, files);
    }

    getImages(itemUUID: string) {
        return this.api.invokeRequest('GET', 'api/Public/Inventory/' + itemUUID + '/Images' );
    }
}
