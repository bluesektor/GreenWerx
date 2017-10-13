// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { GeneralComponent } from './general.component';
import { CategoriesComponent } from './categories.component';
import { UnitsOfMeasureComponent } from './unitsofmeasure.component';


const generalRoutes: Routes = [
    { path: 'measures', component: UnitsOfMeasureComponent },
    { path: 'categories', component: CategoriesComponent },
    { path: '', component: GeneralComponent }
];

@NgModule({
    imports: [ RouterModule.forChild(generalRoutes ) ],
    exports: [ RouterModule ]
  })
export class GeneralRoutingModule { }
