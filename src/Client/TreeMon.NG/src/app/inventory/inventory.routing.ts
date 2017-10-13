// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';
import { InventoryComponent } from './inventory.component';

 const inventoryRoutes:  Routes = [
    { path: 'inventory', component: InventoryComponent }
];

@NgModule({
    imports: [ RouterModule.forChild( inventoryRoutes ) ],
    exports: [ RouterModule ]
  })
  export class InventoryRoutingModule { }
