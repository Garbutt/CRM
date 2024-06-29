import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { authGuard } from './auth.guard';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { ApproveUserComponent } from './approve-user/approve-user.component';

const routes: Routes = [
  {path: 'G&D', component: LandingPageComponent},
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'getAllUsers', component: ApproveUserComponent},
  {path: 'dashboard', component: DashboardComponent, canActivate: [authGuard] },
  {path: '', redirectTo: '/login', pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
