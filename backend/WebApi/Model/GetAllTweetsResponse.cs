using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Model
{
    public class GetAllTweetsResponse
    {
        public List<Datum> data { get; set; }
        public Includes includes { get; set; }
        public Meta meta { get; set; }
    }

    public class User
    {
        public string name { get; set; }
        public string profile_image_url { get; set; }
        public string username { get; set; }
        public string id { get; set; }
    }


    public class Attachments
    {
        public List<string> media_keys { get; set; }
    }

    public class Datum
    {
        public string text { get; set; }
        public PublicMetrics public_metrics { get; set; }
        public string id { get; set; }
        public string author_id { get; set; }
        public DateTime created_at { get; set; }
        public List<string> edit_history_tweet_ids { get; set; }
        public Attachments attachments { get; set; }
    }

    public class Includes
    {
        public List<User> users { get; set; }
        public List<Medium> media { get; set; }
    }

    public class Medium
    {
        public string media_key { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string preview_image_url { get; set; }
    }

    public class Meta
    {
        public int result_count { get; set; }
        public string newest_id { get; set; }
        public string oldest_id { get; set; }
        public string next_token { get; set; }
    }

    public class PublicMetrics
    {
        public int retweet_count { get; set; }
        public int reply_count { get; set; }
        public int like_count { get; set; }
        public int quote_count { get; set; }
        public int bookmark_count { get; set; }
        public int impression_count { get; set; }
    }


}
