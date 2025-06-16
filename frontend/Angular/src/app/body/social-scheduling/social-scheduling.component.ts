import { Component, OnInit ,AfterViewInit,Renderer2 } from '@angular/core';
import { TwitterUserProfile } from '../../Models/twitter-user-profile.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-social-scheduling',
  imports: [CommonModule,FormsModule],
  templateUrl: './social-scheduling.component.html',
  styleUrl: './social-scheduling.component.css' 
})



export class SocialSchedulingComponent implements OnInit {
  currentStep: number = 1;

  userProfile = {
    screenName: '',
    UserName: '',
    Location: '',
    Description: '',
    FollowersCount: '',
    TweetCount: '',
    Email: '',
    profileImageUrl: '',
    TwitterUID: ''
  };

  postContent: string = '';
  previewImages: string[] = [];
  previewVideo: string | null = null;

  publishOption: 'now' | 'later' = 'now';
  selectedDate: string = '';
  selectedTime: string = '';
  minDate: string = '';
  minTime: string = '';

  ngOnInit(): void {
    const profileJson = localStorage.getItem('twitterUserProfileData');
    if (profileJson) {
      const profile = JSON.parse(profileJson);
      this.userProfile.screenName = profile.screenName;
      this.userProfile.UserName = '@' + (profile.UserName || '').toLowerCase().replace(/\s+/g, '');
      this.userProfile.profileImageUrl = profile.profileImageUrl;
    }

    // Set today's date in IST as minDate
    const todayIST = new Date(new Date().toLocaleString('en-US', { timeZone: 'Asia/Kolkata' }));
    this.minDate = todayIST.toISOString().split('T')[0];
  }

  goToNextStep() {
    if (this.currentStep < 4) this.currentStep++;
  }

  goToPrevStep() {
    if (this.currentStep > 1) this.currentStep--;
  }

  updatePostContent() {
    console.log("Post Content:", this.postContent);
  }

  handleImageUpload(event: any) {
    const files: FileList = event.target.files;
    if (files.length > 0) {
      this.previewVideo = null;
      for (let i = 0; i < files.length; i++) {
        const file = files[i];
        const reader = new FileReader();
        reader.onload = (e: any) => {
          this.previewImages.push(e.target.result);
        };
        reader.readAsDataURL(file);
      }
    }
    event.target.value = null;
  }

  handleVideoUpload(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.previewImages = [];
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.previewVideo = e.target.result;
      };
      reader.readAsDataURL(file);
    }
    event.target.value = null;
  }

  removeImage(index: number) {
    this.previewImages.splice(index, 1);
  }

  removeVideo() {
    this.previewVideo = null;
  }

  onPublishOptionChange() {
    if (this.publishOption === 'now') {
      console.log('User selected Publish Now');
    } else {
      this.onDateChange(); // Ensure time gets recalculated
    }
  }

  onDateChange() {
  const selected = new Date(this.selectedDate);
  const nowIST = new Date(new Date().toLocaleString('en-US', { timeZone: 'Asia/Kolkata' }));

  const isToday =
    selected.getFullYear() === nowIST.getFullYear() &&
    selected.getMonth() === nowIST.getMonth() &&
    selected.getDate() === nowIST.getDate();

  if (isToday) {
    const hours = nowIST.getHours().toString().padStart(2, '0');
    const minutes = nowIST.getMinutes().toString().padStart(2, '0');
    this.minTime = `${hours}:${minutes}`;
  } else {
    this.minTime = '00:00';
  }

  // Also validate time again in case it's now invalid
  this.onTimeChange();
}


onTimeChange() {
  if (this.selectedDate && this.selectedTime) {
    const nowIST = new Date(new Date().toLocaleString('en-US', { timeZone: 'Asia/Kolkata' }));

    const selectedDateTime = new Date(`${this.selectedDate}T${this.selectedTime}:00+05:30`);

    if (selectedDateTime < nowIST) {
      alert('Please select a future time.');
      this.selectedTime = ''; // Reset the invalid time
    } else {
      console.log('Selected Time:', this.selectedTime);
    }
  }
}


  publishPost() {
    if (this.publishOption === 'now') {
      console.log('Publishing immediately...');
    } else {
      const scheduledDateTime = `${this.selectedDate}T${this.selectedTime}:00+05:30`;
      console.log('Scheduled Publish DateTime (IST):', scheduledDateTime);
    }
  }
}





