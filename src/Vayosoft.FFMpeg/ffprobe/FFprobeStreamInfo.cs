using Newtonsoft.Json;

namespace Vayosoft.FFMpeg.ffprobe
{
    [JsonObject]
    public class FFprobeStreamInfo
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("codec_name")]
        public string CodecName { get; set; }
        [JsonProperty("codec_long_name")]
        public string CodecLongName { get; set; }
        [JsonProperty("profile")]
        public string Profile { get; set; }

        [JsonProperty("codec_type")]
        public string CodecType { get; set; }
        [JsonIgnore] public bool IsVideo => "video".Equals(CodecType);
        [JsonIgnore] public bool IsAudio => "audio".Equals(CodecType);

        [JsonProperty("codec_tag_string")]
        public string CodecTagString { get; set; }
        [JsonProperty("codec_tag")]
        public string CodecTag { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("coded_width")]
        public int CodedWidth { get; set; }
        [JsonProperty("coded_height")]
        public int CodedHeight { get; set; }
        [JsonProperty("display_aspect_ratio")]
        public string DisplayAspectRatio { get; set; }
        [JsonProperty("pix_fmt")]
        public string PixFmt { get; set; }
        [JsonProperty("avg_frame_rate")]
        public string AvgFramerate { get; set; }

        //public string DurationStr { get; set; }
        [JsonProperty("duration")]
        public string Duration { get; set; }

        //public string BitrateStr { get; set; }
        [JsonProperty("bit_rate")]
        public int Bitrate { get; set; }

        [JsonProperty("tags")]
        public FFprobeStreamTags Tags { get; set; }
    }
}
