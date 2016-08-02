using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Globalization;

namespace GHDY.SyncEngine
{
    public class IndexGrammar : Grammar
    {
        public int Index { get; private set; }
        public string Original { get; private set; }

        public RecognitionResult RecoResult { get; set; }

        public IndexGrammar(int index, string syncText, string cultureName)
            : base(new GrammarBuilder(NormalizeSentence(syncText)) { Culture = CultureInfo.GetCultureInfo(cultureName) })
        {
            this.Index = index;
            this.Original = syncText;
        }

        private static string NormalizeSentence(string syncText)
        {
            return syncText.Replace("\"", String.Empty);
        }

        public static IEnumerable<IndexGrammar> BuildGrammars(string[] syncTexts, string cultureName)
        {
            int index = 0;
            return syncTexts.Select(text => new IndexGrammar(index++, text, cultureName));
        }
    }
}
