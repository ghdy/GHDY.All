using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace GHDY.Core.LearningContentProviderCore
{
    public abstract class BaseReader
    {
        public string TargetID { get { return this.Target.ID; } }

        public BaseTarget Target { get;private set; }

        public abstract EpisodeContent GetEpisodeContent(string episodeID, string albumID);

        public BaseReader(BaseTarget target)
        {
            this.Target = target;
        }

        public string GetSingleMatch(string patten, string target)
        {
            Regex regex = new Regex(patten);
            Match match = regex.Match(target);
            if (match.Success == true)
                return match.Value;
            else
                return string.Empty;
        }

        public string GetSingleMatch(string patten, string target, string groupName)
        {
            Regex regex = new Regex(patten);
            Match match = regex.Match(target);
            if (match.Success == true)
                return match.Groups[groupName].Value;
            else
                return string.Empty;
        }

        public void ThrowNotHasContentMessage(string filepath)
        {
            throw new Exception(string.Format("[{0}]:'{1}' Not has Content, please check!", 
                this.TargetID, 
                Path.GetFileName(filepath)));
        }

        private static readonly List<BaseReader> _Readers = new List<BaseReader>();
        public static BaseReader GetReader(string targetID)
        {
            foreach (var reader in _Readers)
            {
                if (string.Equals(reader.TargetID, targetID) == true)
                    return reader;
            }

            return null;
        }

        public static void AddReader(BaseReader readerModel)
        {
            foreach (var reader in _Readers)
            {
                if (string.Equals(reader.TargetID, readerModel.TargetID) == true)
                    return;
            }

            _Readers.Add(readerModel);
        }

    }

    public static class ReaderExtends
    {
        public static string FormatHtmlString(this string htmlString)
        {
            return System.Web.HttpUtility.HtmlDecode(htmlString);
        }
    }
}
