import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HeaderComponent } from '../header/header.component';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { BodyComponent } from '../body/body.component';
import { FooterComponent } from '../footer/footer.component';
import { RouterModule } from '@angular/router';


@Component({
  selector: 'app-login-success',
  imports: [HeaderComponent,SidebarComponent,BodyComponent,FooterComponent,RouterModule],
  templateUrl: './login-success.component.html',
  styleUrl: './login-success.component.css'
})
export class LoginSuccessComponent {

constructor(private route: ActivatedRoute, private router: Router) {}


ngOnInit() {
  this.route.queryParams.subscribe(params => {
    console.log("CONGRATULATIONS I AM CALLED");

    const status = params['Status'];

    if (status === "Success") {
      const userProfile = {
        screenName: params['screenName'],
        UserName: params['UserName'],
        Location: params['Location'],
        Description: params['Description'], 
        profileImageUrl: params['profileImageUrl'],
        Email: params['Email'],
        followersCount: params['FollowersCount'],      
        followingCount: params['followingCount'],
        tweetCount: params['TweetCount'],
        mediaCount: params['mediaCount'],
        likeCount:params['likeCount'],
        TwitterUID:params['TwitterUID']
      };

      console.log("Login success for", userProfile);

      // Store in localStorage
      localStorage.setItem('twitterUserProfileData', JSON.stringify(userProfile));

      // Optionally navigate
      this.router.navigate(['/login-success']);
    }
  });
}



// ngOnInit() {
//   this.route.queryParams.subscribe(params => {
//     console.log("CONGRATULATIONS I AM CALLED");
//     const Status = params['Status'];

//     if (Status == "Success") {
//       console.log("Login success for");
//       this.router.navigate(['/login-success']);
//     }

//   });
// }





}
