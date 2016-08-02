using GHDY.Core.DocumentModel;
using GHDY.Workflow.Recognize.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using GHDY.Workflow.WpfLibrary.UxService;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class PronunciationMarkupViewModel : BaseStateControlViewModel,INotifyPronunciationMarkup
    {
        public Action<DMDocument> DocumentChangedAction { get; set; }

        public PronunciationMarkupViewModel(UserControl control)
            : base(control)
        {

        }

        #region BaseStateControlViewModel
        public override Recognize.RecognizeTransition State
        {
            get { return Recognize.RecognizeTransition.ActualPronounce; }
        }

        public override string Title
        {
            get { return "PronunciationMarkup"; }
        }

        public override string Description
        {
            get { return "Markup real Pronunciation to Word & Phrase."; }
        }

        public override void StateLoaded()
        {
            this.CanSelectNextPage = true;
            this.BusyManager.NotBusy();
        }

        protected override void Initialize()
        {
            
        }
        #endregion

        #region INotifyPronunciationMarkup
        public void LoadSyncDocument(string filePath)
        {

            this.ParentWindow.Dispatcher.Invoke(() =>
            {
                DMDocument doc = DMDocument.Load(filePath);
                this.DocumentChangedAction(doc);

                using (CompositionContainer container = new CompositionContainer())
                {
                    KaraokeHighlightService karaokeService = new KaraokeHighlightService();

                    container.ComposeParts(this.AudioPlayer,doc, karaokeService);
                }
            });
        }
        #endregion
    }
}
