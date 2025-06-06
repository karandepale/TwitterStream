namespace WebApi.Interfaces
{
    public interface ITweetDashboardWrapper
    {
        public List<string> GetTokens(string tweetUID);
    }
}
