import { Component, OnInit } from '@angular/core';
import { TwitterUserProfile } from '../../Models/twitter-user-profile.model';
import { CentralizeServiceService } from '../../centralize-service.service';
import { CommonModule } from '@angular/common';
 
@Component({
  selector: 'app-my-tweets',
  imports: [CommonModule],
  templateUrl: './my-tweets.component.html',
  styleUrl: './my-tweets.component.css'
})
export class MyTweetsComponent implements OnInit{

 
  TwitterUID!: string;
  tweets: any[] = [];
  showAll: boolean = false;
  userProfile!: TwitterUserProfile;

  constructor(private centralService: CentralizeServiceService) {}
  ngOnInit(): void {
  const data = localStorage.getItem('twitterUserProfileData');

  if (data) {
    const user = JSON.parse(data) as TwitterUserProfile;
    this.userProfile = user;
    this.TwitterUID = user.TwitterUID;

    console.log("TwitterUID on my tweet component:" + this.TwitterUID);

    this.centralService.MyTweets(this.TwitterUID).subscribe({
      next: (res) => {
        console.log('Tweets response on MyTweetcomponent:', res);
        this.tweets = res.data; 
      },
      error: (err) => {
        console.error('Error fetching tweets:', err);
      }
    });
  }
}


  // Getter to control how many tweets to display
  get displayedTweets(): any[] {
    return this.showAll ? this.tweets : this.tweets.slice(0, 2);
  }

  // Method to toggle between showing 2 tweets and all tweets
 toggleViewAll() {
  this.showAll = !this.showAll;
  console.log('Toggled:', this.showAll);
}




}
 