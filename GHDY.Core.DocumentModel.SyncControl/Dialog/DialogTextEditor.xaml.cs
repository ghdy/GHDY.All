using System;
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
using System.Windows.Shapes;

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{

    /// <summary>
    /// Interaction logic for DialogTextEditor.xaml
    /// </summary>
    public partial class DialogTextEditor : Window
    {
        private DMSentence _sentence = null;
        public DMSentence Sentence
        {
            get { return this._sentence; }
            set
            {
                this._sentence = value;
                if (this.Sentence != null)
                    this._word = null;
                if (this.IsLoaded == true)
                {
                    SetText();
                }
            }
        }

        private void SetText()
        {
            if (this._sentence != null)
                this.txtSentenceText.Text = this._sentence.ToString();
            if (this._word != null)
                this.txtSentenceText.Text = this._word.Text;
        }

        private SyncableWord _word = null;
        public SyncableWord Word
        {
            get { return this._word; }
            set
            {
                this._word = value;
                if (this.Word != null)
                    this._sentence = null;

                if (this.IsLoaded == true)
                {
                    SetText();
                }
            }
        }

        public DialogTextEditor()
        {
            InitializeComponent();
        }

        #region Commands
        public DelegateCommand CmdSaveText
        {
            get
            {
                return new DelegateCommand(new Action<object>((sender) =>
                {
                    if (this.Sentence != null)
                        this.Sentence.Initialize(this.txtSentenceText.Text);
                    else if (this.Word != null)
                        this.Word.Text = this.txtSentenceText.Text;
                }));
            }
        }

        public DelegateCommand CmdClose
        {
            get
            {
                return new DelegateCommand(new Action<object>((sender) =>
                {
                    this.Sentence = null;
                    this.Word = null;
                    this.Close();
                }));
            }
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            KeyBinding kb = new KeyBinding(this.CmdSaveText, Key.Enter, ModifierKeys.Control);
            this.InputBindings.Add(kb);

            kb = new KeyBinding(this.CmdClose, Key.Escape, ModifierKeys.None);
            this.InputBindings.Add(kb);

            this.txtSentenceText.Focus();
            this.txtSentenceText.SelectAll();

            this.DataContext = this;
            SetText();
        }
    }
}
