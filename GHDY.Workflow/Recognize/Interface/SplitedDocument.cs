using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize.Interface
{
    public class SplitedDocument
    {
        public ObservableCollection<SplitedParagraph> Paragraphs { get; set; }

        public SplitedDocument()
        {
            this.Paragraphs = new ObservableCollection<SplitedParagraph>();
        }

        public SplitedDocument(IEnumerable<SplitedParagraph> paragraphs):this()
        {
            foreach (var para in paragraphs)
            {
                this.Paragraphs.Add(para);
            }
        }
    }
}
