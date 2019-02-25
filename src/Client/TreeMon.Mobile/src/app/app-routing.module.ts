import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent } from './not-found.component';
const routes: Routes = [
  { path: '', loadChildren: './pages/tabs/tabs.module#TabsPageModule' },
  { path: 'login', loadChildren: './pages/login/login.module#LoginModule' },
  { path: 'signup', loadChildren: './pages/signup/signup.module#SignUpModule' },
  { path: 'admin', loadChildren: './pages/admin/admin.module#AdminModule' },
  { path: 'membership', loadChildren: './pages/membership/membership.module#MembershipModule' },
  { path: 'validate', loadChildren: './pages/membership/membership.module#MembershipModule' },
  { path: 'account', loadChildren: './pages/account/account.module#AccountModule' },
  { path: 'support', loadChildren: './pages/support/support.module#SupportModule' },
  { path: 'tutorial', loadChildren: './pages/tutorial/tutorial.module#TutorialModule' },
 // XXXX { path: 'membership', loadChildren: 'app/membership/membership.module#MembershipModule' },
  { path: 'membership/validate', loadChildren: './pages/membership/membership.module#MembershipModule' },
  // XXX { path: 'membership', loadChildren: 'membership/membership.module#MembershipModule' },
  // XXX { path: 'membership', loadChildren: './membership/membership.module#MembershipModule' },
  { path: 'not-found', component: NotFoundComponent },
  { path: '**', redirectTo: 'not-found' },
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
