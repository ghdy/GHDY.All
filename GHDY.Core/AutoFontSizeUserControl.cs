using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GHDY.Core
{
    public class AutoFontSizeUserControl:UserControl
    {
        public AutoFontSizeUserControl():base()
        {
            this.SizeChanged += AutoFontSizeUserControl_SizeChanged;
        }

        void AutoFontSizeUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var temp = e.NewSize.Width / 1280;
            if (e.NewSize.Width > 0)
            {
                this.FontSize = Convert.ToInt32(temp * 20);
            }
        }
    }
}
