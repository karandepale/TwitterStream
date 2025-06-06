import { Component, Input, OnInit } from '@angular/core';
import { TwitterUserProfile } from '../../Models/twitter-user-profile.model';

@Component({
  selector: 'app-stat-card',
  imports: [],
  templateUrl: './stat-card.component.html',
  styleUrl: './stat-card.component.css'
})
export class StatCardComponent implements OnInit{
   
  StatsDashboardData !: TwitterUserProfile;

    Stats = {
       tweetCount : '',
       followersCount : '',
       followingCount : '',
       mediaCount : '',
       likeCount : ''
    };


  ngOnInit(): void {
    const StatsData = localStorage.getItem('twitterUserProfileData');

    console.log("StatsData:" + StatsData);
    if (StatsData) {
      this.StatsDashboardData = JSON.parse(StatsData) as TwitterUserProfile;

      this.Stats.tweetCount = this.StatsDashboardData.tweetCount;
      this.Stats.followersCount = this.StatsDashboardData.followersCount;
      this.Stats.followingCount = this.StatsDashboardData.followingCount;
      this.Stats.mediaCount = this.StatsDashboardData.mediaCount;
      this.Stats.likeCount = this.StatsDashboardData.likeCount;
      
    }
  }
  
}
