using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Services;
using Newtonsoft.Json;
using WebApi.HybridCacheService;
using WebApi.Interfaces;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetDashboardController : ControllerBase
    {
        private readonly ITweetDashboardLogic _tweetDashboard;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly IHybridCache _hybridCache;
        private readonly ILogger<LoginController> _logger;

        public TweetDashboardController(ITweetDashboardLogic tweetDashboard, IHttpContextAccessor httpContextAccessor, IHybridCache hybridCache, ILogger<LoginController> logger)
        {
            _tweetDashboard = tweetDashboard;
            HttpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
            _logger = logger;
        }


        [HttpGet("GetAllTweets")]
        public async Task<IActionResult> GetAllTweets(string TweetUID)
        {
            try
            {
                if (string.IsNullOrEmpty(TweetUID))
                {
                    return BadRequest(new { message = "TweetUID is required", Status = false });
                }

                string cacheKey = $"tweets_{TweetUID}";

                var getAllTweetsRes = await _hybridCache.GetOrSetAsync(cacheKey, async () =>
                {
                    return await _tweetDashboard.GetAllTweets(TweetUID);
                }, TimeSpan.FromMinutes(4));

                _logger.LogWarning($"TweetDashboardController: GetAllTweets(): TweetUID->{TweetUID} , getAllTweetsRes->{getAllTweetsRes?.data.Count}");


                if (getAllTweetsRes?.data == null)
                {
                    return BadRequest(new
                    {
                        message = "No tweets found for the provided UID",
                        Status = false
                    });
                }
                else
                {
                    return Ok(new
                    {
                        message = "Tweets fetched successfully",
                        data = getAllTweetsRes.data,
                        Status = true
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TweetDashboardController: GetAllTweets() failed at {DateTime.UtcNow}. Exception: {ex.Message} ,  TweetUID->{TweetUID}");
                return BadRequest(new
                {
                    message = "Error occurred while fetching tweets",
                    error = ex.Message,
                    Status = false
                });
            }
        }


        [HttpPost("ComposeTweet")]
        public async Task<IActionResult> ComposeTweet(ComposeTweetRequest param)
        {
            try
            {
                var AccessKey = HttpContextAccessor.HttpContext.Request.Headers["AccessKey"];
                var data =HttpContextAccessor.HttpContext.Request.BodyReader.ReadAsync();
                var composeTweetRes = await _tweetDashboard.ComposeTweet(param.tweetContent, param.TweetUID);

                _logger.LogWarning($"TweetDashboardController: ComposeTweet() , param(TweetUID)->{param.TweetUID} ,param(tweetContent)->{param.tweetContent} ");

                if (composeTweetRes != null || composeTweetRes?.data?.id != null)
                {
                    return Ok(new
                    {
                        message = "Tweet composed successfully",
                        content = composeTweetRes,
                        Status = true
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Error occurred while composing tweet",
                        error = "500",
                        Status = false
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TweetDashboardController: ComposeTweet() failed at {DateTime.UtcNow}. Exception: {ex.Message} ,  TweetUID->{param.TweetUID} , tweetContent->{param.tweetContent}");
                return BadRequest(new
                {
                    message = "Error occurred while composing tweet",
                    error = ex.Message,
                    Status = false
                });
            }
        }


        [HttpPost("ComposeTweetWithMedia")]
        public async Task<IActionResult> ComposeTweetWithMedia([FromForm] ComposeTweetWithMediaRequest request)
        {
            try
            {
                var mediaIds = new List<string>();
                foreach (var file in request.MediaFiles)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    var mediaBytes = ms.ToArray();

                    string mediaId = await _tweetDashboard.GenerateMediaID(mediaBytes, file.ContentType, request.TwitterUID.ToString());
                    if (!string.IsNullOrEmpty(mediaId))
                       mediaIds.Add(mediaId);
                }

                var composeTweetRes = await _tweetDashboard.ComposeTweetWithMedia(request.TweetContent , request.TwitterUID, mediaIds);

                _logger.LogWarning($"TweetDashboardController: ComposeTweet() , composeTweetRes->{composeTweetRes.data} , TweetUID->{request.TwitterUID} ,tweetContent->{request.TweetContent} ");


                if (composeTweetRes != null || composeTweetRes?.data?.id != null)
                {
                    return Ok(new
                    {
                        message = "Tweet composed successfully",
                        content = composeTweetRes,
                        Status = true
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "Error occurred while composing tweet",
                        error = "500",
                        Status = false
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TweetDashboardController: ComposeTweet() failed at {DateTime.UtcNow}. Exception: {ex.Message} ,  TweetUID->{request.TwitterUID} , tweetContent->{request.TweetContent} , MediaFiles->{request.MediaFiles}");
                return BadRequest(new { error = ex.Message });
            }
        }



        [HttpGet("SearchTweeterProfile")]
        public async Task<IActionResult> SearchTweeterProfile(string UserNames)
        {
            try
            {

                string cacheKey = $"tweets_{UserNames}";

                var searchResult = await _hybridCache.GetOrSetAsync(cacheKey, async () =>
                {
                    return await _tweetDashboard.SearchTweeterProfile(UserNames);
                }, TimeSpan.FromMinutes(2));
             
                _logger.LogWarning($"TweetDashboardController: SearchTweeterProfile() , searchResult->{searchResult.Data} ,UserNames->{UserNames} ");


                if (searchResult != null || searchResult.Data != null || searchResult.Data.Count != 0)
                {
                    return Ok(new
                    {
                        message = "Search results for Twitter profiles",
                        data = searchResult.Data,
                        Status = true
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        message = "No Twitter profiles found for the provided usernames",
                        Status = false
                    });
                }
                   
            }
            catch (Exception ex)
            {
                _logger.LogError($"TweetDashboardController: SearchTweeterProfile() failed at {DateTime.UtcNow}. Exception: {ex.Message} , UserNames->{UserNames}");
                return BadRequest(new
                {
                    message = "Error occurred while searching for Twitter profiles",
                    error = ex.Message,
                    Status = false
                });
            }
        }



        [HttpGet("GetAnalytics")]
        public async Task<IActionResult> GetAnalytics(string ContentUrl)
        {
            try
            {
                string tweetId = ContentUrl.Split('/').LastOrDefault();

                string cacheKey = $"analytics_{tweetId}";

                var analyticsData = await _hybridCache.GetOrSetAsync(cacheKey, async () =>
                {
                    return await _tweetDashboard.GetAnalytics(ContentUrl);
                }, TimeSpan.FromMinutes(3));

                _logger.LogWarning($"TweetDashboardController: GetAnalytics() , analyticsData->{analyticsData} ,ContentUrl->{ContentUrl} ");

                if (analyticsData != null)
                {
                    return Ok(new
                    {
                        message = "Analytics fetched successfully",
                        data = analyticsData,
                        Status = true
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        message = "No analytics data found for the provided UID",
                        Status = false
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TweetDashboardController: GetAnalytics() failed at {DateTime.UtcNow}. Exception: {ex.Message} , ContentUrl->{ContentUrl}");
                return BadRequest(new
                {
                    message = "Error occurred while fetching analytics",
                    error = ex.Message,
                    Status = false
                });
            }
        }


        [HttpGet("GetTrendsPlaces")]
        public async Task<IActionResult> GetTrendsPlaces()
        {
            try
            {
                var trendsPlaces = await _tweetDashboard.FetchTrendsLocations();
                _logger.LogWarning($"TweetDashboardController: GetTrendsPlaces() , trendsPlaces->{JsonConvert.SerializeObject(trendsPlaces)}");
                if (trendsPlaces != null)
                {
                    return Ok(new
                    {
                        message = "Trends places fetched successfully",
                        data = trendsPlaces,
                        Status = true
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        message = "No trends places found",
                        Status = false
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TweetDashboardController: GetTrendsPlaces() failed at {DateTime.UtcNow}. Exception: {ex.Message}");
                return BadRequest(new
                {
                    message = "Error occurred while fetching trends places",
                    error = ex.Message,
                    Status = false
                });
            }
        }


        [HttpGet("TrendsByWoeid")]
        public async Task<IActionResult> TrendsByWoeid(string woeid)
        {
            try
            {
                var res = await _tweetDashboard.TrendsByWoeid(woeid);
                _logger.LogWarning($"TweetDashboardController: TrendsByWoeid() , trendsPlaces->{JsonConvert.SerializeObject(res)}");
                if (res != null)
                {
                    return Ok(new
                    {
                        message = "Trends places fetched successfully",
                        data = res,
                        Status = true
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        message = "No trends places found",
                        Status = false
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TweetDashboardController: GetTrendsPlaces() failed at {DateTime.UtcNow}. Exception: {ex.Message}");
                return BadRequest(new
                {
                    message = "Error occurred while fetching trends places",
                    error = ex.Message,
                    Status = false
                });
            }
        }





    }
}
