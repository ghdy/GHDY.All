using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.ComponentModel;

namespace GHDY.Core.LearningContentProviderCore
{
    public class XAlbum : XElement, INotifyPropertyChanged
    {
        public const string ElementName = "Album";
        public const string AttrID = "ID";
        //public const string AttrSourceID = "SourceID";
        public const string AttrTitle = "Title";
        public const string AttrURL = "URL";
        public const string AttrDate = "Date";

        public string ID
        {
            get { return (string)this.Attribute(XAlbum.AttrID); }
            set { this.RealElement.SetAttributeValue(XAlbum.AttrID, value); }
        }

        public string TargetID
        {
            get
            {
                var attr = this.Parent.Attribute(XTarget.AttrID);
                if (attr != null)
                    return attr.Value;
                else
                    return "";
            }
        }

        public string Title
        {
            get { return (string)this.Attribute(XAlbum.AttrTitle); }
            set
            {
                this.RealElement.SetAttributeValue(XAlbum.AttrTitle, value);
            }
        }

        public Uri URL
        {
            get { return new Uri(this.Attribute(XAlbum.AttrURL).Value); }
            set { this.RealElement.SetAttributeValue(XAlbum.AttrURL, value.AbsoluteUri); }
        }

        public DateTime Date
        {
            get { return DateTime.Parse(this.Attribute(XAlbum.AttrDate).Value); }
            set
            {
                this.RealElement.SetAttributeValue(XAlbum.AttrDate, value.ToString());
                NotifyPropertyChanged("Date");
            }
        }

        public DateTime NewDate
        {
            get;
            private set;
        }

        public XElement RealElement { get; private set; }

        //bool _hasNewModify = false;
        public bool HasNewModify
        {
            get
            {
                DateTime newDt = DateTime.MinValue;
                var result = BaseTarget.HasNewModify(this.URL, this.Date, out newDt);
                this.NewDate = newDt;
                return result;
            }

            //get { return _hasNewModify; }
            //set
            //{
            //    this._hasNewModify = value;
            //    string propertyName = "HasNewModify";
            //    NotifyPropertyChanged(propertyName);
            //}
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public XAlbum(XElement element)
            : base(element)
        {
            this.RealElement = element;
        }

        public XAlbum(string id, string title, Uri url)
            : base(XAlbum.ElementName)
        {
            this.RealElement = this;

            this.ID = id;
            this.Title = title;
            this.URL = url;
            this.Date = DateTime.MinValue;
            this.NewDate = DateTime.MinValue;
        }

        public IEnumerable<XEpisode> Episodes
        {
            get
            {
                foreach (var element in this.RealElement.Elements(XEpisode.ElementName))
                {
                    yield return new XEpisode(element);
                }
            }
        }

        public XEpisode GetEpisode(string episodeID)
        {
            var result = this.Episodes.SingleOrDefault((xepisode) =>
            {
                if (xepisode.ID == episodeID) return true;
                else
                    return false;
            });

            return result;
        }

        public void AddEpisode(XEpisode episode)
        {
            this.RealElement.Add(episode);

            if (this.RealElement != this)
            {
                this.Add(episode);
            }
        }

        public bool HasNewEpisodes
        {
            get
            {
                return this.GetNewEpisodes().Count > 0;
            }
        }

        public List<XEpisode> GetNewEpisodes()
        {
            List<XEpisode> result = new List<XEpisode>();

            foreach (var episode in Episodes)
            {
                if (episode.IsWebPageDownloaded == false || episode.IsContentDownloaded == false)
                    result.Add(episode);
            }

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
