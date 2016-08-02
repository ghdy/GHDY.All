using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Net;
using System.IO;
using System.Threading;
using GHDY.Core;
using System.Threading.Tasks;
using GHDY.Workflow.Download.Interface;

namespace GHDY.Workflow.Download
{
    public enum DownloadFileResult
    {
        Success, Exists, Fail, Downloading
    }

    /// <summary>
    /// Activity based on NativeActivity<TResult>
    /// </summary>
    /// 
    //[Designer(typeof(DownloadFileActivityDesigner))]
    public sealed class DownloadFileActivity : NativeActivity<DownloadFileResult>
    {
        public const string UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 Safari/537.36";

        //AutoResetEvent autoResetEvent = new AutoResetEvent(false);

        // Define an activity input argument of Type String
        [RequiredArgument]
        public InArgument<Uri> SourceUrl { get; set; }

        [RequiredArgument]
        public InArgument<String> DownloadFilePath { get; set; }

        private InArgument<bool> _notifyDownloadProgress = false;
        public InArgument<bool> NotifyDownloadProgress
        {
            get { return this._notifyDownloadProgress; }
            set { this._notifyDownloadProgress = value; }
        }

        private InArgument<int> _minFileSizeKB = 1;
        public InArgument<int> MinFileSizeKB
        {
            get { return this._minFileSizeKB; }
            set { this._minFileSizeKB = value; }
        }

        private InArgument<int> _delaySeconds = 2;
        public InArgument<int> DelaySeconds
        {
            get { return this._delaySeconds; }
            set { this._delaySeconds = value; }
        }

        private string _fileName = "";
        private bool _isNotifyDownloading = false;
        private INotifyEpisodeContentDownloadState _notifyEpisodeContentDownloadState = null;

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override void Execute(NativeActivityContext context)
        {
            var url = context.GetValue<Uri>(this.SourceUrl);
            var filePath = context.GetValue<string>(this.DownloadFilePath);
            _fileName = Path.GetFileName(filePath);

            this._isNotifyDownloading = context.GetValue<bool>(this.NotifyDownloadProgress);

            var minFileSizeKB = context.GetValue<int>(this.MinFileSizeKB);
            var delaySeconds = context.GetValue<int>(this.DelaySeconds);

            //set _notifyEpisodeContentDownloadState before Notify
            this._notifyEpisodeContentDownloadState = context.GetExtension<INotifyEpisodeContentDownloadState>();
            NotifyDownloadStart(_fileName, url);

            if (this.CheckFileIsOK(filePath, minFileSizeKB) == true)
            {
                this.Result.Set(context, DownloadFileResult.Exists);
                NotifyDownloadEnd(_fileName, DownloadFileResult.Exists);
            }
            else
            {
                DownloadClient downloadClient = new DownloadClient();

                if (this._isNotifyDownloading == true)
                {
                    downloadClient.DownloadProgressChanged += downloadClient_DownloadProgressChanged;
                }

                downloadClient.DownloadFileCompleted += downloadClient_DownloadFileCompleted;

                var task = downloadClient.DownloadFileAsync(filePath, url);

                task.Wait();
                //autoResetEvent.WaitOne();

                if (this.CheckFileIsOK(filePath, minFileSizeKB) == true)
                {
                    this.Result.Set(context, DownloadFileResult.Success);
                    NotifyDownloadEnd(_fileName, DownloadFileResult.Success);

                    if (delaySeconds > 0)
                        Thread.Sleep(TimeSpan.FromSeconds(delaySeconds));
                }
                else
                {
                    this.Result.Set(context, DownloadFileResult.Fail);
                    NotifyDownloadEnd(_fileName, DownloadFileResult.Fail);
                }
            }
        }

        #region NotifyEpisodeContentDownloadStateChanged
        private void NotifyDownloadStart(string fileName, Uri url)
        {
            if (this._notifyEpisodeContentDownloadState != null)
                this._notifyEpisodeContentDownloadState.NotifyEpisodeContentDownloading(fileName, url);
        }

        private void NotifyDownloadEnd(string fileName, DownloadFileResult result)
        {
            if (this._notifyEpisodeContentDownloadState != null)
                this._notifyEpisodeContentDownloadState.NotifyEpisodeContentDownloaded(fileName, result);
        }


        private void NotifyDownloadPersantage(string fileName, int percentage)
        {
            if (this._notifyEpisodeContentDownloadState != null)
                this._notifyEpisodeContentDownloadState.NotifyDownloadProgress(fileName, percentage);
        }
        #endregion

        void downloadClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this._isNotifyDownloading == true)
                Console.WriteLine();

            if (e.Error != null)
                Console.WriteLine("Download Error:" + e.Error.Message);
            else if (e.Cancelled == true)
                Console.WriteLine("Download Error");

            //this.autoResetEvent.Set();
        }

        void downloadClient_DownloadProgressChanged(object sender, DownloadedChangedEventArgs e)
        {
            Console.Write(e.ProgressPercentage + "%_");

            this.NotifyDownloadPersantage(_fileName, e.ProgressPercentage);
        }

        private bool CheckFileIsOK(string filePath, double minFileSizeKB)
        {
            if (File.Exists(filePath) == true)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                double fileSize = (double)fileInfo.Length / 1024;

                if (fileSize > minFileSizeKB)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            // Register In arguments
            RuntimeArgument urlArg = new RuntimeArgument("SourceUrl", typeof(Uri), ArgumentDirection.In);
            metadata.AddArgument(urlArg);
            metadata.Bind(this.SourceUrl, urlArg);

            // [SourceUrl] Argument must be set
            if (this.SourceUrl == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[SourceUrl] argument must be set!",
                        false,
                        "SourceUrl"));
            }

            // Register In arguments
            RuntimeArgument filePathArg = new RuntimeArgument("DownloadFilePath", typeof(String), ArgumentDirection.In);
            metadata.AddArgument(filePathArg);
            metadata.Bind(this.DownloadFilePath, filePathArg);

            // [SourceUrl] Argument must be set
            if (this.DownloadFilePath == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[DownloadFilePath] argument must be set!",
                        false,
                        "DownloadFilePath"));
            }

            // Register In arguments
            RuntimeArgument notifyDownloadingArg = new RuntimeArgument("NotifyDownloading", typeof(bool), ArgumentDirection.In);
            metadata.AddArgument(notifyDownloadingArg);
            metadata.Bind(this.NotifyDownloadProgress, notifyDownloadingArg);

            // Register In arguments
            RuntimeArgument minSizeArg = new RuntimeArgument("MinFileSizeKB", typeof(int), ArgumentDirection.In);
            metadata.AddArgument(minSizeArg);
            metadata.Bind(this.MinFileSizeKB, minSizeArg);

            // Register In arguments
            RuntimeArgument delaySecondsArg = new RuntimeArgument("DelaySeconds", typeof(int), ArgumentDirection.In);
            metadata.AddArgument(delaySecondsArg);
            metadata.Bind(this.DelaySeconds, delaySecondsArg);

            // Register Out arguments
            RuntimeArgument resultArg = new RuntimeArgument("Result", typeof(DownloadFileResult), ArgumentDirection.Out);
            metadata.AddArgument(resultArg);
            metadata.Bind(this.Result, resultArg);

            // [Result] Argument must be set
            if (this.Result == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[Result] argument must be set!",
                        false,
                        "Result"));
            }

            // TODO : Add arguments ... etc ...
        }
    }
}
