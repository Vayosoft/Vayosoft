using Newtonsoft.Json;

namespace Vayosoft.FFMpeg.ffprobe
{
    [JsonObject]
    public class FFprobeFormatInfo
    {
        [JsonProperty("filename")]
        public string Filename { get; set; }
        [JsonProperty("nb_streams")]
        public int NbStreamsCount { get; set; }
        [JsonProperty("nb_programs")]
        public int NbProgramsCount { get; set; }
        [JsonProperty("format_name")]
        public string FormatName { get; set; }
        [JsonProperty("format_long_name")]
        public string FormatLongName { get; set; }
        [JsonProperty("duration")]
        public double Duration { get; set; }
        [JsonProperty("size")]
        public ulong Size { get; set; }
        [JsonProperty("bit_rate")]
        public uint BitRate { get; set; }
        [JsonProperty("tags")]
        public FFprobeFormatTags Tags { get; set; }
    }
}
