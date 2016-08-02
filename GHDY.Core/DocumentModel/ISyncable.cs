using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHDY.Core.DocumentModel
{
    public interface ISyncable
    {
        TimeSpan BeginTime { get; set; }
        TimeSpan EndTime { get; set; }

        double Confidence { get; set; }

        string ToSpeechText();
    }

    public static class SyncableExtentions
    {
        public static bool ContainsTimeSpan(this ISyncable syncObj, TimeSpan timeSpan)
        {
            return syncObj.BeginTime < timeSpan && timeSpan < syncObj.EndTime;
        }
    }
}
