using WebApi.Model;

namespace WebApi.Interfaces
{
    public interface ILoginLogic
    {
        string GenerateLoginUrl();
        string GenerateJWTToken(string email, string userId);
        string GetUserIdFromJwtToken(string token);
        public Task<TwitterAuthorizationCodeResponse> ExchangeCodeForAccessToken(string code);
        public Task<TwitterUserProfileResponse> GetTwitterUserProfile(TwitterAuthorizationCodeResponse TokensData, string userID);
        public string InsertTokensAndUserDataInDB(TwitterAuthorizationCodeResponse twitterAuthorizationResponse, TwitterUserProfileResponse twitterUserProfileResponse);
        public string Logout(string twitteruid);


    }
}
