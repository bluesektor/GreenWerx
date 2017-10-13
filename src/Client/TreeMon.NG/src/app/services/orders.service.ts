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
import { Order } from '../models/order';
import { FinanceAccount } from '../models/financeaccount';
import { FinanceAccountTransaction } from '../models/financeaccountransaction';
import { PriceRule } from '../models/pricerule';
import { BasicValidators } from '../common/basicValidators';

@Injectable()
export class OrdersService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    getOrders(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Orders/' + '?filter=' + JSON.stringify(filter));
    }

    getOrder(name: string) {
        return this.invokeRequest('GET', 'api/Orders/' + name);
    }

    getOrderSymbols(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Orders/Symbols');
    }
    getAssetClasses(filter?: Filter) {
        return this.invokeRequest('GET', 'api/AssetClasses');
    }


    addOrder(currency: Order) {
        return this.invokeRequest('POST', 'api/Orders/Add', JSON.stringify(currency));
    }

    updateOrder(currency: Order) {
        return this.invokeRequest('PATCH', 'api/Orders/Update', JSON.stringify(currency));
    }

    deleteOrder(orderUUID: string) {
        return this.invokeRequest('DELETE', 'api/Orders/Delete/' + orderUUID);
    }
}
