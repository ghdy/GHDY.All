using GHDY.Core.Episode;
using GHDY.Workflow.Recognize.Interface;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class InitViewModel : BaseStateControlViewModel, INotifyInitialize
    {
        readonly StringBuilder _message = new StringBuilder();
        public string Message { get { return this._message.ToString(); } }

        LocalEpisode _episode = null;
        public LocalEpisode Episode
        {
            get { return this._episode; }
            private set
            {
                this._episode = value;
                NotifyAllPropertiesChanged();
            }
        }

        private void NotifyAllPropertiesChanged()
        {
            this.NotifyPropertyChanged("EpisodeID");
            this.NotifyPropertyChanged("AudioInfo");
            this.NotifyPropertyChanged("LyricsInfo");
            this.NotifyPropertyChanged("DictationInfo");
            this.NotifyPropertyChanged("DocumentInfo");
        }

        #region For UIVIew

        public string EpisodeID
        {
            get
            {
                if (this.Episode != null)
                    return this.Episode.ID;
                else
                    return "";
            }
        }

        public string AudioInfo
        {
            get
            {
                if (this.Episode == null)
                    return "";
                return GetFileInfo(this.Episode.AudioFilePath);
            }
        }

        public string LyricsInfo
        {
            get
            {
                if (this.Episode == null)
                    return "";
                return GetFileInfo(this.Episode.SubtitleFilePath);
            }
        }

        public string DictationInfo
        {
            get
            {
                if (this.Episode == null)
                    return "";
                return GetFileInfo(this.Episode.DictationDocumentFilePath);
            }
        }

        public string DocumentInfo
        {
            get
            {
                if (this.Episode == null)
                    return "";
                return GetFileInfo(this.Episode.SyncDocumentFilePath);
            }
        }

        private string GetFileInfo(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);
            if (fi.Exists == true)
            {
                return String.Format("[{0}] Exists, FileSize:{1}KB", fi.Name, (fi.Length / 1024.0).ToString());
            }
            else
                return String.Format("Not Found.");
        }

        #endregion

        public InitViewModel(UserControl uControl)
            : base(uControl)
        {
        }

        public override Recognize.RecognizeTransition State
        {
            get { return Recognize.RecognizeTransition.Init; }
        }

        public override string Title
        {
            get { return "Initialize Workflow"; }
        }

        public override string Description
        {
            get { return "Initialize StateMachine, check Episode resources, Set Workflow Params. "; }
        }

        public override void StateLoaded()
        {
            this.CanSelectNextPage = true;
            this.NotifyAllPropertiesChanged();
            this.BusyManager.NotBusy();
        }

        protected override void Initialize()
        {
            this.CanCancel = true;

        }

        #region INotifyInitialize
        public void NotifyMessage(string message)
        {
            try
            {
                this.ParentWindow.Dispatcher.Invoke(() =>
                {
                    this._message.AppendLine(message);
                    this.NotifyPropertyChanged("Message");
                });
            }
            catch (Exception exp)
            {
                throw new Exception("Error in INotifyInitialize.NotifyMessage(message)", exp);
            }
        }

        public void NotifyEpisode(Core.Episode.LocalEpisode episode)
        {
            this.ParentWindow.Dispatcher.Invoke(() => { this.Episode = episode; });

        }
        #endregion
    }
}
