interface PublicMetrics {
  followers_count: number;
  following_count: number;
  tweet_count: number;
  listed_count: number;
  like_count: number;
  media_count: number;
}

interface UserProfile {
  id: string;
  name: string;
  username: string;
  created_at: string;
  description: string;
  location: string;
  profile_image_url: string;
  public_metrics: PublicMetrics;
  url: string;
  verified: boolean;
  verified_type: string;
}
