import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UpdateUserStatus {
  id: number;
  status: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserActionsService {
 apiUrl = 'https://localhost:7201/api/user';

  constructor(private http: HttpClient) { }

  updateUserStatus(user: UpdateUserStatus): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'authorization': `Bearer ${localStorage.getItem('token')}`
    })
    return this.http.post(`${this.apiUrl}/updateUserStatus`, user, { headers: headers});
  }
}
