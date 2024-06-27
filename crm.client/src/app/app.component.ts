import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

interface User{
  name: string;
  email: string;
  password: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent{
  public user : User = {name: "", email: "", password: ""}

  constructor(private http: HttpClient) {}



  registerUser(){
    this.http.post('/api/user/signup', this.user).subscribe({
      next:(response) =>{
        console.log('Registration successful', response);
        alert('Registration successful!');
      },
      error:(error) => {
        console.error('Registration failed', error);
        alert('Registration failed. Please try again.');
      }
  });
  }

  title = 'crm.client';
}
