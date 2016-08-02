using GHDY.Core;
using GHDY.Workflow.Download;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Download.Interface
{
    public interface INotifyEpisodeContentDownloadState
    {
        void NotifyEpisodeContentDownloading(string fileName, Uri url);
        void NotifyEpisodeContentDownloaded(string fileName, DownloadFileResult result);

        void NotifyDownloadProgress(string fileName, int percentage);
    }
}
