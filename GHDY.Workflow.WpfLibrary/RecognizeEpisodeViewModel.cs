using GHDY.Core;
using GHDY.Core.AudioPlayer;
using GHDY.Core.Episode;
using GHDY.Core.LearningContentProviderCore;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Workflow.Recognize;
using GHDY.Workflow.WpfLibrary.Control;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GHDY.Core.DocumentModel;

namespace GHDY.Workflow.WpfLibrary
{
    [Export(typeof(RecognizeEpisodeViewModel))]
    [Export(typeof(IBusyManager))]
    public class RecognizeEpisodeViewModel : INotifyPropertyChanged, INotifyRecognizeStateChanged, IBusyManager, IDisposable
    {
        #region Export: [WorkFlowApp], [AudioPlayer]

        [Export(typeof(WorkflowApplication))]
        public WorkflowApplication WorkFlowApp { get; private set; }

        [Export(typeof(AudioPlayerBase))]
        public AudioPlayerBase AudioPlayer { get; private set; }

        #endregion

        #region ImportMany: [BaseStateControlViewModel]

        [ImportMany]
        public IEnumerable<BaseStateControlViewModel> StateControlViewModels { get; set; }

        #endregion

        public Action<RecognizeTransition> RecognizeStateChanged { get; set; }

        public Window Window { get; private set; }

        public BaseTarget Target { get; private set; }

        public XEpisode Episode { get; private set; }

        private string _message;
        public string Message
        {
            get { return this._message; }
            set
            {
                this._message = value;
                this.NotifyPropertyChanged("Message");
            }
        }

        private EpisodeContent _episodeContent = null;
        public EpisodeContent EpisodeContent
        {
            get
            {
                if (this._episodeContent == null)
                    this._episodeContent = this.LocalEpisode.Content;
                return this._episodeContent;
            }
        }

        private LocalEpisode _localEpisode = null;
        public LocalEpisode LocalEpisode
        {
            get
            {
                if (this._localEpisode == null)
                    this._localEpisode = new LocalEpisode(this.Target.GetDownloadEpisodeContentFolderPath(this.Episode.ID, this.Episode.AlbumID));
                return this._localEpisode;
            }
        }

        public RecognizeEpisodeViewModel(BaseTarget target, XEpisode episode, Window view)
        {
            this.Target = target;
            this.Episode = episode;
            this.Window = view;

            this.Window.Closing += Window_Closing;
            this.Window.DataContext = this;

            this.AudioPlayer = new MediaAudioPlayer();

            this.AudioPlayer.Load(this.LocalEpisode.AudioFilePath);

            BuildWorkflowApp();

            if (this.Window.IsLoaded == true)
            {
                Init();
            }
            else
            {
                this.Window.Loaded += ParentWindow_Loaded;
            }
        }

        void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.WorkFlowApp != null)
            {
                this.WorkFlowApp.Abort();
                this.WorkFlowApp = null;
            }
        }

        void ParentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            WorkFlowApp.Extensions.Add(this);
            foreach (var vm in this.StateControlViewModels)
            {
                WorkFlowApp.Extensions.Add(vm);
            }

            this.NotifyPropertyChanged("Episode");
            this.NotifyPropertyChanged("EpisodeContent");
        }

        public void RunWorkflow()
        {
            this.Message = "Recognizing Episode:" + this.Episode.ID;

            WorkFlowApp.Run(TimeSpan.FromSeconds(1));
        }

        public void BuildWorkflowApp()
        {
            ActivityRecognizeEpisodeStateMachine activity = new ActivityRecognizeEpisodeStateMachine();

            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            Dictionary<string, string> replaces = new Dictionary<string, string>();
            dictionary.Add("episode", this.Episode);
            dictionary.Add("target", this.Target);

            this.WorkFlowApp = new WorkflowApplication(activity, dictionary)
            {
                Completed = new Action<WorkflowApplicationCompletedEventArgs>((args) =>
                {
                    var key = "result";
                    if (args.Outputs.ContainsKey(key) == true)
                    {
                        var result = args.Outputs[key].ToString();
                        this.Message = "Recognize Completed! Result:" + result;
                    }
                    this.WorkFlowApp = null;
                    this.Window.Dispatcher.Invoke(new Action(() =>
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }));
                }),
                OnUnhandledException = new Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction>((args) =>
                {
                    this.Message = "Workflow Error:" + args.UnhandledException.Message;
                    this.WorkFlowApp = null;
                    this.Window.Dispatcher.Invoke(new Action(() =>
                    {
                        CommandManager.InvalidateRequerySuggested();
                    }));
                    return UnhandledExceptionAction.Cancel;
                }),
                Idle = new Action<WorkflowApplicationIdleEventArgs>((args) =>
                {
                    foreach (var bookmark in args.Bookmarks)
                    {
                        RecognizeTransition state;
                        bool flag = Enum.TryParse<RecognizeTransition>(bookmark.BookmarkName, out state);
                        if (flag == true)
                        {
                            this.Window.Dispatcher.Invoke(new Action(() =>
                            {
                                Console.WriteLine("[Idle] Bookmark: " + state.ToString());
                                foreach (var viewModel in this.StateControlViewModels)
                                {
                                    if (viewModel.State == state)
                                    {
                                        viewModel.StateLoaded();
                                        //this.NotBusy();
                                    }
                                }
                            }));

                        }
                    }
                }),
            };
        }

        public void CloseWorkflowApp()
        {
            this.WorkFlowApp.Cancel();
            this.WorkFlowApp = null;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region INotifyRecognizeStateChanged
        public void NotifyRecognizeStateChanged(RecognizeTransition state)
        {
            if (this.RecognizeStateChanged != null)
                this.RecognizeStateChanged(state);
            //this.SetBusy("Busy");
        }
        #endregion

        #region IBusyManager
        string _busyTitle = "Busy";
        public string BusyTitle
        {
            get { return this._busyTitle; }
            private set
            {
                this._busyTitle = value;
                this.NotifyPropertyChanged("BusyTitle");
            }
        }

        string _busyMessage = "Please Wait...";
        public string BusyMessage
        {
            get { return this._busyMessage; }
            private set
            {
                this._busyMessage = value;
                this.NotifyPropertyChanged("BusyMessage");
            }
        }

        bool _isBusy = false;
        public bool IsBusy
        {
            get { return this._isBusy; }
            private set
            {
                this._isBusy = value;
                this.NotifyPropertyChanged("IsBusy");
            }
        }

        public void SetBusy(string title)
        {
            this.IsBusy = true;
            if (string.IsNullOrEmpty(title) == true)
                title = "Busy";
            this.BusyTitle = title;
            this.BusyMessage = "Please Wait...";
        }

        public void ShowMessage(string message)
        {
            this.BusyMessage = message;
        }

        public void NotBusy()
        {
            this.IsBusy = false;
        }
        #endregion

        public void Dispose()
        {
            if (this.WorkFlowApp != null)
            {
                this.WorkFlowApp.Cancel();
                this.WorkFlowApp = null;
            }
        }
    }
}
