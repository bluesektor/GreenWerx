// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { CookieService } from 'angular2-cookie/services/cookies.service';

import { CheckboxModule } from 'primeng/primeng';
import { MessageBoxesModule } from '../common/messageboxes.module';
import { DataTableModule, SharedModule, DialogModule, AccordionModule, DropdownModule,
     InputSwitchModule, FileUploadModule} from 'primeng/primeng';
import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';

import { CommontmModule } from '../common/commontm.module';
import { InventoryComponent } from './inventory.component';

import { SessionService } from '../services/session.service';

import { InventoryService } from '../services/inventory.service';
import { InventoryRoutingModule} from './inventory.routing';

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
        DropdownModule,
        InputSwitchModule,
        FileUploadModule,
        InventoryRoutingModule,
        MessageBoxesModule
    ]
    ,
    declarations: [
        InventoryComponent    ],
    exports: [
        InventoryComponent
    ],
    providers: [
        InventoryService,
        PreventUnsavedChangesGuard,
        CookieService,
        SessionService
    ]
})
export class InventoryModule {

}
