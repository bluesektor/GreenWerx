import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserProfileComponent } from './profile.component';
import { UsersComponent } from './users.component';
import { PreventUnsavedChangesGuard } from '../../prevent-unsaved-changes-guard.service';
import { LoginComponent } from './login.component';
import { LoginHelpComponent } from './login-help.component';
import { ChangePasswordComponent } from './change-password.component';

import { UsersValidateComponent} from './users-validate.component';


 const usersRouting:  Routes = [
  { path: 'users', component: UsersComponent },
  { path: 'Profile', component: UserProfileComponent },
    { path: 'login', component: LoginComponent },
    { path: 'changepassword', component: ChangePasswordComponent },
    { path: 'changepassword/operation/:operation/code/:code', component: ChangePasswordComponent },
    { path: 'validate/type/:type/operation/:operation/code/:code', component: UsersValidateComponent },
    { path: 'login-help', component: LoginHelpComponent },
    { path: ':id', component: UserProfileComponent, canDeactivate: [PreventUnsavedChangesGuard] },
    { path: 'new', component: UserProfileComponent, canDeactivate: [PreventUnsavedChangesGuard] }


];

@NgModule({
  imports: [ RouterModule.forChild( usersRouting ) ],
  exports: [ RouterModule ]
})
export class UsersRoutingModule { }
