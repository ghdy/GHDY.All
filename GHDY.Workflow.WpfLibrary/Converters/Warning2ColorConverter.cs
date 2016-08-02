using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GHDY.Workflow.WpfLibrary.Converters
{
    public class Warning2ColorConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = bool.Parse(value.ToString());

            if (result == true)
                return Brushes.Yellow;
            else
                return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
