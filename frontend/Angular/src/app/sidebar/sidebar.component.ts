import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-sidebar',
  imports: [],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.css'
})
export class SidebarComponent { 

constructor(private router: Router) {}

navigateTo(path: string) {
  console.log("Navigating to:", path);
  this.router.navigate([path]);
}


}
