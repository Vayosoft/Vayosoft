using Newtonsoft.Json;

namespace Vayosoft.FFMpeg.ffprobe
{
    [JsonObject]
    public class FFprobeStreamTags
    {
        [JsonProperty("rotate")]
        public int Rotate { get; set; }
        [JsonProperty("creation_time")]
        public string CreationTime { get; set; }
        [JsonProperty("language")]
        public string Language { get; set; }
        [JsonProperty("handler_name")]
        public string HandlerName { get; set; }
    }
}
