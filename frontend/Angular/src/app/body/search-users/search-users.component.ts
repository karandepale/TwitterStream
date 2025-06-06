import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CentralizeServiceService } from '../../centralize-service.service';

@Component({
  selector: 'app-search-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-users.component.html',
  styleUrl: './search-users.component.css'
})
export class SearchUsersComponent {
  searchText: string = '';
  userProfiles: any[] = [];

  constructor(private service: CentralizeServiceService) {}

  SearchUserProfile() {
    if (!this.searchText.trim()) {
      console.log('Please enter a username.');
      return;
    }

    this.service.SearchTweetProfiles(this.searchText).subscribe({
      next: (response) => {
        console.log('API response:', response);

        // Normalize keys
        this.userProfiles = (response.data || []).map((user: any) => ({
          id: user.id,
          name: user.name,
          username: user.username,
          created_at: user.createdAt,
          description: user.description,
          location: user.location,
          profile_image_url: user.profileImageUrl,
          public_metrics: {
            followers_count: user.publicMetrics.followersCount,
            following_count: user.publicMetrics.followingCount,
            tweet_count: user.publicMetrics.tweetCount,
            listed_count: user.publicMetrics.listedCount,
            like_count: user.publicMetrics.likeCount,
            media_count: user.publicMetrics.mediaCount
          },
          url: user.url,
          verified: user.verified,
          verified_type: user.verifiedType
        }));
      },
      error: (err) => {
        console.error('Error while fetching profiles:', err);
      }
    });
  }

}
