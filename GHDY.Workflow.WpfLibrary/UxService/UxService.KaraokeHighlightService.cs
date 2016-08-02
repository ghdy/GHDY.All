using GHDY.Core.AudioPlayer;
using GHDY.Core.DocumentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace GHDY.Workflow.WpfLibrary.UxService
{
    [Export("HighLightService")]
    [Export(typeof(IUxService))]
    public class KaraokeHighlightService : IUxService
    {
        [Import(typeof(AudioPlayerBase))]
        public AudioPlayerBase AudioPlayer { get; set; }

        [Import(typeof(DMDocument))]
        public DMDocument Document { get; set; }

        public Dictionary<Type, object> SelectedObjectDict { get; private set; }

        public DMParagraph SelectedParagraph
        {
            get
            {
                if (this.SelectedObjectDict.ContainsKey(typeof(DMParagraph)) == true)
                    return this.SelectedObjectDict[typeof(DMParagraph)] as DMParagraph;
                else
                    return null;
            }
            set
            {
                if (this.SelectedParagraph != null)
                {
                    this.SelectedParagraph.SetIsCurrent(false);
                }

                if(value != null)
                    value.SetIsCurrent(true);
                this.SelectedObjectDict[typeof(DMParagraph)] = value;
            }
        }

        public DMSentence SelectedSentence
        {
            get
            {
                if (this.SelectedObjectDict.ContainsKey(typeof(DMSentence)) == true)
                    return this.SelectedObjectDict[typeof(DMSentence)] as DMSentence;
                else
                    return null;
            }
            set
            {
                if (this.SelectedSentence != null)
                {
                    this.SelectedSentence.SetIsCurrent(false);
                }

                if (value != null)
                    value.SetIsCurrent(true);
                this.SelectedObjectDict[typeof(DMSentence)] = value;
            }
        }

        public SyncableWord SelectedWord
        {
            get
            {
                if (this.SelectedObjectDict.ContainsKey(typeof(SyncableWord)) == true)
                    return this.SelectedObjectDict[typeof(SyncableWord)] as SyncableWord;
                else
                    return null;
            }
            set
            {
                if (this.SelectedWord != null)
                {
                    this.SelectedWord.SetIsCurrent(false);
                }

                if (value != null)
                    value.SetIsCurrent(true);
                this.SelectedObjectDict[typeof(SyncableWord)] = value;
            }
        }

        public KaraokeHighlightService()
        {

            this.SelectedObjectDict = new Dictionary<Type, object>();
        }

        #region IUxService

        bool _isActivated = false;
        public bool IsActivated
        {
            get { return this._isActivated; }
        }

        public void Activate()
        {
            if (this.AudioPlayer != null)
            {
                this.AudioPlayer.PositionChanged += AudioPlayer_PositionChanged;
                this._isActivated = true;
            }
        }

        public void Deactivate()
        {
            this.AudioPlayer.PositionChanged -= AudioPlayer_PositionChanged;
            this._isActivated = false;
        }

        #endregion

        void AudioPlayer_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            if (this.Document == null) return;

            var position=e.CurrentPosition;

            DMParagraph newSelectedParagraph = null;
            DMSentence newSelectedSentence = null;

            bool isInCurrentPara = this.CheckIsInCurrentParagraph(position);
            if (isInCurrentPara == true)
            {
                newSelectedParagraph = this.SelectedParagraph;

                var isInCurrentSen = this.CheckIsInCurrentSentence(position);
                if (isInCurrentSen == true)
                {
                    newSelectedSentence = this.SelectedSentence;
                }
                else
                    newSelectedSentence = this.SelectedParagraph.GetSentence(position);
            }
            else
            {
                newSelectedParagraph = this.Document.GetParagraph(position);
                if (newSelectedParagraph != null)
                    newSelectedSentence = newSelectedParagraph.GetSentence(position);
            }

            if(newSelectedParagraph != this.SelectedParagraph)
                this.SelectedParagraph = newSelectedParagraph;

            if(newSelectedSentence != this.SelectedSentence)
                this.SelectedSentence = newSelectedSentence;

            SyncableWord word = null;
            if (newSelectedSentence != null)
                word = newSelectedSentence.GetWord(position);

            if (word != this.SelectedWord)
                this.SelectedWord = word;
        }

        private bool CheckIsInCurrentParagraph(TimeSpan position)
        {
            var para = this.SelectedParagraph;
            if (para != null && para.ContainsTimeSpan(position))
                return true;
            else
                return false;
        }

        private bool CheckIsInCurrentSentence(TimeSpan position)
        {
            var sentence = this.SelectedSentence;
            if (sentence != null && sentence.ContainsTimeSpan(position))
                return true;
            else
                return false;
        }
    }
}
