using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApi.DBEntityFramework
{
    [Table("PersonalProjectTable")]
    public class DataBaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LoggedinUserID { get; set; }

        // TwitterAuthorizationCodeResponse fields
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
        public string AccessToken { get; set; }
        public string Scope { get; set; }
        public string RefreshToken { get; set; }

        // TwitterUserProfileResponse > UserProfileData
        public string TwitterUserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool Verified { get; set; }
        public string Description { get; set; }
        public bool Protected { get; set; }
        public DateTime CreatedAt { get; set; }

        // TwitterUserProfilePublicMetrics
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int TweetCount { get; set; }
        public int ListedCount { get; set; }
        public int LikeCount { get; set; }
        public int MediaCount { get; set; }

        // Additional fields
        public bool IsExpired { get; set; } = false;
        public bool IsExist { get; set; } = true;
        public DateTime CreatedTimeIST { get; set; } = DateTime.Now;

        public string OAuth2ClientID { get; set; }
        public string OAuth2ClientSecret { get; set; }
        public string Email { get; set; }
    }
}
