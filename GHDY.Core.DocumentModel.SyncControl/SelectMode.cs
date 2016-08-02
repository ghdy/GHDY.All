using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Documents;

namespace GHDY.Core.DocumentModel.SyncControl
{
    public enum SelectMode
    {
        Word = 0,
        Phrase = 1,
        Sentence = 2,
        Paragraph = 3
    }

    public static class SelectModeExtension
    {
        public static ModifierKeys GetModifierKey(this SelectMode mode)
        {
            ModifierKeys result = ModifierKeys.None;
            switch (mode)
            {
                case SelectMode.Word:
                    result = ModifierKeys.Control;
                    break;
                case SelectMode.Phrase:
                    result = ModifierKeys.Control | ModifierKeys.Alt;
                    break;
                case SelectMode.Sentence:
                    result = ModifierKeys.None;
                    break;
                case SelectMode.Paragraph:
                    result = ModifierKeys.Alt;
                    break;
            }

            return result;
        }

        public static SelectMode GetSelectMode(this ModifierKeys mk)
        {
            var result = SelectMode.Sentence;

            switch (mk)
            {
                case ModifierKeys.Alt:
                    result = SelectMode.Paragraph;
                    break;
                case ModifierKeys.Control:
                    result = SelectMode.Word;
                    break;
                case ModifierKeys.None:
                    result = SelectMode.Sentence;
                    break;
                case ModifierKeys.Control | ModifierKeys.Alt:
                    result = SelectMode.Phrase;
                    break;
            }

            return result;
        }

        public static Type GetSelectionType(this SelectMode mode)
        {
            Type result = typeof(DMSentence);
            switch (mode)
            {
                case SelectMode.Word:
                    result = typeof(SyncableWord);
                    break;
                case SelectMode.Phrase:
                    result = typeof(DMPhrase);
                    break;
                case SelectMode.Sentence:
                    result = typeof(DMSentence);
                    break;
                case SelectMode.Paragraph:
                    result = typeof(DMParagraph);
                    break;
            }

            return result;
        }
    }
}
