using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Model
{
    public class ComposeTweetResponse
    {
        public ComposeData data { get; set; }
    }

    public class ComposeData
    {
        public List<string> edit_history_tweet_ids { get; set; }
        public string text { get; set; }
        public string id { get; set; }
    }

}
