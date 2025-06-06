import { Component, OnInit } from '@angular/core';
import { CentralizeServiceService } from '../../centralize-service.service';
import { TwitterUserProfile } from '../../Models/twitter-user-profile.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-compose-tweet',
  imports: [FormsModule,CommonModule],
  templateUrl: './compose-tweet.component.html',
  styleUrl: './compose-tweet.component.css'
})



export class ComposeTweetComponent implements OnInit {

  tweetContent: string = '';
  charCount: number = 0;
  twitterUID: string = ''; 
  showToast: boolean = false;

  selectedMedia: File[] = [];

  constructor(private centralService: CentralizeServiceService) {}

  ngOnInit(): void {
    const data = localStorage.getItem('twitterUserProfileData');

    if (data) {
      const user = JSON.parse(data) as TwitterUserProfile;
      this.twitterUID = user.TwitterUID;
      console.log("TwitterUID on compose tweet component:", this.twitterUID);
    }
  }

  onTextChange(event: Event): void {
    const target = event.target as HTMLTextAreaElement;
    this.tweetContent = target.value;
    this.charCount = this.tweetContent.length;
  }

  onMediaSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

    if (input.files) {
      this.selectedMedia = Array.from(input.files);
    }
  }

  onTweet(): void {
    if (!this.tweetContent.trim()) {
      console.warn('Tweet content is empty.');
      return;
    }

    const formData = new FormData();

    // ✅ Use the exact field names expected by backend (case-sensitive)
    formData.append('TwitterUID', this.twitterUID);
    formData.append('TweetContent', this.tweetContent);

    // ✅ Append all media files with consistent key
    this.selectedMedia.forEach((file) => {
      formData.append('media', file); // or 'media[]' depending on backend
    });

    // Optional: Debug form data
    for (let pair of formData.entries()) {
      console.log(`${pair[0]}:`, pair[1]);
    }

    // ✅ Send data to backend
    this.centralService.ComposeTweetWithMedia(formData).subscribe({
      next: (res) => {
        console.log('Tweet posted successfully:', res);
        this.resetForm();
        this.showSuccessToast();
      },
      error: (err) => {
        console.error('Error posting tweet:', err);
      }
    });
  }

  resetForm(): void {
    this.tweetContent = '';
    this.charCount = 0;
    this.selectedMedia = [];
    
  }

  showSuccessToast(): void {
    this.showToast = true;
    setTimeout(() => {
      this.showToast = false;
    }, 3000); // Show toast for 3 seconds
  }
}

// export class ComposeTweetComponent implements OnInit {

//   tweetContent: string = '';
//   charCount: number = 0;
//   twitterUID: string = '';
//   showToast: boolean = true;

//   constructor(private centralService: CentralizeServiceService) {}

//   ngOnInit(): void {
//     const data = localStorage.getItem('twitterUserProfileData');

//     if (data) {
//       const user = JSON.parse(data) as TwitterUserProfile;
//       this.twitterUID = user.TwitterUID;
//       console.log("TwitterUID on compose tweet component:", this.twitterUID);
//     }
//   }
 
//   onTweet(): void {
//     if (!this.tweetContent.trim()) {
//       console.warn('Tweet content is empty.');
//       return;
//     }

//     this.centralService.ComposeTweet(this.twitterUID, this.tweetContent).subscribe({
//       next: (res) => {
//         console.log('Tweet posted successfully:', res);
//         this.tweetContent = '';
//         this.charCount = 0;
//          this.showSuccessToast();
//       },
//       error: (err) => {
//         console.error('Error posting tweet:', err);
//       }
//     });
//   }

//   onTextChange(event: Event): void {
//     const target = event.target as HTMLTextAreaElement;
//     this.tweetContent = target.value;
//     this.charCount = this.tweetContent.length;
//   }

// showSuccessToast(): void {
//   this.showToast = true;
//   setTimeout(() => {
//     this.showToast = false;
//   }, 3000); // Toast lasts for 3 seconds
// }


// }
