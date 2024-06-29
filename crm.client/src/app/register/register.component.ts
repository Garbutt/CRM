import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface User{
  name: string;
  contactNumber: number;
  email: string;
  password: string;
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  public user : User = {name: "", contactNumber: 0, email: "", password: ""}

  registrationComplete = false;

  constructor(private http: HttpClient) {}


  registerUser(){
    this.http.post('/api/user/signup', this.user).subscribe({
      next:(response) =>{
        console.log('Registration successful', response);
        alert('Registration successful!');
        this.registrationComplete = true;
      },
      error:(error) => {
        console.error('Registration failed', error);
        alert('Registration failed. Please try again.');
      }
  });
  }
  title = 'Register Account';
}
