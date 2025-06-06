using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WebApi.Model
{
    public class SerachByUserNamesResponse
    {
        [JsonProperty("data")]
        public List<UserData> Data { get; init; }
    }

    public record UserData
    {
        [JsonProperty("id")]
        public string Id { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("username")]
        public string Username { get; init; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; init; }

        [JsonProperty("description")]
        public string Description { get; init; }

        [JsonProperty("location")]
        public string Location { get; init; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; init; }

        [JsonProperty("public_metrics")]
        public UserPublicMetricss PublicMetrics { get; init; }

        [JsonProperty("url")]
        public string Url { get; init; }

        [JsonProperty("verified")]
        public bool Verified { get; init; }

        [JsonProperty("verified_type")]
        public string VerifiedType { get; init; }
    }

    public record UserPublicMetricss
    {
        [JsonProperty("followers_count")]
        public int FollowersCount { get; init; }

        [JsonProperty("following_count")]
        public int FollowingCount { get; init; }

        [JsonProperty("tweet_count")]
        public int TweetCount { get; init; }

        [JsonProperty("listed_count")]
        public int ListedCount { get; init; }

        [JsonProperty("like_count")]
        public int LikeCount { get; init; }

        [JsonProperty("media_count")]
        public int MediaCount { get; init; }
    }



}
