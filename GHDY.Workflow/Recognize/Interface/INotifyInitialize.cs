using GHDY.Core.Episode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    //public enum DictationNotifyState
    //{ 
    //    Start, Recognized, Completed
    //}

    public interface INotifyInitialize
    {
        void NotifyMessage(string message);
        void NotifyEpisode(LocalEpisode episode);
        //void NotifyDictation(DictationNotifyState state, object param);
    }
}
