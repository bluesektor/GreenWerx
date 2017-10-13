// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { CookieService } from 'angular2-cookie/services/cookies.service';

import { CheckboxModule } from 'primeng/primeng';

import { DataTableModule, SharedModule, DialogModule, DataGridModule, StepsModule,
         AutoCompleteModule, AccordionModule} from 'primeng/primeng';
import { ConfirmDialogModule, ConfirmationService, GrowlModule, PanelModule,
         InputSwitchModule, FileUploadModule, DropdownModule } from 'primeng/primeng';

import { PreventUnsavedChangesGuard } from '../prevent-unsaved-changes-guard.service';

import { CategoriesModule } from '../common/categories.module';

import { SessionService } from '../services/session.service';

import { InventoryModule } from '../inventory/inventory.module';

import { StoreInventoryComponent } from '../store/storeinventory.component';
import { StoreComponent } from '../store/store.component';
import { DepartmentsComponent} from './departments.component';
import { StoreCategoriesComponent } from './storecategories.component';
import { CartDetailComponent } from './cartdetail.component';
import { CheckOutComponent } from './checkout.component';
import { OrdersComponent } from './orders.component';

import { ProductsComponent } from './products.component';

import { ProductService } from '../services/product.service';

import { PayOptionsComponent } from './payoptions.component';
import { FinanceModule } from '../finance/finance.module';
import { AddressComponent } from './address.component';

import { StoreRoutingModule } from './store.routing';
import { MessageBoxesModule } from '../common/messageboxes.module';

@NgModule({
    imports: [
        CommonModule,
        CategoriesModule,
        FormsModule,
        ReactiveFormsModule,
        HttpModule,
        CheckboxModule,
        DataTableModule,
        SharedModule,
        DialogModule,
        RouterModule,
        InventoryModule,
        DataGridModule,
        PanelModule,
        StepsModule,
        InputSwitchModule,
        FileUploadModule,
        FinanceModule,
        AutoCompleteModule,
        AccordionModule,
        DropdownModule,
        StoreRoutingModule,
        MessageBoxesModule

    ]
    ,
    declarations: [
        DepartmentsComponent,
        ProductsComponent,
        StoreInventoryComponent,
        StoreComponent,
        StoreCategoriesComponent,
        CartDetailComponent,
        CheckOutComponent,
        PayOptionsComponent,
        AddressComponent,
        OrdersComponent
    ],
    exports: [
        DepartmentsComponent,
        ProductsComponent,
        StoreInventoryComponent,
        StoreComponent,
        StoreCategoriesComponent,
        CartDetailComponent,
        CheckOutComponent,
        PayOptionsComponent,
        AddressComponent,
        OrdersComponent
    ],
    providers: [
        ProductService,
        PreventUnsavedChangesGuard,
        CookieService,
        SessionService,
        ConfirmationService
    ]
})
export class StoreModule {

}
