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
        public DMSentence CurrentSentence { get;private  set; }

        public DialogSpeechTextEditor(DMSentence sentence)
        {
            InitializeComponent();

            this.CurrentSentence = sentence;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var words = this.CurrentSentence.Syncables.Where((syncable) => {
                var dpo = syncable as DependencyObject;
                if (dpo != null && (bool)dpo.GetValue(Selector.IsSelectedProperty) == true)
                    return true;
                else
                    return false;
            });

            DMPhrase phrase = new DMPhrase();
            phrase.Inlines.AddRange(words.ToList());
            MessageBox.Show(phrase.ToSpeechText());
        }
    }
}
