import { HttpClient } from '@angular/common/http';
import { AuthService } from '../services/auth.service';

import { Router } from '@angular/router';
import { Component , ChangeDetectorRef } from '@angular/core';

interface LoginResponse{
  token?: string,
  status?: string,
  success: boolean,
  errorMessage?: string
}

interface User{
  email: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  public loginError: string = "";
  public loginComplete: boolean = false;


  public user : User = { email: "", password: "" }

  constructor(private authService: AuthService, private router: Router, private http: HttpClient, private cdRef: ChangeDetectorRef) {}


  loginUser(){
    const { email, password } = this.user;
    this.authService.login(email,password).subscribe({
      next: (result) =>{
        if(result.success) {
          this.router.navigate(['/dashboard'])
          this.loginComplete = true;
          this.cdRef.detectChanges();
          this.loginError = "";
          console.log("success");
        }
        else{
          this.loginError = result.errorMessage || "An error occurred. Please try again.";
          this.cdRef.detectChanges();
        }
      },
      error:() => {
        this.loginError = 'Login failed. Please try again.';
      }
  });
  }
  title = 'Login';

}
