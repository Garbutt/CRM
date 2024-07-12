import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userRole: string = 'user';

  constructor() { }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUserRole(): string {
    const token = this.getToken();
    if(token) {
      const payloadBase64 = token.split('.')[1];
      const decodedJson = atob(payloadBase64);
      const decoded = JSON.parse(decodedJson);
      return decoded.role;
    }
    return 'user';
  }

  setToken(token: string): void {
    localStorage.setItem('token', token);
  }
}
