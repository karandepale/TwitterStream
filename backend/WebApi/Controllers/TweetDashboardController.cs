using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public TweetDashboardController(ITweetDashboardLogic tweetDashboard, IHttpContextAccessor httpContextAccessor, IHybridCache hybridCache)
        {
            _tweetDashboard = tweetDashboard;
            HttpContextAccessor = httpContextAccessor;
            _hybridCache = hybridCache;
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
                }, TimeSpan.FromMinutes(30)); 

                if (getAllTweetsRes?.data == null)
                {
                    return BadRequest(new
                    {
                        message = "No tweets found for the provided UID",
                        Status = false
                    });
                }

                return Ok(new
                {
                    message = "Tweets fetched successfully",
                    data = getAllTweetsRes.data,
                    Status = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Error occurred while fetching tweets",
                    error = ex.Message,
                    Status = false
                });
            }
        }


        //[HttpGet("GetAllTweets")]
        //public async Task <IActionResult> GetAllTweets(string TweetUID)
        //{
        //    try
        //    {
        //        string cacheKey = $"tweets_{TweetUID}";


        //        var getAllTweetsRes = _tweetDashboard.GetAllTweets(TweetUID);
        //        if(getAllTweetsRes.Result.data == null)
        //        {
        //            return BadRequest(new
        //            {
        //                message = "No tweets found for the provided UID",
        //                Status = false
        //            });
        //        }
        //        return Ok(new
        //        {
        //            message = "Tweets fetched successfully",
        //            data = getAllTweetsRes.Result.data,
        //            Status = true
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new
        //        {
        //            message = "Error occurred while fetching tweets",
        //            error = ex.Message,
        //            Status = false
        //        });
        //    }
        //}

        [HttpPost("ComposeTweet")]
        public async Task<IActionResult> ComposeTweet(ComposeTweetRequest param)
        {
            try
            {
                var AccessKey = HttpContextAccessor.HttpContext.Request.Headers["AccessKey"];
                var data =HttpContextAccessor.HttpContext.Request.BodyReader.ReadAsync();
                var composeTweetRes = await _tweetDashboard.ComposeTweet(param.tweetContent, param.TweetUID);
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

              //  var searchResult = await _tweetDashboard.SearchTweeterProfile(UserNames);
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


               // var analyticsData = await _tweetDashboard.GetAnalytics(ContentUrl);
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
                return BadRequest(new
                {
                    message = "Error occurred while fetching analytics",
                    error = ex.Message,
                    Status = false
                });
            }
        }



    }
}
