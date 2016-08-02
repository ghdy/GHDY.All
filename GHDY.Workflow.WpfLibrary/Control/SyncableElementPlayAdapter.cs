using GHDY.Core.AudioPlayer;
using GHDY.Core.DocumentModel.SyncControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class SyncableElementPlayAdapter : IDisposable
    {
        public DMDocumentScrollViewer DocumentViewer { get; private set; }
        public AudioPlayerBase AudioPlayer { get; private set; }

        public SyncableElementPlayAdapter(DMDocumentScrollViewer docViewer, AudioPlayerBase audioPlayer)
        {
            this.DocumentViewer = docViewer;
            this.AudioPlayer = audioPlayer;

            this.DocumentViewer.SelectionChanged += DocumentViewer_SelectionChanged;
        }

        private void DocumentViewer_SelectionChanged(object sender, EventArgs e)
        {
            if (this.DocumentViewer.BeginTime <= TimeSpan.Zero && this.DocumentViewer.EndTime <= TimeSpan.Zero)
                return;

            this.AudioPlayer?.PlayRange(this.DocumentViewer.BeginTime, this.DocumentViewer.EndTime);
        }

        public void Dispose()
        {
            this.DocumentViewer.SelectionChanged -= DocumentViewer_SelectionChanged;

        }
    }
}
