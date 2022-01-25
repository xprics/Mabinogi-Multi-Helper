using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CPU_Preference_Changer.UI.ViewSome
{
    class ViewSomeContentTabMaxWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;


            double windowWidth = (double)value;

            double minimumSize = 30;
            const double windowBtnWidth = 105;

            if (windowWidth > (windowBtnWidth+minimumSize)) {
                return windowWidth - windowBtnWidth;
            } else {
                return minimumSize;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
