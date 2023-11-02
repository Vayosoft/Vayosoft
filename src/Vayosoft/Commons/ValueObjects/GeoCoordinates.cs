using Newtonsoft.Json;

namespace Vayosoft.Commons.ValueObjects
{
    [JsonObject]
    public class GeoCoordinates
    {
        [JsonProperty]
        public double Longitude { get; set; }
        [JsonProperty]
        public double Latitude { get; set; }

        public GeoCoordinates() { }
        public GeoCoordinates(double longitude, double latitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

    }
}
