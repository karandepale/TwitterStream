using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebApi.Interfaces;
using WebApi.Model;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILoginLogic _loginLogic;

    public LoginController(ILoginLogic loginLogic )
    {
        _loginLogic = loginLogic;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new
        {
            message = "Dummy test",
            url = "Google.com",
            JWTToken = "",
            Status = true
        });
    }


    [HttpGet("login")]
    public IActionResult Login()
    {
        try
        {
            var requestUrl = _loginLogic.GenerateLoginUrl();
            string userId = LoginLogic.GenerateRandomNumber(1, 100);
            string jwtToken = _loginLogic.GenerateJWTToken("emailid", userId);

            string state = $"random_state_123|{jwtToken}";
            string encodedState = Uri.EscapeDataString(state);

            requestUrl = $"{requestUrl}&state={encodedState}";

            return Ok(new
            {
                message = $"Authorization URL opened in browser and Email sent to emailid",
                url = requestUrl,
                JWTToken = jwtToken
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "Error occurred while generating login URL",
                error = ex.Message,
                Status = false
            });
        }
    }


    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
    {

        try
        {
            string[] stateParts = state.Split('|');
            string jwttoken = stateParts[1];
            String shouldRediret = string.Empty;
         
            Console.WriteLine($"Extracted JWT token: {jwttoken}");
            
            string userId = _loginLogic.GetUserIdFromJwtToken(jwttoken);

            var twitterAuthorizationResponse = await _loginLogic.ExchangeCodeForAccessToken(code);

            var twitterUserProfileResponse = new TwitterUserProfileResponse();
            if (twitterAuthorizationResponse?.access_token != null)
            {
                shouldRediret = "Success";
                twitterUserProfileResponse = await _loginLogic.GetTwitterUserProfile(twitterAuthorizationResponse, userId);
            }

            string DbInsertStatus = string.Empty;
            if (twitterAuthorizationResponse != null && twitterAuthorizationResponse.access_token != null && twitterUserProfileResponse != null && twitterUserProfileResponse.data.id != null)
            {
               DbInsertStatus =   _loginLogic.InsertTokensAndUserDataInDB(twitterAuthorizationResponse, twitterUserProfileResponse);
            }

            string screenName = Uri.EscapeDataString(twitterUserProfileResponse?.data?.name ?? "");
            string UserName = Uri.EscapeDataString(twitterUserProfileResponse?.data?.username ?? "");
            string location = Uri.EscapeDataString(twitterUserProfileResponse?.data?.location ?? "");
            string description = Uri.EscapeDataString(twitterUserProfileResponse?.data?.description ?? "");
            string followersCount = twitterUserProfileResponse?.data?.public_metrics?.followers_count.ToString() ?? "0";
            string followingCount = twitterUserProfileResponse?.data?.public_metrics?.following_count.ToString() ?? "0";
            string tweetCount = twitterUserProfileResponse?.data?.public_metrics?.tweet_count.ToString() ?? "0";
            string mediaCount = twitterUserProfileResponse?.data?.public_metrics?.media_count.ToString() ?? "0";
            string likeCount = twitterUserProfileResponse?.data?.public_metrics?.like_count.ToString() ?? "0";
            string profileImageUrl= Uri.EscapeDataString(twitterUserProfileResponse?.data?.profile_image_url ?? "");
            string email = Uri.EscapeDataString("Test@gmail.com");
            string TwitterUID = Uri.EscapeDataString(twitterUserProfileResponse?.data?.id ?? "");

            string redirectUrl = $"http://localhost:4200/login-success" +
                                 $"?Status={shouldRediret}" +
                                 $"&screenName={screenName}" +
                                 $"&UserName={UserName}" +
                                 $"&Location={location}" +
                                 $"&Description={description}" +
                                 $"&FollowersCount={followersCount}" +
                                 $"&followingCount={followingCount}" +
                                 $"&mediaCount={mediaCount}" +
                                 $"&TweetCount={tweetCount}" +
                                 $"&likeCount={likeCount}" +
                                 $"&Email={email}" +
                                 $"&TwitterUID={TwitterUID}" +
                                 $"&profileImageUrl={profileImageUrl}";

            return Redirect(redirectUrl);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = "Error occurred during callback processing",
                error = ex.Message,
                Status = false
            });
        }



       /* Console.WriteLine($"Code: {code} , State{state}");
        string redirectUrl = $"http://localhost:4200/login-success?accessToken={code}&email={"karan.depale@test.com"}";
        return Redirect(redirectUrl);*/
    }

}
