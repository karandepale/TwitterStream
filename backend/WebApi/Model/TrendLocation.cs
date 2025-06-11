namespace WebApi.Model
{
    public class TrendLocation
    {
        public string Name { get; set; }
        public int Woeid { get; set; }
        public string Country { get; set; }
        public PlaceType PlaceType { get; set; }
    }

    public class PlaceType
    {
        public int Code { get; set; }
        public string Name { get; set; }
    }
}
