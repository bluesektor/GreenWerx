// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { CookieService } from 'angular2-cookie/services/cookies.service';

import { CheckboxModule } from 'primeng/primeng';

import { DataTableModule, SharedModule, DialogModule, FileUploadModule } from 'primeng/primeng';

import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';

import { CommontmModule } from '../common/commontm.module';

import { SessionService } from '../services/session.service';

import { ToolsComponent } from './tools.component';
import { AdminService } from '../services/admin.service';
import { SettingsService } from '../services/settings.service';
import { UtilitiesRoutingModule} from './utilities.routing';
import { MessageBoxesModule } from '../common/messageboxes.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpModule,
        CommontmModule,
        CheckboxModule,
        DataTableModule,
        SharedModule,
        DialogModule,
        RouterModule,
        FileUploadModule,
        UtilitiesRoutingModule,
        MessageBoxesModule
    ]
    ,
    declarations: [
        ToolsComponent
    ],
    exports: [
        ToolsComponent
    ],
    providers: [
        AdminService,
        SettingsService,
        PreventUnsavedChangesGuard,
        CookieService,
        SessionService
    ]
})
export class UtilitiesModule {

}
