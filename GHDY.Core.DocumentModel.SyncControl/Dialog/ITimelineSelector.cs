using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{
    public class TimelineEventArgs : EventArgs
    {
        public List<ISyncable> SyncableObjects { get; private set; }

        public TimelineEventArgs(ISyncable syncable)
        {
            this.SyncableObjects = new List<ISyncable>();
            this.SyncableObjects.Add(syncable);
        }

        public TimelineEventArgs(List<ISyncable> syncables)
        {
            this.SyncableObjects = syncables;
        }
    }

    [InheritedExport(typeof(ITimelineSelector))]
    public interface ITimelineSelector
    {
        event EventHandler<TimelineEventArgs> SyncableObjectSelected;

        TimeSpan BeginTime { get; }
        TimeSpan EndTime { get; }

        bool CanSelectByTranscript { get; }

        bool CanSelectByCharIndex { get; }

        void SelectByTranscript(string transcript);

        void SelectByCharIndex(int beginCharIndex, int endCharIndex, int allCharCount);

        void Select(TimeSpan time);
        void Select(TimeSpan begin, TimeSpan end);
    }
}
