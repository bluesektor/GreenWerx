// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Component } from '@angular/core';
import { AppService } from './services/app.service';
@Component({
    template: `
    <h2>{{pageSettings.appName}}</h2>
    {{pageSettings.privacyPolicy}}
`,
    providers: [AppService]
})

export class PrivacyPolicyComponent {
    pageSettings: any;

    constructor(private _appService: AppService) {

        this.pageSettings = this._appService.getDashboard('privacy', '');
    }
}
