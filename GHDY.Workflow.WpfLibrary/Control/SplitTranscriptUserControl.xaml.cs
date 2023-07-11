using GHDY.Core;
using GHDY.Core.LearningContentProviderCore;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GHDY.Workflow.WpfLibrary.Control
{
    /// <summary>
    /// Interaction logic for SplitTranscriptUserControl.xaml
    /// </summary>
    public partial class SplitTranscriptUserControl : AutoFontSizeUserControl//System.Windows.Controls.UserControl
    {
        public SplitTranscriptViewModel ViewModel { get; private set; }

        public SplitTranscriptUserControl()
        {
            this.ViewModel = new SplitTranscriptViewModel(this);

            //this.SizeChanged += SplitTranscriptUserControl_SizeChanged;

            InitializeComponent();
        }

        //void SplitTranscriptUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    var temp = e.NewSize.Width / 1280;
        //    if (e.NewSize.Width > 0)
        //    {
        //        this.FontSize = Convert.ToInt32(temp * 20);
        //    }
        //}

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            for (int i = 0; i < this.list_Sentence.Items.Count; i++)
            {
                var sentence = this.list_Sentence.Items[i].ToString();
                if (textbox.Text.Equals(sentence) == true)
                {
                    this.list_Sentence.SelectedIndex = i;
                    return;
                }
            }
        }

        private void Btn_Reload_Click(object sender, RoutedEventArgs e)
        {
            var bindingExp = txt_Paragraph.GetBindingExpression(TextBox.TextProperty);
            if (bindingExp != null)
                bindingExp.UpdateTarget();
        }

        private void AutoFontSizeUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //this.ViewModel.SelectedParagraphIndex = 0;
        }
    }
}
