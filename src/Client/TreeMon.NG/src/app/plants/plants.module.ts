// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { CookieService } from 'angular2-cookie/services/cookies.service';

import { CheckboxModule } from 'primeng/primeng';

import { DataTableModule, SharedModule, DialogModule, DataGridModule, StepsModule, AutoCompleteModule} from 'primeng/primeng';

import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';

import { CommontmModule } from '../common/commontm.module';
import { SessionService } from '../services/session.service';
import { StrainsComponent } from './strains.component';

import { PlantsService } from '../services/plants.service';
import { ConfirmDialogModule, ConfirmationService, GrowlModule, PanelModule, InputSwitchModule, FileUploadModule } from 'primeng/primeng';

import { PlantsRoutingModule } from './plants.routing';
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
        DataGridModule,
        PanelModule,
        StepsModule,
        InputSwitchModule,
        FileUploadModule,
        AutoCompleteModule,
        PlantsRoutingModule,
        MessageBoxesModule


    ]
    ,
    declarations: [
        StrainsComponent
    ],
    exports: [
        StrainsComponent
    ],
    providers: [
        PlantsService,
        PreventUnsavedChangesGuard,
        CookieService,
        SessionService,
        ConfirmationService
    ]
})
export class PlantsModule {

}
