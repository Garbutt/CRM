import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { provideHttpClient, withInterceptorsFromDi} from '@angular/common/http';
import { CommonModule } from '@angular/common';


// For prime ng modules
import { FloatLabelModule } from 'primeng/floatlabel';
import { TableModule } from 'primeng/table';
import { Button } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ChartModule } from 'primeng/chart';
import { SliderModule } from 'primeng/slider';




import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { LandingPageComponent } from './landing-page/landing-page.component';
import { ApproveUserComponent } from './approve-user/approve-user.component';
import { AllSitesComponent } from './all-sites/all-sites.component';
import { SiteDetailComponent } from './site-detail/site-detail.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    ChangePasswordComponent,
    ForgotPasswordComponent,
    DashboardComponent,
    LandingPageComponent,
    ApproveUserComponent,
    AllSitesComponent,
    SiteDetailComponent,

  ],
  imports: [
    BrowserModule, 
    BrowserAnimationsModule,
    FormsModule,
    AppRoutingModule,
    CommonModule,
    FloatLabelModule,
    TableModule,
    Button,
    DataViewModule,
    ChartModule,
    SliderModule,
  ],
  providers: [provideHttpClient(withInterceptorsFromDi())],
  bootstrap: [AppComponent]
})
export class AppModule { }
