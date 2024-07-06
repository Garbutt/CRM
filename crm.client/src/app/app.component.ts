
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router'
import { PrimeNGConfig } from 'primeng/api';



@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit{

  constructor(private router: Router, private primeConfig: PrimeNGConfig) {}

  ngOnInit() {
    this.primeConfig.ripple = true;
  }

 title = 'G&D'

}
