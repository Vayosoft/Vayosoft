using Newtonsoft.Json;

namespace Vayosoft.FFMpeg.ffprobe
{
    public class FFprobeHelper: ProcessAdapter
    {
        private const string EXE_NAME = "ffprobe.exe";
        //private bool _IsFFprobeExist = false;
        private string _FFProbeFullpath;

        private void CheckFFprobePath(string path)
        {
            if (!File.Exists(path))
            {
                _FFProbeFullpath = null;
                //_IsFFprobeExist = false;
                throw new FileNotFoundException($"ffprobe application not found: {path}");
            }

            _FFProbeFullpath = path;
            //_IsFFprobeExist = true;
        }
        public string FFProbeFullpath
        {
            get => _FFProbeFullpath;
            set => CheckFFprobePath(value);
        }

        public FFprobeHelper(string ffprobeFullpath)
        {
            CheckFFprobePath(ffprobeFullpath.EndsWith(EXE_NAME, StringComparison.InvariantCultureIgnoreCase) 
                ? ffprobeFullpath
                : Path.Combine(ffprobeFullpath, EXE_NAME)
            );
        }

        public FFprobeHelper()
        {
            CheckFFprobePath(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), EXE_NAME));
        }

        public FFprobeVideoInfo GetVideoInfo(string filename)
        {
            var d = DateTime.Now;
            if (!File.Exists(filename))
                throw new FileNotFoundException($"Video file: {filename} not found");
            
            var args = $"-i {filename} -v quiet -print_format json -show_format -show_streams -show_error -hide_banner";

            var json = RunProcess(_FFProbeFullpath, args);
            
            try
            {
                var res = JsonConvert.DeserializeObject<FFprobeVideoInfo>(json);
                if (res.Error != null)
                    throw new ArgumentException($"FFprobeHelper.GetVideoInfo failed. Code: {res.Error.Code} Desc: {res.Error.Description}");

                res.FullFilename = filename;
                //TextLogger.Info($"GetVideoInfo({res.Format.Size/1024}kb) {(DateTime.Now -d).TotalSeconds} seconds");
                return res;
            }
            catch (Exception e)
            {
                throw new Exception($"Can't get video info: {e.Message}\r\nRecieved data: {json}");
            }
        }
    }
}
