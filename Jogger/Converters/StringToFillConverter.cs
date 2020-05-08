using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Jogger.Converters
{
    public class StringToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = (string)value;
            if (text != null)
            {
                switch (text)
                {
                    case "green":
                        return Brushes.LightGreen;
                    case "red":
                        return Brushes.IndianRed;
                    default:
                        return Brushes.White;
                }
            }
            else
            {
                return Brushes.Gray;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
