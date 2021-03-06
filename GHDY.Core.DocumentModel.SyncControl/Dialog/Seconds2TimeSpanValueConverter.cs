﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{
    public class Seconds2TimeSpanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double seconds = 0;
            if (double.TryParse(value.ToString(), out seconds) == true)
            {
                return TimeSpan.FromSeconds(seconds);
            }
            else
                return TimeSpan.Zero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan time = TimeSpan.Zero;
            if (TimeSpan.TryParse(value.ToString(), out time) == true)
            {
                return time.TotalSeconds;
            }
            else
                return 0;
        }
    }
}
