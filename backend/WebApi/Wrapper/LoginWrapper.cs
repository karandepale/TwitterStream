using Azure.Core;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using WebApi.DBEntityFramework;
using WebApi.Interfaces;
using WebApi.Model;

namespace WebApi.Wrapper
{
    public class LoginWrapper : ILoginWrapper
    {
        private readonly AppDbContext _dbContext;
        public LoginWrapper(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string InsertTokensAndUserDataInDB(TwitterAuthorizationCodeResponse authResponse, TwitterUserProfileResponse profileResponse)
        {
            try
            {
                var existingUser = _dbContext.PersonalProjectTable
                    .FirstOrDefault(u => u.TwitterUserId == profileResponse.data.id);

                if (existingUser != null)
                {
                    // Only update non-key properties
                    existingUser.AccessToken = authResponse.access_token;
                    existingUser.RefreshToken = authResponse.refresh_token;
                    existingUser.TokenType = authResponse.token_type;
                    existingUser.ExpiresIn = authResponse.expires_in;
                    existingUser.Scope = authResponse.scope;

                    existingUser.Username = profileResponse.data.username;
                    existingUser.Name = profileResponse.data.name;
                    existingUser.Location = profileResponse.data.location;
                    existingUser.ProfileImageUrl = profileResponse.data.profile_image_url;
                    existingUser.Verified = profileResponse.data.verified;
                    existingUser.Description = profileResponse.data.description;
                    existingUser.Protected = profileResponse.data.@protected;
                    existingUser.CreatedAt = profileResponse.data.created_at;

                    existingUser.FollowersCount = profileResponse.data.public_metrics?.followers_count ?? 0;
                    existingUser.FollowingCount = profileResponse.data.public_metrics?.following_count ?? 0;
                    existingUser.TweetCount = profileResponse.data.public_metrics?.tweet_count ?? 0;
                    existingUser.ListedCount = profileResponse.data.public_metrics?.listed_count ?? 0;
                    existingUser.LikeCount = profileResponse.data.public_metrics?.like_count ?? 0;
                    existingUser.MediaCount = profileResponse.data.public_metrics?.media_count ?? 0;

                    existingUser.IsExpired = false;
                    existingUser.IsExist = true;
                    existingUser.CreatedTimeIST = DateTime.Now;
                    existingUser.OAuth2ClientID = "RGxBbEdNaUFFb2tPYU9Fcl9ZbFc6MTpjaQ";
                    existingUser.OAuth2ClientSecret = "PHPXhcKsUM4HGcJeC18f7IFnjvrCtU0T6u-6XEYMELc7zbKceM";
                    existingUser.Email = "Test@gmail.com";
                }
                else
                {
                    // Insert new record
                    var userData = new DataBaseEntity
                    {
                        TokenType = authResponse.token_type,
                        ExpiresIn = authResponse.expires_in,
                        AccessToken = authResponse.access_token,
                        Scope = authResponse.scope,
                        RefreshToken = authResponse.refresh_token,

                        TwitterUserId = profileResponse.data.id,
                        Username = profileResponse.data.username,
                        Name = profileResponse.data.name,
                        Location = profileResponse.data.location,
                        ProfileImageUrl = profileResponse.data.profile_image_url,
                        Verified = profileResponse.data.verified,
                        Description = profileResponse.data.description,
                        Protected = profileResponse.data.@protected,
                        CreatedAt = profileResponse.data.created_at,

                        FollowersCount = profileResponse.data.public_metrics?.followers_count ?? 0,
                        FollowingCount = profileResponse.data.public_metrics?.following_count ?? 0,
                        TweetCount = profileResponse.data.public_metrics?.tweet_count ?? 0,
                        ListedCount = profileResponse.data.public_metrics?.listed_count ?? 0,
                        LikeCount = profileResponse.data.public_metrics?.like_count ?? 0,
                        MediaCount = profileResponse.data.public_metrics?.media_count ?? 0,

                        IsExpired = false,
                        IsExist = true,
                        CreatedTimeIST = DateTime.Now,
                        OAuth2ClientID = "RGxBbEdNaUFFb2tPYU9Fcl9ZbFc6MTpjaQ",
                        OAuth2ClientSecret = "PHPXhcKsUM4HGcJeC18f7IFnjvrCtU0T6u-6XEYMELc7zbKceM",
                        Email = "Test@gmail.com"
                    };

                    _dbContext.PersonalProjectTable.Add(userData);
                }

                _dbContext.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine("DB insert failed: " + ex.Message);
                return "Failure";
            }
        }


    }
}
