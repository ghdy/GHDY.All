using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GHDY.Workflow.WpfLibrary
{
    public static class UserControlHelper
    {
        public static T GetParent<T>(this FrameworkElement element) where T : FrameworkElement
        {
            var parent = element.Parent as FrameworkElement;
            if (parent != null && parent is T == false)
            {
                return parent.GetParent<T>();
            }

            return parent as T;
        }
    }
}
