using GHDY.Workflow.Recognize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{

    public interface INotifyRecognizeStateChanged
    {
        void NotifyRecognizeStateChanged(RecognizeTransition state);
    }
}
