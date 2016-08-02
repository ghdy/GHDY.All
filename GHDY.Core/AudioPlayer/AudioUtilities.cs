using Shell32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace GHDY.Core.AudioPlayer
{
    public class AudioUtilities
    {
        public static TimeSpan GetInfo(string audioPath)
        {
            var dirName = Path.GetDirectoryName(audioPath);
            var SongName = Path.GetFileName(audioPath);//获得歌曲名称
            FileInfo fInfo = new FileInfo(audioPath);
            Shell32.ShellClass sh = new Shell32.ShellClass();
            Folder dir = sh.NameSpace(dirName);
            FolderItem item = dir.ParseName(SongName);
            var timeString =  Regex.Match(dir.GetDetailsOf(item, -1), "\\d:\\d{2}:\\d{2}").Value;//获取歌曲时间

            return TimeSpan.Parse(timeString);
        }
    }
}
