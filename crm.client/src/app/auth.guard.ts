import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './services/auth.service';

@Injectable({
  providedIn: 'root'
})

export class authGuard implements CanActivate{
  constructor(private authService: AuthService,private router: Router){}


  canActivate(): boolean {
    console.log("AuthGuard#canActivate called"); 
    if (this.authService.isLoggedIn()) {
      console.log("atrue")
      return true;
    } else {
      this.router.navigate(['/login']);
      console.log("afalse")
      return false;
    }
  }
}
