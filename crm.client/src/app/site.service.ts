import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Site } from './all-sites/all-sites.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SiteService {

  constructor(private http: HttpClient) { }

  getSiteById(id: number): Observable<Site>{
    return this.http.get<Site>(`https://localhost:7201/api/sites/getSite/${id}`);
  }

  getSitesByName(name: string): Observable<Site[]>{
    return this.http.get<Site[]>(`https://localhost:7201/api/sites/getSitesByName/${name}`);
  }

  getSites(): Observable<Site[]>{
    return this.http.get<Site[]>('https://localhost:7201/api/sites/getSites');
  }

  updateSite(site: Site, photoFile: File | null): Observable<Site>{
    const formData = new FormData();
    formData.append('id', site.id.toString());
    formData.append('name', site.name);
    formData.append('address', site.address);
    formData.append('completion', site.completion.toString());
    if(photoFile){
      formData.append('photo', photoFile);
    }
    return this.http.put<Site>(`https://localhost:7201/api/sites/updateSite`, formData);
  }

}
