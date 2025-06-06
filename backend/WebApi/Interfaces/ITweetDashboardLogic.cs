using Microsoft.AspNetCore.Mvc.ActionConstraints;
using WebApi.Model;

namespace WebApi.Interfaces
{
    public interface ITweetDashboardLogic
    {
      
        public Task<GetAllTweetsResponse> GetAllTweets(string tweetUID);
        public Task<ComposeTweetResponse> ComposeTweet(string tweetContent ,string TweetUID);
        public Task<ComposeTweetResponse> ComposeTweetWithMedia(string tweetContent,string TwitterUID, List<string> mediaIds = null);
        Task<string> GenerateMediaID(byte[] mediaBytes, string contentType, string tweeterUID);
        public Task<SerachByUserNamesResponse> SearchTweeterProfile (string UserNames);
        public Task<GetAnalyticsResponse> GetAnalytics(string ContentUrl);



    }
}
