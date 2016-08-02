using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HtmlAgilityPack;
using GHDY.Core.LearningContentProviderCore;

namespace GHDY.Core.LearningContentProvider.VOA._51VOA
{
    public class Reader_51VOA : BaseReader
    {
        public Reader_51VOA(BaseTarget target)
            : base(target)
        {

        }

        public override EpisodeContent GetEpisodeContent(string episodeID, string albumID)
        {
            string htmlFilepath = Path.Combine(this.Target.SourceFolderPath, albumID, episodeID) + EpisodeFileTypes.HtmlFile.ToExt();

            string htmlString = File.ReadAllText(htmlFilepath, Encoding.UTF8);

            EpisodeContent result = new EpisodeContent(episodeID, albumID, this.TargetID);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlString);

            var contentElement = doc.GetElementbyId("content");

            if (contentElement == null) this.ThrowNotHasContentMessage(htmlFilepath);

            var children = contentElement.ChildNodes;

            bool isTranscriptEnded = false;

            foreach (var child in children)
            {
                var childNode = child as HtmlNode;
                if (childNode != null)
                {
                    if (isTranscriptEnded == false)
                    {
                        int underLineCount = GetUnderLineCount(childNode.InnerText);
                        if (underLineCount >= 10)
                        {
                            isTranscriptEnded = true;
                            continue;
                        }
                    }

                    if (isTranscriptEnded == false)
                    {
                        var nodeName = childNode.Name.ToUpper().Trim();
                        switch (nodeName)
                        {
                            case "P":
                                var emCollection = childNode.SelectNodes("em");

                                if (emCollection == null || emCollection.Count == 0)
                                {
                                    var paraText = childNode.InnerText.Trim();

                                    if (String.IsNullOrEmpty(paraText) == false)
                                    {
                                        result.AddParagraph(paraText);
                                    }
                                }
                                break;
                            case "DIV":
                                var imgNode = childNode.SelectSingleNode("img");
                                if (imgNode != null)
                                {
                                    var src = imgNode.Attributes["src"].Value;
                                    var url = new Uri(new Uri(this.Target.URL), src);

                                    string info = "";
                                    var captionNode = childNode.SelectSingleNode("span[@class='imagecaption']");
                                    if (captionNode != null)
                                        info = captionNode.InnerText;

                                    var episodeImage = new EpisodeImage(result.Paragraphs.Count(), info, url);
                                    result.AddImage(episodeImage);
                                }
                                break;
                            case "Span":
                                break;
                        }
                    }
                    else
                    {

                    }
                }
            }

            var menubarNode = doc.GetElementbyId("menubar");
            if (menubarNode != null)
            {
                var element = menubarNode.SelectSingleNode("a[@id='mp3']");
                if (element != null)
                    result.AudioURL = new Uri(element.Attributes["href"].Value);

                element = menubarNode.SelectSingleNode("a[@id='lrc']");
                if (element != null)
                    result.LrcURL = new Uri(new Uri(Target_51VOA.BaseUrl), element.Attributes["href"].Value);

                element = menubarNode.SelectSingleNode("a[@id='EnPage']");
                if (element != null)
                {
                    var baseUrl = new Uri(Path.Combine(Target_51VOA.BaseUrl, "VOA_Special_English/"));
                    result.TranslationURL = new Uri(baseUrl, element.Attributes["href"].Value);
                }
            }

            return result;
        }

        private int GetUnderLineCount(string text)
        {
            var underLineCount = text.ToArray().Count(new Func<char, bool>((c) =>
            {
                if (c == '_')
                    return true;
                else
                    return false;
            }));


            //Find “_______________”
            //    int underLineCount = text.Sum(new Func<char, int>((c) =>
            //    {
            //        if (c == '_') return 1;
            //        else return 0;
            //    }));

            return underLineCount;
        }

        private string FormatTranscript(string text)
        {
            string result = text.Replace("51voa.com", "learningenglish.voanews.com");

            return result;
        }
    }
}
