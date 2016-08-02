using GHDY.Core;
using GHDY.Core.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    public interface INotifyReferenceSoundMarkup
    {
        void NotifySyncDocument(string documentstring);

        void NotifyLyrics(string lrcstring);

        void NotifyDictation(string dictationstring);
    }
}
