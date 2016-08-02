using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;
using GHDY.Core.DocumentModel.SyncControl.Dialog;

namespace GHDY.Core.DocumentModel.SyncControl
{
    public static class ProcessCommands
    {
        const string keySentenceTextEditor = "SentenceTextEditor";
        #region CMD MergeWords
        public static readonly RoutedUICommand MergeWords = new RoutedUICommand(
            "Merge",
            "Merge Words",
            typeof(DMDocumentScrollViewer));

        public static void CommandBinding_MergeWords_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            var viewer = sender as DMDocumentScrollViewer;

            if (viewer != null && viewer.SelectedElements != null && viewer.SelectedElements.Count > 0)
            {
                object parent = null;
                foreach (var item in viewer.SelectedElements)
                {
                    if (parent == null)
                        parent = item.Parent;
                    else
                    {
                        if (parent != item.Parent)
                        {
                            e.CanExecute = false;
                            break;
                        }
                    }

                    if (item is Run)
                        continue;
                    else if (item is SyncableWord == false)
                    {
                        e.CanExecute = false;
                        break;
                    }
                }
            }
            else
                e.CanExecute = false;

        }

        public static void CommandBinding_MergeWords_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var docView = sender as DMDocumentScrollViewer;

            var first = docView.SelectedElements.First() as Inline;
            var last = docView.SelectedElements.Last() as Inline;
            var elementStart = first.ElementStart;

            var prev = first.PreviousInline;
            var next = last.NextInline;

            Span parentSpan = elementStart.Parent as Span;

            var phrase = new DMPhrase();
            //bool beginSeted = false;
            foreach (var element in docView.SelectedElements)
            {
                Inline inline = element as Inline;
                parentSpan.Inlines.Remove(inline);

                phrase.Inlines.Add(inline);
            }

            if (prev != null)
                parentSpan.Inlines.InsertAfter(prev, phrase);
            else if (next != null)
                parentSpan.Inlines.InsertBefore(next, phrase);
            else
                parentSpan.Inlines.Add(phrase);
        }
        #endregion

        #region CMD SplitPhrase
        public static readonly RoutedUICommand SplitPhrase = new RoutedUICommand(
            "Split",
            "Split Phrase",
            typeof(DMDocumentScrollViewer));

        public static void CommandBinding_SplitPhrase_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            var viewer = sender as DMDocumentScrollViewer;

            if (viewer != null && viewer.SelectedElements != null && viewer.SelectedElements.Count == 1)
            {
                if (viewer.SelectedElements.First() is DMPhrase)
                {
                    e.CanExecute = true;
                }
            }

        }

        public static void CommandBinding_SplitPhrase_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var docView = sender as DMDocumentScrollViewer;
            var phrase = docView.SelectedElements.First() as DMPhrase;

            var sentence = phrase.Parent as Span;
            var inlineList = phrase.Inlines.ToList();

            var prev = phrase.PreviousInline;
            var next = phrase.NextInline;

            phrase.Inlines.Clear();
            sentence.Inlines.Remove(phrase);

            if (prev != null)
            {
                foreach (var inline in inlineList)
                {
                    sentence.Inlines.InsertAfter(prev, inline);
                    prev = inline;
                }

            }
            else if (next != null)
            {
                foreach (var inline in inlineList)
                {
                    sentence.Inlines.InsertBefore(next, inline);

                    next = inline;
                }
            }
            else
                sentence.Inlines.AddRange(inlineList);
        }
        #endregion

        #region CMD EditText
        public static readonly RoutedUICommand EditText = new RoutedUICommand(
            "EditText",
            "Edit Selected Sentence&Word Text",
            typeof(DMDocumentScrollViewer));

        public static void CommandBinding_EditText_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            var viewer = sender as DMDocumentScrollViewer;

            if (viewer != null && 
                viewer.SelectedElements != null && 
                viewer.SelectedElements.Count == 1)
            {
                var selectedItem = viewer.SelectedElements.First();
                if (selectedItem is SyncableWord && e.Parameter.ToString() == "Word")
                {
                    e.CanExecute = true;
                }
                if (selectedItem is DMSentence && e.Parameter.ToString() == "Sentence")
                {
                    e.CanExecute = true;
                }
            }

        }

        public static void CommandBinding_EditText_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DialogTextEditor textEditor = new DialogTextEditor();

            var viewer = sender as DMDocumentScrollViewer;
            var selectedItem = viewer.SelectedElements.First();
            if (selectedItem is SyncableWord)
            {
                textEditor.Word = selectedItem as SyncableWord;
            }
            else if (selectedItem is DMSentence)
            {
                textEditor.Sentence = selectedItem as DMSentence;
            }
            else
                return;

            textEditor.ShowDialog();
        }
        #endregion
    }
}
