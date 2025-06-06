import { Component } from '@angular/core';
import { StatCardComponent } from './stat-card/stat-card.component';
import { RouterModule,Routes } from '@angular/router';

@Component({
  selector: 'app-body',
  imports: [StatCardComponent,RouterModule],
  templateUrl: './body.component.html',
  styleUrl: './body.component.css'
})
export class BodyComponent {

}
