// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { PickListModule } from 'primeng/primeng';
import { DataTableModule, SharedModule, DialogModule, CheckboxModule  } from 'primeng/primeng';
import { MessageBoxesModule } from './messageboxes.module';
import { CategoriesComponent} from './categories.component';


@NgModule({
    imports: [
        FormsModule,
        CommonModule,
        MessageBoxesModule,
        DataTableModule,
        SharedModule,
        DialogModule,
        CheckboxModule
    ],
    declarations: [
        CategoriesComponent
    ],
    exports: [
      CategoriesComponent
    ]
})
export class CategoriesModule {

}
