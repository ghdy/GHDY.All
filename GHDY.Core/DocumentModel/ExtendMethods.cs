using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace GHDY.Core.DocumentModel
{
    public static class ExtendMethods
    {
        private static T FindNext<T>(Block current) where T : TextElement, new()
        {
            var nextBlock = current.NextBlock;
            while (nextBlock != null)
            {
                if (nextBlock is T)
                    return nextBlock as T;
                else
                    nextBlock = nextBlock.NextBlock;
            }
            return null;
        }

        private static T FindNext<T>(Inline current) where T : TextElement, new()
        {
            var nextInline = current.NextInline;
            while (nextInline != null)
            {
                if (nextInline is T)
                    return nextInline as T;
                else
                    nextInline = nextInline.NextInline;
            }
            return null;
        }

        public static T NextSyncable<T>(this T syncable)
            where T : TextElement, ISyncable, new()
        {
            var parent = syncable.Parent;
            var parentClass = parent.GetType();

            if (parentClass.IsSubclassOf(typeof(DMDocument)) == true)
            {
                if (syncable is DMParagraph)
                    return FindNext<T>(syncable as Block);
            }
            else if (parentClass.IsSubclassOf(typeof(Block)) == true)
            {
                return FindNext<T>(syncable as Inline);
            }
            else if (parentClass.IsSubclassOf(typeof(Span)) == true)
            {
                return FindNext<T>(syncable as Inline);
            }

            return null;
        }

        private static T FindPrevious<T>(Block current) where T : TextElement, new()
        {
            var prevBlock = current.PreviousBlock;
            while (prevBlock != null)
            {
                if (prevBlock is T)
                    return prevBlock as T;
                else
                    prevBlock = prevBlock.PreviousBlock;
            }
            return null;
        }

        private static T FindPrevious<T>(Inline current) where T : TextElement, new()
        {
            var prevInline = current.PreviousInline;
            while (prevInline != null)
            {
                if (prevInline is T)
                    return prevInline as T;
                else
                    prevInline = prevInline.PreviousInline;
            }
            return null;
        }

        public static T PreviousSyncable<T>(this T syncable)
            where T : TextElement, ISyncable, new()
        {
            var parent = syncable.Parent;
            var parentClass = parent.GetType();

            if (parentClass.IsSubclassOf(typeof(DMDocument)) == true)
            {
                if (syncable is DMParagraph)
                    return FindPrevious<T>(syncable as Block);
            }
            else if (parentClass.IsSubclassOf(typeof(Block)) == true)
            {
                return FindPrevious<T>(syncable as Inline);
            }
            else if (parentClass.IsSubclassOf(typeof(Span)) == true)
            {
                return FindPrevious<T>(syncable as Inline);
            }

            return null;
        }

        //--------------this is find nextParagraph's first sentence.
        //var sentence = syncable as DMSentence;
        //if (nextSentence == null)
        //{
        //    var paragraph = sentence.Paragraph;
        //    if (paragraph != null)
        //    {
        //        var nextParagraph = paragraph.NextSyncable<DMParagraph>();
        //        if (nextParagraph != null)
        //            return nextParagraph.Sentences.First() as T;
        //    }
        //}
        private static Dictionary<Type, DependencyObject> _lastestCurrentObjectDict = new Dictionary<Type, DependencyObject>();
        public static void SetIsCurrent(this DependencyObject dpObj, bool isCurrent)
        {
            Type type = dpObj.GetType();
            if (_lastestCurrentObjectDict.ContainsKey(type) == true)
            {
                var last = _lastestCurrentObjectDict[type];
                if (last == dpObj)
                    return;
                else
                    last.SetValue(SyncExtension.IsCurrentProperty, false);
            }

            dpObj.SetValue(SyncExtension.IsCurrentProperty, isCurrent);
            _lastestCurrentObjectDict[type] = dpObj;
        }

        public static DMParagraph GetParagraph(this DMDocument document, TimeSpan timeSpan)
        {
            foreach (var para in document.Paragraphs)
            {

                if (para.ContainsTimeSpan(timeSpan))
                    return para;
            }
            return null;
        }

        public static DMSentence GetSentence(this DMParagraph para, TimeSpan timeSpan)
        {
            foreach (var sentence in para.Sentences)
            {
                if (sentence.ContainsTimeSpan(timeSpan))
                    return sentence;
            }
            return null;
        }

        public static ISyncable GetSyncable(this DMSentence sentence, TimeSpan timeSpan)
        {
            foreach (var syncObj in sentence.Syncables)
            {
                if (syncObj.ContainsTimeSpan(timeSpan))
                    return syncObj;
            }
            return null;
        }

        public static SyncableWord GetWord(this DMSentence sentence, TimeSpan timeSpan)
        {
            foreach (var syncObj in sentence.Syncables)
            {
                if (syncObj.ContainsTimeSpan(timeSpan))
                {
                    if (syncObj is SyncableWord)
                        return syncObj as SyncableWord;
                    else if (syncObj is DMPhrase)
                    {
                        var phrase = syncObj as DMPhrase;
                        foreach (var word in phrase.Words)
                        {
                            if (word.ContainsTimeSpan(timeSpan) == true)
                                return word;
                        }
                    }
                }
            }
            return null;
        }
    }
}
