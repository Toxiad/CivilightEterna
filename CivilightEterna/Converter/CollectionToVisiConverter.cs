using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Toxiad.Converters.Converter
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class CollectionToVisiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    var c = (int)value;
                    if (c > 0)
                    {
                        return Visibility.Visible;
                    }
                }
                catch { }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
