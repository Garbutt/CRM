import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserActionsService } from '../services/user-actions.service';
import { RoleService } from '../services/role.service';
import { catchError, pipe, throwError } from 'rxjs';

export interface GetUsers{
  id: number;
  name: string;
  email: string;
  contactNumber: number;
  role: string;
  status: string;
}

export interface UpdateUsers{
  id: number;
  status: string;
}

@Component({
  selector: 'app-approve-user',
  templateUrl: './approve-user.component.html',
  styleUrl: './approve-user.component.css'
})
export class ApproveUserComponent {
  users: GetUsers[] = [];
  updateUsers: UpdateUsers = { id: 0, status: '' };

  successClass = 'success';
  pendingClass = 'pending';

  constructor(private httpClient: HttpClient, private userService: UserActionsService, private roleService: RoleService){}
  
   errorMessage: string = "";
   successMessage: string = "";

  ngOnInit(){
    const token = localStorage.getItem('token');

    if(token){

      const authorizationHeader = new HttpHeaders().set('Authorization',`Bearer ${token}`);

    this.httpClient.get<GetUsers[]>('api/user/getAllUsers', { headers: authorizationHeader })
    .pipe(
      catchError(error => {
        if(error.status === 401){
          this.errorMessage = 'Admin role is required to view all users.';
        }
        return throwError(() => new Error('Error fetching users'));
    }),
  )
    .subscribe({
      next: (data) => {
      this.users = data;
      console.log('Users fetched successfully: ', data);
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
    return status === "true" ? 'Approved' : 'pending';
  }

  getStatusClass(status: string): string{
    return status ==="true" ? 'success' : 'pending';
  }


  updateUserStatus(userId: number, newStatus: string){
    this.updateUsers = { id: userId, status: newStatus };
    this.userService.updateUserStatus(this.updateUsers).subscribe({
      next: (data) => {
        console.log('User status updated successfully: ', data);
        const index = this.users.findIndex(user => user.id === userId);
        this.successMessage = 'User status updated successfully';
        if(index !== -1){
          const updateUsers = [...this.users];
          updateUsers[index] = {...updateUsers[index], status: this.updateUsers.status};
          this.users = updateUsers;
        }
      }, error: error => {
        console.log('Error updating user status: ', error ,"Could not update user Id: ", userId);
        console.log("User ID: ", this.updateUsers.id, "Status: ", this.updateUsers.status);
      }
    });
  }

}
