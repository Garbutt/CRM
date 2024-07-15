import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Site } from '../all-sites/all-sites.component';
import { SiteService } from '../services/site.service';
import { NgClass } from '@angular/common';
import { RoleService } from '../services/role.service';

@Component({
  selector: 'app-site-detail',
  templateUrl: './site-detail.component.html',
  styleUrl: './site-detail.component.css'
})
export class SiteDetailComponent implements OnInit {
  site: Site | null = null;
  data: any | null = null;
  options: any;
  selectedFile: File | null = null;
  userRole: string = 'user';
  activateEdit: boolean = false;
  value: number = this.site?.completion ?? 0;

  successMessage: string = '';
  errorMessage: string = '';


  constructor(private route: ActivatedRoute, private siteService: SiteService, private roleService: RoleService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      const defaultPlaceholder = 'https://localhost:7201/uploads/Placeholder.png';
      const documentStyle = getComputedStyle(document.documentElement);
      if(id){
        this.siteService.getSiteById(id).subscribe(site => {
        if(!site.photoPath)
          {
          site.photoPath = defaultPlaceholder;
         }
          this.site = site;
          
          const completedColor = documentStyle.getPropertyValue('--success');
          const notCompletedColor = documentStyle.getPropertyValue('--danger');
          const completion = this.site?.completion ?? 0;
      
          console.log('completion', completion);
      
          this.data = {
            labels: ['Complete', 'Not Complete'],
            datasets: [
                {
                    data: [completion, 100 - completion],
                    backgroundColor: [completedColor, notCompletedColor],
                    hoverBackgroundColor: [completedColor, notCompletedColor]
                }
            ]
        };
        
    const textColor = documentStyle.getPropertyValue('--text-dark');

    this.options = {
      plugins: {
          legend: {
              labels: {
                  usePointStyle: true,
                  color: textColor
              }
          }
      }
  };
        });
      }
    });
    this.userRole = this.roleService.getUserRole();
  }

  onFileSelected(event: any): void {
    this.selectedFile= event.target.files[0];
    console.log(this.selectedFile);
    this.successMessage = 'Image successfully uploaded';
  }

  editSite(): void {
    this.activateEdit = true;
  }

  updateSite(): void {
    if(this.site) {
      const siteId = this.site.id;
      this.siteService.updateSite(this.site, this.selectedFile).subscribe({
        next: (response) => 
          {console.log('Site updated successfully', response),
          this.successMessage = 'Site updated successfully';
        this.siteService.getSiteById(siteId).subscribe({
       next: (updatedSite) => {
          this.site = updatedSite;
          this.activateEdit = false;
          },
          error: (error) => {
            this.errorMessage = 'Error updating site';
            console.log('Error updating site', error);
          }
        });
      },
        error: (error) => console.log('Error updating site', error)
      });
    }
  }
}
