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

        public DialogSpeechTextEditor(DMSentence sentence)
        {
            InitializeComponent();

            this.CurrentSentence = sentence;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string html = BUildSentenceHtml(this.CurrentSentence);
            webTranscript.NavigateToString(html);
        }

        private string BUildSentenceHtml(DMSentence sentence)
        {
            var sb = new StringBuilder();
            foreach (var syncable in sentence.Syncables.Cast<DependencyObject>())
            {
                if ((bool)syncable.GetValue(Selector.IsSelectedProperty) == true)
                    sb.AppendLine(string.Format("<b style='color:blue;'>{0}</b>",syncable.ToString()));
                else
                    sb.AppendLine(string.Format("<a>{0}</a>", syncable.ToString()));
            }

            return sb.ToString();
        }
    }
}
