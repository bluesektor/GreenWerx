// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { CookieService } from 'angular2-cookie/services/cookies.service';
import { MessageBoxesModule } from '../common/messageboxes.module';
import { CheckboxModule } from 'primeng/primeng';

import {
    DataTableModule, SharedModule, DialogModule, AccordionModule, DropdownModule,
    InputSwitchModule, FileUploadModule, AutoCompleteModule, CalendarModule
} from 'primeng/primeng';
import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';

import { CommontmModule } from '../common/commontm.module';
import { CurrencyComponent } from './currency.component';
import { PriceRulesComponent } from './pricerules.component';
import { FinanceAccountsComponent } from './financeaccounts.component';
import { FinanceAccountTransactionsComponent } from './financeaccounttransactions.component';
import { PayPalComponent } from './gateways/paypal.component';
import { SessionService } from '../services/session.service';

import { FinanceService } from '../services/finance.service';
import { FinanceRoutingModule } from './finance.routing';

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
        AutoCompleteModule,
        CalendarModule,
        FinanceRoutingModule,
        MessageBoxesModule
    ]
    ,
    declarations: [
        CurrencyComponent,
        PriceRulesComponent,
        FinanceAccountsComponent,
        FinanceAccountTransactionsComponent,
        PayPalComponent
    ],
    exports: [
        CurrencyComponent,
        PriceRulesComponent,
        FinanceAccountsComponent,
        FinanceAccountTransactionsComponent,
        PayPalComponent
    ],
    providers: [
        FinanceService,
        PreventUnsavedChangesGuard,
        CookieService,
        SessionService
    ]
})
export class FinanceModule {

}
