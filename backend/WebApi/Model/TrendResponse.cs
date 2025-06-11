namespace WebApi.Model
{
    public class TrendResponse
    {
        public List<Trend> Trends { get; set; }
        public DateTime AsOf { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TrendLocation1> Locations { get; set; }
    }

    public class Trend
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string PromotedContent { get; set; } // Often null, for promoted trends
        public string Query { get; set; } // URL-encoded query for the trend
        public int? TweetVolume { get; set; } // Tweet count, may be null
    }

    public class TrendLocation1
    {
        public string Name { get; set; }
        public int Woeid { get; set; }
    }
}
