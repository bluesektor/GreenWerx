// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HomeComponent } from './home.component';
import { AboutComponent } from './about.component';
import { ContactFormComponent } from './contact-form.component';
import { TermsComponent } from './terms.component';
import { InstallComponent } from './install.component';

import { PrivacyPolicyComponent } from './privacy-policy.component';

import { NotFoundComponent } from './not-found.component';
// import { CategoriesComponent } from '../app/admin/categories.component';

const routes: Routes = [
  // { path: 'categories', component: CategoriesComponent },
  { path: 'system', loadChildren: 'app/system/system.module#SystemModule' },
  { path: 'utilities', loadChildren: 'app/utilities/utilities.module#UtilitiesModule' },
  { path: 'finance', loadChildren: 'app/finance/finance.module#FinanceModule' },
  { path: 'general', loadChildren: 'app/general/general.module#GeneralModule' },
  { path: 'membership', loadChildren: 'app/membership/roles/roles.module#RolesModule' },
  { path: 'membership', loadChildren: 'app/membership/accounts/accounts.module#AccountsModule' },
  { path: 'membership', loadChildren: 'app/membership/users/users.module#UsersModule' },
  { path: 'assets', loadChildren: 'app/store/store.module#StoreModule' },
  { path: 'assets', loadChildren: 'app/plants/plants.module#PlantsModule' },
  { path: 'assets', loadChildren: 'app/inventory/inventory.module#InventoryModule' },
  { path: 'assets', loadChildren: 'app/geo/geo.module#GeoModule' },
  { path: 'store', loadChildren: 'app/store/store.module#StoreModule' },
  { path: '', component: HomeComponent },
  { path: 'home', component: HomeComponent },
  { path: 'about', component: AboutComponent },
  { path: 'contact', component: ContactFormComponent },
  { path: 'privacy', component: PrivacyPolicyComponent },
  { path: 'terms', component: TermsComponent },
  { path: 'install', component: InstallComponent },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: 'not-found' }
];
@NgModule({
  imports: [RouterModule.forRoot(routes
    //   , { enableTracing:true }
  )],
  exports: [RouterModule]
})
export class AppRoutingModule { }



