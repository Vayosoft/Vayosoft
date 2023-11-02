using System.Diagnostics;
using System.Text;

namespace Vayosoft.FFMpeg
{
    public class ProcessAdapter
    {
        public string LastOutput { get; private set; }
        public string RunProcess(string moduleName, string args, bool runAsAdmin = true)
        {
            var res = new StringBuilder();
            //StringBuilder errStr = null;
            var processStartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.GetDirectoryName(moduleName),
                UseShellExecute = false,
                Arguments = args,
                FileName = moduleName
            };

            if (runAsAdmin)
                processStartInfo.Verb = "runas";

            var process = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e) { res.Append(e.Data); }
            );
            process.ErrorDataReceived += new DataReceivedEventHandler
            (
                delegate (object sender, DataReceivedEventArgs e)
                {
                    //if (errStr == null)
                    //    errStr = new StringBuilder();

                    res.Append(e.Data);
                }
            );
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.CancelErrorRead();
            process.CancelOutputRead();

            try
            {
                process?.Close();
            }
            catch (Exception e)
            {
#warning add logger
                //TextLogger.Error($"ProcessAdapter.RunProcess. Close failed: {e.Message}");
            }

            //if (errStr != null && !string.IsNullOrEmpty(errStr.ToString()))
            //    throw new CommonsException($"{Path.GetFileName(moduleName)} error: {errStr.ToString()}");

            LastOutput = res.ToString();

            return LastOutput;
        }
    }
}
