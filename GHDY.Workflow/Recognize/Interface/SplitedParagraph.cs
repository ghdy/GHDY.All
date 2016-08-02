using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GHDY.Workflow.Recognize.Interface
{
    public class SplitedParagraph : INotifyPropertyChanged
    {
        private string _transcript;
        public string Transcript
        {
            get { return this._transcript; }
            set
            {
                this._transcript = value;
                this.NotidyPropertyChanged("Transcript");
            }
        }

        private string _speaker;
        public string Speaker
        {
            get { return this._speaker; }
            set
            {
                this._speaker = value;
                this.NotidyPropertyChanged("Speaker");
            }
        }

        public ObservableCollection<string> Sentences { get; set; }

        bool _hasWarning = false;
        public bool HasWarning
        {
            get { return this._hasWarning; }
            set
            {
                this._hasWarning = value;
                this.NotidyPropertyChanged("HasWarning");
            }
        }

        public SplitedParagraph()
        {
            this.Sentences = new ObservableCollection<string>();

            this.Sentences.CollectionChanged += Sentences_CollectionChanged;
        }

        void Sentences_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (var sentence in this.Sentences)
            {
                var first = sentence.Substring(0, 1);
                if (first != first.ToUpper())
                {
                    this.HasWarning = true;
                }
                var lastChar = sentence.Trim().Last();
                if ('A' <= lastChar && lastChar <= 'z')
                    this.HasWarning = true;
            }
        }

        public SplitedParagraph(string transcript, IEnumerable<string> sentences)
            : this()
        {
            this.Transcript = transcript;

            foreach (var sen in sentences)
            {
                this.Sentences.Add(sen);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotidyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
