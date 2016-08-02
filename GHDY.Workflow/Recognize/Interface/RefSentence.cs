using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    public class RefSentence
    {
        public int Index { get; set; }

        public string Text { get; set; }

        public TimeSpan Begin { get; set; }

        public TimeSpan End { get; set; }

        public string Speaker { get; set; }

        public RefSentence(int index, TimeSpan begin, TimeSpan end)
        {
            this.Index = index;
            this.Begin = begin;
            this.End = end;
            this.Text = string.Empty;
            this.Speaker = string.Empty;
        }
    }
}
