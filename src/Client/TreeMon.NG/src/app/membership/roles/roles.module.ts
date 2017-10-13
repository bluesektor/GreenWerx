// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { PreventUnsavedChangesGuard } from '../../prevent-unsaved-changes-guard.service';

import { AccordionModule } from 'primeng/primeng';
import { CheckboxModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';

import { CommontmModule } from '../../common/commontm.module';
import {RolesComponent } from './roles.component';
import { SessionService } from '../../services/session.service';
import { RoleService } from '../../services/roles.service';

import { ConfirmDialogModule, ConfirmationService, GrowlModule } from 'primeng/primeng';
import {RolesRoutingModule} from './roles.routing';
import { MessageBoxesModule } from '../../common/messageboxes.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpModule,
        CommontmModule ,
        RouterModule,
        AccordionModule,
        CheckboxModule,
        PickListModule,
        ConfirmDialogModule,
        GrowlModule,
        RolesRoutingModule,
        MessageBoxesModule

    ]
    ,
    declarations: [
        RolesComponent

    ],
    exports: [
        RolesComponent
    ]   ,
    providers: [
        RoleService,
        PreventUnsavedChangesGuard,
        SessionService,
        ConfirmationService
    ]
})
export class RolesModule {

}
