using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace GHDY.Core.DocumentModel.SyncControl
{
    public static class Utility
    {
        public static T GetSelectedSyncable<T>(this TextPointer selectionTextPointer) where T : TextElement, ISyncable
        {
            var parent = selectionTextPointer.Parent;
            while (parent != null)
            {
                if (parent is T)
                    return (T)parent;
                else if (parent is TextElement)
                {
                    parent = (parent as TextElement).Parent;
                }
                else
                    break;
            }

            return parent as T;
        }



        public static double GetPercent(int up, int down)
        {
            return ((double)up) / down;
        }
    }
}
