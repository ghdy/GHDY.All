using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.ComponentModel.Composition;

namespace GHDY.Core.LearningContentProviderCore
{
    [InheritedExport(typeof(BaseTarget))]
    public abstract class BaseTarget
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 Safari/537.36";

        private XTarget _currentXTarget = null;
        public XTarget CurrentXTarget
        {
            get
            {
                if (this._currentXTarget == null)
                    this._currentXTarget = this.GetCurrentXTarget();

                return this._currentXTarget;
            }
            private set { this._currentXTarget = value; }
        }

        protected abstract XTarget GetCurrentXTarget();

        //[Export(typeof(Func<IEnumerable<XAlbum>>))]
        public abstract IEnumerable<XAlbum> GetAlbums();

        //[Export(typeof(Func<XAlbum, IEnumerable<Page>>))]
        public abstract IEnumerable<XPage> GetPages(XAlbum album);

        //[Export(typeof(Func<Page, IEnumerable<XEpisode>>))]
        public abstract IEnumerable<XEpisode> GetEpisodes(XPage page);

        public abstract string ID { get; }
        public abstract string URL { get; }
        public abstract Encoding Encoding { get; }
        public abstract BaseReader Reader { get; }

        public string SourceFolderPath
        {
            get
            {
                var result = Path.Combine(Environment.CurrentDirectory, "Targets", this.ID);

                if (Directory.Exists(result) == false)
                    Directory.CreateDirectory(result);
                return result;
            }
        }

        public string SourceFilePath
        {
            get
            {
                return this.SourceFolderPath + ".target";
            }
        }

        public BaseTarget()
        {
            if (File.Exists(this.SourceFilePath) == true)
            {

                var xTarget = new XTarget(XDocument.Load(this.SourceFilePath));
                this.CurrentXTarget = xTarget;
            }
            else
            {
                this.CurrentXTarget = new XTarget(this.ID, new Uri(this.URL, UriKind.Absolute));
                this.CurrentXTarget.Save(this.SourceFilePath, SaveOptions.None);
            }

            this.GetAlbums();
        }

        public void DownloadEpisode(XEpisode episode)
        {
            var episodesFolder = this.GetAlbumFolderPath(episode.AlbumID);
            if (Directory.Exists(episodesFolder) == false) Directory.CreateDirectory(episodesFolder);

            var episodeFilePath = Path.Combine(episodesFolder, episode.ID + ".html");

            //if (File.Exists(episodeFilePath) == true)
            //    File.Delete(episodeFilePath);

            BaseTarget.SaveAndReturn(episode.URL, episodeFilePath, this.Encoding);
            episode.IsWebPageDownloaded = true;

            CheckEpisode(episode);
        }

        public XEpisode CheckEpisode(XEpisode episode)
        {
            var album = this.CurrentXTarget.GetAlbum(episode.AlbumID);

            var findEpisode = album.GetEpisode(episode.ID);
            if (findEpisode != null)
            {
                if (findEpisode.Title != episode.Title)
                    findEpisode.Title = episode.Title;
                if (findEpisode.URL != episode.URL)
                    findEpisode.URL = episode.URL;
                if (findEpisode.Date != episode.Date)
                    findEpisode.Date = episode.Date;

                bool needRefresh = false;
                if (findEpisode.HasLrc != episode.HasLrc)
                {
                    findEpisode.HasLrc = episode.HasLrc;
                    needRefresh = true;
                }
                if (findEpisode.HasTranslation != episode.HasTranslation)
                {
                    findEpisode.HasTranslation = episode.HasTranslation;
                    needRefresh = true;
                }

                if (needRefresh == true)
                {
                    findEpisode.IsWebPageDownloaded = false;
                    findEpisode.IsContentDownloaded = false;
                }
            }
            else
            {
                findEpisode = episode;
                album.AddEpisode(findEpisode);
            }

            this.SaveXTarget();

            return findEpisode;
        }

        public string GetDownloadEpisodeWebPageFilePath(string episodeID, string albumID)
        {
            var albumFolder = this.GetAlbumFolderPath(albumID);

            return EpisodeFileTypes.HtmlFile.ToFileName(Path.Combine(albumFolder, episodeID));
        }

        public string GetDownloadEpisodeContentFolderPath(string episodeID, string albumID)
        {
            var albumFolder = this.GetAlbumFolderPath(albumID);
            var result = Path.Combine(albumFolder, episodeID);
            if (Directory.Exists(result) == false)
                Directory.CreateDirectory(result);

            return result;
        }

        public string GetAlbumFolderPath(string albumID)
        {
            var result = Path.Combine(this.SourceFolderPath, albumID);
            if (Directory.Exists(result) == false)
                Directory.CreateDirectory(result);

            return result;
        }

        public void SaveXTarget()
        {
            this.CurrentXTarget.Save(this.SourceFilePath, SaveOptions.None);
        }

        public void SaveContent(EpisodeContent content)
        {
            var htmlFilePath = this.GetDownloadEpisodeWebPageFilePath(content.ID, content.AlbumID);
            var contentFolder = this.GetDownloadEpisodeContentFolderPath(content.ID, content.AlbumID);

            var contentFilePath = Path.Combine(contentFolder, content.ID) + EpisodeFileTypes.ContentFile.ToExt();
            content.Save(contentFilePath, System.Xml.Linq.SaveOptions.None);
        }

        public void SaveContent(string episodeID, string albumID)
        {
            var content = this.Reader.GetEpisodeContent(episodeID, albumID);
            this.SaveContent(content);
        }

        #region Static Method

        public static string SaveAndReturn(Uri url, string filePath, Encoding encoding)
        {
            var result = "";
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add(HttpRequestHeader.UserAgent, BaseTarget.UserAgent);

                    client.Encoding = encoding;
                    result = client.DownloadString(url);
                }
                File.WriteAllText(filePath, result, Encoding.UTF8);
            }
            catch
            {
                if (File.Exists(filePath) == true)
                {

                    result = File.ReadAllText(filePath, Encoding.UTF8);
                }
            }
            return result;
        }

        public static bool HasNewModify(Uri url, DateTime lastModifyDT, out DateTime modifyDT)
        {
            modifyDT = DateTime.MinValue;
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(url);
            DateTime dt = DateTime.MinValue;
            //myReq.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 Safari/537.36");
            myReq.UserAgent = BaseTarget.UserAgent;

            try
            {
                using (var response = (HttpWebResponse)myReq.GetResponse())
                {
                    if (response.Headers.AllKeys.Contains("Last-Modified") == true)
                    {
                        var modifyDate = response.Headers["Last-Modified"];

                        dt = DateTime.Parse(modifyDate);
                    }
                    else
                        dt = DateTime.Now;
                }
            }
            catch
            {
                return false;
            }

            modifyDT = dt;

            if (dt == lastModifyDT)
                return false;
            else
                return true;

        }

        public static Match GetRegexResult(string pattern, string source)
        {
            Regex regex = new Regex(pattern, RegexOptions.Singleline);
            var match = regex.Match(source);

            if (match.Success == true)
            {
                return match;
            }
            else
                return null;
        }

        public static IEnumerable<Match> GetRegexResults(string pattern, string source)
        {
            Regex regex = new Regex(pattern, RegexOptions.Singleline);
            var matches = regex.Matches(source);

            foreach (Match m in matches)
            {
                if (m.Success == true)
                    yield return m;
            }
        }

        #endregion Static Method
    }
}
