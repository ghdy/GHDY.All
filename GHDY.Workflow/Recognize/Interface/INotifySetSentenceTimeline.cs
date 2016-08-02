using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    public interface INotifySetSentenceTimeline
    {
        void NotifySyncDocument(string documentstring);

        void NotifyDictation(string dictationFilePath);
        void NotifyLyrics(string lyricsFilePath);
    }
}
