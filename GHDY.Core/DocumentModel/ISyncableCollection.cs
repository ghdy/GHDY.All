using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GHDY.Core.DocumentModel
{
    public interface ISyncableCollection
    {
        T GetSyncable<T>(TimeSpan currentTimeSpan) where T : class,ISyncable;
    }
}
