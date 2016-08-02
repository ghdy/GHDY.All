using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Activities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using GHDY.Workflow.Download.Interface;
using GHDY.Core.LearningContentProviderCore;
using GHDY.Workflow.Download;
using GHDY.Workflow.Recognize;
using GHDY.Workflow.WpfLibrary;
using GHDY.Workflow.WpfLibrary.Control;
using GHDY.Workflow.WpfLibrary.UxService;

namespace GHDY.Demo
{
    public class EpisodeDownloadWindowViewModel : INotifyPropertyChanged,
        IResourceReceiver,
        INotifyEpisodeContentDownloadState
    {
        public BaseTarget CurrentTarget { get; private set; }

        public IEnumerable<XAlbum> Albums { get; private set; }
        public IEnumerable<XPage> Pages { get; private set; }
        public IEnumerable<XEpisode> Episodes { get; private set; }
        public ObservableCollection<EpisodeContentDownloadInfo> DownloadInfoCollection { get; set; }

        public WorkflowApplication WorkFlowApp { get; set; }

        private string _message = "";
        public string Message
        {
            get { return _message; }
            set
            {
                this._message = value;
                this.NotifyPropertyChanged("Message");
            }
        }

        public Window CurrentWindow { get;private set; }

        CompositionContainer _container = null;

        public EpisodeDownloadWindowViewModel(BaseTarget target, Window win)
        {
            this.CurrentWindow = win;
            this.CurrentTarget = target;

            this.InitCommands();
            this.CurrentWindow.Closing += CurrentWindow_Closing;

            this.Receive(this.CurrentTarget.GetAlbums());

            this.DownloadInfoCollection = new ObservableCollection<EpisodeContentDownloadInfo>();
        }

        void CurrentWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.WorkFlowApp != null)
            {
                this.WorkFlowApp.Abort();
                this.WorkFlowApp = null;
            }

            if (this._container != null)
            {
                this._container.Dispose();
                this._container = null;
            }
        }

        public void InitCommands()
        {
            this.CurrentWindow.CommandBindings.Add(new CommandBinding(this.CmdDownloadContent, CmdDownloadContent_Executed, CmdDownloadContent_CanExecute));
            this.CurrentWindow.CommandBindings.Add(new CommandBinding(this.CmdCreateDocumentModel, CmdCreateDocumentModel_Executed, CmdCreateDocumentModel_CanExecute));
        }

        #region Commands

        RoutedUICommand _cmdDownloadContent = new RoutedUICommand();
        public RoutedUICommand CmdDownloadContent { get { return _cmdDownloadContent; } }

        private void CmdDownloadContent_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool result = false;
            //var episode = e.Parameter as XEpisode;
            //if (episode != null)
            //    result = episode.IsContentDownloaded == false;

            if (this.WorkFlowApp == null)
                result = true;

            e.CanExecute = result;
        }

        private void CmdDownloadContent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var episode = e.Parameter as XEpisode;
            if (episode == null)
            {
                MessageBox.Show("Error Episode is NULL!");
                return;
            }

            //this.DownloadInfoCollection.Add(new EpisodeContentDownloadInfo("-----------------------", new Uri("http://voa4.us"))
            //{
            //    Result = DownloadFileResult.Fail,
            //    Persentage = 0
            //});
            this.DownloadInfoCollection.Clear();

            ActivityDownloadEpisode activity = new ActivityDownloadEpisode();

            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.Add("target", this.CurrentTarget);
            dictionary.Add("episode", episode);

            this.WorkFlowApp = new WorkflowApplication(activity, dictionary)
            {
                Completed = new Action<WorkflowApplicationCompletedEventArgs>((args) =>
                {
                    var key = "result";
                    if (args.Outputs.ContainsKey(key) == true)
                    {
                        var result = args.Outputs[key].ToString();
                        this.Message = "Download Episode Completed! Result:" + result;
                    }
                    this.WorkFlowApp = null;
                    this.CurrentWindow.Dispatcher.Invoke(new Action(() => {
                        CommandManager.InvalidateRequerySuggested();
                    }));
                }),
                OnUnhandledException = new Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction>((args) =>
                {
                    this.Message = "Workflow Error:" + args.UnhandledException.Message;
                    this.WorkFlowApp = null;
                    this.CurrentWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }));
                    return UnhandledExceptionAction.Cancel;
                }),
            };

            WorkFlowApp.Extensions.Add(this);

            this.Message = "Downloading Episode:" + episode.ID;

            WorkFlowApp.Run();
        }

        //----------------------------------------------------------------------------------------


        RoutedUICommand _cmdCreateDocumentModel = new RoutedUICommand();
        public RoutedUICommand CmdCreateDocumentModel { get { return _cmdCreateDocumentModel; } }

        private void CmdCreateDocumentModel_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            bool result = false;
            var episode = e.Parameter as XEpisode;
            if (episode != null)
                result = episode.IsContentDownloaded == true;

            if (this.WorkFlowApp != null)
                result = false;

            e.CanExecute = result;
        }

        private void CmdCreateDocumentModel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var episode = e.Parameter as XEpisode;
            if (episode == null)
            {
                MessageBox.Show("Error Episode is NULL!");
                return;
            }

            RecognizeEpisodeWizardWindow wizardWindow = new RecognizeEpisodeWizardWindow(this.CurrentTarget, episode);

            InitUserControl initUC = new InitUserControl();
            SplitTranscriptUserControl splitUC = new SplitTranscriptUserControl();
            DictationUserControl dictationUC = new DictationUserControl();
            ReferenceSoundMarkupUserControl refSoundMarkupUC = new ReferenceSoundMarkupUserControl();
            SetDocumentTimelineUserControl setDocTimelineUC = new SetDocumentTimelineUserControl();
            PronunciationMarkupUserControl pronunciationMarkupUC = new PronunciationMarkupUserControl();
            SyncEpisodeUserControl syncEpisodeUC = new SyncEpisodeUserControl();

            //KaraokeHighlightService karaokeService = new KaraokeHighlightService();


            if (this._container != null)
            {
                this._container.Dispose();
                this._container = null;
            }
            this._container = new CompositionContainer();

            this._container.ComposeParts(
                wizardWindow.ViewModel, 

                //UserControl
                initUC.ViewModel, 
                splitUC.ViewModel, 
                dictationUC.ViewModel,
                setDocTimelineUC.ViewModel, 
                refSoundMarkupUC.ViewModel,
                pronunciationMarkupUC.ViewModel,
                syncEpisodeUC.ViewModel
                //services
                //karaokeService
                );

            wizardWindow.Show();
        }

        #endregion

        #region IResourceReceiver

        public void Receive(IEnumerable<XEpisode> episodes)
        {
            this.Episodes = episodes;
            this.NotifyPropertyChanged("Episodes");
        }

        public void Receive(IEnumerable<XAlbum> albums)
        {
            this.Albums = albums;
            this.NotifyPropertyChanged("Albums");
        }

        public void Receive(IEnumerable<XPage> pages)
        {
            this.Pages = pages;
            this.NotifyPropertyChanged("Pages");
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region INotifyEpisodeContentDownloadState

        public void NotifyEpisodeContentDownloading(string fileName, Uri url)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.DownloadInfoCollection.Add(new EpisodeContentDownloadInfo(fileName, url));
            }));
        }

        public void NotifyEpisodeContentDownloaded(string fileName, DownloadFileResult result)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                var downloadInfo = FindDownloadInfo(fileName);

                downloadInfo.Result = result;
            }));
        }

        public void NotifyDownloadProgress(string fileName, int percentage)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                var downloadInfo = FindDownloadInfo(fileName);
                downloadInfo.Persentage = percentage;
            }));
        }

        #endregion

        private EpisodeContentDownloadInfo FindDownloadInfo(string fileName)
        {
            var downloadInfo = this.DownloadInfoCollection.Single((info) =>
            {
                if (info.FileName == fileName)
                    return true;
                else
                    return false;
            });
            return downloadInfo;
        }
    }
}
