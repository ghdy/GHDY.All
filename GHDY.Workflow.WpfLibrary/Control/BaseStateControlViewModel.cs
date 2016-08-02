using GHDY.Core.AudioPlayer;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Workflow.Recognize;
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
using Xceed.Wpf.Toolkit;

namespace GHDY.Workflow.WpfLibrary.Control
{
    [InheritedExport(typeof(BaseStateControlViewModel))]
    public abstract class BaseStateControlViewModel : INotifyPropertyChanged
    {
        [Import(typeof(WorkflowApplication))]
        protected WorkflowApplication WorkflowApp { get; private set; }

        [Import(typeof(IBusyManager))]
        protected IBusyManager BusyManager { get; private set; }

        [Import(typeof(AudioPlayerBase))]
        public AudioPlayerBase AudioPlayer { get; private set; }

        public UserControl View { get; private set; }

        Window _parentWindow = null;
        protected Window ParentWindow
        {
            get
            {
                if (this._parentWindow == null)
                    this._parentWindow = this.View.GetParent<Window>();
                return this._parentWindow;
            }
            private set
            {
                this._parentWindow = value;
            }
        }

        #region For Wizard Page

        bool _canFinish = false;
        public bool CanFinish
        {
            get { return this._canFinish; }
            set
            {
                this._canFinish = value;
                this.NotifyPropertyChanged("CanFinish");
            }
        }

        bool _canCancel = false;
        public bool CanCancel
        {
            get { return this._canCancel; }
            set
            {
                this._canCancel = value;
                this.NotifyPropertyChanged("CanCancel");
            }
        }

        bool _canSelectNextPage = false;
        public bool CanSelectNextPage
        {
            get { return this._canSelectNextPage; }
            set
            {
                this._canSelectNextPage = value;
                this.NotifyPropertyChanged("CanSelectNextPage");
            }
        }

        bool _canSelectPreviousPage = false;
        public bool CanSelectPreviousPage
        {
            get { return this._canSelectPreviousPage; }
            set
            {
                this._canSelectPreviousPage = value;
                this.NotifyPropertyChanged("CanSelectPreviousPage");
            }
        }

        #endregion

        public abstract RecognizeTransition State { get; }

        public abstract string Title { get; }

        public abstract string Description { get; }

        public BaseStateControlViewModel(UserControl view)
        {
            this.View = view;

            this.View.DataContext = this;
            this.View.Loaded += View_Loaded;
        }

        void View_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.Initialize();
        }

        public virtual void StateCompleted()
        {
            this.ResumeBookmatk(this.State.ToString(), null);
        }

        public abstract void StateLoaded();

        protected abstract void Initialize();

        protected void ResumeBookmatk(string bookmark, object value)
        {
            if (this.WorkflowApp != null)
            {
                this.WorkflowApp.ResumeBookmark(bookmark, value);
            }
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
