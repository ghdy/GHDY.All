
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GHDY.Core.DocumentModel;
using GHDY.Core.LearningContentProviderCore;

namespace GHDY.Core.Episode
{
    public interface IEpisode
    {
        string ID { get; set; }

        string AudioFilePath { get; }

        string WaveFilePath { get; }

        DMDocument SyncDocument { get; set; }
        DMDocument DictationDocument { get; set; }
        EpisodeContent Content { get; set; }

        Lyrics Lrc { get; set; }
    }
}
