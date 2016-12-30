using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

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

        public static T GetParent<T>(this ISyncable syncable) where T : class,ISyncable
        {
            var parent = (syncable as TextElement).Parent;
            while (parent != null)
            {
                if (parent is T)
                    return parent as T;
                else if (parent is TextElement)
                {
                    var textElement = (parent as TextElement);
                    if (textElement != null)
                        parent = textElement.Parent;
                    else
                        break;
                }
                else
                    break;
            }
            return parent as T;
        }
    }
}
