using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Interfaces;
using WebApi.Model;

public class LoginLogic : ILoginLogic
{
    private readonly ConfigSettings _twitterSettings;
    private readonly IConfiguration _configuration;
    private readonly ILoginWrapper _loginWrapper;

    public LoginLogic(IOptions<ConfigSettings> twitterOptions, IConfiguration configuration , ILoginWrapper loginWrapper)
    {
        _twitterSettings = twitterOptions.Value;
        _configuration = configuration;
        _loginWrapper = loginWrapper;
    }

    public string GenerateLoginUrl()
    {
        var requestUrl = $"{_twitterSettings.AuthorizeTokenBaseUrl}" +
                         $"?response_type={_twitterSettings.ResponseType}" +
                         $"&client_id={_twitterSettings.ClientId}" +
                         $"&redirect_uri={Uri.EscapeDataString(_twitterSettings.RedirectUri)}" +
                         $"&scope={Uri.EscapeDataString(_twitterSettings.Scope)}" +
                         $"&state={_twitterSettings.State}" +
                         $"&code_challenge={_twitterSettings.CodeChallenge}" +
                         $"&code_challenge_method={_twitterSettings.CodeChallengeMethod}";

        return requestUrl;
    }

    public string GenerateJWTToken(string email, string userId)
    {
        var key = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("Email", email),
            new Claim("UserID", userId)
        };

        var token = new JwtSecurityToken(issuer, audience, claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateRandomNumber(int min, int max)
    {
        return new Random().Next(min, max).ToString();
    }

    public string GetUserIdFromJwtToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var userIdClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "userID")?.Value;

            return userIdClaim;
        }
        catch
        {
            return null;
        }
    }

    public async Task<TwitterAuthorizationCodeResponse> ExchangeCodeForAccessToken(string code)
    {
        try
        {
            var responseObject = new TwitterAuthorizationCodeResponse();

            string clientId = _configuration["Twitter:ClientId"] ?? "";
            string clientSecret = _configuration["Twitter:ClientSecret"] ?? "";
            string redirectUri = _configuration["Twitter:RedirectUri"] ?? "";
            string tokenUrl = _configuration["Twitter:TokenUrl"] ?? "";
            string oAuth2ClientSecret = _configuration["Twitter:ClientSecret"] ?? "";

            var options = new RestClientOptions(tokenUrl);
            var client = new RestClient(options);
            string base64EncodedCredentials = CreateBase64EncodedAuthorization(clientId, oAuth2ClientSecret);

            var request = new RestRequest()
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddParameter("grant_type", "authorization_code")
                .AddParameter("client_id", clientId)
                .AddParameter("code", code)
                .AddParameter("redirect_uri", redirectUri)
                .AddParameter("code_verifier", "12345");
            request.AddHeader("Authorization", $"Basic {base64EncodedCredentials}");


            string credentials = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")
            );
            request.AddHeader("Authorization", $"Basic {credentials}");

            var response = await client.PostAsync(request);
            try
            {
                 responseObject = System.Text.Json.JsonSerializer.Deserialize<TwitterAuthorizationCodeResponse>(response.Content);
            }
            catch (Exception)
            {

            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to exchange code for token. Status: {response.StatusCode}, Message: {response.Content}");
            }

            return responseObject;
        }
        catch (Exception ex)
        {
            throw new Exception($"Token exchange failed: {ex.Message}", ex);
        }
    }

    public static string CreateBase64EncodedAuthorization(string OAuth2clientId, string OAuth2clientSecret)
    {
        try
        {
            string credentials = $"{OAuth2clientId}:{OAuth2clientSecret}";

            byte[] credentialsBytes = Encoding.UTF8.GetBytes(credentials);
            string encodedCredentials = Convert.ToBase64String(credentialsBytes);

            return encodedCredentials;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<TwitterUserProfileResponse> GetTwitterUserProfile(TwitterAuthorizationCodeResponse TokensData, string userID)
    {
        try
        {
            string UserProfileUri = _configuration["GetTwitterUserProfile:UserProfileUri"] ?? "";
            var client = new RestClient(UserProfileUri);
            var request = new RestRequest();
            request.Method = Method.Get;

            request.AddHeader("Authorization", $"Bearer {TokensData.access_token.ToString()}");
            request.AddParameter("user.fields", "created_at,description,id,location,name,profile_image_url,protected,public_metrics,username,verified");

            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful && response.Content != null)
            {
                 var userProfileResponse = JsonConvert.DeserializeObject<TwitterUserProfileResponse>(response.Content);
                return userProfileResponse;
            }
            else
            {
                throw new Exception($"Failed to fetch user profile: {response.StatusCode} - {response.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }

    }


    public  string InsertTokensAndUserDataInDB(TwitterAuthorizationCodeResponse twitterAuthorizationResponse, TwitterUserProfileResponse twitterUserProfileResponse)
    {
        try
        {
            return _loginWrapper.InsertTokensAndUserDataInDB(twitterAuthorizationResponse, twitterUserProfileResponse);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }




}
