using GHDY.Core.LearningContentProviderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Download.Interface
{
    public interface IResourceReceiver
    {
        void Receive(IEnumerable<XEpisode> episodes);
        void Receive(IEnumerable<XPage> pages);
        void Receive(IEnumerable<XAlbum> albums);
    }
}
