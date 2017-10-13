
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule } from '@angular/http';
import { CookieService } from 'angular2-cookie/services/cookies.service';

import { CheckboxModule } from 'primeng/primeng';

import { DataTableModule, SharedModule, DialogModule } from 'primeng/primeng';

import { PreventUnsavedChangesGuard } from '../../prevent-unsaved-changes-guard.service';
import { MessageBoxesModule } from '../../common/messageboxes.module';
import { CommontmModule } from '../../common/commontm.module';

import { SessionService } from '../../services/session.service';

import { User } from '../../models/user';
import { UserProfileComponent } from './profile.component';
import { UsersComponent } from './users.component';
import { UserService } from '../../services/user.service';
import { LoginComponent } from './login.component';
import { LoginHelpComponent } from './login-help.component';
import { ChangePasswordComponent } from './change-password.component';
import { UsersValidateComponent } from './users-validate.component';
import { UsersRoutingModule } from './users.routing';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        HttpModule,
        CommontmModule,
        CheckboxModule,
        DataTableModule,
        SharedModule,
        DialogModule,
        RouterModule,
        UsersRoutingModule,
        MessageBoxesModule
    ]
    ,
    declarations: [
        UserProfileComponent,
        UsersComponent,
        LoginComponent,
        LoginHelpComponent,

        ChangePasswordComponent,
        UsersValidateComponent
    ],
    exports: [
        UserProfileComponent,
        UsersComponent
    ],
    providers: [
        UserService,
        PreventUnsavedChangesGuard,
        CookieService,
        SessionService
    ]
})
export class UsersModule {

}
