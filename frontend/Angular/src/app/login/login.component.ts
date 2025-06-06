import { Component, OnInit } from '@angular/core';
import { CentralizeServiceService } from '../centralize-service.service';
import { interval } from 'rxjs';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {

constructor(private _centralizeService : CentralizeServiceService){}

  ngOnInit(){}

  LoginWithX(){
    console.log("Login with X clicked...");
    this._centralizeService.LoginWithX().subscribe((res)=>{
      console.log(res);
      if(res.url != null){
        window.location.href = res.url;
        
        const tocall = interval(5000);
        tocall.subscribe((res)=>{
          console.log('Demo');
        })

        
      }
    })
  }


}
