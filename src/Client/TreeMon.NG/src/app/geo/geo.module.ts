// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { CookieService } from 'angular2-cookie/services/cookies.service';

import { CheckboxModule } from 'primeng/primeng';

import { DataTableModule, SharedModule, DialogModule, AccordionModule, AutoCompleteModule } from 'primeng/primeng';
import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';

import { CommontmModule } from '../common/commontm.module';
import { LocationsComponent } from './locations.component';
import { MessageBoxesModule } from '../common/messageboxes.module';
import { SessionService } from '../services/session.service';

import { GeoService } from '../services/geo.service';
import {GeoRoutingModule} from './geo.routing';
@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpModule,
        CommontmModule,
        CheckboxModule,
        DataTableModule,
        AccordionModule,
        SharedModule,
        DialogModule,
        RouterModule,
        GeoRoutingModule ,
        MessageBoxesModule,
        AutoCompleteModule
    ]
    ,
    declarations: [
        LocationsComponent
    ],
    exports: [
        LocationsComponent
    ],
    providers: [
        GeoService,
        PreventUnsavedChangesGuard,
        CookieService,
        SessionService
    ]
})
export class GeoModule {

}
