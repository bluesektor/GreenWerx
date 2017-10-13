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
export class ProductService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    getProductCategories(filter: Filter) {
        return this.invokeRequest('GET', 'api/Products/Categories?filter=' + JSON.stringify(filter) , ''    );
    }

    addProduct(product) {
        return this.invokeRequest('POST', 'api/Products/Add', JSON.stringify(product));
    }

    deleteProduct(productUUID) {
        return this.invokeRequest('DELETE', 'api/Products/Delete/' + productUUID, ''    );
    }

    getProducts(filter: Filter) {
        return this.invokeRequest('GET', 'api/Products?filter=' + JSON.stringify(filter) );
    }

    getProduct(productId) {
        return this.invokeRequest('GET', 'api/ProductsBy/' + productId, ''    );
    }

    getProductDetails(productId, productType) {
        return this.invokeRequest('GET', 'api/Product/' + productId + '/' + productType + '/Details' , ''    );
    }

    updateProduct(product) {
        return this.invokeRequest('PATCH', 'api/Products/Update', product);
    }


    // ===--- Vendors ---===
    addVendor(vendor) {
        return this.invokeRequest('POST', 'api/Vendors/Add', JSON.stringify(vendor));
    }

    getVendors(filter?: Filter) {

        return this.invokeRequest('GET', 'api/Vendors?filter=' + JSON.stringify(filter), );
    }

    deleteVendor(vendorUUID) {
        return this.invokeRequest('DELETE', 'api/Vendors/Delete/' + vendorUUID, ''    );
    }

    updateVendor(vendor) {
        return this.invokeRequest('PATCH', 'api/Vendors/Update', vendor);
    }
}
