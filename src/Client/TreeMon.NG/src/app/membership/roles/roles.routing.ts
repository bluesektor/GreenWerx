// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RolesComponent } from './roles.component';
import { PreventUnsavedChangesGuard } from '../../prevent-unsaved-changes-guard.service';


 const rolesRoutes:  Routes = [
    { path: 'roles', component: RolesComponent }
];

@NgModule({
  imports: [ RouterModule.forChild( rolesRoutes ) ],
  exports: [ RouterModule ]
})
export class RolesRoutingModule { }
