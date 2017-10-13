// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AccountsComponent } from './accounts.component';
import { PreventUnsavedChangesGuard } from '../../prevent-unsaved-changes-guard.service';

const accountsRoutes: Routes = [
  { path: 'accounts', component: AccountsComponent },
];

@NgModule({
  imports: [RouterModule.forChild(accountsRoutes)],
  exports: [RouterModule]
})
export class AccountsRoutingModule { }


