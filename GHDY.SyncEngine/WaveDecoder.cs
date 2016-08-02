using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace GHDY.SyncEngine
{
    public class WaveDecoder
    {
        public static string ffmpegPath = "Externals\\ffmpeg.exe";

        public bool? TwoPass = null;

        public int? AR = null;

        public void Process(string input, string output)
        {
            if (File.Exists(input) == false)
                throw new FileNotFoundException(input);

            if (File.Exists(ffmpegPath) == false)
                throw new FileNotFoundException(ffmpegPath);

            var twoPassPara = "";

            if (TwoPass.HasValue) twoPassPara = " -ac " + (bool.Parse(TwoPass.Value.ToString()) ? "2 " : "1 ");

            var ar = "";
            if (AR.HasValue) ar = " -ar " + AR.Value;

            if (File.Exists(output))
                File.Delete(output);

            var args = String.Format(" -i \"{0}\" {1} {2} \"{3}\"", input, twoPassPara, ar, output);

            var processor = new Process()
            {
                StartInfo = new ProcessStartInfo(ffmpegPath, args)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                }
            };
            processor.Start();

            processor.WaitForExit();

            if (File.Exists(output) == false)
            {
                throw new FileNotFoundException(output);
            }
        }

        public void ProcessForRecognize(string input, string output)
        {
            AR = 22050;
            TwoPass = false;

            Process(input, output);
        }
    }
}
