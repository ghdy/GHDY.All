using GHDY.Workflow.Download;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Demo
{
    public class EpisodeContentDownloadInfo : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public Uri Url { get; set; }

        private DownloadFileResult _result = DownloadFileResult.Fail;
        public DownloadFileResult Result
        {
            get { return this._result; }
            set
            {
                this._result = value;
                this.NotifyPropertyChanged("Result");
                if (this._result != DownloadFileResult.Fail)
                    this.Persentage = 100;
            }
        }

        private int _persentage = 0;
        public int Persentage
        {
            get { return this._persentage; }
            set
            {
                this._persentage = value;
                this.NotifyPropertyChanged("Persentage");
            }
        }

        public EpisodeContentDownloadInfo(string fileName, Uri url)
        {
            this.FileName = fileName;
            this.Url = url;
            this.Result = DownloadFileResult.Downloading;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
