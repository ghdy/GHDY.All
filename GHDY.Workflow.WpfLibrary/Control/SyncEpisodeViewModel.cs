using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Workflow.Recognize;
using GHDY.Core.DocumentModel;
using System.Windows.Controls;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class SyncEpisodeViewModel : BaseStateControlViewModel, INotifySyncEpisode
    {
        public SyncEpisodeViewModel(UserControl uControl) : base(uControl)
        {

        }

        #region BaseStateControlViewModel

        public override string Title
        {
            get
            {
                return "SyncEpisode";
            }
        }

        public override RecognizeTransition State
        {
            get
            {
                return Recognize.RecognizeTransition.Recognize;
            }
        }

        public override string Description
        {
            get
            {
                return "Sync audio and transcript.";
            }
        }

        protected override void Initialize()
        {

        }

        public override void StateLoaded()
        {
            this.CanSelectNextPage = true;
            this.BusyManager.NotBusy();
        }

        public override void StateCompleted()
        {
        }
        #endregion

        #region INotifySyncProgress

        public void NotifyRecognizeProgress(DMDocument episodeDocument)
        {
            
        }

        public void NotifyRecognizeCompleted()
        {
            
        }

        #endregion
    }
}
