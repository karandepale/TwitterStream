using WebApi.Model;

namespace WebApi.Interfaces
{
    public interface ILoginWrapper
    {
        string InsertTokensAndUserDataInDB(TwitterAuthorizationCodeResponse authTokensResponse, TwitterUserProfileResponse profileResponse);
    }
}
