using Newtonsoft.Json;

namespace Vayosoft.FFMpeg.ffprobe
{
    [JsonObject]
    public class FFprobeFormatTags
    {
        [JsonProperty("major_brand")]
        public string MajorBrand { get; set; }
        [JsonProperty("minor_version")]
        public string MinorVersion { get; set; }
        [JsonProperty("compatible_brands")]
        public string CompatibleBrands { get; set; }
        [JsonProperty("creation_time")]
        public string CreationTime { get; set; }
        [JsonProperty("location")]
        public string Location { get; set; }
        [JsonProperty("location-eng")]
        public string LocationEng { get; set; }
    }
}
