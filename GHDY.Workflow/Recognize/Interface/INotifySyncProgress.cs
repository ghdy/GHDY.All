using GHDY.Core.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    public interface INotifySyncProgress
    {
        void NotifyRecognizeProgress(DMSentence recognizedSentence);
    }
}
