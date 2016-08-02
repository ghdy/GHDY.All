using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHDY.Workflow.WpfLibrary.UxService
{
    public interface IUxService
    {
        bool IsActivated { get;}
        void Activate();
        void Deactivate();
    }
}
