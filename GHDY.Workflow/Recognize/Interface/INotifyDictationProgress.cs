using GHDY.Core.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    public interface INotifyDictationProgress
    {
        void Recognized(DMSentence sentence, int percentage);
        void RecognizeCompleted();
        void Exists(string filePath);
    }
}
