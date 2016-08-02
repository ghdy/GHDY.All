using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize
{
    public enum RecognizeTransition
    {
        Init,
        DictationAudio,
        Split2Sentences,
        SetTimeline,
        MarkupSound,
        ActualPronounce,
        Recognize,
        Compleded
    }
}
