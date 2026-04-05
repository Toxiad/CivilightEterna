
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Toxiad.Converters.Converter
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringSplitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (value == null) return "ERR";
            if (parameter == null) parameter = "0";
            var lst = ((string)value).Split(';');
            var idx = int.Parse((string)parameter);
            if (lst.Length <= idx) return "ERR";
            return lst[idx];
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
