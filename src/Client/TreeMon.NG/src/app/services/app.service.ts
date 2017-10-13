// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { AppInfo } from '../models/appinfo';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';


@Injectable()
export class AppService  extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }

    testIPN(params: string) {
        return this.invokeRequest('POST', 'api/PayPal/IPN');
    }

    getPublicSettings(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Apps/Public/Settings?filter=' + JSON.stringify(filter) );
    }

    getAppStatus() {
        return this.invokeRequest('GET', 'api/Apps/web/Status' , ''    );
    }

    getDashboard(viewName: string, options: string) {

        const headers = new Headers({ 'Content-Type': 'application/json' });
        const reqOptions = new RequestOptions({ headers: headers });

        const url = 'api/Apps/Dashboard/' + viewName;

        return this.invokeRequest('POST', url, options, reqOptions);
    }


    getTemplate(templateName: string, replaceOptions: string) {

        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        const url = 'api/Apps/Template/' + templateName + '/Replace/' + replaceOptions;

        return this.invokeRequest('GET', url, '', options);
    }

    sendMessage(message) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Site/SendMessage', message, options);
    }

    installApp(appInfo: AppInfo) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Apps/Install', JSON.stringify( appInfo), options);
    }

    CreateDatabase(appInfo: AppInfo) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Apps/Install/CreateDatabase', JSON.stringify( appInfo), options);
    }

    SaveSettings(appInfo: AppInfo) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Apps/Install/SaveSettings', JSON.stringify( appInfo), options);
    }
    SeedDatabase(appInfo: AppInfo) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Apps/Install/SeedDatabase', JSON.stringify( appInfo), options);
    }
    AddAccounts(appInfo: AppInfo) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Apps/Install/Accounts', JSON.stringify( appInfo), options);
    }
    Finalize(appInfo: AppInfo) {
        const headers = new Headers({ 'Content-Type': 'application/json' });
        const options = new RequestOptions({ headers: headers });

        return this.invokeRequest('POST', 'api/Apps/Install/Finalize', JSON.stringify( appInfo), options);
    }
    getDefaults(type: string, filter?: Filter) {
        return this.invokeRequest('GET', 'api/Apps/DefaultData/' + type + '?filter=' + JSON.stringify(filter) );
    }


    dataTypes() {
        return this.invokeRequest('GET', 'api/Apps/DataTypes');

    }

    tableNames() {
        return this.invokeRequest('GET', 'api/Apps/TableNames');
    }

    scanForDuplicates(tableName: string) {
        return this.invokeRequest('GET', 'api/App/Tables/ScanNames/' + tableName);
    }

    searchTables(name: string, values: string[]) {
        return this.invokeRequest('POST', 'api/App/Tables/Search/' + name, JSON.stringify(values) );
    }

    deleteItem(table: string, uuid: string) {
        return this.invokeRequest('DELETE', 'api/Apps/Tables/' + table + '/DeleteItem/' + uuid);
    }
}
