using Microsoft.AspNetCore.Mvc;

namespace WebApi.Model
{
    public class ComposeTweetWithMediaRequest
    {
        [FromForm(Name = "tweetContent")]
        public string TweetContent { get; set; }

        [FromForm(Name = "twitterUID")]
        public string TwitterUID { get; set; }

        [FromForm(Name = "media")]
        public List<IFormFile> MediaFiles { get; set; }
    }
}
