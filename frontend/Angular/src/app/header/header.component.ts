import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TwitterUserProfile } from '../Models/twitter-user-profile.model';
import { CentralizeServiceService } from '../centralize-service.service';


@Component({
  selector: 'app-header',
  imports: [CommonModule,FormsModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
   

constructor(private _centralizeService : CentralizeServiceService){}

  profile!: TwitterUserProfile;
  showUserMenu: boolean = false;
  userProfile = {
    screenName: '',
    UserName: '',
    Location:'',
    Description:'',
    FollowersCount:'',
    TweetCount:'',
    Email:'',
    profileImageUrl:'',
    TwitterUID:''
  };

 
  
  ngOnInit(): void {
    this.showUserMenuFun()
    const profileJson = localStorage.getItem('twitterUserProfileData');
    if (profileJson) {
      this.profile = JSON.parse(profileJson) as TwitterUserProfile;

      this.userProfile.screenName = this.profile.screenName;
      this.userProfile.UserName = '@' + (this.profile.UserName || '').toLowerCase().replace(/\s+/g, '');
      this.userProfile.Location = this.profile.Location;
      this.userProfile.Description = this.profile.Description;
      this.userProfile.FollowersCount = this.profile.followersCount;
      this.userProfile.TweetCount = this.profile.tweetCount;
      this.userProfile.Email = this.profile.Email;
      this.userProfile.profileImageUrl = this.profile.profileImageUrl;
        this.userProfile.TwitterUID = this.profile.TwitterUID;
    }
  }

showUserMenuFun(){
  this.showUserMenu = !this.showUserMenu;
  //alert("Working");
}


onSignOut(){
 // console.log('Sign out TwitterUID:', this.userProfile.TwitterUID);
  this._centralizeService.logout(this.userProfile.TwitterUID).subscribe({
    next: (res) => {
      //console.log('Sign out response:', res);
      localStorage.removeItem('twitterUserProfileData');
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
})};
  


}
