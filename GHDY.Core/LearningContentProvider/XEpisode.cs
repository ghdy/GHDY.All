using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace GHDY.Core.LearningContentProviderCore
{
    public class XEpisode : XElement, INotifyPropertyChanged
    {
        public const string ElementName = "Episode";

        public const string AttrAlbumID = "AlbumID";
        public const string AttrID = "ID";
        public const string AttrTitle = "Title";
        public const string AttrURL = "URL";
        public const string AttrDate = "Date";
        public const string AttrModifyDate = "ModifyDate";
        public const string AttrHasLrc = "HasLrc";
        public const string AttrHasTranslation = "HasTranslation";

        public const string AttrIsWebPageDownloaded = "WebPageDownloaded";
        public const string AttrIsContentDownloaded = "ContentDownloaded";
        public const string AttrIsTranslated = "Translated";
        public const string AttrIsRecognized = "Recognized";

        //public string AlbumID
        //{
        //    get
        //    {
        //        var attr = this.Parent.Attribute(XAlbum.AttrID);
        //        if (attr != null)
        //            return attr.Value;
        //        else
        //            return "";
        //    }
        //}

        public string AlbumID
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrAlbumID);

                if (attr != null)
                    return this.Attribute(XEpisode.AttrAlbumID).Value;
                else
                    return "";
            }
            set { this.RealElement.SetAttributeValue(XEpisode.AttrAlbumID, value); }
        }

        public string ID
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrID);

                if (attr != null)
                    return this.Attribute(XEpisode.AttrID).Value;
                else
                    return "";
            }
            set { this.RealElement.SetAttributeValue(XEpisode.AttrID, value); }
        }

        public string Title
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrTitle);

                if (attr != null)
                    return this.Attribute(XEpisode.AttrTitle).Value;
                else
                    return "";
            }
            set { this.RealElement.SetAttributeValue(XEpisode.AttrTitle, value); }
        }

        public Uri URL
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrURL);

                if (attr != null)
                    return new Uri(this.Attribute(XEpisode.AttrURL).Value);
                else
                    return null;
            }
            set
            {
                this.RealElement.SetAttributeValue(XEpisode.AttrURL, value.AbsoluteUri);
            }
        }

        public DateTime Date
        {
            get { return DateTime.Parse(this.RealElement.Attribute(XEpisode.AttrDate).Value); }
            set { this.RealElement.SetAttributeValue(XEpisode.AttrDate, value.ToString()); }
        }

        public DateTime ModifyDate
        {
            get
            {
                if (this.RealElement.Attribute(XEpisode.AttrModifyDate) == null)
                    this.ModifyDate = DateTime.MinValue;
                return DateTime.Parse(this.RealElement.Attribute(XEpisode.AttrModifyDate).Value);
            }
            set { this.RealElement.SetAttributeValue(XEpisode.AttrModifyDate, value.ToString()); }
        }

        public bool HasLrc
        {
            get
            {
                if (this.RealElement == null)
                    return false;

                var attr = this.RealElement.Attribute(XEpisode.AttrHasLrc);

                if (attr != null)
                    return (bool)this.Attribute(XEpisode.AttrHasLrc);
                else
                    return false;
            }
            set
            {
                this.RealElement.SetAttributeValue(XEpisode.AttrHasLrc, value);
                this.NotifyPropertyChanged("HasLrc");
            }
        }

        public bool HasTranslation
        {
            get
            {
                if (this.RealElement == null)
                    return false;
                var attr = this.RealElement.Attribute(XEpisode.AttrHasTranslation);

                if (attr != null)
                    return (bool)this.Attribute(XEpisode.AttrHasTranslation);
                else
                    return false;
            }
            set
            {
                this.RealElement.SetAttributeValue(XEpisode.AttrHasTranslation, value);
                this.NotifyPropertyChanged("HasTranslation");
            }
        }

        public bool IsWebPageDownloaded
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrIsWebPageDownloaded);

                if (attr != null)
                    return (bool)attr;
                else
                    return false;
            }
            set
            {
                this.RealElement.SetAttributeValue(XEpisode.AttrIsWebPageDownloaded, value);
                NotifyPropertyChanged("IsWebPageDownloaded");
            }
        }

        public bool IsContentDownloaded
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrIsContentDownloaded);

                if (attr != null)
                    return (bool)attr;
                else
                    return false;
            }
            set
            {
                this.RealElement.SetAttributeValue(XEpisode.AttrIsContentDownloaded, value);
                NotifyPropertyChanged("IsContentDownloaded");
            }
        }

        public bool IsTranslated
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrIsTranslated);
                if (attr == null)
                    return false;
                else
                    return (bool)attr;
            }
            set
            {
                this.RealElement.SetAttributeValue(XEpisode.AttrIsTranslated, value);
                NotifyPropertyChanged("IsTranslated");
            }
        }

        public bool IsRecognized
        {
            get
            {
                var attr = this.RealElement.Attribute(XEpisode.AttrIsRecognized);
                if (attr == null)
                    return false;
                else
                    return (bool)attr;
            }
            set
            {
                this.RealElement.SetAttributeValue(XEpisode.AttrIsRecognized, value);
                NotifyPropertyChanged("IsRecognized");
            }
        }

        public XEpisode(XElement element)
            : base(element)
        {
            this.RealElement = element;
        }

        public XEpisode(string albumID, string id, string title, Uri url)
            : base(XEpisode.ElementName)
        {
            this.RealElement = this;

            this.AlbumID = albumID;
            this.ID = id;
            this.Title = title;
            this.URL = url;
            this.Date = DateTime.MinValue;
            this.IsWebPageDownloaded = false;
            this.IsContentDownloaded = false;
            this.IsTranslated = false;
            this.IsRecognized = false;
        }

        public XElement RealElement { get; private set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
