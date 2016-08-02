using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.Recognize
{
    public class RecognizeConfig
    {
        public string Culture { get; private set; }

        public RecognizeConfig(string culture)
        {
            this.Culture = culture;
        }
    }
}
