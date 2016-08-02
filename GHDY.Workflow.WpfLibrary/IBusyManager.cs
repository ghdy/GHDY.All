using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.WpfLibrary
{
    public interface IBusyManager
    {
        void SetBusy(string title);
        void ShowMessage(string message);
        void NotBusy();
    }
}
