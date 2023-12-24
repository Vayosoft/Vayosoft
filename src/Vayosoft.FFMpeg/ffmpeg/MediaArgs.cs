namespace Vayosoft.FFMpeg.ffmpeg
{
    public class MediaArgs
    {
        public string TempFolder { get; set; }
        public string TempFilename { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public MediaMimeTypeCode MimeType { set; get; }
        public int Frames { get; set; }
        public int VBitRate { get; set; }
        public int ABitRate { get; set; }
        public void CheckTempFolder()
        {
            if (string.IsNullOrEmpty(TempFolder))
                throw new ArgumentException($"MediaArgs: Temporary folder is not defined");

            if (!Directory.Exists(TempFolder))
                throw new ArgumentException($"MediaArgs: Temporary folder [{TempFolder}] does not exist");
        }
    }
}
