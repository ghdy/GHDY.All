using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    public class ActualPronounce
    {
        public int SenteceIndex { get;private set; }

        public string PronounceText { get; private set; }

        public int BeginIndex { get; private set; }

        public int EndIndex { get; private set; }

        public string Text { get;  set; }

        public ActualPronounce(int sentenceIndex,int begin,int end, string pronounceText)
        {
            this.SenteceIndex = sentenceIndex;
            this.PronounceText = pronounceText;
            this.BeginIndex = begin;
            this.EndIndex = end;
        }
    }
}
