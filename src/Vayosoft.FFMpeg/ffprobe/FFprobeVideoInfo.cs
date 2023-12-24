using Newtonsoft.Json;
using Vayosoft.Commons.ValueObjects;

namespace Vayosoft.FFMpeg.ffprobe
{
    [JsonObject]
    public class FFprobeVideoInfo
    {
        [JsonProperty("error")]
        public FFprobeError Error { get; set; }
        [JsonProperty]
        public string FullFilename { get; set; }
        [JsonProperty("streams")]
        public List<FFprobeStreamInfo> Streams { get; set; }
        [JsonProperty("format")]
        public FFprobeFormatInfo Format { get; set; }

        public FFprobeStreamInfo GetVideoStreamInfo()
        {
            var vStream = Streams?.FirstOrDefault(s => s.IsVideo);
            if (vStream == null)
                throw new ArgumentException("Video stream was not found");

            return vStream;
        }

        [JsonIgnore]
        public int Rotation
        {
            get
            {
                var vi = GetVideoStreamInfo();
                return vi?.Tags?.Rotate ?? 0;
            }
        }
        [JsonIgnore] public int Height => GetVideoStreamInfo().Height;
        [JsonIgnore] public int Width => GetVideoStreamInfo().Width;
        [JsonIgnore] public int VideoBitrate => GetVideoStreamInfo().Bitrate;
        [JsonIgnore] public double Framerate
        {
            get
            {
                var s = GetVideoStreamInfo().AvgFramerate;
                if (string.IsNullOrEmpty(s))
                    throw new ArgumentException("Stream frame rate is not found");
                var a = s.Split('/');
                if (a.Length == 1)
                    return double.Parse(s);

                return double.Parse(a[0]) / double.Parse(a[1]);
            }
        }

        [JsonIgnore]
        public GeoCoordinates Coordinates
        {
            get
            {
                string c = null;
                try
                {
                    if (Format?.Tags == null)
                        return null;

                    c = Format.Tags.Location;
                    if (string.IsNullOrEmpty(c))
                        c = Format.Tags.LocationEng;
                    if (string.IsNullOrEmpty(c))
                        return null;

                    //"+31.2361+034.8239/"
                    c = c.Replace("/", "");
                    var p = c.IndexOf("+", 2);
                    if (p == -1)
                        p = c.IndexOf("-", 2);

                    if (p == -1)
                        throw new Exception($"Invalid geo location: {c}");

                    var lon = c.Substring(0, p);
                    var lat = c.Substring(p);

                    return new GeoCoordinates(Double.Parse(lon), Double.Parse(lat));
                }
                catch (Exception e)
                {
#warning add logger
                    //TextLogger.Error($"GeoCoordinates[tag={c}]: {e}");
                    return null;
                }
                
            }
        }

        public bool HasVideoCodec(string codecName)
        {
            if (Streams == null)
                throw new Exception("Stream list is not assigned");

            return Streams.Any(s =>
                s.IsVideo && s.CodecName.Equals(codecName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
