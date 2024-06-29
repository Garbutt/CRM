import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

interface LoginResponse{
  token: string;
  status: string;
}

interface LoginResult{
  success: boolean;
  status: string;
  errorMessage?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private isAuthenticated = false;

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<LoginResult> {
    return this.http.post<LoginResponse>('/api/user/login', {email,password})
    .pipe(
      map(response => {
         // Check if the login was successful and the user is approved
        if( response.token && response.status === 'true'){
          localStorage.setItem('token', response.token);
          console.log(response);
          this.isAuthenticated = true;
          return { success: true, status: 'true'};
        }
        else if(response.status === 'false'){
          return { success: false, status: 'false', errorMessage: 'Your account is waiting for admin approval.'};
        }
        else{
          return{ success: false, status: 'false', errorMessage: 'Login failed. Please check your credentials and try again.'};
        }
      }),
      catchError((error: HttpErrorResponse) =>{
        let errorMessage = 'An error occurred. Please try again.';
        if(error.error && error.error.message){
          errorMessage = error.error.message;
        }
        else if (error.message){
          errorMessage = error.message;
        }
        console.log('Login error: ', error);
        return of({ success: false, status: 'false', errorMessage });
      })
    );
  }
  logout(): void{
    localStorage.removeItem('authToken');
    this.isAuthenticated = false;
  }

  isLoggedIn(): boolean{
    return this.isAuthenticated || !!localStorage.getItem('authToken')
  }
}
