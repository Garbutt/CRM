import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

export interface GetUsers{
  Id: number;
  name: string;
  email: string;
  contactNumber: number;
  role: string;
  status: string;
}

@Component({
  selector: 'app-approve-user',
  templateUrl: './approve-user.component.html',
  styleUrl: './approve-user.component.css'
})
export class ApproveUserComponent {
  users: GetUsers[] = [];

  constructor(private httpClient: HttpClient){}
  
  
  ngOnInit(){
    const token = localStorage.getItem('token');
    console.log('Retrieved token:', token);


    if(token){

      const authorizationHeader = new HttpHeaders().set('Authorization',`Bearer ${token}`);
      console.log(authorizationHeader)

    this.httpClient.get<GetUsers[]>('api/user/getAllUsers', { headers: authorizationHeader })
    .subscribe({
      next: (data) => {
      this.users = data;
    }, error: error => {
      console.log('Error fetching users: ', error);
    }
    });
  }
  else{
    console.log('Token not found');
  }
  }

  getApprovalStatus(status: string): string {
    return status === "true" ? "Approved" : "Pending";
  }

}
