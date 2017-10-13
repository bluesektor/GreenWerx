// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component } from '@angular/core';
import { AppService } from './services/app.service';

@Component({
    template: `<h1>Home</h1>`
})
export class HomeComponent {

    baseUrl: string;
    // <router-outlet></router-outlet>

    constructor( private _appService: AppService ) {
        this.baseUrl = this._appService.BaseUrl();
    }

}
