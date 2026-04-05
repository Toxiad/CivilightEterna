using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Toxiad.Converters.Converter
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class BooleanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(string)) return "";
            var strarr = ((string)parameter).Split(';');
            return (bool)value ? strarr[0] : strarr[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
