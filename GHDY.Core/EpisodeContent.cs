using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GHDY.Core
{
    public class EpisodeContent : XElement
    {
        public const string AttrNameTargetID = "TargetID";
        public const string AttrNameID = "ID";
        public const string AttrTitle = "Title";
        public const string AttrNameAudioURL = "AudioURL";
        public const string AttrNameLrcURL = "LrcURL";
        public const string AttrNameTranslationURL = "TranslationURL";
        public const string AttrNameAlbumID = "AlbumID";

        public const string ElementNameImages = "Images";
        public const string ElementNameParagraphs = "Paragraphs";

        public string ID
        {
            get { return this.Attribute(EpisodeContent.AttrNameID).Value; }
            private set { this.SetAttributeValue(EpisodeContent.AttrNameID, value); }
        }

        public string TargetID
        {
            get { return this.Attribute(EpisodeContent.AttrNameTargetID).Value; }
            private set { this.SetAttributeValue(EpisodeContent.AttrNameTargetID, value); }
        }

        public string AlbumID
        {
            get { return this.Attribute(EpisodeContent.AttrNameAlbumID).Value; }
            private set { this.SetAttributeValue(EpisodeContent.AttrNameAlbumID, value); }
        }

        public Uri AudioURL
        {
            get
            {
                var attr = this.Attribute(EpisodeContent.AttrNameAudioURL);
                if (attr == null)
                    return null;
                else
                    return new Uri(attr.Value);
            }
            set { this.SetAttributeValue(EpisodeContent.AttrNameAudioURL, value); }
        }

        public string AudioUrlString { get { return this.AudioURL.ToString(); } }

        public string AudioFileName
        {
            get
            {
                if (this.AudioURL == null)
                    return "";
                return Path.GetFileName(this.AudioURL.AbsolutePath);
            }
        }

        public Uri LrcURL
        {
            get
            {
                var attr = this.Attribute(EpisodeContent.AttrNameLrcURL);
                if (attr == null)
                    return null;
                else
                    return new Uri(attr.Value);
            }
            set { this.SetAttributeValue(EpisodeContent.AttrNameLrcURL, value); }
        }

        public string LrcFileName
        {
            get
            {
                if (this.LrcURL == null)
                    return "";

                return Path.GetFileName(this.LrcURL.AbsolutePath);
            }
        }

        public Uri TranslationURL
        {
            get
            {
                var attr = this.Attribute(EpisodeContent.AttrNameTranslationURL);
                if (attr == null)
                    return null;
                else
                    return new Uri(attr.Value);
            }
            set { this.SetAttributeValue(EpisodeContent.AttrNameTranslationURL, value); }
        }

        public string TranslationFileName
        {
            get
            {
                if (this.TranslationURL == null)
                    return "";

                return Path.GetFileName(this.TranslationURL.AbsolutePath);
            }
        }

        #region Images
        public XElement ImagesElement
        {
            get
            {
                var element = this.Element(EpisodeContent.ElementNameImages);
                if (element == null)
                {
                    element = new XElement(EpisodeContent.ElementNameImages);
                    this.Add(element);
                }

                return element;
            }
        }

        public IEnumerable<EpisodeImage> Images
        {
            get
            {
                foreach (var element in this.ImagesElement.Elements(EpisodeImage.ElementName))
                {
                    yield return new EpisodeImage(element);
                }
            }
        }

        public void AddImage(EpisodeImage image)
        {
            this.ImagesElement.Add(image);
        }
        #endregion Images

        #region Paragraphs
        public XElement ParagraphsElement
        {
            get
            {
                var element = this.Element(EpisodeContent.ElementNameParagraphs);
                if (element == null)
                {
                    element = new XElement(EpisodeContent.ElementNameParagraphs);
                    this.Add(element);
                }

                return element;
            }
        }

        public IEnumerable<string> Paragraphs
        {
            get
            {
                foreach (var element in this.ParagraphsElement.Elements())
                {
                    yield return element.Value;
                }
            }
        }

        public void AddParagraph(string paragraph)
        {
            this.ParagraphsElement.Add(new XElement("P", paragraph));
        }

        public void ChangeParagraph(int index, string value)
        {
            var i = 0;
            foreach (var element in this.ImagesElement.Elements())
            {
                if (index == i)
                {
                    element.Value = value;
                    break;
                }
                i += 1;
            }
        }
        #endregion

        //public EpisodeContent(string id, string targetID)
        //    : base("Content")
        //{
        //    this.TargetID = targetID;
        //    this.ID = id;
        //    this.AlbumID = "";
        //}

        public EpisodeContent(string id, string album, string targetID)
            : base("Content")
        {
            this.TargetID = targetID;
            this.ID = id;
            this.AlbumID = album;
        }

        public EpisodeContent(XElement element)
            : base(element)
        {

        }
    }
}
