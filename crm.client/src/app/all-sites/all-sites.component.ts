import { Component } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';


export interface Site{
  id: number;
  name: string;
  address: string;
  completion: number;
  photoPath?: string;
}

@Component({
  selector: 'app-all-sites',
  templateUrl: './all-sites.component.html',
  styleUrl: './all-sites.component.css'
})
export class AllSitesComponent {
   _getSites: Site[] = [];

  newSite: Site = { id: 0, name: '', address: '', completion: 0};
  sites: Site[] = [];
  isFormVisible: boolean = false;
  selectedFile: File | null = null;
  

  constructor(private http: HttpClient, private router: Router){}

  ngOnInit(){
    this.getSite();
  }

  toggleFormVisibility(){
    this.isFormVisible = !this.isFormVisible;
  }

  addSite(siteForm: NgForm){
    if(siteForm.invalid){
      console.log("Invalid form.");
      return;
    }

    const formData = new FormData();
    formData.append('name', this.newSite.name);
    formData.append('address', this.newSite.address);
    if(this.selectedFile){
      formData.append('PhotoFile', this.selectedFile, this.selectedFile.name);
    }
    
    const token = localStorage.getItem('token');
    console.log("Retrieved token: ", token);

    if(token){
      const headers =  new HttpHeaders().set('Authorization',`Bearer ${token}`);

      this.http.post<Site[]>('https://localhost:7201/api/sites/addSite', formData , {
        headers: headers,
        reportProgress: true,
        observe: 'body'
      })
      .subscribe({
        next: (response) =>{
         console.log("Site added successfully.", response);

         this.newSite = {id: 0, name: '', address: '', completion: 0};
         this.selectedFile = null;
         this.getSite(); //Refresh the list of sites
        },error: error =>{
          console.log("Error adding sites: ", error);
        }
      });
    }
    else{
      console.log("Token not found.")
    }
  }

  onFileSelected(event: Event){
      const element = event.currentTarget as HTMLInputElement;
      let fileList: FileList | null = element.files;
      if(fileList){
        this.selectedFile = fileList[0];
        console.log("Selected file: ", this.selectedFile);
      }
  }

  getSite(){
    const defaultPlaceholder = 'https://localhost:7201/uploads/Placeholder.png';

    this.http.get<Site[]>('https://localhost:7201/api/sites/getSite')
    .subscribe({
      next: (sites) => {
        console.log("Sites: ", sites);
        this._getSites = sites.map(site => ({
          ...site,
          photoPath: site.photoPath || defaultPlaceholder
        }));
      }, error: error => {
        console.log("Error getting sites: ", error);
      }
    });
  }

  navigateToSiteDetail(siteId: number){
    this.router.navigate([`/dashboard/siteDetails/${siteId}`]);
    console.log("Navigating to site details for site id: ", siteId);
  }
}
