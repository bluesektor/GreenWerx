// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { APIComponent } from './api.component';
import { KeysComponent } from './keys.component';
import { PreventUnsavedChangesGuard } from '../../prevent-unsaved-changes-guard.service';

const apiRoutes: Routes = [
  { path: 'api', component: APIComponent },
  { path: 'apikeys', component: KeysComponent }
];

@NgModule({
  imports: [RouterModule.forChild(apiRoutes)],
  exports: [RouterModule]
})
export class APIRoutingModule { }


