// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component, Injectable } from '@angular/core';
import { isDevMode } from '@angular/core';
import { Http, RequestOptionsArgs, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { SessionService } from './session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
@Injectable()
export class WebApiService {
    protected _url: string;
    protected _apiRoute: string;

    public BaseUrl() {
        return this._url;
    }

    constructor(protected _http: Http, protected _sessionService: SessionService ) {
     /*   console.log('isDevMode', isDevMode());
        if ( isDevMode() ) {
            this._url = 'http://localhost:51859/';
        } else {
            this._url = 'http://dev.treemon.org/';
        }
        */
       // this._url = 'http://localhost:51859/';
       this._url = 'https://localhost:44318/';
     // this._url = 'https://dev.treemon.org/';
    }

    invokeRequest(verb: string, endPoint: string, parameters?: string, options?: RequestOptionsArgs) {

        if (options == null || options === 'undefined') {
            const headers = new Headers({
                'Content-Type': 'application/json', 'Authorization': 'Bearer ' + this._sessionService.CurrentSession.authToken
            });
            options = new RequestOptions({ headers: headers });
        }
        const url = this._url +  endPoint;

        console.log('invoke:', url);

        switch (verb.toLowerCase()) {
            case 'get':
                return this._http.get(url, options)
                                        .map(res => res.json());
            case 'post':
                return this._http.post(url, parameters, options)
                                        .map(res => res.json());
            case 'patch':
                return this._http.patch(url, parameters, options)
                                        .map(res => res.json());
            case 'put':
                return this._http.put(url, parameters, options)
                                        .map(res => res.json());
            case 'delete':
                return this._http.delete(url, options)
                                        .map(res => res.json());
        }
    }

    // not the best place but I need this for most the objects.
    importItem(item: Node) {
        return this.invokeRequest('POST', 'api/Apps/Import/Item/', JSON.stringify(item));
    }

}
