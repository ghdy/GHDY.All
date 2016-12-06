using GHDY.Core.DocumentModel;
using GHDY.Core.DocumentModel.SyncControl.Dialog;
using GHDY.Core.LearningContentProvider.VOA._51VOA;
using GHDY.Core.LearningContentProviderCore;
using GHDY.Workflow.Download;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GHDY.Demo
{
    /// <summary>
    /// Interaction logic for EpisodeDownloadWindow.xaml
    /// </summary>
    public partial class EpisodeDownloadWindow : Window
    {
        public EpisodeDownloadWindowViewModel ViewModel { get; private set; }



        public EpisodeDownloadWindow()
        {
            InitializeComponent();
            //this.ViewModel.AlbumsChanged = () => {
            //    if (this.selector_Album.SelectedItem == null)
            //        this.selector_Album.SelectedItem = this.ViewModel.Albums.First();
            //};
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel = new EpisodeDownloadWindowViewModel(new Target_51VOA(2013), this);
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, CmdOpenEpisode_Executed, CmdOpenEpisode_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll,CmdAutoDownloadAlbum_Executed,CmdAutoDownloadAlbum_CanExecute));

            this.DataContext = this.ViewModel;

            var sentence = new DMSentence();
            SyncableWord word = null;

            word = new SyncableWord("This");
            sentence.Inlines.Add(word);

            word = new SyncableWord("is");
            sentence.Inlines.Add(word);

            word = new SyncableWord("the");
            sentence.Inlines.Add(word);

            word = new SyncableWord("VOA");
            word.SetValue(Selector.IsSelectedProperty, true);
            sentence.Inlines.Add(word);

            word = new SyncableWord("Special");
            word.SetValue(Selector.IsSelectedProperty,true);
            sentence.Inlines.Add(word);

            word = new SyncableWord("English.");
            sentence.Inlines.Add(word);

            

            DialogSpeechTextEditor editor = new DialogSpeechTextEditor(sentence,(speechText)=> { MessageBox.Show(speechText); });
            editor.Show();
        }

        #region commands

        private void CmdOpenEpisode_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = selector_Episode.SelectedIndex >= 0;
        }

        private void CmdOpenEpisode_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var episode = selector_Episode.SelectedItem as XEpisode;
            if (episode != null)
            {
                var folder = this.ViewModel.CurrentTarget.GetDownloadEpisodeContentFolderPath(episode.ID, episode.AlbumID);
                System.Diagnostics.Process.Start("explorer.exe", folder);
            }
        }


        private void CmdAutoDownloadAlbum_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var album = e.Parameter as XAlbum;
            e.CanExecute = album != null;
        }

        private void CmdAutoDownloadAlbum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var album = e.Parameter as XAlbum;

            ActivityAutoDownloadAlbum activity = new ActivityAutoDownloadAlbum();

            IDictionary<string, object> dictionary = new Dictionary<string, object>();

            dictionary.Add("argsAlbum", album);
            dictionary.Add("argsTarget", this.ViewModel.CurrentTarget);

            var wfApp = new WorkflowApplication(activity, dictionary)
            {
                OnUnhandledException = new Func<WorkflowApplicationUnhandledExceptionEventArgs, UnhandledExceptionAction>((args) =>
                {
                    MessageBox.Show("Download Error:" + args.UnhandledException.Message);
                    return UnhandledExceptionAction.Cancel;
                }),
                Completed = new Action<WorkflowApplicationCompletedEventArgs>((args) => {
                    MessageBox.Show("Download Completed!");
                }),
            };

            wfApp.Run();

            var bindingExp = this.selector_Episode.GetBindingExpression(DataGrid.ItemsSourceProperty);
            if (bindingExp != null)
                bindingExp.UpdateTarget();
        }



        #endregion commands

        private void selector_Album_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var album = e.NewValue as XAlbum;
            if (album != null)
            {
                var pages = this.ViewModel.CurrentTarget.GetPages(album);
                this.ViewModel.Receive(pages);

                if (selector_Page.Items.Count > 0)
                    selector_Page.SelectedIndex = 0;
            }
        }

        private void selector_Page_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = sender as Selector;
            var page = selector.SelectedItem as XPage;
            if (page != null)
            {
                var episodes = this.ViewModel.CurrentTarget.GetEpisodes(page);

                this.ViewModel.Receive(episodes);
                if (selector_Episode.Items.Count > 0)
                    selector_Episode.SelectedIndex = 0;
            }
        }


    }
}
