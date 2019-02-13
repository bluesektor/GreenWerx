import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ValidatePage } from './validate';

const routes: Routes = [
  {
    path: '',
    component: ValidatePage
  },
  {
    path: 'validate',
    component: ValidatePage
  },
  { path: 'validate/type/:type/operation/:operation/code/:code', component: ValidatePage }
 // , { path: 'changepassword/operation/:operation/code/:code', component: ChangePasswordComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MembershipRoutingModule { }
