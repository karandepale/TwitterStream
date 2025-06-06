using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Model
{

    public class TwitterUserProfileResponse
    {
        public UserProfileData data { get; set; }
    }

    public class UserProfileData
    {
        public string username { get; set; }
        public string name { get; set; }
        public UserProfilePublicMetrics public_metrics { get; set; }
        public string location { get; set; }
        public string profile_image_url { get; set; }
        public bool verified { get; set; }
        public string description { get; set; }
        public bool @protected { get; set; }
        public DateTime created_at { get; set; }
        public string id { get; set; }
    }

    public class UserProfilePublicMetrics
    {
        public int followers_count { get; set; }
        public int following_count { get; set; }
        public int tweet_count { get; set; }
        public int listed_count { get; set; }
        public int like_count { get; set; }
        public int media_count { get; set; }
    }

   



}
