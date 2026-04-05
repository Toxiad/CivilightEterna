using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Toxiad.Converters.Converter
{
    [ValueConversion(typeof(double), typeof(string))]
    public class StorageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            int a = 0;
            double st = (double)value;
            string[] unit = { "B", "KB", "MB", "GB", "TB" };
            while (st > 9999 && a < 4) {
                st /= 1024;
                a++;
            }
            return $"{st:f2}{unit[a]}";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
