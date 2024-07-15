import { NgModule } from '@angular/core';
import { RouterModule, Routes, Router, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';

import { DashboardComponent } from './dashboard/dashboard.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ApproveUserComponent } from './approve-user/approve-user.component';
import { AllSitesComponent } from './all-sites/all-sites.component';
import { SiteDetailComponent } from './site-detail/site-detail.component';

import { authGuard } from './auth.guard';


const routes: Routes = [
  {
  path: 'dashboard',
  component : DashboardComponent,
  children: [

  {path: 'user/getAllUsers', component: ApproveUserComponent},
  {path: 'user/forgotPassword', component: ForgotPasswordComponent},
  {path: 'user/changePassword', component: ChangePasswordComponent},
  {path: 'sites/getSite', component: AllSitesComponent},
  {path: 'siteDetails/:id', component: SiteDetailComponent}

  ],
  canActivate: [authGuard]
},


{path: 'user/forgotPassword', component: ForgotPasswordComponent},
{path: 'user/changePassword', component: ChangePasswordComponent},
{path: 'login', component: LoginComponent},
{path: 'register', component: RegisterComponent},
{path: '', redirectTo: '/login', pathMatch: 'full'},
{path: 'dashboard', component: DashboardComponent, canActivate: [authGuard]}
]

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { 
  constructor(private router: Router) {
   
  }
}
