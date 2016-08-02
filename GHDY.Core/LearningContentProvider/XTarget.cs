using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GHDY.Core.LearningContentProviderCore
{
    public class XTarget : XDocument
    {
        public const string ElementName = "Target";
        public const string AttrID = "ID";
        public const string AttrURL = "URL";

        public string ID
        {
            get { return (string)this.Root.Attribute(XTarget.AttrID); }
            set { this.Root.SetAttributeValue(XTarget.AttrID, value); }
        }

        public Uri URL
        {
            get { return new Uri(this.Root.Attribute(XTarget.AttrURL).Value); }
            set { this.Root.SetAttributeValue(XTarget.AttrURL, value.AbsoluteUri); }
        }

        public IEnumerable<XAlbum> Albums
        {
            get
            {
                if (this.Root.HasElements == true)
                    return this.Root.Elements(XAlbum.ElementName).Select(element => new XAlbum(element));
                else
                    return null;
            }
        }

        public XTarget(XDocument document)
            : base(document)
        {

        }

        public XTarget(string name, Uri url)
            : base()
        {
            this.Add(new XElement(XTarget.ElementName));

            this.ID = name;
            this.URL = url;
        }

        public XAlbum GetAlbum(string albumID)
        {
            var result = this.Albums.Single((album) => {
                if (album.ID == albumID)
                    return true;
                else
                    return false;
            });

            return result;
        }
    }
}
