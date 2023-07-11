using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml.Linq;
using HtmlAgilityPack;
using GHDY.Core.LearningContentProviderCore;
using LanguageResourceProvider.VOA;

namespace GHDY.Core.LearningContentProvider.VOA._51VOA
{
    public class Target_51VOA : BaseTarget
    {
        public const string TargetID = "51VOA";
        public const string BaseUrl = "http://www.51voa.com/";

        public override string ID
        {
            get { return TargetID; }
        }

        public override string URL
        {
            get { return BaseUrl; }
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        private BaseReader _reader = null;
        public override BaseReader Reader
        {
            get
            {
                if (this._reader == null)
                    this._reader = new Reader_51VOA(this);
                return this._reader;
            }
        }

        public int FromYear { get; private set; }

        public Target_51VOA(int fromYear)
        {
            this.FromYear = fromYear;
        }

        [Export(typeof(Func<XTarget>))]
        protected override XTarget GetCurrentXTarget()
        {
            return this.CurrentXTarget;
        }

        [Export(typeof(Func<IEnumerable<XAlbum>>))]
        public override IEnumerable<XAlbum> GetAlbums()
        {
            if (this.CurrentXTarget.Albums == null || this.CurrentXTarget.Albums.Count() < 1)
            {
                this.CurrentXTarget.Root.Add(new XAlbum("TechnologyReport", "Technology Report", new Uri(this.CurrentXTarget.URL, "Technology_Report_1.html")));
                this.CurrentXTarget.Root.Add(new XAlbum("AgricultureReport", "Agriculture Report", new Uri(this.CurrentXTarget.URL, "Agriculture_Report_1.html")));
                this.CurrentXTarget.Root.Add(new XAlbum("HealthReport", "Health Report", new Uri(this.CurrentXTarget.URL, "Health_Report_1.html")));
                this.CurrentXTarget.Root.Add(new XAlbum("EducationReport", "Education Report", new Uri(this.CurrentXTarget.URL, "Education_Report_1.html")));
                this.CurrentXTarget.Root.Add(new XAlbum("EconomicsReport", "Economics Report", new Uri(this.CurrentXTarget.URL, "Economics_Report_1.html")));
                this.CurrentXTarget.Root.Add(new XAlbum("InTheNews", "In The News", new Uri(this.CurrentXTarget.URL, "In_the_News_1.html")));
                this.CurrentXTarget.Root.Add(new XAlbum("WordsAndTheirStories", "Words And Their Stories", new Uri(this.CurrentXTarget.URL, "Words_And_Their_Stories_1.html")));

                this.SaveXTarget();
            }
            return this.CurrentXTarget.Albums;
        }

        [Export(typeof(Func<XAlbum, IEnumerable<XPage>>))]
        public override IEnumerable<XPage> GetPages(XAlbum album)
        {
            //var folderPath = this.GetAlbumFolderPath(album.ID);

            string albumHtml;
            var albumHtmlFilePath = Path.Combine(this.SourceFolderPath, album.ID + ".html");

            var hasNew = album.HasNewModify;
            if (hasNew == true)
            {
                album.Date = album.NewDate;
                SaveXTarget();
                albumHtml = BaseTarget.SaveAndReturn(album.URL, albumHtmlFilePath, this.Encoding);
            }
            else
                albumHtml = File.ReadAllText(albumHtmlFilePath, Encoding.UTF8);

            //var match = this.GetRegexResult("<b>\\d+?</b>/<b>(?<MaxPage>\\d+?)</b>\\s*?每页\\s*?<b>(?<PageEpisodes>\\d+?)</b>\\s*?共\\s*?<b>(?<EpisodeCount>\\d+?)</b>", albumHtml);
            var matches = BaseTarget.GetRegexResults("<a\\shref=\"(?<URL>[0-9a-zA-Z\\._]+?)\">.*?\\[(?<Index>\\d+?)\\].*?</a>", albumHtml);
            foreach (var match in matches)
            {
                yield return new XPage(album.ID, int.Parse(match.Groups["Index"].Value), new Uri(this.CurrentXTarget.URL, match.Groups["URL"].Value));
            }
        }

        [Export(typeof(Func<XPage, IEnumerable<XEpisode>>))]
        public override IEnumerable<XEpisode> GetEpisodes(XPage page)
        {
            var html = BaseTarget.SaveAndReturn(page.URL,
                Path.Combine(this.GetAlbumFolderPath(page.AlbumID), page.Index.ToString() + ".html"),
                this.Encoding);

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var listNode = htmlDoc.GetElementbyId("list");

            if (listNode == null)
                listNode = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@class='list']");

            if (listNode != null)
            {
                var liCollection = listNode.SelectNodes("ul/li");

                foreach (HtmlNode li in liCollection)
                {
                    //process episode title,date,contentUrl
                    var a = li.Element("a");

                    var episodeUrl = new Uri(this.CurrentXTarget.URL, a.Attributes["href"].Value);
                    var id = Path.GetFileNameWithoutExtension(episodeUrl.ToString());

                    #region Find and Check Date

                    var match = GetRegexResult("\\d+-\\d+-\\d+", li.InnerHtml);
                    var dateString = match.Value.Trim();

                    var array = dateString.Split('-');
                    int year = int.Parse(array[0]);
                    int month = int.Parse(array[1]);
                    int day = int.Parse(array[2]);

                    if (year < 2000)
                        year += 2000;

                    //Only Return Episodes these after 2012

                    if (year < FromYear)
                        break;
                    if (VOAUtilities.ChackEpisodeDate(page.AlbumID, year, month, day, out DateTime date) == false)
                        date = new DateTime(year, month, day);

                    #endregion

                    var newEpisode = new XEpisode(page.AlbumID, id, a.InnerText.Trim(), episodeUrl) { Date = date };


                    //process hasLrc,hasTranslation
                    var images = li.Elements("img");
                    foreach (HtmlNode image in images)
                    {
                        var imgText = image.Attributes["src"].Value;
                        if (imgText.Contains("lrc.gif") == true)
                        {
                            newEpisode.HasLrc = true;
                        }

                        if (imgText.Contains("yi.gif") == true)
                        {
                            newEpisode.HasTranslation = true;
                        }
                    }

                    newEpisode = this.CheckEpisode(newEpisode);

                    yield return newEpisode;
                }
            }
        }
    }
}
