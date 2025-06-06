
using Azure.Core;
using Newtonsoft.Json;
using RestSharp;
using System.Text.Json;

//using System.Text.Json;
using System.Text.RegularExpressions;
using WebApi.Interfaces;
using WebApi.Model;
using static System.Net.Mime.MediaTypeNames;

namespace WebApi.BusinessLogic
{
    public class TweetDashboardLogic : ITweetDashboardLogic
    {
        private readonly ITweetDashboardWrapper _tweetDashboardWrapper;
        private readonly IConfiguration _configuration;

        public TweetDashboardLogic(ITweetDashboardWrapper tweetDashboardWrapper, IConfiguration configuration)
        {
            _tweetDashboardWrapper = tweetDashboardWrapper;
            _configuration = configuration;
        }

        public async Task<GetAllTweetsResponse> GetAllTweets(string tweetUID)
        {
            try
            {
                var Tokens = _tweetDashboardWrapper.GetTokens(tweetUID);

                var getTweetRes = new GetAllTweetsResponse();
                if (Tokens != null && Tokens.Count > 0)
                {
                    getTweetRes = await GetTweets(Tokens, tweetUID);
                }

                return getTweetRes;
            }
            catch (Exception)
            {
                return null;
            }
        }


        private async Task<GetAllTweetsResponse> GetTweets(List<string> tokens, string twitterUID)
        {
            try
            {
                string nextToken = null;
                var accumulatedResponse = new GetAllTweetsResponse
                {
                    data = new List<Datum>(),
                    includes = new Includes(), // assuming includes is a class
                    meta = new Meta()          // assuming meta is a class
                };

                do
                {
                    string BaseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                    string endpoint = $"/users/{twitterUID}/tweets";

                    var client = new RestClient(BaseUrl);
                    var request = new RestRequest(endpoint, Method.Get);

                    string temptoken = string.Empty;
                    temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";
                  
                    request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? tokens[0] : temptoken)}");

                    request.AddQueryParameter("max_results", "100");
                    request.AddQueryParameter("tweet.fields", "created_at,public_metrics");
                    request.AddQueryParameter("expansions", "author_id,attachments.media_keys");
                    request.AddQueryParameter("user.fields", "profile_image_url,username");
                    request.AddQueryParameter("media.fields", "url,preview_image_url");

                    if (!string.IsNullOrEmpty(nextToken))
                        request.AddQueryParameter("pagination_token", nextToken);

                    var response = await client.ExecuteAsync(request);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    // var pageResponse = JsonSerializer.Deserialize<GetAllTweetsResponse>(response.Content, options);
                    var pageResponse = JsonConvert.DeserializeObject <GetAllTweetsResponse>(response.Content);

                    if (pageResponse?.data != null)
                        accumulatedResponse.data.AddRange(pageResponse.data);

                    // Handle includes and meta if needed
                    if (pageResponse?.includes != null)
                    {
                        if (accumulatedResponse.includes == null)
                            accumulatedResponse.includes = pageResponse.includes;
                        else
                        {
                            // Merge logic if includes has lists like users/media
                            // Implement merging for includes.media, includes.users etc.
                        }
                    }

                    nextToken = pageResponse?.meta?.next_token;
                }
                while (!string.IsNullOrEmpty(nextToken));

                return accumulatedResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<ComposeTweetResponse> ComposeTweet(string tweetContent, string TweetUID)
        {
            try
            {
                var composeTweetRes = new ComposeTweetResponse();
                var Tokens = _tweetDashboardWrapper.GetTokens(TweetUID);

                if (Tokens != null && Tokens.Count > 0)
                {
                    composeTweetRes = await PostTweet(Tokens, tweetContent);
                }
                return composeTweetRes;

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private async Task<ComposeTweetResponse> PostTweet(List<string> tokens, string tweetContent)
        {
            try
            {
                string BaseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                string endpoint = "/tweets";
                var client = new RestClient(BaseUrl);
                var request = new RestRequest(endpoint, Method.Post);

                string temptoken = string.Empty;
                temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";

                request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? tokens[0] : temptoken)}");
                request.AddHeader("Content-Type", "application/json");
                var body = new
                {
                    text = tweetContent
                };
                request.AddJsonBody(body);

                var response = await client.ExecuteAsync(request);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
              
                var composeResponse = JsonConvert.DeserializeObject<ComposeTweetResponse>(response.Content);
                return composeResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }


        //Media upload:
        public async Task<string> GenerateMediaID(byte[] mediaBytes, string contentType, string twitterUid)
        {
            try
            {
                string mediaCategory = string.Empty;
                if(contentType == "image/jpeg" || contentType == "image/png" )
                {
                    mediaCategory = "tweet_image";
                }else if(contentType == "image/gif")
                {
                    mediaCategory = "tweet_gif";
                }else if(contentType == "video/mp4")
                {
                    mediaCategory = "tweet_video";
                }

                var Tokens = _tweetDashboardWrapper.GetTokens(twitterUid);
                string mediaID = string.Empty;
                if (Tokens != null || Tokens.Count != 0)
                {
                     mediaID = await UploadMediaInChunks(mediaBytes, contentType, Tokens[0] , mediaCategory);
                }
                return mediaID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // Upload media in chunks to Twitter
        private async Task<string> UploadMediaInChunks(byte[] mediaBytes, string contentType, string token , string mediaCategory)
        {
            try
            {
              string mediaID = await InitializeMediaUpload(mediaBytes.Length, contentType, token, mediaCategory);
             
                const int chunkSize = 5000000;
                int i2 = 0;
                for (int i = 0; i < mediaBytes.Length; i += chunkSize)
                {
                    var chunk = mediaBytes.Skip(i).Take(chunkSize).ToArray();
                   
                    string uploadResponse = await AppendMediaChunk(mediaID, chunk, i2, token, contentType);
                    i2++;
                    if (string.IsNullOrEmpty(uploadResponse))
                        return null;
                }
                // Finalize the upload
                bool finalizeResponse = await FinalizeMediaUpload(mediaID, token);
                return mediaID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<string> InitializeMediaUpload(long mediaSize, string mediaType, string token, string mediaCategory)
        {
            try
            {
                string BaseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                string endpoint = "/media/upload/initialize";
                var client = new RestClient(BaseUrl);
                var request = new RestRequest(endpoint, Method.Post);

                string temptoken = string.Empty;
                temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";

                request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? token : temptoken)}");

                request.AddHeader("Content-Type", "application/json");

                request.AddJsonBody(new
                {
                    total_bytes = mediaSize,
                    media_type = mediaType,
                    media_category = mediaCategory
                });

                var response = await client.ExecuteAsync(request);
                var initJsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                string mediaID = initJsonResponse?.data?.id?.ToString();

                return mediaID; 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private async Task<string> AppendMediaChunk(string mediaID, byte[] chunk, int segmentIndex, string token ,string mediaType)
        {
            try
            {
                string BaseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                string endpoint = $"media/upload/{mediaID}/append";

                var client = new RestClient(BaseUrl);
                var request = new RestRequest(endpoint, Method.Post);

                string temptoken = string.Empty;
                temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";

                request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? token : temptoken)}");


                request.AddHeader("Content-Type", "multipart/form-data");

                request.AddParameter("segment_index", segmentIndex, ParameterType.GetOrPost);
                request.AddFile("media", chunk, $"chunk_{segmentIndex}", mediaType);

                var response = await client.ExecuteAsync(request);
                return response.IsSuccessful ? mediaID : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private async Task <bool> FinalizeMediaUpload(string mediaID, string token)
        {
            try
            {
                string BaseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                string endpoint = $"media/upload/{mediaID}/finalize";

                string temptoken = string.Empty;
                temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";

               


                var client = new RestClient(BaseUrl);
                var request = new RestRequest(endpoint, Method.Post);
                request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? token : temptoken)}");

                request.AddHeader("Content-Type", "application/json");

                request.AddJsonBody(new
                {
                });

                var response = await client.ExecuteAsync(request);
                return response.IsSuccessful;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public async Task<ComposeTweetResponse> ComposeTweetWithMedia( string tweetContent , string twitterUID, List<string> mediaIds = null)
        {
            try
            {
                var Tokens = _tweetDashboardWrapper.GetTokens(twitterUID);

                string BaseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                string endpoint = "/tweets";
                var client = new RestClient(BaseUrl);
                var request = new RestRequest(endpoint, Method.Post);

                string temptoken = string.Empty;
                temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";

                request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? Tokens[0] : temptoken)}");


                request.AddHeader("Content-Type", "application/json");

                // Construct tweet body with or without media
                var body = new Dictionary<string, object>
                {
                   { "text", tweetContent }
                };

                if (mediaIds != null && mediaIds.Any())
                {
                    body["media"] = new
                    {
                        media_ids = mediaIds
                    };
                }

                request.AddJsonBody(body);

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    Console.WriteLine("Tweet API error: " + response.Content);
                    return null;
                }

                var composeResponse = JsonConvert.DeserializeObject<ComposeTweetResponse>(response.Content);
                return composeResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }



        public async Task<SerachByUserNamesResponse> SearchTweeterProfile(string userNames)
        {
            try
            {
                var searchResponse = new SerachByUserNamesResponse();
                var Tokens = _tweetDashboardWrapper.GetTokens("");
                if (Tokens != null && Tokens.Count > 0)
                {
                    searchResponse = await SearchProfilesByScreenNames(Tokens, userNames);
                }
                return searchResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        private async Task<SerachByUserNamesResponse> SearchProfilesByScreenNames(List<string> tokens, string UserNames)
        {
            try
            {
                var validUsernames = UserNames
                    .Split(',')
                    .Select(u => u.Trim())
                    .Where(u => Regex.IsMatch(u, @"^[A-Za-z0-9_]{1,15}$"))
                    .ToList();

                var finalResult = new SerachByUserNamesResponse
                {
                    Data = new List<UserData>(),
                };

                const int batchSize = 100;
                string baseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                string endpoint = "/users/by";

                string temptoken = string.Empty;
                temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";

              

                for (int i = 0; i < validUsernames.Count; i += batchSize)
                {
                    var batch = validUsernames.Skip(i).Take(batchSize);
                    string cleanedBatch = string.Join(",", batch);

                    var client = new RestClient(baseUrl);
                    var request = new RestRequest(endpoint, Method.Get);
                    request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? tokens[0] : temptoken)}");

                    request.AddQueryParameter("usernames", cleanedBatch);
                    request.AddQueryParameter("user.fields", "created_at,description,location,public_metrics,profile_image_url,url,verified,verified_type,pinned_tweet_id");
                    request.AddQueryParameter("expansions", "pinned_tweet_id");
                    request.AddQueryParameter("tweet.fields", "text,public_metrics");

                    var response = await client.ExecuteAsync(request);
                    if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
                        continue;

                 
                    var resData = JsonConvert.DeserializeObject<SerachByUserNamesResponse>(response.Content);
                    if (resData?.Data != null)
                        finalResult.Data.AddRange(resData.Data);

                  
                }

                return finalResult;
            }
            catch (Exception)
            {
                return null;
            }
        }



        public async Task<GetAnalyticsResponse> GetAnalytics(string ContentUrl)
        {
            try
            {
                var analyticsResponse = new GetAnalyticsResponse();
                var Tokens = _tweetDashboardWrapper.GetTokens("");
                if (Tokens != null && Tokens.Count > 0)
                {
                    analyticsResponse = await FetchAnalytics(Tokens, ContentUrl);
                }
                return analyticsResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<GetAnalyticsResponse> FetchAnalytics(List<string> tokens, string ContentUrl)
        {
            try
            {
                string BaseUrl = _configuration["Twitter:BaseUri"] ?? "https://api.x.com/2";
                string tweetId = ContentUrl.Split('/').LastOrDefault();

                string endpoint = $"/tweets/{tweetId}";
                var client = new RestClient(BaseUrl);
                var request = new RestRequest(endpoint, Method.Get);

                string temptoken = string.Empty;
                temptoken = _configuration["GetTwitterUserProfile:TempToken"] ?? "";

                request.AddHeader("Authorization", $"Bearer {(string.IsNullOrEmpty(temptoken) ? tokens[0] : temptoken)}");
                request.AddHeader("Content-Type", "application/json");

                request.AddQueryParameter("tweet.fields", "created_at,public_metrics,entities,lang,source,possibly_sensitive,in_reply_to_user_id,referenced_tweets,context_annotations,edit_history_tweet_ids,edit_controls,conversation_id,reply_settings,withheld,author_id,attachments,geo");
                request.AddQueryParameter("expansions", "author_id,in_reply_to_user_id,referenced_tweets.id,referenced_tweets.id.author_id,attachments.media_keys,geo.place_id");
                request.AddQueryParameter("user.fields", "created_at,description,location,profile_image_url,public_metrics,url,verified,verified_type,withheld");
                request.AddQueryParameter("media.fields", "media_key,type,url,preview_image_url,height,width,public_metrics");
                request.AddQueryParameter("place.fields", "id,name,country,place_type,full_name,geo");

                var response = await client.ExecuteAsync(request);

                var analyticsResponse = JsonConvert.DeserializeObject<GetAnalyticsResponse>(response.Content);


                return analyticsResponse;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }


        }
    }
}
