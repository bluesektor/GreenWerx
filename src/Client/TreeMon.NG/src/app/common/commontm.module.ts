// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PickListModule } from 'primeng/primeng';


import { PickListComponent } from './picklist.component';

@NgModule({
    imports: [
        CommonModule,
        PickListModule
    ],
    declarations: [
        PickListComponent
    ],
    exports: [
        PickListComponent
    ]
})
export class CommontmModule {

}
