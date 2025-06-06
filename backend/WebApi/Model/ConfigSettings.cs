namespace WebApi.Model
{
    public class ConfigSettings
    {
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public string AuthorizeTokenBaseUrl { get; set; }
        public string CodeChallenge { get; set; }
        public string State { get; set; }
        public string ResponseType { get; set; }
        public string CodeChallengeMethod { get; set; }
    }
}
