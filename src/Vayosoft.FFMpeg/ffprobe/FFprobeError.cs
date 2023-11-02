using Newtonsoft.Json;

namespace Vayosoft.FFMpeg.ffprobe
{
    [JsonObject]
    public class FFprobeError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("string")]
        public string Description { get; set; }

    }
}
