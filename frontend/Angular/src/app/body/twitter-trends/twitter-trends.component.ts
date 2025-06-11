import { Component, OnInit } from '@angular/core';
import { CentralizeServiceService } from '../../centralize-service.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

interface TrendPlace {
  name: string;
  woeid: number;
  country?: string;
}

interface TrendItem {
  name: string;
  url: string;
  promotedContent: any;
  query: string;
  tweetVolume: number | null;
}

interface TrendsResponse {
  message: string;
  data: {
    trends: TrendItem[];
    asOf: string;
    createdAt: string;
    locations: Array<{
      name: string;
      woeid: number;
    }>;
  };
  status: boolean;
}

@Component({
  selector: 'app-twitter-trends',
  imports: [FormsModule, CommonModule],
  templateUrl: './twitter-trends.component.html',
  styleUrl: './twitter-trends.component.css'
})
export class TwitterTrendsComponent implements OnInit {
  TrendPlaces: TrendPlace[] = [];
  uniqueCountries: string[] = [];
  filteredPlaces: TrendPlace[] = [];
  selectedCountry: string | null = null;
  selectedPlace: number | null = null;
  selectedWoeid: number | null = null;
  trendsData: TrendItem[] = [];
  isLoading: boolean = false;
  errorMessage: string = '';

  constructor(private centralService: CentralizeServiceService) {}

  ngOnInit(): void {
    this.fetchTrendPlaces();
  }

  fetchTrendPlaces(): void {
    this.centralService.GetTrendsPlaces().subscribe({
      next: (res) => {
        console.log('Trending Res', res);
        this.TrendPlaces = res.data || [];
        this.uniqueCountries = this.getUniqueCountries(this.TrendPlaces);
        this.filteredPlaces = [];
      },
      error: (err) => {
        console.error('Error fetching trend places:', err);
        this.TrendPlaces = [];
        this.errorMessage = 'Failed to load trending places. Please try again.';
      }
    });
  }

  getUniqueCountries(places: TrendPlace[]): string[] {
    const countries = new Set<string>();
    places.forEach(place => {
      if (place.country) {
        countries.add(place.country);
      }
    });
    return Array.from(countries).sort();
  }

  onCountryChange(): void {
    this.filteredPlaces = this.TrendPlaces.filter(place => place.country === this.selectedCountry);
    this.selectedPlace = null;
    this.trendsData = [];
    this.errorMessage = '';
  }

  updateSelectedWoeid(): void {
    this.selectedWoeid = this.selectedPlace;
    this.errorMessage = '';
    if (this.selectedWoeid) {
      this.fetchTrendsByWoeid();
    } else {
      this.trendsData = [];
    }
  }

  fetchTrendsByWoeid(): void {
    if (!this.selectedWoeid) return;

    this.isLoading = true;
    this.trendsData = [];
    this.errorMessage = '';

    this.centralService.TrendsByWoeid(this.selectedWoeid).subscribe({
      next: (response: TrendsResponse) => {
        console.log('Trends for selected WOEID:', response);
        if (response.status && response.data && response.data.trends) {
          this.trendsData = response.data.trends;
        } else {
          this.trendsData = [];
          this.errorMessage = 'No trends data available for this location.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching trends for WOEID:', err);
        this.isLoading = false;
        this.trendsData = [];
        this.errorMessage = 'Failed to load trends. Please try again.';
      }
    });
  }

  // getSelectedPlaceName(): string {
  //   if (!this.selectedPlace) return '';
  //   const selectedPlaceObj = this.filteredPlaces.find(place => place.woeid === this.selectedPlace);
  //   return selectedPlaceObj ?
  //     `${selectedPlaceObj.name}${selectedPlaceObj.country ? ', ' + selectedPlaceObj.country : ''}` :
  //     'Selected Location';
  // }
  getSelectedPlaceName(): string {
  if (!this.selectedPlace) return '';
  const selectedPlaceObj = this.filteredPlaces.find(place => place.woeid === this.selectedPlace);
  return selectedPlaceObj ? selectedPlaceObj.name : '';
}

  formatTweetVolume(volume: number | null): string {
    if (!volume) return '0';
    if (volume >= 1000000) {
      return (volume / 1000000).toFixed(1) + 'M';
    } else if (volume >= 1000) {
      return (volume / 1000).toFixed(1) + 'K';
    }
    return volume.toString();
  }



  openTrendUrl(url: string): void {
    if (url) {
      window.open(url, '_blank', 'noopener,noreferrer');
    }
  }

  retry2FetchTrends(): void {
    this.errorMessage = '';
    if (this.selectedWoeid) {
      this.fetchTrendsByWoeid();
    } else {
      this.fetchTrendPlaces();
    }
  }

  retryFetchTrends(): void {
    this.errorMessage = '';
    if (this.selectedWoeid) {
      this.fetchTrendsByWoeid();
    } else {
      this.fetchTrendPlaces();
    }
  }

  decodeURIComponent(value: string): string {
    try {
      return decodeURIComponent(value);
    } catch (e) {
      return value;
    }
  }
}
