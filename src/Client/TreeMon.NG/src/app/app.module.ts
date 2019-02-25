// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

// Angular
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { HttpModule } from '@angular/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
// import { HashLocationStrategy, LocationStrategy } from '@angular/common';

// 3rd Party
import { StepsModule, PanelMenuModule, CheckboxModule, ChartModule } from 'primeng/primeng';
import { CookieService } from 'angular2-cookie/services/cookies.service';

// Components
import { AboutComponent } from './about.component';
import { AppComponent } from './app.component';
import { CartDropdownComponent } from './store/cart.dropdown.component';
import { CommontmModule } from './common/commontm.module';
import { ContactFormComponent } from './contact-form.component';
import { HomeComponent } from './home.component';
import { InstallComponent } from './install.component';
import { NavBarAdminComponent } from './navbar.admin.component';
import { NavBarDefaultComponent } from './navbar.default.component';
import { NotFoundComponent } from './not-found.component';
import { PrivacyPolicyComponent } from './privacy-policy.component';
import { TermsComponent } from './terms.component';


// Services
import { SessionService } from './services/session.service';

import { AppRoutingModule } from './app.routing';

// Modules
import { AccountsModule } from './membership/accounts/accounts.module';
import { SystemModule } from './system/system.module';
import { UtilitiesModule } from './utilities/utilities.module';
import { StoreModule } from './store/store.module';

import { FinanceModule } from './finance/finance.module';
import { GeneralModule } from './general/general.module';
import { GeoModule } from './geo/geo.module';
import { InventoryModule } from './inventory/inventory.module';
import { PlantsModule } from './plants/plants.module';
import { RolesModule } from './membership/roles/roles.module';
import { APIModule } from './membership/api/api.module';
import { UsersModule } from './membership/users/users.module';
import { MessageBoxesModule } from './common/messageboxes.module';
import { GraphsModule } from './common/graphs.module';


@NgModule({
  imports: [
    BrowserAnimationsModule,
    CommontmModule,
    FormsModule,
    PanelMenuModule,
    ReactiveFormsModule,
    AppRoutingModule,
    StepsModule,
    HttpModule,
    AccountsModule,
    APIModule,
    SystemModule,
    UtilitiesModule,
    StoreModule,
    UsersModule,
    InventoryModule,
    FinanceModule,
    RolesModule,
    GeoModule,
    PlantsModule,
    MessageBoxesModule,
    CheckboxModule,
    ChartModule,
    GraphsModule
  ],
  declarations: [
    AboutComponent,
    AppComponent,
    CartDropdownComponent,
    ContactFormComponent,
    HomeComponent,
    InstallComponent,
    NavBarAdminComponent,
    NavBarDefaultComponent,
    NotFoundComponent,
    PrivacyPolicyComponent,
    TermsComponent
  ],
  providers: [SessionService, CookieService ], // ,  { provide: LocationStrategy, useClass: HashLocationStrategy}],
  bootstrap: [AppComponent]
})
export class AppModule { }



