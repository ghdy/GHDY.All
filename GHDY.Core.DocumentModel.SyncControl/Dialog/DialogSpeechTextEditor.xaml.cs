using System;
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

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{
    /// <summary>
    /// DialogSpeechTextEditor.xaml 的交互逻辑
    /// </summary>
    public partial class DialogSpeechTextEditor : Window
    {
        public DMSentence CurrentSentence { get; private set; }

        public Action<string> SetSpeechTextCallback { get; private set; }


        #region DP:SpeechText
        public string SpeechText
        {
            get { return (string)GetValue(SpeechTextProperty); }
            set { SetValue(SpeechTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SpeechText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeechTextProperty =
            DependencyProperty.Register("SpeechText", typeof(string), typeof(DialogSpeechTextEditor), new PropertyMetadata(""));

        #endregion

        public DialogSpeechTextEditor(DMSentence sentence, Action<string> setSpeechTextCallback)
        {
            InitializeComponent();

            this.CurrentSentence = sentence;
            this.SetSpeechTextCallback = setSpeechTextCallback;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string html = BUildSentenceHtml(this.CurrentSentence);
            webTranscript.NavigateToString(html);
            this.DataContext = this;
        }

        private string BUildSentenceHtml(DMSentence sentence)
        {
            var sb = new StringBuilder();
            foreach (var syncable in sentence.Syncables.Cast<DependencyObject>())
            {
                if ((bool)syncable.GetValue(Selector.IsSelectedProperty) == true)
                    sb.AppendLine(string.Format("<b style='color:blue;'>{0}</b>", syncable.ToString()));
                else
                    sb.AppendLine(string.Format("<a>{0}</a>", syncable.ToString()));
            }

            return sb.ToString();
        }
        
        #region Commands
        public DelegateCommand CmdConfirm
        {
            get
            {
                return new DelegateCommand(new Action<object>((sender) =>
                {
                    this.SetSpeechTextCallback(this.SpeechText);
                    this.Close();

                }));
            }
        }

        public DelegateCommand CmdCancel
        {
            get
            {
                return new DelegateCommand(new Action<object>((sender) =>
                {
                    this.SetSpeechTextCallback(string.Empty);
                    this.Close();
                }));
            }
        }
        #endregion


    }
}
