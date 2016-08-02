using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GHDY.Core
{
    public class EpisodeImage : XElement
    {
        public const string ElementName = "Image";

        public EpisodeImage(int index, string info, Uri url)
            : base(ElementName)
        {
            this.ParagraphIndex = index;
            this.SentenceIndex = -1;
            this.Info = info;
            this.URL = url;
        }

        public EpisodeImage(XElement element)
            : base(element)
        {

        }

        public int ParagraphIndex
        {
            get
            {
                return (int)this.Attribute("PIndex");
            }
            private set
            {
                this.SetAttributeValue("PIndex", value);
            }
        }

        public int SentenceIndex
        {
            get
            {
                return (int)this.Attribute("SIndex");
            }
            private set
            {
                this.SetAttributeValue("SIndex", value);
            }
        }

        public Uri URL
        {
            get
            {
                return new Uri(this.Attribute("URL").Value);
            }
            private set
            {
                this.SetAttributeValue("URL", value);
            }
        }

        public string FileName
        {
            get
            {
                if (this.URL == null)
                    return "";

                return Path.GetFileName(this.URL.AbsolutePath);
            }
        }

        public string Info
        {
            get
            {
                return this.Attribute("Info").Value;
            }
            private set
            {
                this.SetAttributeValue("Info", value);
            }
        }
    }
}
