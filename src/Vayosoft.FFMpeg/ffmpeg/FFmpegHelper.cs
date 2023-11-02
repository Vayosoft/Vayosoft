using Vayosoft.FFMpeg.ffprobe;

namespace Vayosoft.FFMpeg.ffmpeg
{
    public class FFmpegHelper: ProcessAdapter
    {
        private const string EXE_NAME = "ffmpeg.exe";
        private string _FFmpegFullpath;

        public string ThumbnailArgs { get; set; }
        public string TranscodeArgs { get; set; }

        public static string Rotation2Arg(int rotation)
        {
            switch (rotation)
            {
                case 90:
                    return "transpose=1";
                case 180:
                    return "vflip,hflip";
                case 270:
                    return "transpose=2";
                default:
                    return "";
            }
        }

        private void CheckFFmpegPath(string path)
        {
            if (!File.Exists(path))
            {
                _FFmpegFullpath = null;
                //_IsFFmpegExist = false;
                throw new FileNotFoundException($"FFmpeg application not found: {path}");
            }

            _FFmpegFullpath = path;
            //_IsFFmpegExist = true;
        }
        public string FFmpegFullpath
        {
            get => _FFmpegFullpath;
            set => CheckFFmpegPath(value);
        }

        public FFmpegHelper(string ffmpegFullpath)
        {
            CheckFFmpegPath(ffmpegFullpath.EndsWith(EXE_NAME, StringComparison.InvariantCultureIgnoreCase)
                ? ffmpegFullpath
                : Path.Combine(ffmpegFullpath, EXE_NAME)
            );
        }

        public FFmpegHelper()
        {
            CheckFFmpegPath(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), EXE_NAME));
        }

        public byte[] GetThumbnail(FFprobeVideoInfo inputFile, MediaArgs options)
        {
            options.CheckTempFolder();
           
            if (options.Width <= 0)
            {
                var ratio = (double)inputFile.Height / options.Height;
                options.Width = (int)Math.Round(inputFile.Width / ratio);
            }

            var scaleWidth = Math.Max((int)Math.Truncate(options.Height * ((double)inputFile.Width / inputFile.Height)), options.Width);

            var rotation = Rotation2Arg(inputFile.Rotation);
            
            //var op = new VideoOutput { ImageFormat = options.MimeType.GetImageFormat() };
            if (string.IsNullOrEmpty(ThumbnailArgs))
                throw new ArgumentException("FFmpeg thumbnail args are not defined");

            var args = ThumbnailArgs.Replace("{INPUT_FILE}", inputFile.FullFilename)
                .Replace("{SCALE_WIDTH}", scaleWidth.ToString())
                .Replace("{HEIGHT}", options.Height.ToString())
                .Replace("{WIDTH}", options.Width.ToString())
                .Replace("{ROTATION}", string.IsNullOrEmpty(rotation) ? "" : ","+rotation)
                .Replace("{OUTPUT_FILE}", options.TempFilename);
                //string.Format("-noautorotate -i \"{0}\" -f image2 -vframes 1 -vf \"thumbnail,scale={2}:{3},crop={4}:{3}{1}\" \"{5}\"",
                //input.Path, rotation, scaleWidth, options.Height, options.Width, tempFilePath);
                //"-noautorotate -i &quot;{INPUT_FILE}&quot; -f image2 -vframes 1 -vf &quot;thumbnail,scale={SCALE_WIDTH}:{HEIGHT},crop={WIDTH}:{HEIGHT}{ROTATION}&quot; &quot;{OUTPUT_FILE}&quot;"
                // E:\Vayosoft\Transcoding\ffmpeg\ffmpeg.exe -noautorotate -i "E:\Vayosoft\Transcoding\Temp\d2bd117e-d8b9-4788-994e-2ba8366afa00.MP4" -f image2 -vframes 1 -vf "thumbnail,scale=272:150,crop=148:150,transpose=1" "E:\Vayosoft\Transcoding\Temp\d89797e9-e16d-4b14-aec4-4a501ef46eff.png"
            //TextLogger.Debug($"{_FFmpegFullpath} {args}");
            var d = DateTime.Now;
            var output = RunProcess(_FFmpegFullpath, args);

            var fi = new FileInfo(options.TempFilename);
            if (!fi.Exists)
                throw new ApplicationException($"Thumbnail file [{options.TempFilename}] not found. {output}");
            //TextLogger.Info($"Thumbnail({inputFile.Format.Size / 1024}kb) {(DateTime.Now - d).TotalSeconds} seconds");
            byte[] res;
            using (var fs = fi.OpenRead())
            {
                res = new byte[fs.Length];
                fs.Read(res, 0, res.Length);

                fs.Close();
            }
            fi.Delete();

            return res;
        }

        public double Transcode(FFprobeVideoInfo inputFile, MediaArgs options)
        {
            options.CheckTempFolder();

            if (string.IsNullOrEmpty(options.TempFilename))
                throw new ArgumentException($"FFmpegHelper.Transcode: TempFilename is not defined");

            options.Width = Math.Min(options.Width, inputFile.Width);
            options.Height = (int) Math.Truncate((double) inputFile.Height / inputFile.Width * options.Width);

            /*
            -rem %1 - input file path
            -rem %2 - width
            rem %3 - height
            -rem %4 - rotation
            -rem %5 - bitrate
            rem %6 - file name
            -rem %7 - output file path 
            rem %8 - framerate
             */
            //E:\Vayosoft\Transcoding\qsTranscode\transcode_ios.bat E:\Vayosoft\Transcoding\Temp\61ea154c-ffdf-46e3-aa83-a3ee0fa3432d.mp4 1280 720 90 1500 af662261-d454-4b35-be65-572c82d360aa.mp4 E:\Vayosoft\Transcoding\Temp\18232742-cae2-4651-8939-76db041cf5af.mp4 30
            //-i {INPUT_FILE} -movflags faststart -strict -2 -vcodec libx264 -preset ultrafast -profile:v baseline -level 3.1 -b:v {BITRATE}k -r 30 -bf 0 -acodec aac -ac 2 -ar 44100 -b:a 44k -cpu-used 2 -threads 4 -vf &quot;scale=min({WIDTH}\,iw):trunc(ow/a/2)*2{ROTATION}&quot; -map_metadata -1 {OUTPUT_FILE}
            var rotation = Rotation2Arg(inputFile.Rotation);
            //var tfile = string.IsNullOrEmpty(rotation) ? options.TempFilename : Path.Combine(options.TempFolder, "t_" + Path.GetFileName(options.TempFilename));
            var param = TranscodeArgs.Replace("{INPUT_FILE}", inputFile.FullFilename)
                .Replace("{BITRATE}", Math.Min(options.VBitRate, inputFile.VideoBitrate / 1024).ToString())
                .Replace("{WIDTH}", options.Width.ToString())
                .Replace("{ROTATION}", string.IsNullOrEmpty(rotation) ? "" : "," + rotation)
                .Replace("{OUTPUT_FILE}", options.TempFilename);
            //input.Path, options.Width, options.Height, input.Rotation, Math.Min(options.VBitRate, (int)input.BitRate), fileName, options.Path/*tempFilePath*/, Math.Min(options.Frames, (int)input.FrameRate));

            //TextLogger.Debug($"[VIDEO] : {param}");
            var d = DateTime.Now;
            var output = RunProcess(FFmpegFullpath, param);
            var fi = new FileInfo(options.TempFilename);
            if (!fi.Exists)
                throw new ApplicationException($"FFmpegHelper.Transcode failed: {output}");
            if (fi.Length == 0)
                throw new ApplicationException($"FFmpegHelper.Transcode failed (dest.file has zero length): {output}");

            /*var d2 = DateTime.Now;

            if (!string.IsNullOrEmpty(rotation))
            {
                var args = $"-i {fi.FullName} -c copy -metadata:s:v rotate=-{rotation} {options.TempFilename}";
                var routput = RunProcess(FFmpegFullpath, args);
                var rfi = new FileInfo(options.TempFilename);
                if (!rfi.Exists)
                    throw new CommonsException($"FFmpegHelper.Rotate failed: {routput}");
                if (rfi.Length == 0)
                    throw new CommonsException($"FFmpegHelper.Rotate failed (dest.file has zero length): {routput}");

                fi.Delete();
            }*/
            //if (Regex.Match(output, "(([E|e]rror.*)|(NULL @.*))").Success)
            //    throw new CommonsException($"FFmpegHelper.Transcode: {output}");
            var res = (DateTime.Now - d).TotalSeconds;
            //TextLogger.Info($"Transcode ffmpeg({inputFile.Format.Size / 1024}kb) Full: {res} seconds");
            return res;
        }

        public void ExtractAudio(FFprobeVideoInfo inputFile, MediaArgs options)
        {
            //ffmpeg -i source_video.avi -vn -ar 44100 -ac 2 -ab 192K -f mp3 sound.mp3
            options.CheckTempFolder();

            if (string.IsNullOrEmpty(options.TempFilename))
                throw new ArgumentException($"FFmpegHelper.ExtractAudio: TempFilename is not defined");

            var args = $"-i \"{inputFile.FullFilename}\" -vn -ar 44100 -ac 2 -ab 192K -f {options.MimeType.ToString().ToLower()} \"{options.TempFilename}\"";
            var d = DateTime.Now;
            var output = RunProcess(FFmpegFullpath, args);
            var fi = new FileInfo(options.TempFilename);
            if (!fi.Exists)
                throw new ApplicationException($"FFmpegHelper.ExtractAudio failed: {output}");
            if (fi.Length == 0)
                throw new ApplicationException($"FFmpegHelper.ExtractAudio failed (dest.file has zero length): {output}");
            //TextLogger.Info($"ExtractAudio({inputFile.Format.Size / 1024}kb) {(DateTime.Now - d).TotalSeconds} seconds");
        }
    }
}
