// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
import { NgModule } from '@angular/core';
import { CategoriesModule } from '../common/categories.module';
import { MeasuresModule } from '../common/measures.module';

import { CategoriesComponent } from './categories.component';
import { GeneralComponent } from './general.component';
import { GeneralRoutingModule } from './general.routing';
import { UnitsOfMeasureComponent } from './unitsofmeasure.component';

@NgModule({
    imports: [
        CategoriesModule,
        MeasuresModule,
        GeneralRoutingModule
    ],
    declarations: [
        CategoriesComponent,
        GeneralComponent,
        UnitsOfMeasureComponent
    ],
    exports: [
        CategoriesComponent,
        GeneralComponent,
        UnitsOfMeasureComponent
    ]
})
export class GeneralModule {

}
