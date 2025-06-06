import { Component } from '@angular/core';
import { CentralizeServiceService } from '../../centralize-service.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-analytics',
  imports: [CommonModule,FormsModule], 
  templateUrl: './analytics.component.html',
  styleUrl: './analytics.component.css'
})


export class AnalyticsComponent {
  contentUrl: string = '';
  tweetData: any = null;
  userData: any = null;
  showAnalytics: boolean = false;
  errorMsg: string = '';
  isLoading: boolean = false;

  mediaType: string = '';  // photo, video, animated_gif
  mediaUrl: string = '';   // image/video/gif URL

  constructor(private centralService: CentralizeServiceService) {}

  GetAnalytics() {
    if (!this.contentUrl.trim()) {
      this.errorMsg = 'Please enter a valid Content URL.';
      return;
    }

    this.isLoading = true;
    this.errorMsg = '';
    this.showAnalytics = false;
    this.mediaType = '';
    this.mediaUrl = '';

    this.centralService.GetAnalytics(this.contentUrl).subscribe({
      next: (res) => {
        const response = res.data;

        this.tweetData = response.data;
        this.userData = response.includes?.users?.[0] || null;

        // Extract media
        const media = response.includes?.media?.[0];
        if (media) {
          this.mediaType = media.type;

          if (media.type === 'photo') {
            this.mediaUrl = media.url; // You'll need to ensure your API gives this
          } else if (media.type === 'animated_gif') {
            this.mediaUrl = media.preview_image_url;
          } else if (media.type === 'video') {
            this.mediaUrl = media.preview_image_url; // fallback, if no direct .mp4 URL
          }
        }

        this.showAnalytics = !this.showAnalytics;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching analytics:', err);
        this.errorMsg = 'Failed to fetch analytics data.';
        this.isLoading = false;
      }
    });
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleString();
  }
}
