using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WebApi.Model
{
    public class GetAnalyticsResponse
    {
        [JsonProperty("data")]
        public TweetData Data { get; init; }

        [JsonProperty("includes")]
        public TweetIncludes Includes { get; init; }

        [JsonProperty("errors")]
        public List<TweetError> Errors { get; init; }
    }





    public record TweetData
    {
        [JsonProperty("id")]
        public string Id { get; init; }

        [JsonProperty("text")]
        public string Text { get; init; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; init; }

        [JsonProperty("public_metrics")]
        public PublicMetricsAnalytics PublicMetrics { get; init; }

        [JsonProperty("entities")]
        public Entities Entities { get; init; }

        [JsonProperty("lang")]
        public string Lang { get; init; }

        [JsonProperty("source")]
        public string Source { get; init; }

        [JsonProperty("possibly_sensitive")]
        public bool PossiblySensitive { get; init; }

        [JsonProperty("in_reply_to_user_id")]
        public string InReplyToUserId { get; init; }

        [JsonProperty("referenced_tweets")]
        public List<ReferencedTweet> ReferencedTweets { get; init; }

        [JsonProperty("context_annotations")]
        public List<ContextAnnotation> ContextAnnotations { get; init; }

        [JsonProperty("edit_history_tweet_ids")]
        public List<string> EditHistoryTweetIds { get; init; }

        [JsonProperty("edit_controls")]
        public EditControls EditControls { get; init; }

        [JsonProperty("conversation_id")]
        public string ConversationId { get; init; }

        [JsonProperty("reply_settings")]
        public string ReplySettings { get; init; }

        [JsonProperty("withheld")]
        public Withheld Withheld { get; init; }

        [JsonProperty("author_id")]
        public string AuthorId { get; init; }

        [JsonProperty("attachments")]
        public AttachmentsAnalytics Attachments { get; init; }

        [JsonProperty("geo")]
        public Geo Geo { get; init; }
    }

    public record PublicMetricsAnalytics
    {
        [JsonProperty("retweet_count")]
        public int RetweetCount { get; init; }

        [JsonProperty("reply_count")]
        public int ReplyCount { get; init; }

        [JsonProperty("like_count")]
        public int LikeCount { get; init; }

        [JsonProperty("quote_count")]
        public int QuoteCount { get; init; }

        [JsonProperty("bookmark_count")]
        public int BookmarkCount { get; init; }

        [JsonProperty("impression_count")]
        public int ImpressionCount { get; init; }
    }

    public record Entities
    {
        [JsonProperty("mentions")]
        public List<Mention> Mentions { get; init; }

        [JsonProperty("hashtags")]
        public List<Hashtag> Hashtags { get; init; }

        [JsonProperty("urls")]
        public List<Url> Urls { get; init; }

        [JsonProperty("annotations")]
        public List<Annotation> Annotations { get; init; }
    }

    public record Mention
    {
        [JsonProperty("start")]
        public int Start { get; init; }

        [JsonProperty("end")]
        public int End { get; init; }

        [JsonProperty("username")]
        public string Username { get; init; }
    }

    public record Hashtag
    {
        [JsonProperty("start")]
        public

 int Start
        { get; init; }

        [JsonProperty("end")]
        public int End { get; init; }

        [JsonProperty("tag")]
        public string Tag { get; init; }
    }

    public record Url
    {
        [JsonProperty("start")]
        public int Start { get; init; }

        [JsonProperty("end")]
        public int End { get; init; }

        [JsonProperty("url")]
        public string UrlAddress { get; init; }

        [JsonProperty("expanded_url")]
        public string ExpandedUrl { get; init; }

        [JsonProperty("display_url")]
        public string DisplayUrl { get; init; }
    }

    public record Annotation
    {
        [JsonProperty("start")]
        public int Start { get; init; }

        [JsonProperty("end")]
        public int End { get; init; }

        [JsonProperty("probability")]
        public double Probability { get; init; }

        [JsonProperty("type")]
        public string Type { get; init; }

        [JsonProperty("normalized_text")]
        public string NormalizedText { get; init; }
    }

    public record ReferencedTweet
    {
        [JsonProperty("type")]
        public string Type { get; init; }

        [JsonProperty("id")]
        public string Id { get; init; }
    }

    public record ContextAnnotation
    {
        [JsonProperty("domain")]
        public ContextDomain Domain { get; init; }

        [JsonProperty("entity")]
        public ContextEntity Entity { get; init; }
    }

    public record ContextDomain
    {
        [JsonProperty("id")]
        public string Id { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("description")]
        public string Description { get; init; }
    }

    public record ContextEntity
    {
        [JsonProperty("id")]
        public string Id { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("description")]
        public string Description { get; init; }
    }

    public record EditControls
    {
        [JsonProperty("edits_remaining")]
        public int EditsRemaining { get; init; }

        [JsonProperty("is_edit_eligible")]
        public bool IsEditEligible { get; init; }

        [JsonProperty("editable_until")]
        public string EditableUntil { get; init; }
    }

    public record Withheld
    {
        [JsonProperty("copyright")]
        public bool Copyright { get; init; }

        [JsonProperty("country_codes")]
        public List<string> CountryCodes { get; init; }
    }

    public record AttachmentsAnalytics
    {
        [JsonProperty("media_keys")]
        public List<string> MediaKeys { get; init; }
    }

    public record Geo
    {
        [JsonProperty("place_id")]
        public string PlaceId { get; init; }
    }

    public record TweetIncludes
    {
        [JsonProperty("users")]
        public List<UserAnalytics> Users { get; init; }

        [JsonProperty("media")]
        public List<Media> Media { get; init; }

        [JsonProperty("places")]
        public List<Place> Places { get; init; }

        [JsonProperty("tweets")]
        public List<TweetData> Tweets { get; init; }
    }

    public record UserAnalytics
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
        public UserPublicMetrics PublicMetrics { get; init; }

        [JsonProperty("url")]
        public string Url { get; init; }

        [JsonProperty("verified")]
        public bool Verified { get; init; }

        [JsonProperty("verified_type")]
        public string VerifiedType { get; init; }

        [JsonProperty("withheld")]
        public Withheld Withheld { get; init; }
    }

    public record UserPublicMetrics
    {
        [JsonProperty("followers_count")]
        public int FollowersCount { get; init; }

        [JsonProperty("following_count")]
        public int FollowingCount { get; init; }

        [JsonProperty("tweet_count")]
        public int TweetCount { get; init; }

        [JsonProperty("listed_count")]
        public int ListedCount { get; init; }
    }

    public record Media
    {
        [JsonProperty("media_key")]
        public string MediaKey { get; init; }

        [JsonProperty("type")]
        public string Type { get; init; }

        [JsonProperty("url")]
        public string Url { get; init; }

        [JsonProperty("preview_image_url")]
        public string PreviewImageUrl { get; init; }

        [JsonProperty("height")]
        public int Height { get; init; }

        [JsonProperty("width")]
        public int Width { get; init; }

        [JsonProperty("public_metrics")]
        public MediaPublicMetrics PublicMetrics { get; init; }
    }

    public record MediaPublicMetrics
    {
        [JsonProperty("view_count")]
        public int ViewCount { get; init; }
    }

    public record Place
    {
        [JsonProperty("id")]
        public string Id { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("country")]
        public string Country { get; init; }

        [JsonProperty("place_type")]
        public string PlaceType { get; init; }

        [JsonProperty("full_name")]
        public string FullName { get; init; }

        [JsonProperty("geo")]
        public PlaceGeo Geo { get; init; }
    }

    public record PlaceGeo
    {
        [JsonProperty("type")]
        public string Type { get; init; }

        [JsonProperty("bbox")]
        public List<double> Bbox { get; init; }
    }

    public record TweetError
    {
        [JsonProperty("title")]
        public string Title { get; init; }

        [JsonProperty("detail")]
        public string Detail { get; init; }

        [JsonProperty("type")]
        public string Type { get; init; }

        [JsonProperty("status")]
        public int Status { get; init; }
    }




}
