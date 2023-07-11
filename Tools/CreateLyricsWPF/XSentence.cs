using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace CreateLyricsWPF
{
    public class XContent : XDocument
    {
        public XContent()
            : base()
        {
            this.Add(new XElement("Doc"));
        }

        public XContent(string filePath)
            : base(XDocument.Load(filePath))
        {
            //this.Add(XElement.Load(filePath));
        }

        public IEnumerable<XSentence> Sentences
        {
            get
            {
                foreach (var element in this.Root.Elements())
                {
                    yield return new XSentence(element);
                }
            }
        }
    }

    public class XSentence : XElement, INotifyPropertyChanged
    {
        public const string ElementName = "S";
        public const string BeginAttrName = "B";
        public const string EndAttrName = "E";
        public const string TextAttrName = "T";
        public const string TranAttrName = "F";

        public XElement Original { get; set; }

        public XSentence(XElement element)
            : base(element)
        {
            this.Original = element;
        }

        public XSentence(string transcript)
            : base(XSentence.ElementName)
        {
            this.Original = this;
            this.Text = transcript;
        }

        public XSentence(string transcript,string translation)
            : base(XSentence.ElementName)
        {
            this.Original = this;
            this.Text = transcript;
        }

        public TimeSpan BeginTime
        {
            get
            {
                var attr = this.Original.Attribute(XSentence.BeginAttrName);
                if (attr != null)
                    return (TimeSpan)attr;
                else
                    return TimeSpan.Zero;
            }
            set
            {
                this.Original.SetAttributeValue(XSentence.BeginAttrName, value);
                this.OnPropertyChanged("BeginTime");
                this.OnPropertyChanged("Foreground");
            }
        }

        public TimeSpan EndTime
        {
            get
            {
                var attr = this.Original.Attribute(XSentence.EndAttrName);
                if (attr != null)
                    return (TimeSpan)attr;
                else
                    return TimeSpan.Zero;
            }
            set
            {
                this.Original.SetAttributeValue(XSentence.EndAttrName, value);
                this.OnPropertyChanged("EndTime");
                this.OnPropertyChanged("Foreground");
            }
        }

        public TimeSpan Duration { get { return this.EndTime - this.BeginTime; } }

        public string Text
        {
            get
            {
                var attr = this.Original.Attribute(XSentence.TextAttrName);
                if (attr != null)
                    return attr.Value;
                else
                    return String.Empty;
            }
            set
            {
                this.Original.SetAttributeValue(XSentence.TextAttrName, value);
                this.OnPropertyChanged("Text");
            }
        }

        public string Translation
        {
            get
            {
                var attr = this.Original.Attribute(XSentence.TranAttrName);
                if (attr != null)
                    return attr.Value;
                else
                    return String.Empty;
            }
            set
            {
                this.Original.SetAttributeValue(XSentence.TranAttrName, value);
                this.OnPropertyChanged("Translation");
            }
        }

        public Brush Foreground
        {
            get
            {
                if (this.BeginTime > TimeSpan.Zero && this.EndTime > this.BeginTime)
                    return Brushes.Blue;
                else
                    return Brushes.Black;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
