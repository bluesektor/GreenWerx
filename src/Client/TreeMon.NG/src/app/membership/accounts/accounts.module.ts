// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
    FormsModule,
    ReactiveFormsModule
} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';

import { AccountComponent } from './account.component';
import { AccountsComponent } from './accounts.component';
import { AccountService } from '../../services/account.service';
import { PreventUnsavedChangesGuard } from '../../prevent-unsaved-changes-guard.service';
import { CommontmModule  } from '../../common/commontm.module';
import { MessageBoxesModule } from '../../common/messageboxes.module';
import { SessionService } from '../../services/session.service';
import { AccordionModule } from 'primeng/primeng';
import { CheckboxModule } from 'primeng/primeng';
import { PickListModule } from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule } from 'primeng/primeng';
import { AccountsRoutingModule} from './accounts.routing';

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
        AccountsRoutingModule,
        MessageBoxesModule

    ]
    ,
    declarations: [
        AccountsComponent,
        AccountComponent

    ],
    exports: [
        AccountsComponent,
        AccountComponent
    ]   ,
    providers: [
        AccountService,
        PreventUnsavedChangesGuard,
        SessionService,
        ConfirmationService
    ]
})
export class AccountsModule {

}
