// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductsComponent } from './products.component';
import { StoreCategoriesComponent } from './storecategories.component';
import { DepartmentsComponent } from './departments.component';
import { StoreInventoryComponent } from './storeinventory.component';
import { StoreComponent } from './store.component';
import { CartDetailComponent } from './cartdetail.component';
import { CheckOutComponent } from './checkout.component';
import { PayOptionsComponent } from './payoptions.component';
import { OrdersComponent } from './orders.component';

const storeRoutes: Routes = [
    { path: 'departments', component: DepartmentsComponent },
    { path: 'categories', component: StoreCategoriesComponent },
    { path: 'products', component: ProductsComponent },
    { path: 'orders', component: OrdersComponent },
    { path: 'inventory', component: StoreInventoryComponent },
    { path: 'shoppingcart', component: CartDetailComponent },
    { path: 'checkout', component: CheckOutComponent },
    { path: 'payoptions', component: PayOptionsComponent },
    { path: 'store',          component: StoreComponent 	},
    { path: '', component: StoreComponent }
];

@NgModule({
    imports: [ RouterModule.forChild(storeRoutes ) ],
    exports: [ RouterModule ]
  })
export class StoreRoutingModule { }
