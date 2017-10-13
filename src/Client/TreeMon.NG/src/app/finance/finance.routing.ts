// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';
import { CurrencyComponent } from './currency.component';
import { PriceRulesComponent } from './pricerules.component';
import { FinanceAccountsComponent } from './financeaccounts.component';
import { FinanceAccountTransactionsComponent } from './financeaccounttransactions.component';

const financeRoutes: Routes = [
  { path: 'pricerules', component: PriceRulesComponent },
  { path: 'currency', component: CurrencyComponent },
  { path: 'accounts', component: FinanceAccountsComponent },
  { path: 'transactions', component: FinanceAccountTransactionsComponent }
];

@NgModule({
  imports: [RouterModule.forChild(financeRoutes)],
  exports: [RouterModule]
})
export class FinanceRoutingModule { }
