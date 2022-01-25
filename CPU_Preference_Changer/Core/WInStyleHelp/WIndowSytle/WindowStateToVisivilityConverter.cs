using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CPU_Preference_Changer.Core.WInStyleHelp.WIndowSytle
{
    public class WindowStateToVisivilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter==null) return Visibility.Visible;
            var v = (WindowState)value ;
            string curGrid = parameter as string;
            if (curGrid == null) return Visibility.Visible;
            if( v == WindowState.Maximized) {
                if (curGrid.Equals("maxGrid")) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
            if (curGrid.Equals("maxGrid")) {
                return Visibility.Visible;
            } else {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
