using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GHDY.Core.LearningContentProviderCore
{
    //No one use it as XElement
    public class XPage : XElement
    {
        public const string ElementName = "Page";

        public string AlbumID { get; set; }

        public int Index { get; set; }

        public Uri URL { get; set; }

        public XPage(XElement element)
            : base(element)
        {

        }

        public XPage(string albumID, int index, Uri url)
            : base(ElementName)
        {
            this.AlbumID = albumID;
            this.Index = index;
            this.URL = url;
        }
    }
}
