using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Jogger.Converters
{
    class DigitalStateToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DigitalState digitalState = (DigitalState)value;
            switch (digitalState)
            {
                case DigitalState.Active:
                    return Brushes.LightGreen;
                case DigitalState.Inactive:
                    return Brushes.WhiteSmoke;
                case DigitalState.Unknown:
                    return Brushes.Gray;
                default:
                    return Brushes.Black;
            }

        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
