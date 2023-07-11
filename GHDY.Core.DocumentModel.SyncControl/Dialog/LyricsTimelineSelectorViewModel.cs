using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{
    public class LyricsTimelineSelectorViewModel : INotifyPropertyChanged
    {

        Lyrics _lrc = null;
        public Lyrics Lyrics
        {
            get { return this._lrc; }
            set
            {
                this._lrc = value;
                this.NotifyPropertyChanged("Lyrics");
            }
        }

        int _lyricsLength = 0;
        public int LyricsLength
        {
            get
            {
                if (this._lyricsLength < 1 && this.Lyrics != null)
                {
                    this.Lyrics.Phrases.ForEach((phrase) =>
                    {
                        this._lyricsLength += phrase.Text.Length;
                    });
                }

                return this._lyricsLength;
            }
        }

        ObservableCollection<ISyncable> _selectedCollection;
        public ObservableCollection<ISyncable> SelectedCollection
        {
            get
            {
                if (this._selectedCollection == null)
                {
                    this._selectedCollection = new ObservableCollection<ISyncable>();
                    //this._selectedCollection.CollectionChanged += _selectedCollection_CollectionChanged;
                }
                return this._selectedCollection;
            }
        }

        //void _selectedCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if (this.SyncableObjectSelected != null && this.SelectedCollection.Count > 0)
        //    {
        //        this.SyncableObjectSelected(this, new TimelineEventArgs(this.SelectedCollection.ToList()));
        //    }
        //}

        public LyricsTimelineSelectorViewModel(Lyrics lrc)
        {
            this.Lyrics = lrc;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
